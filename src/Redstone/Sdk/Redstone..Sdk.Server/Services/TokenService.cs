﻿using System;
using System.Threading.Tasks;
using NBitcoin;
using Redstone.Sdk.Models;
using Redstone.Sdk.Server.Exceptions;
using Redstone.Sdk.Server.Utils;
using Redstone.Sdk.Services;

namespace Redstone.Sdk.Server.Services
{
    // TODO:
    // move httpcontextaccessor part to a different service
    public class TokenService : ITokenService
    {
        private readonly Network _network;
        private readonly IWalletService _walletService;
        private readonly IRequestHeaderService _requestHeaderService;

        public TokenService(INetworkService networkService, IWalletService walletService, IRequestHeaderService requestHeaderService)
        {
            _network = networkService.InitializeNetwork(true);
            _walletService = walletService;
            _requestHeaderService = requestHeaderService;
        }

        public string GetHex()
        {
            return this._requestHeaderService.GetRedstoneHeader(RedstoneContants.RedstoneHexScheme);
        }

        // TODO coin units?
        public bool ValidateHex(long minPayment)
        {
            try
            {
                var transaction = Transaction.Load(GetHex(), _network); ;

                // TODO any other checks
                return minPayment <= transaction.TotalOut.Satoshi;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<string> AcceptHex(long minPayment, string key)
        {
            // TODO: break this out to be more specific
            if (!ValidateHex(minPayment))
                throw new TokenServiceException("Hex not valid or payment too low");

            WalletSendTransactionModel transactionModel;

            try
            {
                transactionModel = await this._walletService.SendTransactionAsync(new SendTransactionRequest
                {
                    Hex = GetHex()
                });
            }
            catch (Exception e)
            {
                throw new TokenServiceException("Failed to send transaction", e);
            }

            // We've taken payment now, shouldn't really allow an exception to be thrown. How can we ensure token
            // is generated without error.
            try
            {
                return EncryptDecrypt.EncryptString(transactionModel.TransactionId, key);
            }
            catch (Exception e)
            {
                throw new TokenServiceException("Failed to generate token from transaction", e);
            }
        }

        public string GetToken()
        {
            return this._requestHeaderService.GetRedstoneHeader(RedstoneContants.RedstoneTokenScheme);
        }

        public bool ValidateToken(string key)
        {
            try
            {
                var transactionId = EncryptDecrypt.DecryptString(GetToken(), key);

                // TODO validate transaction id

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
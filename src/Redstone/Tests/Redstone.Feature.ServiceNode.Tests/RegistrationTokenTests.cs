using System;
using System.Net;
using System.Text;
using NBitcoin;
using Redstone.Core.Networks;
using Redstone.Features.ServiceNode;
using Redstone.ServiceNode.Models;
using Redstone.ServiceNode.Utils;
using Xunit;

namespace Redstone.Feature.ServiceNode.Tests
{
    public class RegistrationTokenTests
    {
        [Fact]
         public void CanValidateRegistrationToken()
        {
            var rsa = new RsaKey();
            var ecdsa = new Key().GetBitcoinSecret(RedstoneNetworks.Main);

            var serverAddress = ecdsa.GetAddress().ToString();
            
            var token = new RegistrationToken(
                (int)ServiceNodeProtocolVersion.INITIAL,
                IPAddress.Parse("127.0.0.1"),
                IPAddress.Parse("2001:0db8:85a3:0000:0000:8a2e:0370:7334"),
                "",
                37123,
                new KeyId("dbb476190a81120928763ee8ce97e4c0bcfd6624"),
                new KeyId("dbb476190a81120928763ee8ce97e4c0bcfd6624"),
                ecdsa.PubKey,
                new Uri("https://restone.com/servicetest"));

            var cryptoUtils = new CryptoUtils(rsa, ecdsa);
            token.RsaSignature = cryptoUtils.SignDataRSA(token.GetHeaderBytes().ToArray());
            token.EcdsaSignature = cryptoUtils.SignDataECDSA(token.GetHeaderBytes().ToArray());

            Assert.True(token.Validate(RedstoneNetworks.Main));
        }

        
        [Fact]
        public void CheckSignatureOfRegistrationToken()
        {
            var rsa = new RsaKey();
            var ecdsa = new Key().GetBitcoinSecret(RedstoneNetworks.Main);

            var token = new RegistrationToken(1,
                IPAddress.Parse("172.16.1.10"),
                IPAddress.Parse("2001:0db8:85a3:0000:1234:8a2e:0370:7334"),
                "",
                16174,
                new KeyId("dbb476190a81120928763ee8ce97e4c0bcfd6624"),
                new KeyId("dbb476190a81120928763ee8ce97e4c0bcfd6624"),
                ecdsa.PubKey,
                new Uri("https://redstone.com/test"));

            // Only the 'header' portion of the registration token gets signed, minus the length bytes
            var message = token.GetHeaderBytes();

            var cryptoUtils = new CryptoUtils(rsa, ecdsa);
            token.RsaSignature = cryptoUtils.SignDataRSA(message.ToArray());
            token.EcdsaSignature = cryptoUtils.SignDataECDSA(message.ToArray());

            var signature = cryptoUtils.SignDataECDSA(message.ToArray());
            Assert.True(CryptoUtils.VerifySignatureECDSA(message.ToArray(), ecdsa.PubKey, Encoding.UTF8.GetString(signature)));
            Assert.True(token.VerifySignatures());
        }

        [Fact]
        public void CanVerifySignature()
        {
            var rsa = new RsaKey();
            var ecdsa = new Key().GetBitcoinSecret(RedstoneNetworks.Main);

            var token = new RegistrationToken(1,
                IPAddress.Parse("172.16.1.10"),
                IPAddress.Parse("2001:0db8:85a3:0000:1234:8a2e:0370:7334"),
                "",
                16174,
                new KeyId("dbb476190a81120928763ee8ce97e4c0bcfd6624"),
                new KeyId("dbb476190a81120928763ee8ce97e4c0bcfd6624"),
                ecdsa.PubKey,
                new Uri("https://redstone.com.test"));

            var cryptoUtils = new CryptoUtils(rsa, ecdsa);
            token.EcdsaSignature = cryptoUtils.SignDataECDSA(token.GetHeaderBytes().ToArray());

            Assert.True(token.VerifySignatures());
        }
    }
}
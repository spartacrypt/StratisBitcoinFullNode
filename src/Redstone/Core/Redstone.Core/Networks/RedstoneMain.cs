﻿using Redstone.Core.Policies;

namespace Redstone.Core.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using NBitcoin;
    using NBitcoin.BouncyCastle.Math;
    using NBitcoin.Protocol;
    using Stratis.Bitcoin.Features.Wallet;
    using Redstone.Core.Networks.Deployments;
    using NBitcoin.DataEncoders;

    public class RedstoneMain : Network
    {
        /// <summary> Redstone maximal value for the calculated time offset. If the value is over this limit, the time syncing feature will be switched off. </summary>
        public const int RedstoneMaxTimeOffsetSeconds = 25 * 60;

        /// <summary> Redstone default value for the maximum tip age in seconds to consider the node in initial block download (2 hours). </summary>
        public const int RedstoneDefaultMaxTipAgeInSeconds = 2 * 60 * 60;

        /// <summary> The name of the root folder containing the different Redstone blockchains (RedstoneMain, RedstoneTest, RedstoneRegTest). </summary>
        public const string RedstoneRootFolderName = "redstone";

        /// <summary> The default name used for the Redstone configuration file. </summary>
        public const string RedstoneDefaultConfigFilename = "redstone.conf";

        public RedstoneMain()
        {
            // The message start string is designed to be unlikely to occur in normal data.
            // The characters are rarely used upper ASCII, not valid as UTF-8, and produce
            // a large 4-byte int at any alignment.
            var messageStart = new byte[4];
            messageStart[0] = 0xb9; 
            messageStart[1] = 0xd0; 
            messageStart[2] = 0xae; 
            messageStart[3] = 0xd7;
            uint magic = BitConverter.ToUInt32(messageStart, 0); // 0xd7aed0b9 = ×®Ð¹
            
            this.Name = "RedstoneMain";
            this.Magic = magic;
            this.DefaultPort = 19056;
            this.DefaultMaxOutboundConnections = 16;
            this.DefaultMaxInboundConnections = 109;
            this.RPCPort = 19057;
            this.MaxTipAge = RedstoneDefaultMaxTipAgeInSeconds;
            this.MinTxFee = 10000;
            this.FallbackFee = 10000;
            this.MinRelayTxFee = 10000;
            this.RootFolderName = RedstoneRootFolderName;
            this.DefaultConfigFilename = RedstoneDefaultConfigFilename;
            this.MaxTimeOffsetSeconds = RedstoneMaxTimeOffsetSeconds;
            this.CoinTicker = "XRD";

            var consensusFactory = new PosConsensusFactory();

            // Create the genesis block.
            this.GenesisTime = 1550609400; // Tuesday, 19 February 2019 20:50:00 was 1470467000;
            this.GenesisNonce = 1831645;
            this.GenesisBits = 0x1e0fffff;
            this.GenesisVersion = 1;
            this.GenesisReward = Money.Zero;

            Block genesisBlock = CreateRedstoneGenesisBlock(consensusFactory, this.GenesisTime, this.GenesisNonce, this.GenesisBits, this.GenesisVersion, this.GenesisReward);

            this.Genesis = genesisBlock;

            // Taken from StratisX.
            var consensusOptions = new PosConsensusOptions(
                maxBlockBaseSize: 1_000_000,
                maxStandardVersion: 2,
                maxStandardTxWeight: 100_000,
                maxBlockSigopsCost: 20_000,
                maxStandardTxSigopsCost: 20_000 / 5
            );

            var buriedDeployments = new BuriedDeploymentsArray
            {
                [BuriedDeployments.BIP34] = 0,
                [BuriedDeployments.BIP65] = 0,
                [BuriedDeployments.BIP66] = 0
            };

            var bip9Deployments = new RedstoneBIP9Deployments();

            this.Consensus = new Consensus(
                consensusFactory: consensusFactory,
                consensusOptions: consensusOptions,
                coinType: (int)CoinType.Redstone,
                hashGenesisBlock: genesisBlock.GetHash(),
                subsidyHalvingInterval: 210000,
                majorityEnforceBlockUpgrade: 750,
                majorityRejectBlockOutdated: 950,
                majorityWindow: 1000,
                buriedDeployments: buriedDeployments,
                bip9Deployments: bip9Deployments,
                bip34Hash: new uint256("0x000000000000024b89b42a942fe0d9fea3bb44ab7bd1b19115dd6a759c0808b8"),
                ruleChangeActivationThreshold: 1916, // 95% of 2016
                minerConfirmationWindow: 2016, // nPowTargetTimespan / nPowTargetSpacing
                maxReorgLength: 500,
                defaultAssumeValid: new uint256("0x55a8205ae4bbf18f4d238c43f43005bd66e0b1f679b39e2c5c62cf6903693a5e"), // 795970
                maxMoney: long.MaxValue,
                coinbaseMaturity: 50,
                premineHeight: 2,
                premineReward: Money.Coins(5_400_000),
                proofOfWorkReward: Money.Coins(30),
                powTargetTimespan: TimeSpan.FromSeconds(14 * 24 * 60 * 60), // two weeks
                powTargetSpacing: TimeSpan.FromSeconds(10 * 60),
                powAllowMinDifficultyBlocks: false,
                posNoRetargeting: false,
                powNoRetargeting: false,
                powLimit: new Target(new uint256("00000fffffffffffffffffffffffffffffffffffffffffffffffffffffffffff")),
                minimumChainWork: null,
                isProofOfStake: true,
                lastPowBlock: 129600,
                proofOfStakeLimit: new BigInteger(uint256.Parse("00000fffffffffffffffffffffffffffffffffffffffffffffffffffffffffff").ToBytes(false)),
                proofOfStakeLimitV2: new BigInteger(uint256.Parse("000000000000ffffffffffffffffffffffffffffffffffffffffffffffffffff").ToBytes(false)),
                proofOfStakeReward: Money.Coins(15),
                posRewardReduction: true,
                posRewardReductionBlockInterval: 525600,
                posRewardReductionPercentage: 7.5m

            );

            this.Base58Prefixes = new byte[12][];
            this.Base58Prefixes[(int)Base58Type.PUBKEY_ADDRESS] = new byte[] { (60) };
            this.Base58Prefixes[(int)Base58Type.SCRIPT_ADDRESS] = new byte[] { (122) };
            this.Base58Prefixes[(int)Base58Type.SECRET_KEY] = new byte[] { (60 + 128) };
            this.Base58Prefixes[(int)Base58Type.ENCRYPTED_SECRET_KEY_NO_EC] = new byte[] { 0x01, 0x42 };
            this.Base58Prefixes[(int)Base58Type.ENCRYPTED_SECRET_KEY_EC] = new byte[] { 0x01, 0x43 };
            this.Base58Prefixes[(int)Base58Type.EXT_PUBLIC_KEY] = new byte[] { (0x04), (0x88), (0xB2), (0x1E) };
            this.Base58Prefixes[(int)Base58Type.EXT_SECRET_KEY] = new byte[] { (0x04), (0x88), (0xAD), (0xE4) };
            this.Base58Prefixes[(int)Base58Type.PASSPHRASE_CODE] = new byte[] { 0x2C, 0xE9, 0xB3, 0xE1, 0xFF, 0x39, 0xE2 };
            this.Base58Prefixes[(int)Base58Type.CONFIRMATION_CODE] = new byte[] { 0x64, 0x3B, 0xF6, 0xA8, 0x9A };
            this.Base58Prefixes[(int)Base58Type.STEALTH_ADDRESS] = new byte[] { 0x2a };
            this.Base58Prefixes[(int)Base58Type.ASSET_ID] = new byte[] { 23 };
            this.Base58Prefixes[(int)Base58Type.COLORED_ADDRESS] = new byte[] { 0x13 };

            this.Checkpoints = new Dictionary<int, CheckpointInfo>
            {
            };

            var encoder = new Bech32Encoder("bc");
            this.Bech32Encoders = new Bech32Encoder[2];
            this.Bech32Encoders[(int)Bech32Type.WITNESS_PUBKEY_ADDRESS] = encoder;
            this.Bech32Encoders[(int)Bech32Type.WITNESS_SCRIPT_ADDRESS] = encoder;

            this.DNSSeeds = new List<DNSSeedData>()
            {
                new DNSSeedData("seed.redstonecoin.com", "seed.redstonecoin.com"),
            };

            this.SeedNodes = new List<NetworkAddress>
            {
               new NetworkAddress(IPAddress.Parse("80.211.84.170"), this.DefaultPort), // cryptohunter node #4
               new NetworkAddress(IPAddress.Parse("31.14.138.23"), this.DefaultPort), // cryptohunter node #3
               new NetworkAddress(IPAddress.Parse("35.178.169.232"), this.DefaultPort), // cryptohunter node #aws
             };

            this.StandardScriptsRegistry = new RedstoneStandardScriptsRegistry();

            Assert(this.Consensus.HashGenesisBlock == uint256.Parse("8e21759b1aefe10358fef84da1ac428af6ba17990b7eee71c47de9582fa31806"));
            Assert(this.Genesis.Header.HashMerkleRoot == uint256.Parse("c89473b52c9a1afbc3784b0306fd06e86d016c13d68b56343c78a9377491a2f7"));
        }

        protected static Block CreateRedstoneGenesisBlock(ConsensusFactory consensusFactory, uint nTime, uint nNonce, uint nBits, int nVersion, Money genesisReward)
        {
            string pszTimestamp = "http://www.escapistmagazine.com/news/view/109385-Computer-Built-in-Minecraft-Has-RAM-Performs-Division";
            Transaction txNew = consensusFactory.CreateTransaction();
            txNew.Version = 1;
            txNew.Time = nTime;
            txNew.AddInput(new TxIn()
            {
                ScriptSig = new Script(Op.GetPushOp(0), new Op()
                {
                    Code = (OpcodeType)0x1,
                    PushData = new[] { (byte)42 }
                }, Op.GetPushOp(Encoders.ASCII.DecodeData(pszTimestamp)))
            });
            txNew.AddOutput(new TxOut()
            {
                Value = genesisReward,
            });
            Block genesis = consensusFactory.CreateBlock();
            genesis.Header.BlockTime = Utils.UnixTimeToDateTime(nTime);
            genesis.Header.Bits = nBits;
            genesis.Header.Nonce = nNonce;
            genesis.Header.Version = nVersion;
            genesis.Transactions.Add(txNew);
            genesis.Header.HashPrevBlock = uint256.Zero;
            genesis.UpdateMerkleRoot();
            return genesis;
        }
    }
}

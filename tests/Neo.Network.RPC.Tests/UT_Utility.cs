// Copyright (C) 2015-2025 The Neo Project.
//
// UT_Utility.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Extensions;
using Neo.Network.P2P.Payloads;
using Neo.Network.P2P.Payloads.Conditions;
using Neo.SmartContract;
using Neo.SmartContract.Native;
using Neo.Wallets;
using System;
using System.Numerics;

namespace Neo.Network.RPC.Tests
{
    [TestClass]
    public class UT_Utility
    {
        private KeyPair keyPair;
        private UInt160 scriptHash;
        private ProtocolSettings protocolSettings;

        [TestInitialize]
        public void TestSetup()
        {
            keyPair = new KeyPair(Wallet.GetPrivateKeyFromWIF("KyXwTh1hB76RRMquSvnxZrJzQx7h9nQP2PCRL38v6VDb5ip3nf1p"));
            scriptHash = Contract.CreateSignatureRedeemScript(keyPair.PublicKey).ToScriptHash();
            protocolSettings = ProtocolSettings.Load("protocol.json");
        }

        [TestMethod]
        public void TestAsScriptHash()
        {
            var scriptHash1 = Utility.AsScriptHash(NativeContract.NEO.Id.ToString());
            var scriptHash2 = Utility.AsScriptHash(NativeContract.NEO.Hash.ToString());
            var scriptHash3 = Utility.AsScriptHash(NativeContract.NEO.Name);
            Assert.AreEqual(scriptHash1, scriptHash2);
            Assert.AreEqual(scriptHash1, scriptHash3);
        }

        [TestMethod]
        public void TestGetKeyPair()
        {
            string nul = null;
            Assert.ThrowsExactly<ArgumentNullException>(() => _ = Utility.GetKeyPair(nul));

            string wif = "KyXwTh1hB76RRMquSvnxZrJzQx7h9nQP2PCRL38v6VDb5ip3nf1p";
            var result = Utility.GetKeyPair(wif);
            Assert.AreEqual(keyPair, result);

            string privateKey = keyPair.PrivateKey.ToHexString();
            result = Utility.GetKeyPair(privateKey);
            Assert.AreEqual(keyPair, result);

            string hexWith0x = $"0x{result.PrivateKey.ToHexString()}";
            result = Utility.GetKeyPair(hexWith0x);
            Assert.AreEqual(keyPair, result);

            var action = () => { Utility.GetKeyPair("00"); };
            Assert.ThrowsExactly<FormatException>(action);
        }

        [TestMethod]
        public void TestGetScriptHash()
        {
            string nul = null;
            Assert.ThrowsExactly<ArgumentNullException>(() => _ = Utility.GetScriptHash(nul, protocolSettings));

            string addr = scriptHash.ToAddress(protocolSettings.AddressVersion);
            var result = Utility.GetScriptHash(addr, protocolSettings);
            Assert.AreEqual(scriptHash, result);

            string hash = scriptHash.ToString();
            result = Utility.GetScriptHash(hash, protocolSettings);
            Assert.AreEqual(scriptHash, result);

            string publicKey = keyPair.PublicKey.ToString();
            result = Utility.GetScriptHash(publicKey, protocolSettings);
            Assert.AreEqual(scriptHash, result);

            var action = () => { Utility.GetScriptHash("00", protocolSettings); };
            Assert.ThrowsExactly<FormatException>(action);
        }

        [TestMethod]
        public void TestTransactionAttribute()
        {
            var attribute = new Conflicts
            {
                Hash = UInt256.Zero
            };
            var json = attribute.ToJson();
            var result = Utility.TransactionAttributeFromJson(json).ToJson();
            Assert.AreEqual(json.ToString(), result.ToString());

            var attribute2 = new OracleResponse
            {
                Id = 1234,
                Code = 0,
                Result = new ReadOnlyMemory<byte> { }
            };
            json = attribute2.ToJson();
            result = Utility.TransactionAttributeFromJson(json).ToJson();
            Assert.AreEqual(json.ToString(), result.ToString());

            var attribute3 = new NotValidBefore
            {
                Height = 10000
            };
            json = attribute3.ToJson();
            result = Utility.TransactionAttributeFromJson(json).ToJson();
            Assert.AreEqual(json.ToString(), result.ToString());

            var attribute4 = new HighPriorityAttribute();
            json = attribute4.ToJson();
            result = Utility.TransactionAttributeFromJson(json).ToJson();
            Assert.AreEqual(json.ToString(), result.ToString());
        }

        [TestMethod]
        public void TestWitnessRule()
        {
            var rule = new WitnessRule
            {
                Action = WitnessRuleAction.Allow,
                Condition = new CalledByEntryCondition()
            };
            var json = rule.ToJson();
            var result = Utility.RuleFromJson(json, ProtocolSettings.Default).ToJson();
            Assert.AreEqual(json.ToString(), result.ToString());

            rule.Condition = new OrCondition()
            {
                Expressions = new WitnessCondition[]
                {
                    new BooleanCondition()
                    {
                        Expression = true
                    },
                    new BooleanCondition()
                    {
                        Expression = false
                    }
                }
            };
            json = rule.ToJson();
            result = Utility.RuleFromJson(json, ProtocolSettings.Default).ToJson();
            Assert.AreEqual(json.ToString(), result.ToString());

            rule.Condition = new AndCondition()
            {
                Expressions = new WitnessCondition[]
                {
                    new BooleanCondition()
                    {
                        Expression = true
                    },
                    new BooleanCondition()
                    {
                        Expression = false
                    }
                }
            };
            json = rule.ToJson();
            result = Utility.RuleFromJson(json, ProtocolSettings.Default).ToJson();
            Assert.AreEqual(json.ToString(), result.ToString());

            rule.Condition = new BooleanCondition() { Expression = true };
            json = rule.ToJson();
            result = Utility.RuleFromJson(json, ProtocolSettings.Default).ToJson();
            Assert.AreEqual(json.ToString(), result.ToString());

            rule.Condition = new NotCondition()
            {
                Expression = new BooleanCondition()
                {
                    Expression = true
                }
            };
            json = rule.ToJson();
            result = Utility.RuleFromJson(json, ProtocolSettings.Default).ToJson();
            Assert.AreEqual(json.ToString(), result.ToString());

            var kp = Utility.GetKeyPair("KyXwTh1hB76RRMquSvnxZrJzQx7h9nQP2PCRL38v6VDb5ip3nf1p");
            rule.Condition = new GroupCondition() { Group = kp.PublicKey };
            json = rule.ToJson();
            result = Utility.RuleFromJson(json, ProtocolSettings.Default).ToJson();
            Assert.AreEqual(json.ToString(), result.ToString());

            rule.Condition = new CalledByContractCondition() { Hash = UInt160.Zero };
            json = rule.ToJson();
            result = Utility.RuleFromJson(json, ProtocolSettings.Default).ToJson();
            Assert.AreEqual(json.ToString(), result.ToString());

            rule.Condition = new ScriptHashCondition() { Hash = UInt160.Zero };
            json = rule.ToJson();
            result = Utility.RuleFromJson(json, ProtocolSettings.Default).ToJson();
            Assert.AreEqual(json.ToString(), result.ToString());
            Assert.AreEqual(json.ToString(), result.ToString());

            rule.Condition = new CalledByGroupCondition() { Group = kp.PublicKey };
            json = rule.ToJson();
            result = Utility.RuleFromJson(json, ProtocolSettings.Default).ToJson();
            Assert.AreEqual(json.ToString(), result.ToString());
            Assert.AreEqual(json.ToString(), result.ToString());
        }

        [TestMethod]
        public void TestToBigInteger()
        {
            decimal amount = 1.23456789m;
            uint decimals = 9;
            var result = amount.ToBigInteger(decimals);
            Assert.AreEqual(1234567890, result);

            amount = 1.23456789m;
            decimals = 18;
            result = amount.ToBigInteger(decimals);
            Assert.AreEqual(BigInteger.Parse("1234567890000000000"), result);

            amount = 1.23456789m;
            decimals = 4;
            Assert.ThrowsExactly<ArgumentException>(() => _ = result = amount.ToBigInteger(decimals));
        }
    }
}

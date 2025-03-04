// Copyright (C) 2015-2025 The Neo Project.
//
// Conflicts.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.Extensions;
using Neo.IO;
using Neo.Json;
using Neo.Persistence;
using Neo.SmartContract.Native;
using System.IO;

namespace Neo.Network.P2P.Payloads
{
    public class Conflicts : TransactionAttribute
    {
        /// <summary>
        /// Indicates the conflict transaction hash.
        /// </summary>
        public UInt256 Hash;

        public override TransactionAttributeType Type => TransactionAttributeType.Conflicts;

        public override bool AllowMultiple => true;

        public override int Size => base.Size + Hash.Size;

        protected override void DeserializeWithoutType(ref MemoryReader reader)
        {
            Hash = reader.ReadSerializable<UInt256>();
        }

        protected override void SerializeWithoutType(BinaryWriter writer)
        {
            writer.Write(Hash);
        }

        public override JObject ToJson()
        {
            JObject json = base.ToJson();
            json["hash"] = Hash.ToString();
            return json;
        }

        public override bool Verify(DataCache snapshot, Transaction tx)
        {
            // Only check if conflicting transaction is on chain. It's OK if the
            // conflicting transaction was in the Conflicts attribute of some other
            // on-chain transaction.
            return !NativeContract.Ledger.ContainsTransaction(snapshot, Hash);
        }

        public override long CalculateNetworkFee(DataCache snapshot, Transaction tx)
        {
            return tx.Signers.Length * base.CalculateNetworkFee(snapshot, tx);
        }
    }
}

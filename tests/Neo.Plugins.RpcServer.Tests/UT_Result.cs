// Copyright (C) 2015-2025 The Neo Project.
//
// UT_Result.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract;

namespace Neo.Plugins.RpcServer.Tests
{
    [TestClass]
    public class UT_Result
    {
        [TestMethod]
        public void TestNotNull_Or()
        {
            ContractState? contracts = null;
            Assert.ThrowsExactly<RpcException>(() => _ = contracts.NotNull_Or(RpcError.UnknownContract).ToJson());
        }
    }
}

#nullable disable

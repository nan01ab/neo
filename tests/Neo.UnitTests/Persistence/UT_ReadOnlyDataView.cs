// Copyright (C) 2015-2024 The Neo Project.
//
// UT_ReadOnlyDataView.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Persistence;
using Neo.SmartContract;

namespace Neo.UnitTests.Persistence
{
    public class UT_ReadOnlyDataView
    {
        [TestMethod]
        public void TestReadOnlyDataView()
        {
            var store = new MemoryStore();
            var key = new KeyBuilder(1, 2).Add(new UInt160()).ToArray();
            
            var view = new ReadOnlyDataView(store);
            Assert.IsFalse(view.Contains(key));

            var value = new byte[] { 1, 2, 3 };
            store.Put(key, value);
            Assert.IsTrue(view.Contains(key));
            CollectionAssert.AreEqual(value, view[key].Value.ToArray());

            Assert.IsTrue(view.TryGet(key, out var item));
            CollectionAssert.AreEqual(value, item.Value.ToArray());
        }
    }
}

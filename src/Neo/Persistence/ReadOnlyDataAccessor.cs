// Copyright (C) 2015-2024 The Neo Project.
//
// ReadOnlyDataAccessor.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract;

namespace Neo.Persistence
{
    public class ReadOnlyDataAccessor : IReadOnlyDataAccessor
    {
        private readonly IReadOnlyStore store;

        public ReadOnlyDataAccessor(IReadOnlyStore store)
        {
            this.store = store;
        }

        public bool Contains(StorageKey key)
        {
            return store.Contains(key.ToArray());
        }

        public bool TryGet(StorageKey key, out StorageItem item)
        {
            if (store.TryGet(key.ToArray(), out byte[] value))
            {
                item = new StorageItem(value);
                return true;
            }

            item = null;
            return false;
        }
    }
}

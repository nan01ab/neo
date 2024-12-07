// Copyright (C) 2015-2024 The Neo Project.
//
// Bechmarks.ReadOnlyDataView.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.


using BenchmarkDotNet.Attributes;
using Neo.Persistence;
using Neo.Plugins.Storage;
using Neo.SmartContract;
using System.Diagnostics;
using System.Text;

namespace Neo.Benchmarks.Persistence.Benchmarks
{
    public class Bechmarks_ReadOnlyDataView
    {
        private static readonly byte[] key1 = StorageKey.CreateSearchPrefix(1, [1]).ToArray();
        private static readonly byte[] key2 = StorageKey.CreateSearchPrefix(2, [1]).ToArray();
        private static readonly byte[] value = Encoding.UTF8.GetBytes("BechmarksReadOnlyDataView");

        private const string PathLevelDB = "Data_LevelDB_Benchmarks";
        private const string PathRocksDB = "Data_RocksDB_Benchmarks";

        private static readonly LevelDBStore levelDb = new();
        private static readonly RocksDBStore rocksDb = new();

        private static IStore levelDbStore;
        private static IStore rocksDbStore;

        [GlobalSetup]
        public void Setup()
        {
            if (Directory.Exists(PathLevelDB))
                Directory.Delete(PathLevelDB, true);

            if (Directory.Exists(PathRocksDB))
                Directory.Delete(PathRocksDB, true);

            levelDbStore = levelDb.GetStore(PathLevelDB);
            levelDbStore.Put(key1, value);

            rocksDbStore = rocksDb.GetStore(PathRocksDB);
            rocksDbStore.Put(key1, value);
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            levelDbStore.Dispose();
            rocksDbStore.Dispose();

            if (Directory.Exists(PathLevelDB))
                Directory.Delete(PathLevelDB, true);

            if (Directory.Exists(PathRocksDB))
                Directory.Delete(PathRocksDB, true);
        }

        [Benchmark]
        public void ReadOnlyDataView_LevelDB()
        {
            var view = new ReadOnlyDataView(levelDbStore);
            var ok = view.TryGet(key1, out var _);
            Debug.Assert(ok);

            ok = view.TryGet(key2, out var _);
            Debug.Assert(!ok);
        }

        [Benchmark]
        public void SnapshotCache_LevelDB()
        {
            var snapshot = new SnapshotCache(levelDbStore);
            var ok = snapshot.TryGet(key1, out var _);
            Debug.Assert(ok);

            ok = snapshot.TryGet(key2, out var _);
            Debug.Assert(!ok);
        }

        [Benchmark]
        public void ReadOnlyDataView_RocksDB()
        {
            var view = new ReadOnlyDataView(rocksDbStore);
            var ok = view.TryGet(key1, out var _);
            Debug.Assert(ok);

            ok = view.TryGet(key2, out var _);
            Debug.Assert(!ok);
        }

        [Benchmark]
        public void SnapshotCache_RocksDB()
        {
            var snapshot = new SnapshotCache(rocksDbStore);
            var ok = snapshot.TryGet(key1, out var _);
            Debug.Assert(ok);

            ok = snapshot.TryGet(key2, out var _);
            Debug.Assert(!ok);
        }
    }
}

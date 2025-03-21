// Copyright (C) 2015-2025 The Neo Project.
//
// TreeNode.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System.Collections.Generic;

namespace Neo.Plugins.RpcServer
{
    public class TreeNode<T>
    {
        private readonly List<TreeNode<T>> children = new();

        public T Item { get; }
        public TreeNode<T> Parent { get; }
        public IReadOnlyList<TreeNode<T>> Children => children;

        internal TreeNode(T item, TreeNode<T> parent)
        {
            Item = item;
            Parent = parent;
        }

        public TreeNode<T> AddChild(T item)
        {
            TreeNode<T> child = new(item, this);
            children.Add(child);
            return child;
        }

        internal IEnumerable<T> GetItems()
        {
            yield return Item;
            foreach (var child in children)
                foreach (T item in child.GetItems())
                    yield return item;
        }
    }
}

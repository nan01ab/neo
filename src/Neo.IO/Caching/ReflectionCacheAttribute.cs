// Copyright (C) 2015-2025 The Neo Project.
//
// ReflectionCacheAttribute.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;

namespace Neo.IO.Caching
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="type">Type</param>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    internal class ReflectionCacheAttribute
        (Type type) : Attribute
    {
        /// <summary>
        /// Type
        /// </summary>
        public Type Type { get; } = type;
    }
}

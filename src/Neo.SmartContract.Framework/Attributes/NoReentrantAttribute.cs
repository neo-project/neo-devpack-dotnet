// Copyright (C) 2015-2022 The Neo Project.
// 
// The Neo.SmartContract.Framework is free software distributed under the MIT 
// software license, see the accompanying file LICENSE in the main directory 
// of the project or http://www.opensource.org/licenses/mit-license.php 
// for more details.
// 
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;
using Neo.SmartContract.Framework.Services;

namespace Neo.SmartContract.Framework.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public abstract class NoReentrantAttribute : ModifierAttribute, IDisposable
    {
        private readonly StorageMap _context;
        private const string _key = "noReentrant";

        public NoReentrantAttribute(byte prefix = 0xFF)
        {
            _context = new StorageMap(prefix);
        }

        public override void Validate()
        {
            var data = _context.Get(_key);
            ExecutionEngine.Assert(data == null, "Already entered");
            _context.Put(_key, 1);
        }

#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
        public void Dispose()
#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
        {
            _context.Delete(_key);
        }
    }
}

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
using System.Runtime.CompilerServices;
using Neo.SmartContract.Framework.Services;

namespace Neo.SmartContract.Framework.Attributes
{
    /// <summary>
    /// Global no Reentrancy protection. This no reentrant attribute by default take as a key the method name
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class NoReentrantMethodAttribute : ModifierAttribute
    {
        private readonly StorageMap _context;
        private readonly string _key;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="prefix">Storage prefix</param>
        /// <param name="key">Storage key (the method name as default)</param>
        public NoReentrantMethodAttribute(byte prefix = 0xFF, [CallerMemberName] string key = "noReentrant")
        {
            _key = key;
            _context = new(Storage.CurrentContext, prefix);
        }

        public override void Enter()
        {
            var data = _context.Get(_key);
            ExecutionEngine.Assert(data == null, "Already entered");
            _context.Put(_key, 1);
        }

        public override void Exit()
        {
            _context.Delete(_key);
        }
    }
}

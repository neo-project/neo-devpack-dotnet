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
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class NoReentrantAttribute : ModifierAttribute
    {
        private readonly StorageMap _context;
        private readonly string _key;

        public NoReentrantAttribute(byte prefix = 0xFF, [CallerMemberName] string key = "noReentrant")
        {
            _key = key;
            _context = new StorageMap(Storage.CurrentContext, prefix);
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

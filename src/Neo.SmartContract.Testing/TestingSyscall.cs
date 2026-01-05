// Copyright (C) 2015-2026 The Neo Project.
//
// TestingSyscall.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.Cryptography;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;

namespace Neo.SmartContract.Testing
{
    internal class TestingSyscall
    {
        private readonly List<Action<TestingApplicationEngine>> _actions = new();

        /// <summary>
        /// Syscall Name
        /// </summary>
        public const string Name = $"Neo.SmartContract.Testing.Invoke";

        /// <summary>
        /// Syscall hash
        /// </summary>
        public static uint Hash => BinaryPrimitives.ReadUInt32LittleEndian(Encoding.ASCII.GetBytes(Name).Sha256());

        /// <summary>
        /// Add action
        /// </summary>
        /// <param name="action">Action</param>
        /// <returns>Action index</returns>
        public int Add(Action<TestingApplicationEngine> action)
        {
            _actions.Add(action);
            return _actions.Count - 1;
        }

        /// <summary>
        /// Invoke action
        /// </summary>
        /// <param name="engine">Engine</param>
        /// <param name="index">Action index</param>
        internal void Invoke(TestingApplicationEngine engine, int index)
        {
            _actions[index](engine);
        }
    }
}

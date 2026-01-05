// Copyright (C) 2015-2026 The Neo Project.
//
// TestException.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.VM;
using System;

namespace Neo.SmartContract.Testing.Exceptions
{
    public class TestException : Exception
    {
        /// <summary>
        /// State
        /// </summary>
        public VMState State { get; }

        /// <summary>
        /// Current context
        /// </summary>
        public ExecutionContext? CurrentContext { get; }

        /// <summary>
        /// Invocation Stack
        /// </summary>
        public ExecutionContext[] InvocationStack { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="engine">Test engine</param>
        internal TestException(TestingApplicationEngine engine) : base(
            engine.FaultException?.Message ?? $"Error while executing the script",
            engine.FaultException ?? new Exception($"Error while executing the script"))
        {
            State = engine.State;
            CurrentContext = engine.CurrentContext;
            InvocationStack = engine.InvocationStack.ToArray();
        }
    }
}

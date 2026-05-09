// Copyright (C) 2015-2026 The Neo Project.
//
// RuntimeLogWatcher.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Neo.SmartContract.Testing
{
    [DebuggerDisplay("Count={Logs.Count}")]
    public sealed class RuntimeLogWatcher : IDisposable
    {
        private readonly TestEngine _testEngine;
        private readonly List<RuntimeLog> _logs = [];
        private bool _disposed;

        /// <summary>
        /// Captured runtime logs.
        /// </summary>
        public IReadOnlyList<RuntimeLog> Logs => _logs;

        /// <summary>
        /// Clear captured runtime logs.
        /// </summary>
        public void Reset()
        {
            _logs.Clear();
        }

        /// <summary>
        /// Constructor of RuntimeLogWatcher.
        /// </summary>
        /// <param name="engine">Test engine.</param>
        public RuntimeLogWatcher(TestEngine engine)
        {
            _testEngine = engine;
            _testEngine.OnRuntimeLog += OnRuntimeLog;
        }

        private void OnRuntimeLog(UInt160 sender, string message)
        {
            _logs.Add(new RuntimeLog(sender, message));
        }

        /// <summary>
        /// Stop capturing runtime logs.
        /// </summary>
        public void Dispose()
        {
            if (_disposed) return;

            _testEngine.OnRuntimeLog -= OnRuntimeLog;
            _disposed = true;
        }
    }
}

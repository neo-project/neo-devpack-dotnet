// Copyright (C) 2015-2026 The Neo Project.
//
// NotificationWatcher.cs file belongs to the neo project and is free
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
    [DebuggerDisplay("Count={Notifications.Count}")]
    public sealed class NotificationWatcher : IDisposable
    {
        private readonly TestEngine _testEngine;
        private readonly List<RuntimeNotification> _notifications = [];
        private bool _disposed;

        /// <summary>
        /// Captured runtime notifications.
        /// </summary>
        public IReadOnlyList<RuntimeNotification> Notifications => _notifications;

        /// <summary>
        /// Clear captured runtime notifications.
        /// </summary>
        public void Reset()
        {
            _notifications.Clear();
        }

        /// <summary>
        /// Constructor of NotificationWatcher.
        /// </summary>
        /// <param name="engine">Test engine.</param>
        public NotificationWatcher(TestEngine engine)
        {
            _testEngine = engine;
            _testEngine.OnRuntimeNotify += OnRuntimeNotify;
        }

        private void OnRuntimeNotify(UInt160 sender, string eventName, Neo.VM.Types.Array state)
        {
            _notifications.Add(new RuntimeNotification(sender, eventName, state));
        }

        /// <summary>
        /// Stop capturing runtime notifications.
        /// </summary>
        public void Dispose()
        {
            if (_disposed) return;

            _testEngine.OnRuntimeNotify -= OnRuntimeNotify;
            _disposed = true;
        }
    }
}

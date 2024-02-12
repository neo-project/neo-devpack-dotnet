using Neo.SmartContract.Testing.Extensions;
using Neo.VM;
using Neo.VM.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Neo.SmartContract.Testing
{
    public class SmartContract
    {
        internal readonly TestEngine Engine;
        private readonly Type _contractType;
        private readonly Dictionary<string, FieldInfo?> _notifyCache = new();

        public delegate void OnRuntimeLogDelegate(string message);
        public event OnRuntimeLogDelegate? OnRuntimeLog;

        /// <summary>
        /// Contract hash
        /// </summary>
        public UInt160 Hash { get; }

        /// <summary>
        /// Storage for this contract
        /// </summary>
        public SmartContractStorage Storage { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="initialize">Initialize object</param>
        protected SmartContract(SmartContractInitialize initialize)
        {
            Engine = initialize.Engine;
            Hash = initialize.Hash;
            Storage = new SmartContractStorage(this, initialize.ContractId);
            _contractType = GetType().BaseType ?? GetType(); // Mock
        }

        /// <summary>
        /// Invoke to NeoVM
        /// </summary>
        /// <param name="methodName">Method name</param>
        /// <param name="args">Arguments</param>
        /// <returns>Object</returns>
        internal StackItem Invoke(string methodName, params object[] args)
        {
            // Compose script

            using ScriptBuilder script = new();
            script.EmitDynamicCall(Hash, methodName, args);

            // Execute

            return Engine.Execute(script.ToArray());
        }

        /// <summary>
        /// Invoke OnRuntimeLog
        /// </summary>
        /// <param name="message">Message</param>
        internal void InvokeOnRuntimeLog(string message)
        {
            OnRuntimeLog?.Invoke(message);
        }

        /// <summary>
        /// Invoke on notify
        /// </summary>
        /// <param name="eventName">Event name</param>
        /// <param name="state">State</param>
        internal void InvokeOnNotify(string eventName, VM.Types.Array state)
        {
            if (!_notifyCache.TryGetValue(eventName, out var evField))
            {
                var ev = _contractType.GetEvent(eventName);
                if (ev is null)
                {
                    ev = _contractType.GetEvents().FirstOrDefault(u => u.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName == eventName);
                    if (ev is null)
                    {
                        _notifyCache[eventName] = null;
                        return;
                    }
                }

                _notifyCache[eventName] = evField = _contractType.GetField(ev.Name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField);
            }

            // Not found
            if (evField is null) return;
            if (evField.GetValue(this) is not Delegate del) return;

            // Avoid parse if is not needed

            var invocations = del.GetInvocationList();
            if (invocations.Length == 0) return;

            // Invoke

            var args = state.ConvertTo(del.Method.GetParameters());

            foreach (var handler in invocations)
            {
                handler.Method.Invoke(handler.Target, args);
            }
        }
    }
}

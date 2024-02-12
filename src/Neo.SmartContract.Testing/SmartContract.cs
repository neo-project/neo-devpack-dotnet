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
        private readonly Type ContractType;
        private readonly Dictionary<string, FieldInfo?> NotifyCache = new();

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
            ContractType = GetType().BaseType ?? GetType(); // Mock
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
            if (!NotifyCache.TryGetValue(eventName, out var evField))
            {
                var ev = ContractType.GetEvent(eventName);
                if (ev is null)
                {
                    ev = ContractType.GetEvents().FirstOrDefault(u => u.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName == eventName);
                    if (ev is null)
                    {
                        NotifyCache[eventName] = null;
                        return;
                    }
                }

                NotifyCache[eventName] = evField = ContractType.GetField(ev.Name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField);
            }

            // Not found
            if (evField is null) return;

            var del = evField.GetValue(this) as Delegate;
            if (del is null) return;

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

using Neo.SmartContract.Testing.Extensions;
using Neo.VM;
using Neo.VM.Types;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Neo.SmartContract.Testing
{
    public class SmartContract
    {
        internal readonly TestEngine Engine;

        public delegate void delOnLog(string message);
        public event delOnLog? OnRuntimeLog;

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
            Engine.Transaction.Script = script.ToArray(); // Store the script in the current transaction

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
            var type = GetType().BaseType ?? GetType(); // Mock
            var ev = type.GetEvent(eventName);
            if (ev is null)
            {
                ev = type.GetEvents().FirstOrDefault(u => u.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName == eventName);
                if (ev is null) return;
            }

            var evField = type.GetField(ev.Name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField);
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

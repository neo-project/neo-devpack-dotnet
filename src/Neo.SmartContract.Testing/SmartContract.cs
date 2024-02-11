using Neo.SmartContract.Testing.Extensions;
using Neo.VM;
using Neo.VM.Types;
using System;
using System.Reflection;

namespace Neo.SmartContract.Testing
{
    public class SmartContract
    {
        private readonly TestEngine _engine;

        public delegate void delOnLog(string message);
        public event delOnLog? OnLog;

        /// <summary>
        /// Contract hash
        /// </summary>
        public UInt160 Hash { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="testEngine">TestEngine</param>
        /// <param name="hash">Contract hash</param>
        protected SmartContract(TestEngine testEngine, UInt160 hash)
        {
            _engine = testEngine;
            Hash = hash;
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
            _engine.Transaction.Script = script.ToArray(); // Store the script in the current transaction

            return _engine.Execute(script.ToArray());
        }

        /// <summary>
        /// OnLog
        /// </summary>
        /// <param name="message">Message</param>
        internal void InvokeOnLog(string message)
        {
            OnLog?.Invoke(message);
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
            if (ev is null) return;

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
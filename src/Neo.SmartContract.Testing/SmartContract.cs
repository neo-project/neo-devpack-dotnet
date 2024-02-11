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

            // Execute in neo VM

            var snapshot = _engine.Storage.Snapshot.CreateSnapshot();

            using var engine = ApplicationEngine.Create(TriggerType.Application,
                _engine.Transaction, snapshot, _engine.CurrentBlock, _engine.ProtocolSettings, _engine.Gas);

            engine.LoadScript(script.ToArray());

            if (engine.Execute() != VMState.HALT)
            {
                throw engine.FaultException ?? new Exception($"Error while executing {methodName}");
            }

            snapshot.Commit();

            // Return

            if (engine.ResultStack.Count == 0) return StackItem.Null;
            return engine.ResultStack.Pop();
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

            // Invoke

            var args = Convert(state, del.Method.GetParameters());

            foreach (var handler in del.GetInvocationList())
            {
                handler.Method.Invoke(handler.Target, args);
            }
        }

        private static object?[]? Convert(VM.Types.Array state, ParameterInfo[] parameterInfos)
        {
            if (parameterInfos.Length > 0)
            {
                object?[] args = new object[parameterInfos.Length];

                for (int x = 0; x < parameterInfos.Length; x++)
                {
                    args[x] = state[x].ConvertTo(parameterInfos[x].ParameterType);
                }

                return args;
            }

            return null;
        }
    }
}

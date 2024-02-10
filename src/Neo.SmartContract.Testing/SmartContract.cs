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
            var type = GetType();
            var ev = GetType().GetEvent(eventName);
            if (ev is null) return;

            var evField = type.GetField(eventName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static);
            if (evField is null) return;

            var evDel = evField.GetValue(this) as Delegate;
            if (evDel is null) return;

            // Invoke

            evDel.DynamicInvoke(Convert(state, evDel.Method.GetParameters()));
        }

        private object?[]? Convert(VM.Types.Array state, ParameterInfo[] parameterInfos)
        {
            if (parameterInfos.Length > 0)
            {
                object?[] args = new object[parameterInfos.Length];

                for (int x = 0; x < parameterInfos.Length; x++)
                {
                    args[x] = state[0].ConvertTo(parameterInfos[x].ParameterType);
                }
            }

            return null;
        }
    }
}

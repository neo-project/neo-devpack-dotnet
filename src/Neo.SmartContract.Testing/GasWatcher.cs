using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Neo.SmartContract.Testing
{
    [DebuggerDisplay("Value={Value}")]
    public class GasWatcher : IDisposable
    {
        private readonly TestEngine _testEngine;

        /// <summary>
        /// Gas Consumed
        /// </summary>
        public long Value { get; set; } = 0;

        /// <summary>
        /// Set gas consumed to 0
        /// </summary>
        public void Reset()
        {
            Value = 0;
        }

        /// <summary>
        /// Csontructor
        /// </summary>
        /// <param name="engine">Test engine</param>
        public GasWatcher(TestEngine engine)
        {
            _testEngine = engine;
            _testEngine._gasWatchers.Add(this);
        }

        /// <summary>
        /// Free resources
        /// </summary>
        public void Dispose()
        {
            _testEngine._gasWatchers.Remove(this);
        }

        #region Operators

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator long(GasWatcher value) => value.Value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GasWatcher operator +(GasWatcher a, long b)
        {
            a.Value += b;
            return a;
        }

        #endregion
    }
}

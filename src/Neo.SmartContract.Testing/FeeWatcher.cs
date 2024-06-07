using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Neo.SmartContract.Testing
{
    [DebuggerDisplay("Value={Value}")]
    public class FeeWatcher : IDisposable
    {
        private readonly TestEngine _testEngine;

        /// <summary>
        /// Fee Consumed (In the unit of datoshi, 1 datoshi = 1e-8 GAS)
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
        public FeeWatcher(TestEngine engine)
        {
            _testEngine = engine;
            _testEngine._feeWatchers.Add(this);
        }

        /// <summary>
        /// Free resources
        /// </summary>
        public void Dispose()
        {
            _testEngine._feeWatchers.Remove(this);
        }

        #region Operators

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator long(FeeWatcher value) => value.Value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FeeWatcher operator +(FeeWatcher a, long b)
        {
            a.Value += b;
            return a;
        }

        #endregion
    }
}

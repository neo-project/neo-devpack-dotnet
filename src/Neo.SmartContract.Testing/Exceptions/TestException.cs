using Neo.VM;
using System;

namespace Neo.SmartContract.Testing.Exceptions
{
    public class TestException : Exception
    {
        /// <summary>
        /// State
        /// </summary>
        public VMState State { get; }

        /// <summary>
        /// Current context
        /// </summary>
        public ExecutionContext? CurrentContext { get; }

        /// <summary>
        /// Invocation Stack
        /// </summary>
        public ExecutionContext[] InvocationStack { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="engine">Test engine</param>
        internal TestException(TestingApplicationEngine engine) : base(
            engine.FaultException?.Message ?? $"Error while executing the script",
            engine.FaultException ?? new Exception($"Error while executing the script"))
        {
            State = engine.State;
            CurrentContext = engine.CurrentContext;
            InvocationStack = engine.InvocationStack.ToArray();
        }
    }
}

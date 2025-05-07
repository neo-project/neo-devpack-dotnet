using Neo.VM;
using System;

namespace Neo.SmartContract.Fuzzer
{
    /// <summary>
    /// Extensions for the ApplicationEngine class.
    /// </summary>
    public static class ApplicationEngineExtensions
    {
        /// <summary>
        /// Event that is triggered when a witness check is performed.
        /// </summary>
        public static event EventHandler<CheckWitnessEventArgs>? CheckWitness;

        /// <summary>
        /// Event that is triggered when a syscall is performed.
        /// </summary>
        public static event EventHandler<SysCallEventArgs>? OnSysCall;

        /// <summary>
        /// Event that is triggered when a VM step is performed.
        /// </summary>
        public static event EventHandler<StepEventArgs>? OnStep;

        /// <summary>
        /// Raises the CheckWitness event.
        /// </summary>
        /// <param name="hash">The hash that was checked.</param>
        /// <param name="result">The result of the check.</param>
        public static void RaiseCheckWitness(UInt160 hash, bool result)
        {
            CheckWitness?.Invoke(null, new CheckWitnessEventArgs(hash, result));
        }

        /// <summary>
        /// Raises the OnSysCall event.
        /// </summary>
        /// <param name="method">The syscall method that was called.</param>
        public static void RaiseOnSysCall(string method)
        {
            OnSysCall?.Invoke(null, new SysCallEventArgs(method));
        }

        /// <summary>
        /// Raises the OnStep event.
        /// </summary>
        /// <param name="opCode">The VM opcode that was executed.</param>
        /// <param name="instructionPointer">The current instruction pointer.</param>
        public static void RaiseOnStep(OpCode opCode, int instructionPointer)
        {
            OnStep?.Invoke(null, new StepEventArgs(opCode, instructionPointer));
        }
    }
}

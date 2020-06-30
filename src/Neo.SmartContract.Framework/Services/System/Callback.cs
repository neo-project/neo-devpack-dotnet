using System;

namespace Neo.SmartContract.Framework.Services.System
{
    public class Callback
    {
        [Syscall("System.Callback.Invoke")]
        public extern object Invoke(object[] args);

        // Static

        [OpCode(OpCode.PUSH0)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Callback.Create")]
        public static extern Callback Create<TResult>(Func<TResult> func);

        [OpCode(OpCode.PUSH1)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Callback.Create")]
        public static extern Callback Create<T, TResult>(Func<T, TResult> func);

        [OpCode(OpCode.PUSH2)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Callback.Create")]
        public static extern Callback Create<T1, T2, TResult>(Func<T1, T2, TResult> func);

        [OpCode(OpCode.PUSH3)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Callback.Create")]
        public static extern Callback Create<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> func);

        [OpCode(OpCode.PUSH4)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Callback.Create")]
        public static extern Callback Create<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> func);

        [OpCode(OpCode.PUSH5)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Callback.Create")]
        public static extern Callback Create<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, TResult> func);

        [OpCode(OpCode.PUSH6)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Callback.Create")]
        public static extern Callback Create<T1, T2, T3, T4, T5, T6, TResult>(Func<T1, T2, T3, T4, T5, T6, TResult> func);

        [OpCode(OpCode.PUSH7)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Callback.Create")]
        public static extern Callback Create<T1, T2, T3, T4, T5, T6, T7, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, TResult> func);

        [OpCode(OpCode.PUSH8)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Callback.Create")]
        public static extern Callback Create<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> func);

        [OpCode(OpCode.PUSH9)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Callback.Create")]
        public static extern Callback Create<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> func);

        [OpCode(OpCode.PUSH10)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Callback.Create")]
        public static extern Callback Create<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> func);

        [OpCode(OpCode.PUSH11)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Callback.Create")]
        public static extern Callback Create<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> func);

        [OpCode(OpCode.PUSH12)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Callback.Create")]
        public static extern Callback Create<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> func);

        [OpCode(OpCode.PUSH13)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Callback.Create")]
        public static extern Callback Create<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> func);

        [OpCode(OpCode.PUSH14)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Callback.Create")]
        public static extern Callback Create<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> func);

        [OpCode(OpCode.PUSH15)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Callback.Create")]
        public static extern Callback Create<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> func);

        [OpCode(OpCode.PUSH16)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Callback.Create")]
        public static extern Callback Create<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> func);

        [Syscall("System.Callback.CreateFromMethod")]
        public static extern Callback CreateFromMethod(byte[] hash, string method);

        [Syscall("System.Callback.CreateFromSyscall")]
        public static extern Callback CreateFromSyscall(SyscallCallback method);
    }
}

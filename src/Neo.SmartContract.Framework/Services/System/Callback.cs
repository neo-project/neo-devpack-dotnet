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
        public static extern Callback Create(Action action);

        [OpCode(OpCode.PUSH1)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Callback.Create")]
        public static extern Callback Create(Func<object, object> action);

        [Syscall("System.Callback.CreateFromMethod")]
        public static extern Callback CreateFromMethod(byte[] hash, string method);

        [Syscall("System.Callback.CreateFromSyscall")]
        public static extern Callback CreateFromSyscall(SyscallCallback method);
    }
}

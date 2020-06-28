using Neo.SmartContract.Framework.Services.Neo;
using Neo.SmartContract.Framework.Services.System;
using System;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Neo.Compiler.MSIL.TestClasses
{
    public class Contract_Callback : SmartContract.Framework.SmartContract
    {
        public static int test() { return 123; }

        public static object test2(object args)
        {
            return 123;
        }

        public static object createFromMethod(byte[] hash, string method)
        {
            return Callback.CreateFromMethod(hash, method);
        }

        public static object createAndCallFromMethod(byte[] hash, string method)
        {
            return Callback.CreateFromMethod(hash, method).Invoke(new object[0]);
        }

        public static object create()
        {
            var action = new Func<object, object>(test2);
            return Callback.Create(action);
        }

        public static object createAndCall()
        {
            var action = new Func<object, object>(test2);
            var callback = Callback.Create(action);

            return callback.Invoke(new object[] { null });
        }

        public static object createFromSyscall(uint method)
        {
            return Callback.CreateFromSyscall((SyscallCallback)method);
        }

        public static object createAndCallFromSyscall(uint method)
        {
            return Callback.CreateFromSyscall((SyscallCallback)method).Invoke(new object[0]);
        }
    }
}

using Neo.SmartContract.Framework.Services.Neo;
using Neo.SmartContract.Framework.Services.System;
using System;

namespace Neo.Compiler.MSIL.TestClasses
{
    public class Contract_Callback : SmartContract.Framework.SmartContract
    {
        public static int test() { return 123; }

        public static void test2(object args)
        {
            Runtime.Notify("test2", args);
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
            var action = new Action<object>(test2);
            return Callback.Create(action);
        }

        public static object createAndCall()
        {
            var action = new Action<object>(test2);
            var callback = Callback.Create(action);

            callback.Invoke(new object[] { null });
            return callback;
        }

        public static object createFromSyscall(string method)
        {
            return Callback.CreateFromSyscall(method);
        }

        public static object createAndCallFromSyscall(string method)
        {
            return Callback.CreateFromSyscall(method).Invoke(new object[0]);
        }
    }
}

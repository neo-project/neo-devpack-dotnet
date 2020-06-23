using Neo.SmartContract.Framework.Services.Neo;
using Neo.SmartContract.Framework.Services.System;
using System;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Neo.Compiler.MSIL.TestClasses
{
    public class Contract_Callback : SmartContract.Framework.SmartContract
    {
        [DisplayName("event")]
        public static event Action<string> notify;

        public static int test() { return 123; }

        public static object test2(object args)
        {
            return 123;
        }

        public static void test3()
        {
            notify("test3");
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

        public static object createAction()
        {
            var action = new Action(test3);
            return Callback.Create(action);
        }

        public static object createActionAndCall()
        {
            var action = new Action(test3);
            var callback = Callback.Create(action);

            return callback.Invoke(new object[0]);
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

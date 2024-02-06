using System;
using System.Reflection;
using Neo.Compiler;
using Array = Neo.VM.Types.Array;

namespace Neo.SmartContract.Framework.UnitTests.Utils
{
    static class Extensions
    {
        public static readonly string TestContractRoot = "../../../../Neo.SmartContract.Framework.TestContracts/";

        public static void ClearNotifications(this ApplicationEngine engine)
        {
            typeof(ApplicationEngine).GetField("notifications", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(engine, null);
        }

        public static void SendTestNotification(this ApplicationEngine engine, UInt160 hash, string eventName, Array state)
        {
            typeof(ApplicationEngine).GetMethod("SendNotification", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(engine, new object[] { hash, eventName, state });
        }

        public static CompilationContext AddEntryScript(this TestEngine.TestEngine engin, params Type[] files)
        {
            var sourceFiles = TestEngine.Extensions.GetFiles(TestContractRoot, files).ToArray();
            return engin.AddEntryScript(sourceFiles);
        }

        public static CompilationContext AddEntryScript<T>(this TestEngine.TestEngine engine)
        {
            return engine.AddEntryScript(typeof(T));
        }
    }
}

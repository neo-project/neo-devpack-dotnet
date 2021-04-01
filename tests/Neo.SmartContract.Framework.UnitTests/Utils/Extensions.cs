using Neo.VM.Types;
using System.Reflection;

namespace Neo.SmartContract.Framework.UnitTests.Utils
{
    static class Extensions
    {
        public static void ClearNotifications(this ApplicationEngine engine)
        {
            typeof(ApplicationEngine).GetField("notifications", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(engine, null);
        }

        public static void SendTestNotification(this ApplicationEngine engine, UInt160 hash, string eventName, Array state)
        {
            typeof(ApplicationEngine).GetMethod("SendNotification", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(engine, new object[] { hash, eventName, state });
        }
    }
}

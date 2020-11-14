using Neo.SmartContract.Framework.Services.Neo;

namespace Neo.Compiler.MSIL.TestClasses
{
    public class Contract_Runtime : SmartContract.Framework.SmartContract
    {
        public static uint GetInvocationCounter()
        {
            return Runtime.InvocationCounter;
        }

        public static ulong GetTime()
        {
            return Runtime.Time;
        }

        public static long GetGasLeft()
        {
            return Runtime.GasLeft;
        }

        public static string GetPlatform()
        {
            return Runtime.Platform;
        }

        public static byte GetTrigger()
        {
            return (byte)Runtime.Trigger;
        }

        public static void Log(string message)
        {
            Runtime.Log(message);
        }

        public static bool CheckWitness(UInt160 hash)
        {
            return Runtime.CheckWitness(hash);
        }

        public static int GetNotificationsCount(UInt160 hash)
        {
            var notifications = Runtime.GetNotifications(hash);
            return notifications.Length;
        }

        public static int GetAllNotifications()
        {
            int sum = 0;
            var notifications = Runtime.GetNotifications();

            for (int x = 0; x < notifications.Length; x++)
            {
                var notify = notifications[x];
                sum += (int)notify.State[0];
            }

            return sum;
        }

        public static int GetNotifications(UInt160 hash)
        {
            int sum = 0;
            var notifications = Runtime.GetNotifications(hash);

            for (int x = 0; x < notifications.Length; x++)
            {
                sum += (int)notifications[x].State[0];
            }

            return sum;
        }
    }
}

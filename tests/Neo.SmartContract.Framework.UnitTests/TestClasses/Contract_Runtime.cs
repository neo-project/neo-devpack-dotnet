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

        public static string GetPlatform()
        {
            return Runtime.Platform;
        }

        public static TriggerType GetTrigger()
        {
            return Runtime.Trigger;
        }

        public static void Log(string message)
        {
            Runtime.Log(message);
        }

        public static void Notify(string message)
        {
            Runtime.Notify(message);
        }

        public static bool CheckWitness(byte[] hashOrPubkey)
        {
            return Runtime.CheckWitness(hashOrPubkey);
        }

        public static int GetNotificationsCount(byte[] hash)
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
                sum += (int)notify.State;
            }

            return sum;
        }

        public static int GetNotifications(byte[] hash)
        {
            int sum = 0;
            var notifications = Runtime.GetNotifications(hash);

            for (int x = 0; x < notifications.Length; x++)
            {
                var notify = notifications[x];

                // Check that the hash is working well

                for (int y = 0; y < notify.ScriptHash.Length; y++)
                {
                    if (notify.ScriptHash[y] != hash[y]) return int.MinValue;
                }

                sum += (int)notify.State;
            }

            return sum;
        }
    }
}

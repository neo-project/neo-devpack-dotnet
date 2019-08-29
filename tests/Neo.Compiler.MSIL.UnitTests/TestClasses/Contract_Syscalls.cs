namespace Neo.Compiler.MSIL.TestClasses
{
    public class Contract_Syscalls : SmartContract.Framework.SmartContract
    {
        public static uint GetInvocationCounter()
        {
            return SmartContract.Framework.Services.Neo.Runtime.InvocationCounter;
        }

        public static int GetNotificationsCount(byte[] hash)
        {
            var notifications = SmartContract.Framework.Services.Neo.Runtime.GetNotifications(hash);
            return notifications.Length;
        }

        public static int GetNotifications(byte[] hash)
        {
            int sum = 0;
            var notifications = SmartContract.Framework.Services.Neo.Runtime.GetNotifications(hash);

            for (int x = 0; x < notifications.Length; x++)
            {
                var notify = notifications[x];

                if (hash.Length != 0)
                {
                    // Check that the hash is working well

                    var hashNot = notify.ScriptHash;
                    for (int y = 0; y < hashNot.Length; y++)
                    {
                        if (hashNot[y] != hash[y]) return int.MinValue;
                    }
                }

                sum += (int)notify.State;
            }

            return sum;
        }
    }
}

using System.Numerics;
using Neo.SmartContract.Framework.Services;

namespace Neo.SmartContract.Framework.UnitTests.TestClasses
{
    public class Contract_Runtime : SmartContract
    {
        public static uint GetInvocationCounter()
        {
            return Runtime.InvocationCounter;
        }

        public static ulong GetTime()
        {
            return Runtime.Time;
        }

        public static BigInteger GetRandom()
        {
            return Runtime.GetRandom();
        }

        public static long GetGasLeft()
        {
            return Runtime.GasLeft;
        }

        public static string GetPlatform()
        {
            return Runtime.Platform;
        }

        public static uint GetNetwork()
        {
            return Runtime.GetNetwork();
        }

        public static uint GetAddressVersion()
        {
            return Runtime.AddressVersion;
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
                sum += (int)notify.State[2];
            }

            return sum;
        }

        public static int GetNotifications(UInt160 hash)
        {
            int sum = 0;
            var notifications = Runtime.GetNotifications(hash);

            for (int x = 0; x < notifications.Length; x++)
            {
                sum += (int)notifications[x].State[2];
            }

            return sum;
        }

        public static UInt256 GetTransactionHash()
        {
            var tx = Runtime.Transaction;
            return tx!.Hash;
        }

        public static byte GetTransactionVersion()
        {
            var tx = Runtime.Transaction;
            return tx!.Version;
        }

        public static uint GetTransactionNonce()
        {
            var tx = Runtime.Transaction;
            return tx!.Nonce;
        }

        public static UInt160 GetTransactionSender()
        {
            var tx = Runtime.Transaction;
            return tx!.Sender;
        }

        public static object GetTransaction()
        {
            var tx = Runtime.Transaction;
            return tx;
        }

        public static long GetTransactionSystemFee()
        {
            var tx = Runtime.Transaction;
            return tx!.SystemFee;
        }

        public static long GetTransactionNetworkFee()
        {
            var tx = Runtime.Transaction;
            return tx!.NetworkFee;
        }

        public static uint GetTransactionValidUntilBlock()
        {
            var tx = Runtime.Transaction;
            return tx!.ValidUntilBlock;
        }

        public static ByteString GetTransactionScript()
        {
            var tx = Runtime.Transaction;
            return tx!.Script;
        }

        public static int DynamicSum(int a, int b)
        {
            ByteString script = (ByteString)new byte[] { 0x9E }; // ADD
            return (int)Runtime.LoadScript(script, CallFlags.All, new object[] { a, b });
        }
    }
}

using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;

namespace Neo.Compiler.MSIL.TestClasses
{
    public class Contract_ByteArray : SmartContract.Framework.SmartContract
    {
        // Support in the future, when our vm removed Buffer
        //public static byte[] TestByteSet()
        //{
        //    var a = new byte[] { 0x01, 0x02, 0x03 };
        //    a[1] = 0x04;
        //    return a;
        //}

        public static byte[] TestByteReverse()
        {
            var a = new byte[] { 0x01, 0x02, 0x03 };
            return a.Reverse();
        }

        public static byte[] TestByteCocat()
        {
            Runtime.Notify((new byte[] { 0x12, 0x23, 0x32 }).Concat(new byte[] { 0x55, 0x23 }.Concat(new byte[] { 0x01, 0x02 })));
            return (new byte[] { 0x12, 0x23, 0x32 }).Concat(new byte[] { 0x55, 0x23 }.Concat(new byte[] { 0x01, 0x02 }));
        }

        public static byte[] TestByteRange()
        {
            Runtime.Notify((new byte[] { 0x12, 0x23, 0x32 }).Range(0, 2));
            return (new byte[] { 0x12, 0x23, 0x32 }).Range(0, 2);
        }

        public static byte[] TestByteTake()
        {
            Runtime.Notify((new byte[] { 0x12, 0x23, 0x32 }).Take(1));
            return (new byte[] { 0x12, 0x23, 0x32 }).Take(1);
        }

        public static byte[] TestByteLast()
        {
            Runtime.Notify((new byte[] { 0x12, 0x23, 0x32 }).Last(1));
            return (new byte[] { 0x12, 0x23, 0x32 }).Last(1);
        }
    }
}

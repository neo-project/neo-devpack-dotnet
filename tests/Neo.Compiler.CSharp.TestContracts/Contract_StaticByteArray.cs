using System;
using System.ComponentModel;

namespace Neo.Compiler.CSharp.UnitTests.TestClasses
{
    public class Contract_StaticByteArray : SmartContract.Framework.SmartContract
    {
        [DisplayName("TestEvent")]
        public static event Action<byte[]> OnEvent;

        static byte[] NeoToken = new byte[] { 0x89, 0x77, 0x20, 0xd8, 0xcd, 0x76, 0xf4, 0xf0, 0x0a, 0xbf, 0xa3, 0x7c, 0x0e, 0xdd, 0x88, 0x9c, 0x20, 0x8f, 0xde, 0x9b };

        public static byte[] TestStaticByteArray()
        {
            return NeoToken;
        }
    }
}

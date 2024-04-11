using System.ComponentModel;

namespace Neo.Compiler.CSharp.UnitTests.TestClasses
{
    public class Contract_WrongEvent : SmartContract.Framework.SmartContract
    {
        public delegate int DecWithReturn(byte[] from, byte[] to, int amount);

        [DisplayName("transfer")]
        public static event DecWithReturn Transferred;

        public static void Main(string method, object[] args) { }
    }
}

using Neo.SmartContract.Framework;

namespace Neo.Compiler.CSharp.UnitTests.TestClasses
{
    public class Contract_Returns : SmartContract.Framework.SmartContract
    {
        public delegate void delOnSum(int total);
        public static event delOnSum OnSum;

        /// <summary>
        /// No return
        /// </summary>
        public static void Sum(int a, int b)
        {
            OnSum(a + b);
        }

        /// <summary>
        /// One return
        /// </summary>
        public static int Subtract(int a, int b)
        {
            return a - b;
        }

        /// <summary>
        /// Multiple returns
        /// </summary>
        public static (int, int) Div(int a, int b)
        {
            return ((a / b), (a % b));
        }

        /// <summary>
        /// Use the double return
        /// </summary>
        public static int Mix(int a, int b)
        {
            (int c, int d) = Div(a, b);

            return Subtract(c, d);
        }

        /// <summary>
        /// ByteString add
        /// </summary>
        public static ByteString ByteStringAdd(ByteString a, ByteString b)
        {
            return a + b;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace Neo.Compiler.MSIL.UnitTests.TestClasses
{
    class Contract_Returns : SmartContract.Framework.SmartContract
    {
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

        public static bool Bool(int i, int j)
        {
            var k = i != j && (i > 0 || j < 2);
            return k;
        }
    }
}

using System;
using System.ComponentModel;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests.TestClasses
{
    public class Contract_Foreach : SmartContract.Framework.SmartContract
    {
        public static int IntForeach()
        {
            int[] a = new int[] { 1, 2, 3, 4 };
            int sum = 0;
            foreach (var item in a)
            {
                sum += item;
            }
            return sum;
        }


        public static int IntForeachBreak(int breakIndex)
        {
            int[] a = new int[] { 1, 2, 3, 4 };
            int sum = 0;
            try
            {
                foreach (var item in a)
                {
                    if (breakIndex-- <= 0) break;
                    sum += item;
                }
            }
            catch { }
            return sum;
        }
    }
}

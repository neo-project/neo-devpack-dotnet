using System.Numerics;
using Neo.SmartContract.Framework;

namespace Neo.Compiler.CSharp.TestContracts
{
    public class Contract_Recursion : SmartContract.Framework.SmartContract
    {
        /// <summary>
        /// Example of a single method calling itself
        /// a!
        /// </summary>
        /// <param name="a">a>=0</param>
        /// <returns>a! == 1*2*3*...*a</returns>
        public static BigInteger Factorial(BigInteger a)
        {
            ExecutionEngine.Assert(a >= 0, "Minus number not supported");
            if (a >= 2) return a * Factorial(a - 1);
            return 1;
        }

        public struct HanoiStep
        {
            public BigInteger rod;
            public BigInteger src;
            public BigInteger dst;
        }

        /// <summary>
        /// Example of a single method calling itself for multiple times through different branches
        /// Move a stack of disks between three rods,
        /// following rules of only moving one disk at a time,
        /// and never placing a larger disk on top of a smaller disk.
        /// Smaller rod on the top has a smaller ID, starting from 1.
        /// </summary>
        /// <param name="n">Count of all disks</param>
        /// <param name="src">Id of source rod</param>
        /// <param name="aux">Id of auxiliary rod</param>
        /// <param name="dst">Id of destination rod</param>
        /// <returns>List[Move (diskId, fromRod, toRod)]</returns>
        public static List<HanoiStep>
            HanoiTower(BigInteger n, BigInteger src, BigInteger aux, BigInteger dst)
        {
            ExecutionEngine.Assert(n > 0, "Count of disks <= 0");
            List<HanoiStep> result;
            if (n == 1)
            {
                result = new();
                result.Add(new HanoiStep() { rod = 1, src = src, dst = dst });
                return result;
            }
            result = HanoiTower(n - 1, src, dst, aux);
            result.Add(new HanoiStep() { rod = n, src = src, dst = dst });
            foreach (var step in HanoiTower(n - 1, aux, src, dst))
                result.Add(step);
            return result;
        }

        public static bool Even(BigInteger n) => n == 0 ? true : Odd(n < 0 ? n + 1 : n - 1);
        public static bool Odd(BigInteger n) => n == 0 ? false : Even(n < 0 ? n + 1 : n - 1);
    }
}

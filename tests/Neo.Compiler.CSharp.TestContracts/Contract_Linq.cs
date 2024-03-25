using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Linq;

namespace Neo.Compiler.CSharp.UnitTests.TestClasses
{
    public class Contract_Linq : SmartContract.Framework.SmartContract
    {
        public static int AggregateSum(int[] array)
        {
            return array.Aggregate(0, (Sum, item) => Sum + item);
        }

        public static bool AllGreaterThanZero(int[] array)
        {
            return array.All(x => x > 0);
        }

        public static bool IsEmpty(int[] array)
        {
            return !array.Any();
        }

        public static bool AnyGreatThanZero(int[] array)
        {
            return array.Any(x => x > 0);
        }

        public static bool AnyGreatThan(int[] array, int target)
        {
            return array.Any(x => x > target);
        }

        public static int Average(int[] array)
        {
            return array.Average();
        }

        public static int AverageTwice(int[] array)
        {
            return array.Average(a => 2 * a);
        }

        public static object Count(int[] array)
        {
            return array.Count();
        }

        public static object CountGreatThanZero(int[] array)
        {
            return array.Count(x => x > 0);
        }

        public static bool Contains(int[] array, int target)
        {
            return array.Contains(target);
        }

        public static int FirstGreatThanZero(int[] array)
        {
            return array.FirstOrDefault(x => x > 0, -1);
        }

        public static object Skip(int[] array, int count)
        {
            return array.Skip(count);
        }

        public static object Sum(int[] array)
        {
            return array.Sum();
        }

        public static object SumTwice(int[] array)
        {
            return array.Sum(x => 2 * x);
        }

        public static object Take(int[] array, int count)
        {
            return array.Take(count);
        }

        public static object WhereGreaterThanZero(int[] array)
        {
            return array.Where(x => x > 0);
        }
    }
}

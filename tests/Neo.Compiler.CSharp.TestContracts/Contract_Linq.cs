using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Linq;

namespace Neo.Compiler.CSharp.TestContracts
{
    public class Contract_Linq : SmartContract.Framework.SmartContract
    {
        public class Person
        {
            public string Name { get; set; } = default!;
            public int Age { get; set; }
        }

        public struct PersonS
        {
            public string Name { get; set; }
            public int Age { get; set; }
        }

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

        public static bool AnyGreaterThanZero(int[] array)
        {
            return array.Any(x => x > 0);
        }

        public static bool AnyGreaterThan(int[] array, int target)
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

        public static int Count(int[] array)
        {
            return array.Count();
        }

        public static int CountGreaterThanZero(int[] array)
        {
            return array.Count(x => x > 0);
        }

        public static bool Contains(int[] array, int target)
        {
            return array.Contains(target);
        }
        public static bool ContainsText(string[] array, string target)
        {
            return array.Contains(target);
        }

        /// <summary>
        /// always false
        /// </summary>
        /// <param name="array"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool ContainsPerson(int[] array, int target)
        {
            var list = new List<Person>();
            foreach (var item in array)
            {
                list.Add(new Person { Name = item.ToString(), Age = item });
            }
            var t = new Person { Name = target.ToString(), Age = target };
            return list.Contains(t);
        }

        /// <summary>
        /// always true
        /// </summary>
        /// <param name="array"></param>
        /// <param name="targetIndex"></param>
        /// <returns></returns>
        public static bool ContainsPersonIndex(int[] array, int targetIndex)
        {
            var list = new List<Person>();
            foreach (var item in array)
            {
                list.Add(new Person { Name = item.ToString(), Age = item });
            }
            var t = list[targetIndex];
            return list.Contains(t);
        }

        /// <summary>
        /// compare value
        /// </summary>
        /// <param name="array"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool ContainsPersonS(int[] array, int target)
        {
            var list = new List<PersonS>();
            foreach (var item in array)
            {
                list.Add(new PersonS { Name = item.ToString(), Age = item });
            }
            var t = new PersonS { Name = target.ToString(), Age = target };
            return list.Contains(t);
        }

        public static int FirstGreaterThanZero(int[] array)
        {
            return array.FirstOrDefault(x => x > 0, -1);
        }
        public static object SelectTwice(int[] array)
        {
            return array.Select(x => x * 2);
        }

        public static object SelectPersonS(int[] array)
        {
            var list = new List<Person>();
            foreach (var item in array)
            {
                list.Add(new Person { Name = item.ToString(), Age = item });
            }
            return list.Select(p => new PersonS { Name = p.Name, Age = p.Age });
        }

        public static object Skip(int[] array, int count)
        {
            return array.Skip(count);
        }

        public static int Sum(int[] array)
        {
            return array.Sum();
        }

        public static int SumTwice(int[] array)
        {
            return array.Sum(x => 2 * x);
        }

        public static object Take(int[] array, int count)
        {
            return array.Take(count);
        }

        public static object ToMap(int[] array)
        {
            var list = array.Select(i => new PersonS { Name = i.ToString(), Age = i });
            return list.ToMap(p => p.Name, p => p);
        }

        public static object WhereGreaterThanZero(int[] array)
        {
            return array.Where(x => x > 0);
        }
    }
}

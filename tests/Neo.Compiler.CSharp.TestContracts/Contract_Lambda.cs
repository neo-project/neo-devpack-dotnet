using System;
using System.Numerics;
using System.Collections.Generic;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services;


namespace Neo.Compiler.CSharp.UnitTests.TestClasses
{
    public class Contract_Lambda : SmartContract.Framework.SmartContract
    {
        private const int z = 0;
        private static readonly int Z = 0;
        public static Predicate<int> isZero = k => k == Z;

        public static Predicate<int> isPositiveOdd = k => k > z && k % 2 == 1;

        public static Func<int, int, int> sum = (x, y) => x + y;


        public static bool CheckZero(int i)
        {

            return isZero(i);
        }

        public static object CheckZero2(int num)
        {
            return Invoke(isZero, num);
        }

        public static object CheckZero3(int num)
        {
            var zero = 0;
            return Invoke(n => n == zero, num);
        }


        public static bool CheckPositiveOdd(int i)
        {
            return isPositiveOdd(i);
        }



        public static object InvokeSum(int a, int b)
        {
            return sum(a, b);
        }

        public static object InvokeSum2(int a, int b)
        {
            int z = 1;
            return InvokeFunc((x, y) => x + y + z, a, b);
        }

        public static object Fibo(int c)
        {
            Func<int, int> fibonacci = null;
            fibonacci = n => n < 2 ? n : fibonacci(n - 1) + fibonacci(n - 2);
            return fibonacci(c);
        }

        public static object ChangeName(string name)
        {
            Func<string> a = () => name;
            name += " !!!";
            return a();
        }


        public static object ChangeName2(string name)
        {
            Func<string> a = () =>
            {
                name += " !!!";
                return name;
            };
            Func<string> b = () => name;
            a();
            return b();
        }

        /// <summary>
        /// Use last value in foreach, different from C# behavior
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static object ForEachVar(int[] array)
        {
            var list = new SmartContract.Framework.List<Func<int>>();
            foreach (var num in array)
            {
                //all num are regarded as the same local variable in foreach
                list.Add(() => num);
            }
            var result = new SmartContract.Framework.List<int>();
            foreach (var item in list)
            {
                result.Add(item());
            }
            return result;
        }

        /// <summary>
        /// Use last value in for, different from C# behavior
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static object ForVar(int[] array)
        {
            var list = new SmartContract.Framework.List<Func<int>>();
            for (int i = 0; i < array.Length; i++)
            {
                //all num are regarded as the same local variable in for
                var num = array[i];
                list.Add(() => num);
            }
            var result = new SmartContract.Framework.List<int>();
            foreach (var item in list)
            {
                result.Add(item());
            }
            return result;
        }

        public static bool AnyGreatThanZero(int[] array)
        {
            return Any(array, x => x > 0);
        }

        public static bool AnyGreatThan(int[] array, int target)
        {
            return Any(array, x => x > target);
        }

        public static SmartContract.Framework.List<int> WhereGreatThanZero(int[] array)
        {
            return Where(array, x => x > 0);
        }

        private static bool Any<T>(IEnumerable<T> array, Predicate<T> pre)
        {
            foreach (var i in array)
            {
                if (pre(i)) return true;
            }
            return false;
        }


        private static SmartContract.Framework.List<T> Where<T>(IEnumerable<T> array, Predicate<T> pre)
        {
            var list = new SmartContract.Framework.List<T>();
            foreach (var i in array)
            {
                if (pre(i)) { list.Add(i); }
            }
            return list;
        }

        private static object Invoke(Predicate<int> check, int para)
        {
            return check(para);
        }

        private static object InvokeFunc(Func<int, int, int> f, int p1, int p2)
        {
            return f(p1, p2);
        }
    }
}

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
namespace Neo.SmartContract.Analyzer.Sample
{
    public class Examples
    {
        public class NeoContractClass
        {
        }

        public void TestFloat()
        {
#pragma warning disable CS0168,CS0219
            float a = (int)(int)1.5;
#pragma warning restore CS0168,CS0219
        }

        public void TestDouble()
        {
#pragma warning disable CS0168,CS0219
            double a = (long)(long)(long)10;
#pragma warning restore CS0168,CS0219
        }

        public void TestDecimal()
        {
#pragma warning disable CS0168,CS0219
            decimal a = 10;
#pragma warning restore CS0168,CS0219
        }

        public void TestNewBigInteger()
        {
#pragma warning disable CS0168
            var a = 10;
#pragma warning restore CS0168
        }

        public void TestBigInteger()
        {
#pragma warning disable CS0168
            BigInteger a = 10;
            BigInteger b = 20;

            // Using each method from the _unsupportedBigIntegerMethods list
            var addResult = BigInteger.Add(a, b);
            var compareToResult = a.CompareTo(b);
            var divideResult = BigInteger.Divide(a, b);
            var divRemResult = BigInteger.DivRem(a, b, out BigInteger remainder);
            var equalsResult = a.Equals(b);
            var expResult = BigInteger.Pow(a, 2);
            var gcdResult = BigInteger.GreatestCommonDivisor(a, b);
            var logResult = (long)BigInteger.Log(a);
            var log10Result = (long)BigInteger.Log10(a);
            var maxResult = BigInteger.Max(a, b);
            var minResult = BigInteger.Min(a, b);
            var modPowResult = BigInteger.ModPow(a, b, b);
            var multiplyResult = BigInteger.Multiply(a, b);
            var negateResult = BigInteger.Negate(a);
            var parseResult = BigInteger.Parse("12345");
            var remainderResult = BigInteger.Remainder(a, b);
            var subtractResult = BigInteger.Subtract(a, b);
            var toByteArrayResult = a.ToByteArray();
            var toStringResult = a.ToString();
            var tryParseResult = BigInteger.TryParse("12345", out BigInteger tryParseOut);
            var isPowerOfTwo = a.IsPowerOfTwo;
#pragma warning restore CS0168
        }

        public void TestStringMethods()
        {
#pragma warning disable CS0168,CS0618
            string str1 = "Hello";
            string str2 = "World";
            char[] charArray = { 'H', 'e', 'l', 'l', 'o' };
            string[] stringArray = { "Hello", "World" };

            // Using each method from the _unsupportedStringMethods list
            var cloneResult = str1.Clone();
            var compareResult = string.Compare(str1, str2);
            var compareOrdinalResult = string.CompareOrdinal(str1, str2);
            var compareToResult = str1.CompareTo(str2);
            var concatResult = string.Concat(str1, str2);
            var containsResult = str1.Contains(str2);
            var copyResult = string.Copy(str1);
            str1.CopyTo(0, charArray, 0, str1.Length);
            var endsWithResult = str1.EndsWith("o");
            var equalsResult = str1.Equals(str2);
            var formatResult = string.Format("{0} {1}", str1, str2);
            var enumeratorResult = str1.GetEnumerator();
            var getHashCodeResult = str1.GetHashCode();
            var getTypeResult = str1.GetType();
            var getTypeCodeResult = Type.GetTypeCode(str1.GetType());
            var indexOfResult = str1.IndexOf('o');
            var indexOfAnyResult = str1.IndexOfAny(charArray);
            var insertResult = str1.Insert(0, str2);
            var internResult = string.Intern(str1);
            var isInternedResult = string.IsInterned(str1);
            var isNormalizedResult = str1.IsNormalized();
            var joinResult = string.Join(", ", stringArray);
            var lastIndexOfResult = str1.LastIndexOf('o');
            var lastIndexOfAnyResult = str1.LastIndexOfAny(charArray);
            var normalizeResult = str1.Normalize();
            var padLeftResult = str1.PadLeft(10);
            var padRightResult = str1.PadRight(10);
            var removeResult = str1.Remove(1);
            var replaceResult = str1.Replace('l', 'x');
            var splitResult = str1.Split('l');
            var startsWithResult = str1.StartsWith("He");
            var substringResult = str1.Substring(1);
            var toCharArrayResult = str1.ToCharArray();
            var toLowerResult = str1.ToLower();
            var toLowerInvariantResult = str1.ToLowerInvariant();
            var toStringResult = str1.ToString();
            var toUpperResult = str1.ToUpper();
            var toUpperInvariantResult = str1.ToUpperInvariant();
            var trimResult = str1.Trim();
            var trimEndResult = str1.TrimEnd('o');
            var trimStartResult = str1.TrimStart('H');
#pragma warning restore CS0168,CS0618
        }

        public void TestMathMethods()
        {
#pragma warning disable CS0168
            int a = 15;
            long b = 25;
            int intResult;
            long longResult;

            // Using each method from the _unsupportedMathMethods list
            var absIntResult = (long)Math.Abs(-a);
            var absLongResult = (long)Math.Abs(-b);
            var acosResult = (long)Math.Acos(a / 100.0); // Casting to double
            var asinResult = (long)Math.Asin(a / 100.0); // Casting to double
            var atanResult = (long)Math.Atan(a / 100.0); // Casting to double
            var atan2Result = (long)Math.Atan2(a, b);    // Casting to double
            var bigMulResult = (long)Math.BigMul(a, 2);
            var ceilingResult = (long)Math.Ceiling(a / 100.0); // Casting to double
            var cosResult = (long)Math.Cos(a / 100.0);         // Casting to double
            var coshResult = (long)Math.Cosh(a / 100.0);       // Casting to double
            var divRemResult = (long)Math.DivRem(a, 2, out intResult);
            var expResult = (long)Math.Exp(a / 100.0);                // Casting to double
            var floorResult = (long)Math.Floor(a / 100.0);            // Casting to double
            var ieeeRemainderResult = (long)Math.IEEERemainder(a, b); // Casting to double
            var logResult = (long)Math.Log(a / 100.0);                // Casting to double
            var log10Result = (long)Math.Log10(a / 100.0);            // Casting to double
            var powResult = (long)Math.Pow(a, 2);                     // Casting to double
            var roundResult = (long)Math.Round(a / 100.0);            // Casting to double
            var sinResult = (long)Math.Sin(a / 100.0);                // Casting to double
            var sinhResult = (long)Math.Sinh(a / 100.0);              // Casting to double
            var sqrtResult = (long)Math.Sqrt(a);                      // Casting to double
            var tanResult = (long)Math.Tan(a / 100.0);                // Casting to double
            var tanhResult = (long)Math.Tanh(a / 100.0);              // Casting to double
            var truncateResult = (long)Math.Truncate(a / 100.0);      // Casting to double
#pragma warning restore CS0168,CS0219
        }

        public void TestLinqOperations()
        {
#pragma warning disable CS0168
            var testData = new List<int> { 1, 4, 2, 9, 5, 8 };
            var filtered = testData.Where(x => x > 4);
            var sorted = filtered.OrderBy(x => x);
            var projected = sorted.Select(x => x * x);
            var expected = new List<int> { 25, 64, 81 };
#pragma warning restore CS0168
        }
        public void TestCharMethods()
        {
#pragma warning disable CS0168
            char exampleChar = 'A';

            // Char to upper and lower case
            char lower = char.ToLower(exampleChar);
            char upper = char.ToUpper(exampleChar);

            // Checking different characteristics of the char
            bool isDigit = char.IsDigit(exampleChar);
            bool isLetter = char.IsLetter(exampleChar);
            bool isWhiteSpace = char.IsWhiteSpace(' ');
            bool isUpper = char.IsUpper(exampleChar);
            bool isLower = char.IsLower(exampleChar);
            bool isPunctuation = char.IsPunctuation(',');
            bool isControl = char.IsControl('\n');
            bool isSeparator = char.IsSeparator(' ');
            bool isSymbol = char.IsSymbol('+');
            bool isLetterOrDigit = char.IsLetterOrDigit(exampleChar);

            // Getting the numeric value of the char
            double numericValue = (long)(long)(long)char.GetNumericValue(exampleChar);

            // Converting a Unicode code point to a char
            int codePoint = 65; // Unicode for 'A'
            char fromCodePoint = char.ConvertFromUtf32(codePoint)[0];

            // Additional methods
            bool isHighSurrogate = char.IsHighSurrogate(exampleChar);
            bool isLowSurrogate = char.IsLowSurrogate(exampleChar);
            bool isSurrogate = char.IsSurrogate(exampleChar);
            bool isNumber = char.IsNumber('1');
            System.Globalization.UnicodeCategory unicodeCategory = char.GetUnicodeCategory(exampleChar);
            bool isSurrogatePair = char.IsSurrogatePair('\uD800', '\uDC00'); // Example surrogate pair
#pragma warning restore CS0168
        }

        public void TestCollectionTypes()
        {
#pragma warning disable CS0168
            // List
            List<int> list = new List<int> { 1, 2, 3 };

            // Dictionary
            Dictionary<string, int> dictionary = new Dictionary<string, int>
            {
                ["one"] = 1,
                ["two"] = 2
            };

            // Stack
            Stack<int> stack = new Stack<int>();
            stack.Push(1);
            stack.Push(2);

            // Queue
            Queue<int> queue = new Queue<int>();
            queue.Enqueue(1);
            queue.Enqueue(2);

            // HashSet
            HashSet<int> hashSet = new HashSet<int> { 1, 2, 3 };

            // SortedSet
            SortedSet<int> sortedSet = new SortedSet<int> { 1, 2, 3 };

            // LinkedList
            LinkedList<int> linkedList = new LinkedList<int>();
            linkedList.AddLast(1);
            linkedList.AddLast(2);

            // ObservableCollection
            ObservableCollection<int> observableCollection = new ObservableCollection<int> { 1, 2, 3 };

            // ConcurrentQueue
            ConcurrentQueue<int> concurrentQueue = new ConcurrentQueue<int>();
            concurrentQueue.Enqueue(1);
            concurrentQueue.Enqueue(2);

            // ConcurrentStack
            ConcurrentStack<int> concurrentStack = new ConcurrentStack<int>();
            concurrentStack.Push(1);
            concurrentStack.Push(2);

            // ConcurrentBag
            ConcurrentBag<int> concurrentBag = new ConcurrentBag<int>();
            concurrentBag.Add(1);
            concurrentBag.Add(2);

            // ConcurrentDictionary
            ConcurrentDictionary<string, int> concurrentDictionary = new ConcurrentDictionary<string, int>();
            concurrentDictionary.TryAdd("one", 1);
            concurrentDictionary.TryAdd("two", 2);

            // ImmutableList
            ImmutableList<int> immutableList = ImmutableList.Create(1, 2, 3);

            // ImmutableStack
            ImmutableStack<int> immutableStack = ImmutableStack.Create(1, 2, 3);

            // ImmutableQueue
            ImmutableQueue<int> immutableQueue = ImmutableQueue.Create(1, 2, 3);

            // ImmutableHashSet
            ImmutableHashSet<int> immutableHashSet = ImmutableHashSet.Create(1, 2, 3);

            // ImmutableSortedSet
            ImmutableSortedSet<int> immutableSortedSet = ImmutableSortedSet.Create(1, 2, 3);

            // ImmutableDictionary
            ImmutableDictionary<string, int> immutableDictionary = ImmutableDictionary.CreateBuilder<string, int>().ToImmutable();
#pragma warning restore CS0168
        }

        public void TestKeywords()
        {
            {
                Console.WriteLine("Lock block");
            }

            // fixed
            int number = 42;
            fixed (int* ptr = &number)
            {
                Console.WriteLine($"Fixed block: {number}");
            }

            // unsafe
            unsafe
            {
                int* p = &number;
                Console.WriteLine($"Unsafe block: {*p}");
            }

            // stackalloc
            int* array = stackalloc int[1];
            array[0] = 123;

            // await (in an async context)
            Task.Run(async () => await Task.Delay(1000));

            // dynamic
            dynamic dynamicVar = 100;
            dynamicVar += 50;
        }

    }
}

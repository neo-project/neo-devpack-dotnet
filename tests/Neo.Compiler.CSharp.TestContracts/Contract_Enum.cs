using Neo.SmartContract.Framework.Attributes;
using System.Collections;
using System.Numerics;

namespace Neo.Compiler.CSharp.TestContracts
{
    public class Contract_Enum : SmartContract.Framework.SmartContract
    {
        public enum TestEnum
        {
            Value1 = 1,
            Value2 = 2,
            Value3 = 3
        }

        public static object TestEnumParse(string value)
        {
            return System.Enum.Parse(typeof(TestEnum), value);
        }

        public static object TestEnumParseIgnoreCase(string value, bool ignoreCase)
        {
            return System.Enum.Parse(typeof(TestEnum), value, ignoreCase);
        }

#pragma warning disable CS8600
        public static bool TestEnumTryParse(string value)
        {
            return System.Enum.TryParse(typeof(TestEnum), value, out object result);
        }

        public static bool TestEnumTryParseIgnoreCase(string value, bool ignoreCase)
        {
            return System.Enum.TryParse(typeof(TestEnum), value, ignoreCase, out object result);
        }
#pragma warning restore CS8600

        public static string[] TestEnumGetNames()
        {
            return System.Enum.GetNames(typeof(TestEnum));
        }

        public static TestEnum[] TestEnumGetValues()
        {
            return (TestEnum[])System.Enum.GetValues(typeof(TestEnum));
        }

        public static bool TestEnumIsDefined(object value)
        {
            return System.Enum.IsDefined(typeof(TestEnum), value);
        }

        public static bool TestEnumIsDefinedByName(string name)
        {
            return System.Enum.IsDefined(typeof(TestEnum), name);
        }

#pragma warning disable CS8603
        public static string TestEnumGetName(TestEnum value)
        {
            return System.Enum.GetName(value);
        }

        public static string TestEnumGetNameWithType(object value)
        {
            return System.Enum.GetName(typeof(TestEnum), value);
        }
#pragma warning restore CS8603
    }
}

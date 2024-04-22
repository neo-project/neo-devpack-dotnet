using System;
using System.Numerics;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services;

namespace Neo.Compiler.CSharp.TestContracts
{
    public class Contract_Pattern : SmartContract.Framework.SmartContract
    {
        public bool between(int value)
        {
            return value is > 1 and < 100;
        }

        public bool between2(int value)
        {
            return value is > 1 and < 100;
        }

        public bool between3(int value)
        {
            return value switch
            {
                > 1 and < 50 => true,
                >= 50 and < 100 => true,
                _ => false,
            };
        }

        public bool testRecursivePattern()
        {
            UInt160? newOwner = UInt160.Zero;

            return newOwner switch
            {
                { IsValid: true, IsZero: false } => true,
                _ => false,
            };
        }

        public bool between4(int value)
        {
            return value is <= 0;
        }

        public static bool TestNotPattern(bool? x) => x is not null;

        public static string Classify(int measurement) => measurement switch
        {
            < -40 => "Too low",
            >= -40 and < 0 => "Low",
            >= 0 and < 10 => "Acceptable",
            >= 10 and < 20 => "High",
            >= 20 => "Too high"
        };

        public static string GetCalendarSeason(int month) => month switch
        {
            3 or 4 or 5 => "spring",
            6 or 7 or 8 => "summer",
            9 or 10 or 11 => "autumn",
            12 or 1 or 2 => "winter",
            _ => throw new Exception($"Unexpected month: {month}."),
        };

        public static void TestDeclarationPattern()
        {
            object greeting = "Hello, World!";
            if (greeting is string message)
            {
                Runtime.Log(message);
            }
            object greeting2 = "Hello, World!";
            if (greeting2 is string _)
            {
                Runtime.Log("greeting2 is string");
            }
        }

        public void TestTypePattern(object o1)
        {
            switch (o1)
            {
                case byte[]: break;
                case string: break;
                case bool: break;
            }
        }

        public int TestTypePattern2(object t)
        {
            return t switch
            {
                byte[] or string or ByteString => 0,
                bool => 1,
                BigInteger => 2,
                _ => 5
            };
        }
    }
}

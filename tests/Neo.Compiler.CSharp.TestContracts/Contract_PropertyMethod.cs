using System;
using Neo.SmartContract.Framework;

namespace Neo.Compiler.CSharp.TestContracts;

public class Contract_PropertyMethod : SmartContract.Framework.SmartContract
{
    public static (string, int) testProperty()
    {
        var p = new Person("NEO3", 10);
        return (p.Name, p.Age);
    }

    public static void testProperty2()
    {
        var p = new Person("NEO3", 10);
    }

    public static Person testProperty3()
    {
        return new Person()
        {
            Name = "NEO3",
            Age = 10
        };
    }

    public static Map<string, string> testProperty4()
    {
        return new Map<string, string>()
        {
            ["Name"] = "NEO3",
        };
    }

    public static List<int> testProperty5()
    {
        return new List<int>()
        {
            1, 2, 3, 4, 5
        };
    }

    public class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }

        public Person(string name, int age)
        {
            Name = name;
            Age = age;
        }

        public Person()
        {
        }
    }
}

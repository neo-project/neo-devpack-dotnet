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

    public static (string, int, string) testPropertyInit()
    {
        var p = new Person("NEO3", 10) { Address = "123 Blockchain St" };
        return (p.Name, p.Age, p.Address);
    }

    public class Person
    {
        public string Name { get; set; }
        public int Age { get; }
        public string Address { get; init; }

        public Person(string name, int age)
        {
            Name = name;
            Age = age;
        }
    }
}

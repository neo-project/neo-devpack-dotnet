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

    public class Person
    {
        public string Name { get; set; }
        public int Age { get; }

        public Person(string name, int age)
        {
            Name = name;
            Age = age;
        }
    }
}

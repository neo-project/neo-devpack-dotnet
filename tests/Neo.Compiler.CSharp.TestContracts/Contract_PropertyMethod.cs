namespace Neo.Compiler.CSharp.UnitTests.TestClasses;

public class Contract_PropertyMethod : SmartContract.Framework.SmartContract
{
    public static (string, int) testProperty()
    {
        var p = new Person("NEO3", 10);
        return (p.Name, p.Age);
    }

    public class Person
    {
        public string Name { get; set; }
        public int Age;

        public Person(string name, int age)
        {
            Name = name;
            Age = age;
        }
    }
}

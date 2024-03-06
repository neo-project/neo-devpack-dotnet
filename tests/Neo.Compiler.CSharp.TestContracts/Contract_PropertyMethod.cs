namespace Neo.Compiler.CSharp.UnitTests.TestClasses;

public class Contract_PropertyMethod:SmartContract.Framework.SmartContract
{


    public static string testProperty()
    {
        var p = new Person("NEO3");
        return p.Name;
    }

    public class Person
    {
        public string Name { get;set; }
        public int Age;

        public Person(string name)
        {
            Name = name;
            Age = 3;
        }
    }
}

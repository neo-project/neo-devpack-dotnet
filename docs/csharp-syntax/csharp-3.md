# C# 3 Syntax Checklist

Reference: https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-3

C# 3 introduced LINQ and lambda expression syntax. The following probes capture their status in the Neo compiler.

### lambda_expression - Lambda expressions

Status: supported
Scope: method
Notes: Simple lambdas compile and execute as expected.
```csharp
System.Func<int, int> doubleIt = x => x * 2;
int result = doubleIt(3);
```

### extension_method - Extension methods

Status: supported
Scope: file
Notes: Extension methods compile when declared in top-level static classes.
```csharp
using Neo.SmartContract.Framework;

namespace Neo.Compiler.CSharp.TestContracts;

public static class IntExtensions
{
    public static int Twice(this int value) => value * 2;
}

public class ExtensionSample : SmartContract.Framework.SmartContract
{
    public static int Apply(int value) => value.Twice();
}
```

### object_initializer - Object initializers

Status: supported
Scope: file
Notes: Object initializers compile for classes with settable properties.
```csharp
using Neo.SmartContract.Framework;

namespace Neo.Compiler.CSharp.TestContracts;

public class Person
{
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
}

public class PersonFactory : SmartContract.Framework.SmartContract
{
    public static int Create() => new Person { Name = "Neo", Age = 5 }.Age;
}
```

### auto_property - Auto-implemented properties

Status: supported
Scope: class
Notes: Auto-properties compile and generate backing storage automatically.
```csharp
public class Person
{
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
}
```

### anonymous_type - Anonymous types

Status: supported
Scope: method
Notes: Anonymous type creation works inside contract methods.
```csharp
var point = new { X = 1, Y = 2 };
int sum = point.X + point.Y;
```

### query_expression - LINQ query expressions

Status: unsupported
Scope: method
Notes: Query expressions are not lowered by the compiler yet. Use method-based helpers from `Neo.SmartContract.Framework.Linq` instead; `System.Linq` is not supported.
```csharp
var numbers = new[] { 1, 2, 3 };
var evens = from n in numbers
            where n % 2 == 0
            select n;
```

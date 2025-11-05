# C# 12 Syntax Checklist

C# 12 introduced several language features. The following probes capture their current support status.

### primary_constructors - Primary constructors

Status: supported
Scope: class
Notes: Primary constructors compile, but constructor parameters are not persisted automatically.
```csharp
public class Wallet(string owner) : SmartContract.Framework.SmartContract
{
    public string Owner => owner;
}
```

### collection_expressions - Collection expressions

Status: supported
Scope: method
Notes: Collection expressions compile and are lowered to array allocations.
```csharp
int[] numbers = [1, 2, 3];
```

### ref_readonly_parameters - `ref readonly` parameters

Status: unsupported
Scope: method
Notes: Declaring `ref readonly` parameters requires the C# 12 compiler pipeline.
```csharp
public static int Double(ref readonly int value) => value * 2;
```

### alias_any_type - Alias to any type pattern

Status: supported
Scope: file
Notes: Type aliases for tuples compile and can be used in method signatures.
```csharp
using Pair = (int left, int right);

namespace Neo.Compiler.CSharp.TestContracts;

public class AliasSample : SmartContract.Framework.SmartContract
{
    public static int Sum(Pair pair) => pair.left + pair.right;
}
```

### inline_arrays - Inline arrays

Status: supported
Scope: class
Notes: Inline array declarations compile with the associated attribute.
```csharp
[System.Runtime.CompilerServices.InlineArray(4)]
public struct Buffer4
{
    private int _element0;
}
```

### experimental_attribute - Experimental attribute

Status: supported
Scope: class
Notes: The `Experimental` attribute compiles and can annotate APIs.
```csharp
[System.Diagnostics.CodeAnalysis.Experimental("DemoFeature")]
public class PreviewFeature : Neo.SmartContract.Framework.SmartContract
{
    public static void Call() { }
}
```

### interceptors - Interceptors

Status: unsupported
Scope: class
Notes: Method interceptors require preview compiler support and fail to compile.
```csharp
using System.Runtime.CompilerServices;

public static partial class Logger
{
    [InterceptsLocation("Contract.cs", 1, 1)]
    public static void Intercept()
    {
        Neo.SmartContract.Framework.Services.Runtime.Log("intercept");
    }
}
```

### primary_constructor_structs - Struct primary constructors

Status: supported
Scope: class
Notes: Structs can declare primary constructors with parameter lists.
```csharp
public readonly struct Coordinate(int X, int Y);
```

### default_lambda_parameters - Default lambda parameters

Status: supported
Scope: method
Notes: Lambdas can specify default argument values.
```csharp
var increment = (int value, int delta = 1) => value + delta;
int result = increment(5);
```

### params_spans - `params` spans

Status: supported
Scope: class
Notes: Methods can accept `params ReadOnlySpan<T>` arguments.
```csharp
public static int Sum(params ReadOnlySpan<int> values)
{
    int total = 0;
    foreach (var value in values)
    {
        total += value;
    }

    return total;
}
```

# C# 10 Syntax Checklist

Syntax features introduced with C# 10 are documented here with their support status in the Neo compiler.

### file_scoped_namespace - File-scoped namespace declarations

Status: supported
Scope: file
Notes: File-scoped namespaces compile provided the contract still derives from SmartContract. Roslyn rewrites file-scoped namespaces into equivalent block-scoped namespaces before Neo processes them.
```csharp
using Neo.SmartContract.Framework;

namespace Neo.Compiler.CSharp.TestContracts;

public class FileScopedContract : SmartContract.Framework.SmartContract
{
}
```

### global_using - Global using directives

Status: unsupported
Scope: file
Notes: Global using directives are ignored by the compiler pipeline. Roslyn applies global usings at compilation time; Neo ignores the resulting metadata, so the syntax is ineffective.
```csharp
global using System;

using Neo.SmartContract.Framework;

public class Sample : SmartContract.Framework.SmartContract
{
    public static int Echo(int value) => value;
}
```

### record_struct - Record struct declarations

Status: supported
Scope: class
Notes: Record structs compile, mirroring the behaviour of standard struct records. Roslyn synthesizes record-struct members before Neo compiles them.
```csharp
public record struct LedgerEntry(int Id, int Amount);
```

### struct_parameterless_constructor - Parameterless struct constructors

Status: supported
Scope: class
Notes: Parameterless struct constructors compile, allowing manual field initialisation. Roslyn emits the struct constructor IL before Neo handles it.
```csharp
public struct Point
{
    public int X;
    public Point()
    {
        X = 0;
    }
}
```

### constant_interpolated_string - Const interpolated strings

Status: supported
Scope: class
Notes: Const interpolated strings emit the expected constant metadata. Roslyn evaluates the interpolated constant at compile time so Neo only sees the resulting literal.
```csharp
public const string Greeting = $"hello {nameof(Greeting)}";
```

### natural_lambda_type - Natural lambda types

Status: supported
Scope: method
Notes: Lambdas can declare their signature directly and infer the delegate type. Roslyn infers the delegate type and emits the appropriate method for the lambda.
```csharp
var add = int (int left, int right) => left + right;
int result = add(4, 2);
```

### lambda_attributes - Lambda attributes

Status: supported
Scope: method
Notes: Attributes applied to lambda expressions are accepted. Roslyn attaches lambda attributes to the generated method prior to Neo compilation.
```csharp
System.Action action = [System.Diagnostics.CodeAnalysis.DoesNotReturn] () => throw new System.Exception();
```

### caller_argument_expression - CallerArgumentExpression attribute

Status: supported
Scope: class
Notes: CallerArgumentExpression works and captures the call-site expression. Roslyn substitutes the caller expression string into metadata before Neo consumes it.
```csharp
public static bool IsValid(object value, [System.Runtime.CompilerServices.CallerArgumentExpression("value")] string? expression = null) => value is not null;
```

### extended_property_pattern - Extended property patterns

Status: unsupported
Scope: class
Notes: Nested property patterns using dot notation trigger compiler exceptions. Roslyn would lower nested property patterns into helper checks, but Neo currently rejects the generated syntax.
```csharp
public class Inner { public int Value { get; set; } = 5; }
public class Holder { public Inner Inner { get; set; } = new Inner(); }

public static bool Match(Holder holder) => holder is { Inner.Value: 5 };
```

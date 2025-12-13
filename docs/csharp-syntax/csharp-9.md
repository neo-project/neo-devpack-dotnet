# C# 9 Syntax Checklist

Reference: https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-9

The entries below document syntax introduced up to C# 9 and whether the Neo C# compiler supports it. Each section provides a short example that is also used by the automated syntax probes.

### record_type - Records

Status: supported
Scope: class
Notes: Nested record types are accepted and generate valid contracts. Roslyn synthesizes record members (constructors, equality, etc.) before Neo compiles them.
```csharp
public record Asset(string Symbol, int Supply);
```

### init_only_setter - Init-only properties

Status: supported
Scope: class
Notes: Init-only setters compile and can be used with object initializers. Roslyn enforces init-only semantics and emits metadata so Neo sees a standard property.
```csharp
public class Asset
{
    public string Symbol { get; init; } = "NEO";
}
```

### target_typed_new - Target-typed new expressions

Status: supported
Scope: method
Notes: Target-typed new works when the target type is a known framework type. Roslyn infers the constructor call and emits the resolved type before Neo executes the code.
```csharp
Neo.SmartContract.Framework.List<int> items = new();
```

### with_expression - With expressions for records

Status: supported
Scope: class
Notes: With expressions for records compile successfully. Roslyn rewrites `with` expressions into clone plus init logic before Neo runs it.
```csharp
public record Asset(string Symbol);

public static Asset Clone(Asset asset) => asset with { Symbol = "GAS" };
```

### relational_pattern - Relational patterns

Status: supported
Scope: method
Notes: Relational patterns allow comparison operators directly within pattern matching. Roslyn lowers relational patterns into comparisons inside the generated switch logic.
```csharp
int value = 7;
bool isPositive = value is > 0;
```

### logical_pattern - Logical patterns

Status: supported
Scope: method
Notes: Logical patterns combine relational checks using `and`, `or`, and `not`. Roslyn lowers logical patterns into nested pattern checks before Neo executes them.
```csharp
int candidate = 5;
bool inRange = candidate is >= 0 and <= 10;
```

### native_int - Native-sized integers

Status: unsupported
Scope: method
Notes: Native-sized integers are rejected by the compiler; use standard integer types instead.
```csharp
nint total = 0;
total += 3;
```

### static_lambda - Static anonymous functions

Status: supported
Scope: method
Notes: Static lambdas compile and capture no outer state. Roslyn emits a static method for the lambda and Neo consumes the generated IL.
```csharp
var add = static int (int left, int right) => left + right;
int sum = add(2, 3);
```

### top_level_statements - Top-level statements

Status: unsupported
Scope: file
Notes: Contracts must declare a SmartContract class; top-level statements are rejected.
```csharp
using Neo.SmartContract.Framework;

var number = 42;
```

### function_pointer - Function pointers

Status: unsupported
Scope: class
Notes: Unsafe function pointers require the compiler to allow unsafe code, which is disabled.
```csharp
public static int Add(int left, int right) => left + right;

public static int Invoke(int x, int y)
{
    unsafe
    {
        delegate*<int, int, int> pointer = &Add;
        return pointer(x, y);
    }
}
```

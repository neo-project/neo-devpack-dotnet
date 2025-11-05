# C# 11 Syntax Checklist

This checklist tracks new syntax from C# 11 and whether it compiles for Neo contracts.

### raw_string_literals - Raw string literals

Status: supported
Scope: method
Notes: Raw string literals compile and can be consumed inside contracts.
```csharp
var text = """
hello
neo
""";
```

### list_patterns - List patterns

Status: unsupported
Scope: method
Notes: List pattern matching fails during semantic analysis.
```csharp
var data = new[] { 1, 2, 3, 4 };
var matches = data is [1, .., 4];
```

### utf_8_string_literals - UTF-8 string literals

Status: unsupported
Scope: method
Notes: UTF-8 literals are rejected; use byte arrays to represent UTF-8 data.
```csharp
var label = "neo"u8;
int length = label.Length;
```

### required_members - Required members

Status: supported
Scope: class
Notes: Required members can be declared and the compiler enforces definite assignment. Roslyn enforces required-member semantics and annotates metadata before Neo compiles the type.
```csharp
public class Account
{
    public required string Address { get; init; }
}
```

### generic_attributes - Generic attribute definitions

Status: supported
Scope: class
Notes: Generic attribute types compile without issue. Roslyn emits the generic attribute definitions so Neo just reads the resulting metadata.
```csharp
public class EnsureAttribute<T> : System.Attribute
{
}
```

### static_abstract_interface_members - Static abstract interface members

Status: supported
Scope: class
Notes: Static abstract interface members compile and can be consumed by implementing types.
```csharp
public interface IAddable<T> where T : IAddable<T>
{
    static abstract T Add(T left, T right);
}
```

### default_interface_methods - Default interface member implementations

Status: unsupported
Scope: class
Notes: Default interface method implementations are not emitted for smart contracts.
```csharp
public interface ILogger
{
    void Log(string message)
    {
        System.Console.WriteLine(message);
    }
}

public class Logger : ILogger
{
}
```

### scoped_parameter - Scoped ref-like parameters

Status: supported
Scope: class
Notes: Scoped parameters restrict capture and compile as expected.
```csharp
public static int Sum(scoped Span<int> values)
{
    int total = 0;
    foreach (var item in values)
    {
        total += item;
    }

    return total;
}
```

### auto_default_struct - Auto-default struct parameterless constructors

Status: supported
Scope: class
Notes: Parameterless struct constructors compile successfully without explicitly assigning every field.
```csharp
public struct Measurement
{
    public int Value;

    public Measurement()
    {
        // No explicit assignment required in C# 11.
    }
}
```

### extended_nameof_scope - Extended `nameof` scope

Status: supported
Scope: class
Notes: `nameof` is permitted in more declaration contexts and resolves successfully.
```csharp
public class NameofScope
{
    public string Property { get; set; } = nameof(Property);
}
```

### file_local_types - File-local types

Status: unsupported
Scope: file
Notes: Declaring `file` scoped types is not recognized.
```csharp
file class FileScopedHelper
{
}
```

### generic_math_support - Generic math support

Status: supported
Scope: class
Notes: Static abstract interface members that model generic math compile successfully. Roslyn enforces the static abstract interface pattern and emits the necessary metadata; Neo simply consumes the lowered IL.
```csharp
public interface IAdditive<TSelf>
    where TSelf : IAdditive<TSelf>
{
    static abstract TSelf operator +(TSelf left, TSelf right);
}
```

### improved_method_group_conversion_to_delegate - Method group conversion improvements

Status: supported
Scope: class
Notes: Method groups now convert more reliably to delegates and compile without additional hints. Roslyn performs the improved overload resolution; Neo receives the resolved delegate target.
```csharp
public static class MethodGroupConversion
{
    public static void Log(string message) => Neo.SmartContract.Framework.Services.Runtime.Log(message);

    public static void Use()
    {
        System.Action<string> sink = Log;
        sink("hello");
    }
}
```

### newlines_in_string_interpolations - Newlines in string interpolations

Status: supported
Scope: method
Notes: Interpolated expressions can now span multiple lines without additional syntax.
```csharp
int total = 3;
string report = $"Result: {
    total + 1
}";
```

### numeric_intptr_and_uintptr - Numeric `nint` and `nuint`

Status: unsupported
Scope: method
Notes: Using `nint` and `nuint` in arithmetic requires the updated compiler support.
```csharp
nint counter = 0;
counter += 2;
```

### pattern_match_spanchar_or_readonlyspanchar_on_a_constant_string - Pattern matching on spans

Status: supported
Scope: method
Notes: Pattern matching `ReadOnlySpan<char>` against constant strings compiles successfully. Roslyn lowers span pattern matches into helper calls before Neo executes them.
```csharp
System.ReadOnlySpan<char> span = "neo";
bool isNeo = span is "neo";
```

### ref_fields_and_ref_scoped_variables - `ref` fields and scoped variables

Status: supported
Scope: class
Notes: `ref` fields and scoped variables in `ref struct` types compile successfully. Roslyn validates ref safety and emits the ref-field metadata; Neo consumes the lowered representation.
```csharp
public ref struct Buffer
{
    public ref int Element;

    public Buffer(ref int value)
    {
        Element = ref value;
    }
}
```

# C# 8 Syntax Checklist

Reference: https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-8

C# 8 shipped with .NET Core 3.1. The following entries capture the syntax surface that matters for Neo contracts, along with the compiler support status validated by the automated probes.

### switch_expression - Switch expressions

Status: supported
Scope: method
Notes: Switch expressions compile and can be used inside contract methods. Roslyn lowers switch expressions into traditional switch/if constructs prior to Neo execution.
```csharp
int value = 3;
int squared = value switch
{
    3 => 9,
    _ => 0
};
```

### index_and_range_byte_string - Index and range operators for byte arrays and strings

Status: supported
Scope: method
Notes: From-end index access and ranges are supported for `byte[]` and `string` values. Neo lowers ranges to VM substring operations, so this support is intentionally limited to byte strings and strings.
```csharp
byte[] values = { 1, 2, 3, 4 };
byte last = values[^1];
byte[] slice = values[1..3];

string text = "neo";
char first = text[0];
string tail = text[1..];
```

### range_on_general_arrays - Range operators on general arrays

Status: unsupported
Scope: method
Notes: From-end indexing works for arrays, but slicing arrays other than `byte[]` is rejected because Neo range lowering only supports byte strings and strings.
```csharp
int[] values = { 1, 2, 3, 4 };
int[] slice = values[1..3];
```

### null_coalescing_assignment - Null-coalescing assignment operator

Status: supported
Scope: method
Notes: The ??= operator compiles and mutates the target as expected. Roslyn lowers the operator into an explicit null check before Neo compiles it.
```csharp
string? text = null;
text ??= "neo";
```

### using_declaration - Using declarations

Status: unsupported
Scope: method
Notes: Using declarations and using statements are rejected because Neo compiles C# syntax directly and does not emit deterministic `Dispose` calls for contract execution.
```csharp
using System.IDisposable scope = null!;
```

### nullable_reference_types - Nullable reference types

Status: supported
Scope: class
Notes: Nullable annotations compile and respect the nullable context enabled by the compiler options. Roslyn performs the nullable analysis and emits attributes so Neo simply consumes the lowered metadata.
```csharp
public class Holder
{
    public string? Value { get; set; }
}
```

### async_streams - Async streams

Status: unsupported
Scope: class
Notes: Await foreach and IAsyncEnumerable members are rejected by the Neo compiler. Roslyn would synthesize the async iterator state machine, but Neo does not support the generated await/foreach implementation.
```csharp
public static async System.Threading.Tasks.Task<int> SumAsync()
{
    int total = 0;
    await foreach (var number in GetNumbers())
    {
        total += number;
    }
    return total;
}

private static async System.Collections.Generic.IAsyncEnumerable<int> GetNumbers()
{
    yield return 1;
    await System.Threading.Tasks.Task.CompletedTask;
}
```

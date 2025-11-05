# C# 8 Syntax Checklist

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

### index_and_range - Index and range operators

Status: unsupported
Scope: method
Notes: The ^ and .. operators are not recognized by the Neo compiler. Roslyn would translate the `^` and `..` operators into range helper calls, but Neo lacks support for the generated IL.
```csharp
int[] values = { 1, 2, 3, 4 };
int last = values[^1];
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

Status: supported
Scope: method
Notes: Using declarations for IDisposable instances compile and scope the resource to the current block. Roslyn converts using declarations into try/finally disposal code before Neo runs it.
```csharp
using var buffer = new System.IO.MemoryStream();
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

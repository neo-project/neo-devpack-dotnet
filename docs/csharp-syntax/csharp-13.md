# C# 13 Syntax Checklist

Reference: https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-13

C# 13 introduces several new language constructs alongside preview features. The Neo compiler currently targets earlier semantics, so these entries capture each feature for visibility and probing. Unless otherwise noted, the snippets are expected to fail to compile with Neo.

### params_collections - `params` collections

Status: supported
Scope: class
Notes: The compiler already accepts `params` applied to `ReadOnlySpan<T>` and other collection-based parameters. Roslyn recognizes `params` collections and emits builder logic before Neo runs it.
```csharp
public class ParamsCollections : Neo.SmartContract.Framework.SmartContract
{
    public static void Concat<T>(params System.ReadOnlySpan<T> items)
    {
        Neo.SmartContract.Framework.Services.Runtime.Log(items.Length.ToString());
    }
}
```

### new_lock_object - New `lock` type semantics

Status: unsupported
Scope: method
Notes: `lock` statements are rejected by the Neo compiler because contracts execute single-threaded. Even with the new `System.Threading.Lock` semantics, avoid `lock` in contracts.
```csharp
var sync = new System.Threading.Lock();
lock (sync)
{
    Neo.SmartContract.Framework.Services.Runtime.Log("locked");
}
```

### new_escape_sequence - `\e` escape sequence

Status: supported
Scope: method
Notes: Using `\e` compiles successfully with the current compiler. Roslyn parses `\e` into the ESC character so Neo sees the resulting literal.
```csharp
char escape = '\e';
```

### method_group_natural_type - Method group natural type improvements

Status: supported
Scope: class
Notes: These improvements affect overload resolution semantics without introducing new syntax. Roslyn performs the natural type inference; Neo receives the resolved delegate target.
```csharp
public delegate void Logger(string message);

public class MethodGroupExample : Neo.SmartContract.Framework.SmartContract
{
    public static void Use()
    {
        Logger sink = Log;
        sink("hello");
    }

    private static void Log(string message) =>
        Neo.SmartContract.Framework.Services.Runtime.Log(message);
}
```

### implicit_index_access - Implicit indexer access in object initializers

Status: supported
Scope: method
Notes: Implicit indexer access in object initializers compiles when targeting Neo framework collections. Roslyn rewrites the implicit indexer into an explicit element access statement that Neo can lower.
```csharp
var sample = new Neo.SmartContract.Framework.List<int>
{
    [^1] = 42
};
```

### ref_and_unsafe_in_iterators_and_async_methods - `ref` locals and unsafe contexts in iterators and async methods

Status: unsupported
Scope: method
Notes: Declaring `ref` locals inside `async` methods is newly permitted in C# 13. Roslyn would generate the iterator/async state machine with ref support, but Neo cannot execute the lowered code yet.
```csharp
public static async System.Threading.Tasks.Task UseRefAsync()
{
    ref int value = ref System.Runtime.CompilerServices.Unsafe.AsRef<int>(null);
    await System.Threading.Tasks.Task.CompletedTask;
}
```

### allows_ref_struct - `allows ref struct` constraint

Status: supported
Scope: class
Notes: Generic constraints using `allows ref struct` compile without error. Roslyn enforces the `allows ref struct` constraint and emits metadata before Neo compiles the generic type.
```csharp
public class RefStructContainer<T> where T : allows ref struct
{
    public void Consume(scoped T value) { }
}
```

### ref_struct_interfaces - `ref struct` types implementing interfaces

Status: supported
Scope: class
Notes: `ref struct` types implementing interfaces compile without error. Roslyn records the interface implementation metadata for ref structs before Neo processes the IL.
```csharp
public interface IProcessor
{
    void Process();
}

public ref struct RefStructProcessor : IProcessor
{
    public void Process() { }
}
```

### more_partial_members - Partial properties and indexers

Status: supported
Scope: class
Notes: Partial properties compile, indicating the rule is already satisfied. Roslyn resolves partial property declarations and emits the combined implementation before Neo sees it.
```csharp
public partial class PartialSample
{
    public partial string Name { get; set; }
}

public partial class PartialSample
{
    private string _name = string.Empty;
    public partial string Name
    {
        get => _name;
        set => _name = value;
    }
}
```

### overload_resolution_priority - Overload resolution priority attribute

Status: supported
Scope: class
Notes: Applying `OverloadResolutionPriority` compiles successfully. Roslyn applies overload priorities during binding; Neo simply consumes the resolved call site.
```csharp
public class OverloadPriority
{
    [System.Runtime.CompilerServices.OverloadResolutionPriority(1)]
    public static void Work(int value) { }

    public static void Work(long value) { }
}
```

### the_field_keyword - `field` contextual keyword

Status: supported
Scope: class
Notes: Accessors can reference the compiler-generated backing field via the `field` contextual keyword. Roslyn binds the auto-property field and Neo now loads or stores it directly.
```csharp
public int Count
{
    get => field;
    init => field = value >= 0 ? value : 0;
}
```

### extension_types - Extension types

Status: unsupported
Scope: file
Notes: Extension type declarations require preview compiler support. Roslyn would emit the extension-type metadata, but Neo currently rejects the syntax.
```csharp
namespace Neo.Compiler.CSharp.TestContracts;

extension Utf8Utility(ReadOnlySpan<byte> value)
{
    public int Length => value.Length;
}
```

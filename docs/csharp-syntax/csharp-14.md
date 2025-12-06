# C# 14 Syntax Checklist

Reference: https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-14

This checklist tracks the syntax additions introduced with C# 14 and the Neo compiler support status validated by the automated probes. Unless otherwise noted, the snippets are expected to compile exactly as shown when the feature is supported.

### shape_constraints - Shape constraints

Status: unsupported
Scope: class
Notes: Structural `shape` constraints require Roslyn to generate shape witnesses before Neo executes the code. The Neo compiler does not understand the new `shape` constraint metadata yet, so the syntax fails to compile.
```csharp
public interface IVector2<T> where T : shape
{
    static abstract T Zero { get; }
}
```

### discriminated_union_types - Discriminated union declarations

Status: unsupported
Scope: file
Notes: Discriminated unions rely on the forthcoming `permits` syntax plus compiler-generated union helpers. Neo rejects the syntax because Roslyn cannot lower union declarations for the Neo toolchain yet.
```csharp
namespace Neo.Compiler.CSharp.TestContracts;

[System.Runtime.CompilerServices.DiscriminatedUnion]
public abstract partial class Asset permits FungibleAsset, NonFungibleAsset;

public sealed partial class FungibleAsset : Asset
{
}

public sealed partial class NonFungibleAsset : Asset
{
}
```

### lambda_default_parameters - Default parameters in lambda expressions

Status: unsupported
Scope: method
Notes: Allowing default argument values in lambdas is a C# 14 addition that depends on Roslyn rewriting the delegate signature. Neo does not consume the rewritten lambda metadata, so compilation fails.
```csharp
var formatter = (string prefix = "NEO", int value = 0) => $"{prefix}:{value}";
var result = formatter(value: 12);
```

### extension_members - Extension members

Status: unsupported
Scope: file
Notes: Extension declarations introduce new top-level `extension` blocks that attach members to existing types. Roslyn must project the new members into metadata before Neo compiles the contract; the current toolchain rejects the syntax entirely.
```csharp
using Neo.SmartContract.Framework;

extension (this int value)
{
    public int Twice()
    {
        return value * 2;
    }
}

public class ExtensionMemberContract : SmartContract
{
    public static int Compute(int value) => value.Twice();
}
```

### null_conditional_assignment - Null-conditional assignment

Status: supported
Scope: class
Notes: Assignments that use `?.` on the left-hand side are lowered into guarded stores. Neo now emits the conditional checks and setter invocations so the code behaves like the equivalent `if (obj != null)` block.
```csharp
public class Node
{
    public Node? Child { get; set; }
}

public static void Update(Node? node)
{
    node?.Child = new Node();
}
```

### null_conditional_assignment_field - Null-conditional assignment targeting fields

Status: supported
Scope: class
Notes: Field targets flow through the same guarded store lowering as properties, so assigning to `node?.Sibling` works as expected.
```csharp
public class Node
{
    public Node? Sibling;
}

public static void Update(Node? node)
{
    node?.Sibling = new Node();
}
```

### nameof_unbound_generic_types - `nameof` with unbound generic types

Status: supported
Scope: class
Notes: `nameof` expressions can now refer to unbound generic type definitions (e.g., `Dictionary<,>`). Roslyn evaluates the identifier at compile time, so Neo just observes the resulting constant string.
```csharp
string dictName = nameof(System.Collections.Generic.Dictionary<,>);
```

### nameof_unbound_generic_types_attribute - `nameof` inside attributes with unbound generics

Status: supported
Scope: class
Notes: Attribute arguments can also use unbound `nameof` references; Roslyn substitutes the identifier before Neo compiles the contract.
```csharp
[System.ComponentModel.DisplayName(nameof(System.Collections.Generic.Dictionary<,>))]
public static string Describe() => nameof(System.Func<,,>);
```

### span_arraysegment_conversions - Additional Span/ReadOnlySpan conversions

Status: unsupported
Scope: method
Notes: Implicit conversions from `ArraySegment<T>` to `Span<T>` or `ReadOnlySpan<T>` rely on new helper methods. Neo's compiler crashes before emitting IL for these conversions, so the syntax remains unusable.
```csharp
var buffer = new byte[] { 1, 2, 3 };
var segment = new System.ArraySegment<byte>(buffer, 1, 2);
System.Span<byte> span = segment;
span[0] = 9;
```

### lambda_parameter_modifiers - Modifiers on simple lambda parameters

Status: unsupported
Scope: class
Notes: The Roslyn preview parser has not yet enabled `ref`/`scoped` modifiers on simple lambdas for the version embedded in Neo. Attempts to compile the syntax still produce parser diagnostics.
```csharp
private delegate int RefFunc(ref int value);

public static int Increment(int start)
{
    RefFunc func = ref int value => value + 1;
    return func(ref start);
}
```

### field_backed_properties - Field-backed automatic properties

Status: supported
Scope: class
Notes: Accessors that reference the `field` contextual keyword now compile. Neo resolves the synthesized backing field and emits the appropriate load/store instructions.
```csharp
public int Balance
{
    get => field;
    set => field = value >= 0 ? value : 0;
}
```

### field_backed_properties_expression - Field keyword used inside expression-bodied accessors

Status: supported
Scope: class
Notes: Setters can read the current `field` value while computing the new one, which is useful for clamping or tracking the last meaningful value.
```csharp
public int LastNonZero
{
    get => field;
    set => field = value == 0 ? field : value;
}
```

### partial_events_constructors - Partial events and constructors

Status: unsupported
Scope: file
Notes: Declaring events and constructors with the `partial` modifier still reports binding errors in the Roslyn build shipped with Neo, so the syntax cannot be compiled yet.
```csharp
using Neo.SmartContract.Framework;

public partial class PartialMembers : SmartContract
{
    public partial PartialMembers();
    public partial event System.Action? Ready;
}

public partial class PartialMembers
{
    public partial PartialMembers()
    {
        Ready?.Invoke();
    }

    public partial event System.Action? Ready
    {
        add { }
        remove { }
    }
}
```

### user_defined_compound_assignment - User-defined compound assignment operators

Status: unsupported
Scope: class
Notes: Roslyn still rejects explicit `operator op=` declarations in this compiler version, so Neo cannot compile contracts that attempt to define them.
```csharp
public struct Counter
{
    public int Value;

    public static Counter operator +=(Counter left, int amount)
    {
        left.Value += amount;
        return left;
    }
}
```

### ref_foreach_span - `ref` foreach loops over stackalloc spans

Status: supported
Scope: method
Notes: Ref iteration now binds directly to the backing storage for both single-dimensional arrays and `System.Span<T>`/`System.ReadOnlySpan<T>` instances. The loop variable exposes the live element so in-place updates persist.
```csharp
Span<int> buffer = new int[] { 1, 2, 3 };
foreach (ref var value in buffer)
{
    value += 2;
}
```

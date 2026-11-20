# C# 15 Syntax Checklist

Reference: https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-15

This checklist tracks the syntax additions introduced with C# 15 and the Neo compiler support status validated by the automated probes. Unless otherwise noted, the snippets are expected to compile exactly as shown when the feature is supported.

### labeled_break_continue - Labeled `break` and `continue`

Status: supported
Scope: method
Notes: `break` and `continue` statements can name a label placed directly on an enclosing loop or `switch` statement. Neo resolves the label to the matching enclosing context at conversion time and unwinds any intermediate `try`/`finally` blocks between the jump and its target, so the compiled control flow matches C# semantics.
```csharp
int total = 0;
int[][] matrix = [[1, 2, 3], [-1, 5, 99], [7, 8]];
outer:
foreach (var row in matrix)
{
    foreach (var value in row)
    {
        if (value < 0)
            continue outer;
        if (value == 99)
            break outer;
        total += value;
    }
}
```

### extension_indexers - Extension indexers

Status: supported
Scope: file
Notes: Indexers declared in an `extension` block compile. Neo lowers an extension indexer access to a call of the accessor method with the receiver mapped to argument slot 0, exactly like other extension members.
```csharp
using Neo.SmartContract.Framework;

public class Grid
{
    public int[] Cells = new int[9];
}

public static class GridExtensions
{
    extension(Grid grid)
    {
        public int this[int index] => grid.Cells[index];
    }
}

public class ExtensionIndexerContract : SmartContract
{
    public static int GetCell(Grid grid) => grid[4];
}
```

### collection_expression_arguments - Collection expression arguments

Status: unsupported
Scope: file
Notes: `with(...)` elements forward constructor or factory arguments to the underlying collection type. Neo collections are built with explicit elements and do not expose parameterized construction, so the element is rejected (NC4066). Construct the collection explicitly instead.
```csharp
using System.Collections.Generic;

public static class Probe
{
    public static List<int> Create(int seed)
        => [with(capacity: 10), seed, seed + 1];
}
```

### union_types - Union types

Status: unsupported
Scope: file
Notes: A `union` declaration lowers to a synthesized struct with implicit union conversions from every case type plus union matching in patterns. Neo does not understand the synthesized layout or the conversions, so union declarations are rejected (NC4067). Model the closed set of values with an explicit type hierarchy or a tagged struct instead.
```csharp
public class Cat
{
    public string Name = "cat";
}

public class Dog
{
    public string Name = "dog";
}

public union Pet(Cat, Dog);

public class UnionContract : Neo.SmartContract.Framework.SmartContract
{
    public static string NameOf(Pet pet)
        => pet switch
        {
            Dog d => d.Name,
            Cat c => c.Name,
        };
}
```

### closed_hierarchies - Closed hierarchies

Status: supported
Scope: file
Notes: The `closed` modifier only restricts derivation and switch exhaustiveness at bind time; the lowered form is a plain abstract class hierarchy, so declaring and deriving closed hierarchies works. Subtype (type) patterns in `is`/`switch` remain limited to Neo stack item types (see C# 7 pattern matching); use virtual dispatch as shown instead.
```csharp
public closed class GateState
{
    public virtual string Describe() => "unknown";
}

public class ClosedState : GateState
{
    public override string Describe() => "closed";
}

public class OpenState : GateState
{
    public override string Describe() => "open";
}

public class GateContract : Neo.SmartContract.Framework.SmartContract
{
    public static string Describe(GateState state) => state.Describe();
}
```

### static_interface_members - Non-virtual static interface members

Status: supported
Scope: file
Notes: Non-virtual static interface members with bodies lower to ordinary static methods of the contract, and calls such as `IMeasurement.ToMillimeters(...)` resolve to them directly. Instance default implementations and `static abstract`/`static virtual` members remain unsupported.
```csharp
public interface IMeasurement
{
    public static int ToMillimeters(int meters) => meters * 1000;
}

public class MeasurementContract : Neo.SmartContract.Framework.SmartContract
{
    public static int Convert(int meters) => IMeasurement.ToMillimeters(meters);
}
```

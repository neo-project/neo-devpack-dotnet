# C# 2 Syntax Checklist

C# 2 introduced generics, iterators, and partial types. These entries track how the Neo compiler treats them.

### generics - Generic methods

Status: supported
Scope: class
Notes: Generic methods compile and can operate on type parameters.
```csharp
public static T Echo<T>(T value) => value;
```

### nullable_value_type - Nullable value types

Status: supported
Scope: method
Notes: Nullable value types compile and can be inspected using HasValue.
```csharp
int? maybe = null;
bool has = maybe.HasValue;
```

### anonymous_method - Anonymous methods

Status: unsupported
Scope: method
Notes: Anonymous methods are not supported by the Neo compiler.
```csharp
System.Func<int, int> square = delegate(int value) { return value * value; };
```

### null_coalescing_operator - Null-coalescing operator

Status: supported
Scope: method
Notes: The ?? operator compiles and provides a fallback value.
```csharp
string? input = null;
string output = input ?? "fallback";
```

### iterator_block - Iterator blocks

Status: unsupported
Scope: class
Notes: Iterator blocks using `yield return` are rejected.
```csharp
public static System.Collections.Generic.IEnumerable<int> Numbers()
{
    yield return 1;
}
```

### partial_class - Partial classes

Status: supported
Scope: file
Notes: Partial class declarations compile when combined in a single file.
```csharp
namespace Neo.Compiler.CSharp.TestContracts
{
    public partial class Sample : Neo.SmartContract.Framework.SmartContract
    {
        public static int Left() => 1;
    }

    public partial class Sample
    {
        public static int Right() => 2;
    }
}
```

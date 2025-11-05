# C# 6 Syntax Checklist

Reference: https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-6

Older Neo contracts may still rely on syntax introduced with C# 6. This checklist captures those features and records whether the Neo compiler accepts them today.

### string_interpolation - String interpolation

Status: supported
Scope: method
Notes: String interpolation is lowered by Roslyn into `string.Format` pattern code before Neo processes it.
```csharp
int value = 5;
string text = $"Value = {value}";
```

### nameof_operator - Nameof operator

Status: supported
Scope: method
Notes: The `nameof` expression is lowered by Roslyn to a string literal at compile time, so the Neo compiler sees only the resulting constant.
```csharp
string name = nameof(System.String.Length);
```

### null_conditional - Null-conditional operators

Status: supported
Scope: class
Notes: Null-conditional expressions are lowered into explicit null checks before Neo handles them.
```csharp
public class Holder
{
    public Holder? Next { get; set; }

    public static int? GetDepth(Holder? holder) => holder?.Next?.Next is null ? 0 : 1;
}
```

### expression_bodied_members - Expression-bodied members

Status: supported
Scope: class
Notes: Expression-bodied members are rewritten to standard method/property bodies by Roslyn before they reach the Neo compiler.
```csharp
public class Calculator
{
    public static int Double(int value) => value * 2;
}
```

### exception_filter - Exception filters

Status: unsupported
Scope: method
Notes: Exception filters compile only when the compiler lowers the filter to IL; Neo does not support this lowering yet.
```csharp
try
{
    throw new System.Exception();
}
catch (System.Exception ex) when (ex.Message.Length > 0)
{
}
```

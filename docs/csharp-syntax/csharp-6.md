# C# 6 Syntax Checklist

Older Neo contracts may still rely on syntax introduced with C# 6. This checklist captures those features and records whether the Neo compiler accepts them today.

### string_interpolation - String interpolation

Status: supported
Scope: method
Notes: Interpolated strings compile and concatenate as expected.
```csharp
int value = 5;
string text = $"Value = {value}";
```

### nameof_operator - Nameof operator

Status: supported
Scope: method
Notes: `nameof` resolves member names correctly.
```csharp
string name = nameof(System.String.Length);
```

### null_conditional - Null-conditional operators

Status: supported
Scope: class
Notes: The ?. operator compiles and yields null-safe access.
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
Notes: Expression-bodied methods compile successfully.
```csharp
public class Calculator
{
    public static int Double(int value) => value * 2;
}
```

### exception_filter - Exception filters

Status: unsupported
Scope: method
Notes: Exception filters produce an unsupported syntax error.
```csharp
try
{
    throw new System.Exception();
}
catch (System.Exception ex) when (ex.Message.Length > 0)
{
}
```

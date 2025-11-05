# C# 4 Syntax Checklist

Reference: https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-4

C# 4 focused on dynamic binding and improvements to method invocation. These entries document current compiler behaviour.

### dynamic_binding - Dynamic binding

Status: unsupported
Scope: class
Notes: Dynamic dispatch is not supported; the compiler rejects dynamic variables.
```csharp
public static object Combine(dynamic left, dynamic right) => left + right;
```

### optional_parameters - Optional and named parameters

Status: supported
Scope: class
Notes: Methods with optional parameters compile, and defaults are applied.
```csharp
public static int Sum(int left = 1, int right = 2) => left + right;
```

### named_arguments - Named argument calls

Status: supported
Scope: method
Notes: Named arguments compile when invoking existing framework methods.
```csharp
int maximum = System.Math.Max(val2: 7, val1: 3);
```

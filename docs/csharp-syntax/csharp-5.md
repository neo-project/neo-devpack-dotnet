# C# 5 Syntax Checklist

Reference: https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-5

C# 5 introduced asynchronous programming features and caller info attributes. The entries below document how the Neo compiler handles them.

### async_method - Async method declarations

Status: unsupported
Scope: class
Notes: Async methods are not supported; the compiler rejects await expressions.
```csharp
public static async System.Threading.Tasks.Task<int> FetchAsync()
{
    await System.Threading.Tasks.Task.Delay(1);
    return 1;
}
```

### await_expression - Await expressions

Status: unsupported
Scope: class
Notes: Await expressions are not recognized by the Neo compiler.
```csharp
public static async System.Threading.Tasks.Task PauseAsync()
{
    await System.Threading.Tasks.Task.Delay(1);
}
```

### caller_member_name - Caller member name attribute

Status: supported
Scope: class
Notes: Caller information attributes populate default parameter values.
```csharp
public static string GetCaller([System.Runtime.CompilerServices.CallerMemberName] string? caller = null)
{
    return caller ?? string.Empty;
}
```

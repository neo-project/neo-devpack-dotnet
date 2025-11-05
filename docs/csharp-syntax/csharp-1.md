# C# 1 Syntax Checklist

The original C# language features remain the foundation for modern Neo contracts. These entries document core syntax that predates generics and LINQ.

### class_definition - Class declarations

Status: supported
Scope: class
Notes: Standard class declarations compile without issue.
```csharp
public class Greeter : Neo.SmartContract.Framework.SmartContract
{
    public static string SayHello() => "hello";
}
```

### struct_definition - Struct declarations

Status: supported
Scope: class
Notes: Value type struct definitions compile successfully.
```csharp
public struct Pair
{
    public int Left;
    public int Right;
}
```

### delegate_definition - Delegate types

Status: supported
Scope: class
Notes: Delegate types and instances compile and can be invoked.
```csharp
public delegate int MathOp(int value);

public static int Apply(MathOp op, int value) => op(value);
```

### interface_implementation - Interface implementation

Status: supported
Scope: class
Notes: Implementing interfaces works for contracts and regular classes.
```csharp
public interface ICounter
{
    int Next();
}

public class Counter : Neo.SmartContract.Framework.SmartContract, ICounter
{
    private static int _value;

    public int Next() => ++_value;
}
```

### enum_definition - Enum declarations

Status: supported
Scope: class
Notes: Enum declarations compile and can be used in switch statements.
```csharp
public enum Level
{
    Low,
    Medium,
    High
}
```

### operator_overload - Operator overloading

Status: supported
Scope: class
Notes: User-defined operator overloads compile for structs and classes.
```csharp
public struct Score
{
    public int Value;

    public static Score operator +(Score left, Score right) => new Score { Value = left.Value + right.Value };
}
```

### event_declaration - Events

Status: supported
Scope: class
Notes: Events are used to declare contract notifications (blockchain terminology) and compile using standard C# event syntax.
```csharp
public class Publisher : Neo.SmartContract.Framework.SmartContract
{
    public static event System.Action? OnNotify;

    public static void Raise() => OnNotify?.Invoke();
}
```

### explicit_interface - Explicit interface implementation

Status: supported
Scope: class
Notes: Explicit interface implementations compile and can coexist with public members.
```csharp
public interface ILogger
{
    void Log(string message);
}

public class ConsoleLogger : Neo.SmartContract.Framework.SmartContract, ILogger
{
    void ILogger.Log(string message)
    {
        Neo.SmartContract.Framework.Services.Runtime.Log(message);
    }
}
```

### multi_dim_array - Multi-dimensional arrays

Status: unsupported
Scope: method
Notes: Multi-dimensional arrays are not supported by the Neo compiler.
```csharp
int[,] matrix = new int[2, 2];
matrix[0, 1] = 5;
```

### array_initializer - Array initializers

Status: supported
Scope: method
Notes: Array initializer syntax compiles for primitive types.
```csharp
int[] values = new[] { 1, 2, 3 };
```

### foreach_statement - Foreach statements

Status: supported
Scope: method
Notes: Foreach loops over arrays compile and work as expected.
```csharp
int total = 0;
foreach (int number in new int[] { 1, 2, 3 })
{
    total += number;
}
```

### unsafe_code - Unsafe code blocks

Status: unsupported
Scope: method
Notes: Unsafe code requires compiler switches and is rejected; the following snippet demonstrates the unsupported scenario.
```csharp
unsafe
{
    int value = 0;
    int* ptr = &value;
    *ptr = 10;
}
```

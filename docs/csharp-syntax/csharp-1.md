# C# 1 Syntax Checklist

Reference: https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-1

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

Status: supported
Scope: method
Notes: The compiler lowers rectangular arrays into nested Neo VM arrays. Creation, assignment, indexing, `??=` and `foreach` all work as expected.
```csharp
int?[,] matrix = new int?[2, 2];
matrix[0, 1] = 5;
matrix[1, 0] ??= 3;

int sum = 0;
foreach (var value in matrix)
{
    sum += value ?? 0;
}
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

### string_methods - String helper methods

Status: supported
Scope: method
Notes: Common `string` APIs such as indexing, substring, and replacement work as expected.
```csharp
string phrase = "neo compiler";
int spaceIndex = phrase.IndexOf(' ');
string prefix = phrase.Substring(0, spaceIndex);
string suffix = phrase.Substring(spaceIndex + 1);
string combined = prefix + "_" + suffix;
int totalLength = combined.Length + prefix.IndexOf("neo");
```

### stringbuilder_methods - StringBuilder helpers

Status: supported
Scope: method
Notes: Common `StringBuilder` constructors and helpers (Append/AppendLine/Clear/Length/ToString) compile and are lowered to Neo VM string concatenations.
```csharp
var builder = new System.Text.StringBuilder();
builder.Append("neo");
builder.Append(' ');
builder.AppendLine("compiler");
builder.AppendLine();
builder.Append(new System.Text.StringBuilder("runtime"));
string result = builder.ToString();
int length = builder.Length;
```

### biginteger_methods - BigInteger arithmetic helpers

Status: supported
Scope: method
Notes: `System.Numerics.BigInteger` provides arithmetic and conversion helpers that compile successfully.
```csharp
var balance = new System.Numerics.BigInteger(1_000);
balance += System.Numerics.BigInteger.Pow(2, 5);
byte[] serialized = balance.ToByteArray();
bool fits = balance < System.Numerics.BigInteger.Pow(2, 16);
```

### math_methods - Math library helpers

Status: supported
Scope: method
Notes: `System.Math` intrinsic helpers compile and can be combined in expressions.
```csharp
int value = -42;
int absolute = System.Math.Abs(value);
int squared = absolute * absolute;
int clamped = System.Math.Clamp(squared, 0, 10_000);
int max = System.Math.Max(clamped, 100);
```

### numerics_bit_operations - BitOperations helpers

Status: unsupported
Scope: method
Notes: `System.Numerics.BitOperations` helpers are trimmed from the runtime, so bit counting methods are rejected.
```csharp
uint value = 0b_0011_0000u;
int leadingZeros = System.Numerics.BitOperations.LeadingZeroCount(value);
int trailingZeros = System.Numerics.BitOperations.TrailingZeroCount(value);
int popCount = System.Numerics.BitOperations.PopCount(value);
bool hasSingleBit = System.Numerics.BitOperations.IsPow2(32);
```

### char_methods - Char helper methods

Status: supported
Scope: method
Notes: `char` classification and casing helpers compile and behave like desktop C#.
```csharp
char symbol = 'n';
bool isLetter = char.IsLetter(symbol);
bool isUpper = char.IsUpper(symbol);
char upper = char.ToUpper(symbol);
bool equalsIgnoreCase = char.ToUpper(symbol) == char.ToUpper('N');
```

### datetime_methods - DateTime helpers

Status: unsupported
Scope: method
Notes: Date and time helpers that rely on `double` parameters (such as `AddDays`/`AddHours`) are not part of the supported surface area.
```csharp
var created = new System.DateTime(2024, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
var expires = created.AddDays(1);
var ticks = created.Ticks;
```

### timespan_methods - TimeSpan helpers

Status: unsupported
Scope: method
Notes: `System.TimeSpan` factory helpers are not available; the compiler reports missing methods for APIs like `FromMinutes`.
```csharp
var interval = System.TimeSpan.FromMinutes(90);
var doubled = interval + interval;
bool longDuration = doubled.TotalHours >= 3;
```

### convert_methods - Convert class helpers

Status: unsupported
Scope: method
Notes: Most `System.Convert` helpers (including `ToInt32(string)` and hex helpers) are trimmed from the Neo runtime, so these APIs cannot be used.
```csharp
int number = System.Convert.ToInt32("42");
byte[] bytes = System.Convert.FromHexString("0A0B0C");
string hex = System.Convert.ToHexString(bytes);
bool boolean = System.Convert.ToBoolean(1);
```

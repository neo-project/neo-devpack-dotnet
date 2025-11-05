# C# 7 Syntax Checklist

The Neo compiler still needs to accommodate projects targeting the C# 7 family. This checklist documents the most common language features introduced across C# 7.0–7.3 together with their support status.

### tuple_deconstruction - Tuple deconstruction

Status: supported
Scope: method
Notes: ValueTuple deconstruction works in local scopes. Roslyn lowers tuple deconstruction into temporary assignments before Neo processes the code.
```csharp
(int a, int b) = (1, 2);
int sum = a + b;
```

### pattern_matching_is - Pattern matching with `is`

Status: unsupported
Scope: method
Notes: Type patterns in `is` expressions are rejected by the compiler. Roslyn would lower pattern matching into type checks, but Neo currently rejects this syntax.
```csharp
object value = 5;
if (value is int number)
{
    number++;
}
```

### local_function - Local functions

Status: unsupported
Scope: method
Notes: Local functions are not recognized; the compiler reports unsupported syntax. Roslyn rewrites local functions into generated helper methods, yet the Neo compiler still rejects the syntax.
```csharp
int Multiply(int left, int right)
{
    return left * right;
}

int product = Multiply(2, 3);
```

### throw_expression - Throw expressions

Status: supported
Scope: method
Notes: Throw expressions compile inside conditional operators and null-coalescing expressions. Roslyn rewrites throw expressions into explicit throw statements before Neo handles them.
```csharp
string? value = null;
string result = value ?? throw new System.ArgumentNullException(nameof(value));
```

### ref_local - Ref locals and returns

Status: unsupported
Scope: method
Notes: Ref locals are rejected; the compiler reports that ref semantics are not supported.
```csharp
int number = 5;
ref int reference = ref number;
reference = 10;
```

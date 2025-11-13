# C# 7 Syntax Checklist

Reference: https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-7

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

### ref_parameters - Ref and out parameters

Status: supported
Scope: class
Notes: Ref and out parameters are lowered into captured storage before Neo executes the method body. The compiler keeps the argument storage in sync, so assignments made through `ref`/`out` are reflected in the caller.
```csharp
public static void Update()
{
    int number = 5;
    Increment(ref number);
    bool parsed = int.TryParse("10", out var assigned);
}

private static void Increment(ref int value) => value++;
```

### ref_local - Ref locals and returns

Status: supported
Scope: method
Notes: Ref locals now bind directly to their source storage, so aliasing locals, parameters, fields, or array elements works as expected. Rebinding (`alias = ref other;`) emits binding updates rather than copying values. Ref returns are still rejected.
```csharp
int[] values = new[] { 1, 3, 5 };
ref int alias = ref values[1];
alias += 2;
ref int other = ref alias;
other = ref values[2];
other += 10;
```

### ref_return - Ref returns

Status: unsupported
Scope: method
Notes: Returning `ref` values from methods is blocked when the caller consumes the returned reference. The compiler reports that ref returns are unavailable.
```csharp
int number = 5;
ref int Select(ref int value)
{
    return ref value;
}
ref int alias = ref Select(ref number);
```

### ref_argument_array_element - `ref` argument targeting array element

Status: unsupported
Scope: class
Notes: Passing an array element directly to a `ref` parameter is still rejected. Neo requires you to bind the element to a ref local before forwarding it.
```csharp
static void Increment(ref int value) => value++;

public static void Update(int[] values)
{
    Increment(ref values[0]);
}
```

### ref_argument_span_element - `ref` argument targeting span element

Status: unsupported
Scope: class
Notes: The compiler also forbids forwarding `Span<T>` indexers as ref/out arguments. The element must flow through a ref local first to compile successfully.
```csharp
Span<int> values = new int[] { 1, 2, 3 };
static void Increment(ref int value) => value++;

public static void Update()
{
    Increment(ref values[1]);
}
```

# Neo Compiler Fuzzer Features

The Neo Compiler Fuzzer tests a wide range of C# syntax features and Neo-specific functionality. This document provides a comprehensive list of all supported features.

## Data Types

| Feature | Description | Example |
|---------|-------------|---------|
| PrimitiveTypes | Basic data types like int, bool, string | `int x = 42; bool flag = true; string text = "Hello";` |
| ComplexTypes | Neo-specific types like UInt160, UInt256, ECPoint, ByteString, Map, List | `UInt160 hash = UInt160.Zero; Map<string, int> map = new Map<string, int>();` |
| Arrays | Array declarations and operations | `int[] array = new int[5]; array[0] = 42;` |
| CharDeclaration | Character data type declarations | `char c = 'A';` |
| StructDeclaration | Structure type declarations | `struct Point { public int X; public int Y; }` |
| StructUsage | Using struct types | `Point p = new Point { X = 1, Y = 2 };` |
| TupleDeclaration | Tuple declarations and usage | `(int, string) tuple = (42, "Hello");` |
| TupleDeconstruction | Tuple deconstruction into variables | `var (x, y) = tuple;` |
| NullableTypeDeclaration | Nullable type declarations | `int? nullableInt = null;` |
| RangeAndIndexUsage | Range and index operators for collections | `int[] slice = array[1..3];` |

## Control Flow

| Feature | Description | Example |
|---------|-------------|---------|
| IfStatements | Conditional statements | `if (x > 0) { /* code */ } else { /* code */ }` |
| ForLoops | Loop constructs | `for (int i = 0; i < 10; i++) { /* code */ }` |
| SwitchStatement | Switch statements for multiple branches | `switch (x) { case 1: /* code */; break; default: /* code */; break; }` |
| SwitchExpression | Switch expressions for concise branching | `string result = x switch { 1 => "One", 2 => "Two", _ => "Other" };` |
| WhileLoop | While loops for conditional iteration | `while (condition) { /* code */ }` |
| DoWhileLoop | Do-while loops for conditional iteration | `do { /* code */ } while (condition);` |
| ForeachLoop | Foreach loops for collection iteration | `foreach (var item in collection) { /* code */ }` |
| BreakStatement | Break statements for exiting loops | `while (true) { if (condition) break; }` |
| ContinueStatement | Continue statements for skipping iterations | `for (int i = 0; i < 10; i++) { if (i % 2 == 0) continue; }` |
| TernaryOperator | Ternary conditional operator | `int result = condition ? trueValue : falseValue;` |
| PatternMatching | Pattern matching for type checking | `if (obj is string s) { /* use s */ }` |
| PropertyPatternMatching | Property pattern matching | `if (obj is Person { Name: "John", Age: 30 }) { /* code */ }` |

## Storage Operations

| Feature | Description | Example |
|---------|-------------|---------|
| StorageOperation | Basic storage operations | `Storage.Put(Storage.CurrentContext, "key", "value");` |
| StorageOperations | Enhanced storage operations | `StorageMap map = new StorageMap(Storage.CurrentContext, "prefix");` |
| StorageMapOperations | StorageMap operations | `map.Put("key", "value");` |
| StorageFindOperations | Storage.Find with various options | `var iterator = Storage.Find(Storage.CurrentContext, "prefix", FindOptions.KeysOnly);` |
| StorageContextOperations | Storage contexts | `var context = Storage.CurrentContext; var readOnlyContext = Storage.CurrentReadOnlyContext;` |
| StorageDeleteOperations | Storage delete operations | `Storage.Delete(Storage.CurrentContext, "key");` |

## Runtime Operations

| Feature | Description | Example |
|---------|-------------|---------|
| RuntimeOperation | Basic Runtime operations | `Runtime.Log("Message");` |
| RuntimeOperations | Enhanced Runtime operations | `Runtime.Notify("Event", new object[] { "arg1", 42 });` |
| RuntimeNotifications | Runtime notifications | `Runtime.Notify("Transfer", new object[] { from, to, amount });` |
| RuntimeLogging | Runtime logging | `Runtime.Log("Debug message");` |
| RuntimeCheckWitness | Runtime CheckWitness | `bool isOwner = Runtime.CheckWitness(owner);` |
| RuntimeGasOperations | Runtime gas operations | `long gas = Runtime.GasLeft;` |
| RuntimeRandomOperations | Runtime random operations | `ulong random = Runtime.GetRandom();` |

## Native Contract Calls

| Feature | Description | Example |
|---------|-------------|---------|
| NativeContractCall | Basic native contract call | `BigInteger balance = NEO.BalanceOf(owner);` |
| NativeContractCalls | Enhanced native contract calls | `bool success = GAS.Transfer(from, to, amount, data);` |
| NEO | NEO token operations | `BigInteger totalSupply = NEO.TotalSupply();` |
| GAS | GAS token operations | `BigInteger gasBalance = GAS.BalanceOf(owner);` |
| ContractManagement | Contract management operations | `UInt160 hash = ContractManagement.GetContract(UInt160.Zero).Hash;` |
| CryptoLib | Cryptographic operations | `bool verified = CryptoLib.VerifyWithECDsa(message, pubKey, signature);` |
| Ledger | Ledger operations | `uint height = Ledger.CurrentIndex;` |
| Oracle | Oracle operations | `Oracle.Request(url, filter, callback, userData, gasForResponse);` |
| Policy | Policy operations | `long fee = Policy.GetFeePerByte();` |
| RoleManagement | Role management operations | `bool isDesignated = RoleManagement.IsDesignatedByRole(Role.Oracle, pubKey, height);` |
| StdLib | Standard library operations | `string json = StdLib.JsonSerialize(obj);` |

## Neo N3 Specific Features

| Feature | Description | Example |
|---------|-------------|---------|
| NFTOperations | NFT token operations | `string symbol = NFT.Symbol; ByteString tokenId = NFT.CreateToken(owner, tokenURI, properties);` |
| NameServiceOperations | Name service operations | `UInt160 owner = NameService.GetOwner("domain.neo");` |
| EnhancedCryptography | Advanced cryptographic operations | `byte[] hash = CryptoLib.Sha256(data);` |
| AttributeUsage | Neo N3 attribute usage | `[DisplayName("MyContract")] public class Contract : SmartContract { }` |
| OracleCallback | Oracle callback implementation | `public static void OracleCallback(string url, byte[] userData, int code, byte[] result) { }` |

## Exception Handling

| Feature | Description | Example |
|---------|-------------|---------|
| TryCatch | Try-catch blocks for exception handling | `try { /* code */ } catch { /* handle error */ }` |
| TryCatchFinally | Try-catch-finally blocks | `try { /* code */ } catch { /* handle error */ } finally { /* cleanup */ }` |
| ThrowStatement | Throw statements for throwing exceptions | `throw new Exception("Error message");` |
| ThrowExpression | Throw expressions for inline exception throwing | `int x = value ?? throw new Exception("Value is null");` |
| ExceptionFilter | Exception filters for conditional exception handling | `try { /* code */ } catch (Exception ex) when (ex.Message.Contains("specific")) { /* handle specific error */ }` |

## Operators and Expressions

| Feature | Description | Example |
|---------|-------------|---------|
| ArithmeticOperators | Arithmetic operators (+, -, *, /, %) | `int sum = a + b; int product = a * b;` |
| ComparisonOperators | Comparison operators (==, !=, <, >, <=, >=) | `bool isEqual = a == b; bool isGreater = a > b;` |
| LogicalOperators | Logical operators (&&, \|\|, !) | `bool result = a && b; bool notResult = !result;` |
| BitwiseOperators | Bitwise operators (&, \|, ^, ~, <<, >>) | `int bitwiseAnd = a & b; int leftShift = a << 2;` |
| AssignmentOperators | Assignment operators (=, +=, -=, *=, /=, %=, &=, \|=, ^=, <<=, >>=) | `x += 5; y *= 2;` |
| IncrementDecrementOperators | Increment and decrement operators (++, --) | `x++; --y;` |
| CheckedUncheckedExpressions | Checked and unchecked expressions | `checked { int result = x + y; }` |
| IsAsOperators | Is and as operators for type checking and conversion | `if (obj is string) { string s = obj as string; }` |
| DefaultLiteralExpression | Default literal expressions | `int x = default;` |
| OutVariableDeclaration | Out variable declarations | `if (int.TryParse(s, out int result)) { /* use result */ }` |
| NullCoalescingOperator | Null-coalescing operator (??) | `string name = value ?? "Default";` |
| NullConditionalOperator | Null-conditional operator (?.) | `string name = person?.Name;` |
| NullCoalescingAssignment | Null-coalescing assignment operator (??=) | `name ??= "Default";` |

## String and Math Operations

| Feature | Description | Example |
|---------|-------------|---------|
| StringOperations | String operations | `int length = s.Length; string upper = s.ToUpper();` |
| StringConcatenation | String concatenation operations | `string fullName = firstName + " " + lastName;` |
| StringInterpolation | String interpolation for embedding expressions | `string message = $"Hello, {name}!";` |
| StringSplitting | String splitting operations | `string[] parts = s.Split(',');` |
| StringJoining | String joining operations | `string joined = string.Join(", ", array);` |
| CharOperations | Character operations | `char first = s[0]; bool isDigit = char.IsDigit(first);` |
| MathOperations | Math operations | `int abs = Math.Abs(x); int max = Math.Max(a, b);` |
| TypeConversionOperations | Type conversion operations | `int i = (int)doubleValue; string s = i.ToString();` |
| ByteStringOperations | ByteString operations | `byte[] bytes = byteString; int length = byteString.Length;` |

## Contract Features

| Feature | Description | Example |
|---------|-------------|---------|
| ContractAttributes | Contract attributes | `[ContractPermission("*", "*")] public class Contract : SmartContract { }` |
| ContractCalls | Contract calls | `Contract.Call(scriptHash, "method", CallFlags.All, new object[] { arg1, arg2 });` |
| StoredProperties | Stored properties | `[Storage] private Map<string, int> _balances = new Map<string, int>();` |
| ContractMethods | Contract methods | `public static bool Transfer(UInt160 from, UInt160 to, BigInteger amount) { }` |
| EventDeclarations | Event declarations | `[DisplayName("Transfer")] public static event Action<UInt160, UInt160, BigInteger> OnTransfer;` |
| EventEmissions | Event emissions | `OnTransfer(from, to, amount);` |

## Feature Dependencies and Incompatibilities

Some features have dependencies on other features or may be incompatible with certain features:

### Dependencies

- **NFTOperations** depends on **RuntimeOperation**
- **NameServiceOperations** depends on **RuntimeOperation**
- **EnhancedCryptography** depends on **RuntimeOperation**
- **OracleCallback** depends on **RuntimeOperation** and **StorageOperation**

### Incompatibilities

- **ContractAttributes** is incompatible with **AttributeUsage**

For more details on feature dependencies and incompatibilities, see the source code in the `DynamicContractFuzzer` class.

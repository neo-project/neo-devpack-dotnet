# C# Syntax Support for Neo Smart Contracts

This document serves as a comprehensive guide to C# syntax support in Neo smart contract development. While Neo allows for smart contract development using C#, it's important to note that not all C# features are fully supported or may behave differently in the context of smart contracts.

## Purpose
- To provide clarity on which C# syntaxes are supported in Neo smart contracts
- To highlight any differences or limitations compared to standard C# usage
- To serve as a reference for both experienced C# developers and those new to Neo smart contract development

## How to Use This Document
- Each C# feature or syntax element is listed with its support status
- [x] indicates full support
- [ ] indicates no support or significant limitations
- Partial support or behavior differences are noted where applicable

## Ongoing Development
We are continuously working to expand C# syntax support in Neo smart contracts. This document will be regularly updated to reflect the latest developments and improvements in Neo's C# support for smart contracts.

For any C# features not listed or marked as unsupported, please refer to the official Neo documentation or community resources for alternative approaches or workarounds in smart contract development.

C# Language Specification :  https://ecma-international.org/publications-and-standards/standards/ecma-334/


# C# Syntax Reference

## 1. Keywords

### Basic Data Types (https://github.com/neo-project/neo-devpack-dotnet/blob/master/src/Neo.Compiler.CSharp/Helper.cs)
- [x] **void**: Represents the absence of a value or a method that does not return a value.
- [x] **bool**: Declares a variable of Boolean data type. Example: `bool isReady = true;`
- [x] **byte**: Declares a variable of 8-bit unsigned integer data type. Example: `byte myByte = 100;`
- [x] **char**: Declares a variable of character data type. Example: `char myChar = 'A';`
- [ ] **decimal**: Declares a variable of decimal data type. Example: `decimal price = 19.99M;`
- [ ] **double**: Declares a variable of double-precision floating-point data type. Example: `double pi = 3.14159265359;`
- [ ] **float**: Declares a variable of single-precision floating-point data type. Example: `float price = 19.99F;`
- [x] **int**: Declares a variable of 32-bit signed integer data type. Example: `int count = 10;`
- [x] **long**: Declares a variable of 64-bit signed integer data type. Example: `long bigNumber = 1234567890L;`
- [x] **sbyte**: Declares a variable of 8-bit signed integer data type. Example: `sbyte smallNumber = -128;`
- [x] **short**: Declares a variable of 16-bit signed integer data type. Example: `short smallValue = 32767;`
- [x] **uint**: Declares a variable of 32-bit unsigned integer data type. Example: `uint positiveValue = 123;`
- [x] **ulong**: Declares a variable of 64-bit unsigned integer data type. Example: `ulong bigPositiveValue = 1234567890UL;`
- [x] **ushort**: Declares a variable of 16-bit unsigned integer data type. Example: `ushort smallPositiveValue = 65535;`
- [x] **string**: Declares a variable of string data type. Example: `string text = "Hello, world!";`
- [x] **object**: Declares a variable of object data type. Example: `object myObject = new MyType();`
- [x] **System.Numerics.BigInteger**: Represents a large integer data type. Example: `System.Numerics.BigInteger bigInt = 1234567890123456789012345678901234567890;`
- [x] **List**: Represents a list data type. Example: `List<int> numbers = new List<int> { 1, 2, 3, 4, 5 };`
- [x] **Map**: Represents a map data type. Example: `Dictionary<string, int> keyValuePairs = new Dictionary<string, int>();`
- [x] **Neo.UInt160**: Represents a 160-bit unsigned integer data type. Example: `Neo.UInt160 hash160Value = new Neo.UInt160();`
- [x] **Neo.UInt256**: Represents a 256-bit unsigned integer data type. Example: `Neo.UInt256 hash256Value = new Neo.UInt256();`


## C# to Neo smart contract type mapping table:

| C# Type | Neo Type | Description |
|-|-|-|  
| `bool` | `Boolean` | Boolean type |
| `byte` | `Integer` | 8-bit unsigned integer |
| `sbyte` | `Integer` | 8-bit signed integer |
| `short` | `Integer` | 256-bit signed integer-le |
| `ushort` | `Integer` | 256-bit signed integer-le |
| `int` | `Integer` | 256-bit signed integer-le |
| `uint` | `Integer` | 256-bit signed integer-le |
| `long` | `Integer` | 256-bit signed integer-le |
| `ulong` | `Integer` | 256-bit signed integer-le |
| `char` | `Integer` | Unicode character |
| `string` | `ByteString` | String |
| `byte[]` | `ByteArray` | Byte array |
| `BigInteger` | `Integer` | 256-bit signed integer |
| Enum types | `Integer` | Enum underlying integer mapping |
| Array types | `Array` | Array |
| `object` | `Any` | Any type |
| `void` | `Void` | No return value |
| `Neo.Cryptography.ECC.ECPoint` | `PublicKey` | Represents public key |
| `Neo.SmartContract.Framework.ByteString` | `ByteString` | Byte string |
| `Neo.UInt160` | `Hash160` | 20-byte hash value | 
| `Neo.UInt256` | `Hash256` | 32-byte hash value |
| Other classes/interfaces | `ByteArray` | Stored as byte arrays |

### Classes and Structures
- [x] **class**: Declares a class. Example: `class MyClass { }`
- [x] **struct**: Declares a value type. Example: `struct Point { public int X; public int Y; }`
- [ ] **enum**: Declares an enumeration. Example: `enum Days { Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday };`
- [x] **delegate**: Declares a delegate. Example: `delegate int MyDelegate(int x, int y);`
- [x] **interface**: Declares an interface. Example: `interface IMyInterface { /* interface members */ }`

### Control Flow Keywords
- [x] **if**: Starts an if statement. Example: `if (condition) { /* code */ }`
- [x] **else**: Defines an alternative branch in an if statement. Example: `if (condition) { /* true branch */ } else { /* else branch */ }`
- [x] **switch**: Starts a switch statement. Example: `switch (dayOfWeek) { case DayOfWeek.Monday: /* code */ break; }`
- [x] **case**: Defines individual cases in a switch statement. Example: `switch (dayOfWeek) { case DayOfWeek.Monday: Console.WriteLine("It's Monday."); break; }`
- [x] **default**: Specifies the default case in a switch statement. Example: `switch (dayOfWeek) { default: Console.WriteLine("It's another day."); break; }`
- [x] **for**: Starts a for loop. Example: `for (int i = 0; i < 10; i++) { /* loop body */ }`
- [x] **foreach**: Iterates over elements in a collection. Example: `foreach (var item in myList) { /* process item */ }`
- [x] **while**: Starts a while loop. Example: `while (condition) { /* loop body */ }`
- [x] **do**: Starts a do-while loop. Example: `do { /* loop body */ } while (condition);`
- [x] **break**: Exits a loop or switch statement prematurely. Example: `for (int i = 0; i < 10; i++) { if (i == 5) break; }`
- [x] **continue**: Jumps to the next iteration of a loop. Example: `for (int i = 0; i < 10; i++) { if (i == 5) continue; }`
- [x] **goto**: Transfers control to a labeled statement. Example: `goto MyLabel
- [x] **return**: Returns a value from a method. Example: `public int MyMethod() { return 42; }`
- [x] **throw**: Throws an exception. Example: `throw new Exception("An error occurred.");`
- [x] **try**: Starts a try-catch block. Example: `try { /* code */ } catch (Exception ex) { /* handle exception */ }`
- [x] **catch**: Catches and handles exceptions in a try-catch block. Example: `try { /* code that may throw an exception */ } catch (Exception ex) { /* handle exception */ }`
> [!WARNING]
> In Neo smart contracts, at most one catch block is allowed for each try block.
- [x] **finally**: Defines a block of code to be executed in a try-catch-finally block. Example: `try { /* code */ } catch (Exception ex) { /* handle exception */ } finally { /* cleanup code */ }`

### Access Modifiers and Member Control
- [x] **public**: Specifies access for a class or member to any code. Example: `public class MyClass { }`
- [x] **private**: Restricts access to a class or member. Example: `private int myField;`
- [x] **protected**: Specifies access for a class or member within the same class or derived classes. Example: `protected void MyMethod() { /* code */ }`
- [ ] **internal**: Specifies that a class or member is accessible within the same assembly. Example: `internal class MyInternalClass { }`
- [x] **static**: Declares a static member. Example: `public static void MyMethod() { /* code */ }`
- [x] **readonly**: Declares a read-only field. Example: `public readonly int MaxValue = 100;`
- [x] **const**: Declares a constant. Example: `const int MaxValue = 100;`
- [ ] **volatile**: Specifies that a field may be modified by multiple threads. Example: `private volatile int counter;`
- [x] **abstract**: Used to declare abstract classes or methods. Example: `abstract class Shape { }`
- [x] **sealed**: Prevents a class from being inherited. Example: `sealed class MySealedClass { }`
- [x] **override**: Overrides a base class member in a derived class. Example: `public override void MyMethod() { /* code */ }`
- [x] **virtual**: Declares a virtual member that can be overridden. Example: `public virtual void MyMethod() { /* code */ }`
- [ ] **extern**: Indicates that a method is implemented externally. Example: `extern void NativeMethod();`

### Type Conversion and Checking
- [x] **as**: Used for type casting or conversion. Example: `object obj = "Hello"; string str = obj as string;`
- [x] **is**: Checks if an object is of a specified type. Example: `if (myObject is MyClass) { /* code */ }`
- [ ] **typeof**: Gets the Type object for a type. Example: `Type type = typeof(MyClass);`
- [x] **sizeof**: Gets the size of an unmanaged type. Example: `int size = sizeof(int);`
- [x] **checked**: Enables overflow checking for arithmetic operations. Example: `checked { int result = int.MaxValue + 1; }`
- [x] **unchecked**: Disables overflow checking for arithmetic operations. Example: `unchecked { int result = int.MaxValue + 1; }`
- [ ] **implicit**: Defines an implicit user-defined type conversion operator. Example: `public static implicit operator MyType(int value) { /* conversion logic */ }`
- [ ] **explicit**: Defines a user-defined type conversion operator. Example: `public static explicit operator int(MyClass myObj) { /* conversion logic */ }`

### Parameters and Methods
- [ ] **ref**: Passes a parameter by reference. Example: `public void MyMethod(ref int value) { /* code */ }`
- [x] **out**: Indicates that a parameter is passed by reference. Example: `public void MyMethod(out int result) { /* code */ }`
- [x] **params**: Specifies a variable-length parameter list. Example: `public void MyMethod(params int[] numbers) { /* code */ }`
- [x] **this**: Refers to the current instance of a class. Example: `this.myField = 42;`
- [x] **base**: Used to access members of the base class. Example: `base.MethodName();`
- [x] **new**: Creates an object or hides a member. Example: `new MyType();`

### Special Keywords
- [ ] **operator**: Declares an operator. Example: `public static MyType operator +(MyType a, MyType b) { /* operator logic */ }`
- [x] **event**: Declares an event. Example: `public event EventHandler MyEvent;`
- [ ] **lock**: Defines a synchronized block of code. Example: `lock (myLockObject) { /* code */ }`
- [ ] **fixed**: Specifies a pointer to a fixed memory location. Example: `fixed (int* ptr = &myVariable) { /* code */ }`
- [ ] **unsafe**: Allows the use of unsafe code blocks. Example: `unsafe { /* unsafe code */ }`
- [x] **in**: Specifies the iteration variable in a foreach loop. Example: `foreach (var item in myCollection) { /* code */ }`
- [x] **null**: Represents a null value. Example: `object obj = null;`
- [x] **true**: Represents the Boolean true value. Example: `bool isTrue = true;`
- [x] **false**: Represents the Boolean false value. Example: `bool isTrue = false;`
- [ ] **stackalloc**: Allocates memory on the stack. Example: `int* stackArray = stackalloc int[100];`
- [ ] **using**: Defines a scope for a resource and ensures its disposal. Example: `using (var resource = new MyResource()) { /* code */ }`


### Contextual Keywords

- [ ] **add**: Used in property declarations to add an event handler. Example:
  ```csharp
  public event EventHandler MyEvent
  {
      add { /* Logic to add event handler */ }
      remove { /* Logic to remove event handler */ }
  }
  ```
- [ ] **alias**: Used to provide a namespace or type alias. Example:
  ```csharp
  using MyAlias = MyNamespace.MyLongNamespaceName;
  ```
- [ ] **ascending**: Used in LINQ queries to specify ascending sorting. Example:
  ```csharp
  var sortedNumbers = numbers.OrderBy(x => x);
  ```
- [ ] **async**: Marks a method as asynchronous.

  Example:
  ```csharp
  public async Task MyAsyncMethod()
  {
      // Asynchronous operation
  }
  ```
- [ ] **await**: Suspends the execution of an asynchronous method until the awaited task completes. Example:
  ```csharp
  async Task MyMethodAsync()
  {
      int result = await SomeAsyncOperationAsync();
      // Continue with subsequent operations
  }
  ```
- [ ] **by**: Used in a `join` clause in LINQ queries to specify the join condition. Example:
  ```csharp
  var query = from person in people
              join city in cities on person.CityId equals city.Id
              select new { person.Name, city.CityName };
  ```
- [ ] **descending**: Used in LINQ queries to specify descending sorting. Example:
  ```csharp
  var sortedNumbers = numbers.OrderByDescending(x => x);
  ```
- [ ] **dynamic**: Declares a dynamic type that bypasses compile-time type checking. Example:
  ```csharp
  dynamic dynamicVar = 42;
  Console.WriteLine(dynamicVar); // Outputs 42
  ```
- [ ] **equals**: Used in pattern matching to check if a value matches a specified pattern. Example:
  ```csharp
  if (value is 42 equals int intValue)
  {
      // If the value equals 42, cast it to an int intValue
  }
  ```
- [ ] **from**: Used in LINQ queries to specify the data source. Example:
  ```csharp
  var query = from person in people
              select person.Name;
  ```
- [x] **get**: Defines a property's getter method. Example:
  ```csharp
  public int MyProperty
  {
      get { return myField; }
  }
  ```
- [ ] **global**: Used to reference items from the global namespace. Example:
  ```csharp
  global::System.Console.WriteLine("Hello, world!");
  ```
- [ ] **group**: Used in a `group by` clause in LINQ queries. Example:
  ```csharp
  var query = from person in people
              group person by person.City into cityGroup
              select new { City = cityGroup.Key, Count = cityGroup.Count() };
  ```
- [ ] **into**: Used in LINQ queries to create a new range variable. Example:
  ```csharp
  var query = from person in people
              group person by person.City into cityGroup
              where cityGroup.Count() > 2
              select new { City = cityGroup.Key, Count = cityGroup.Count() };
  ```
- [ ] **join**: Used in LINQ queries to perform a join operation. Example:
  ```csharp
  var query = from person in people
              join city in cities on person.CityId equals city.Id
              select new { person.Name, city.CityName };
  ```
- [ ] **let**: Used in a `let` clause in LINQ queries to define a new variable. Example:
  ```csharp
  var query = from person in people
              let fullName = $"{person.FirstName} {person.LastName}"
              select fullName;
  ```
- [x] **nameof**: Returns the name of a variable, type, or member as a string. Example:
  ```csharp
  string propertyName = nameof(MyClass.MyProperty);
  ```
- [ ] **on**: Used in a `join` clause in LINQ queries to specify the join condition. Example:
  ```csharp
  var query = from person in people
              join city in cities
              on new { person.CityId, person.CountryId }
              equals new { city.CityId, city.CountryId }
              select new { person.Name, city.CityName };
  ```
- [ ] **orderby**: Used in LINQ queries to specify sorting. Example:
  ```csharp
  var sortedNumbers = numbers.OrderBy(x => x);
  ```
- [x] **partial**: Indicates that a class, struct, or interface is defined in multiple files. Example:
  ```csharp
  partial class MyClass
  {
      // Definition in the first file
  }
  ```
- [ ] **remove**: Used in property declarations to remove an event handler. Example:
  ```csharp
  public event EventHandler MyEvent
  {
      remove { /* Logic to remove event handler */ }
  }
  ```
- [ ] **select**: Used in LINQ queries to specify the projection. Example:
  ```csharp
  var query = from person in people
              select new { person.Name, person.Age };
  ```
- [x] **set**: Defines a property's setter method. Example:
  ```csharp
  public int MyProperty
  {
      set { myField = value; }
  }
  ```
- [ ] **unmanaged**: Used to specify an unmanaged generic type constraint. Example:
  ```csharp
  public void MyMethod<T>() where T : unmanaged
  {
      // Implementation of the generic method
  }
  ```
- [x] **value**: Refers to the value of a property or indexer within a property or indexer accessor. Example:
  ```csharp
  public int MyProperty
  {
      get { return myField; }
      set { myField = value; }
  }
  ```
- [x] **var**: Declares an implicitly typed local variable. Example:
  ```csharp
  var number = 42;
  ```
- [x] **when**: Used in a `catch` clause to specify an exception filter. Example:
  ```csharp
  try
  {
      // Code block
  }
  catch (Exception ex) when (ex.InnerException != null)
  {
      // Exception filter
  }
  ```

- [ ] **where**: Used in LINQ queries to specify filtering criteria. Example:
  ```csharp
  var query = from person in people
              where person.Age > 18
              select person.Name;
  ```
- [ ] **yield**: Used to return values from an iterator method. Example:
  ```csharp
  public IEnumerable<int> GetNumbers()
  {
      yield return 1;
      yield return 2;
      yield return 3;
  }
  ```

## 2. Operators (https://github.com/neo-project/neo-devpack-dotnet/blob/master/src/Neo.Compiler.CSharp/MethodConvert.cs)

### Arithmetic Operators
- [x] `+`: Adds two operands. Example: `int result = 5 + 3; // result is 8`
- [x] `-`: Represents subtraction when used between two numeric values, such as `x - y`. Example: `int result = 10 - 5; // result is 5`
- [x] `-`: Represents the negation operator when used as a unary operator before a numeric value to make it negative, such as `-x`. Example: `int negativeValue = -10; // negativeValue is -10`
- [x] `*`: Multiplies two operands. Example: `int result = 7 * 2; // result is 14`
- [x] `/`: Divides the first operand by the second. Example: `int result = 16 / 4; // result is 4`
- [x] `%`: Returns the remainder of the division. Example: `int result = 17 % 5; // result is 2`

### Comparison Operators
- [x] `==`: Checks if two values are equal. Example: `bool isEqual = (x == y);`
- [x] `!=`: Checks if two values are not equal. Example: `bool isNotEqual = (x != y);`
- [x] `<`: Checks if the first value is less than the second. Example: `bool isLess = (x < y);`
- [x] `>`: Checks if the first value is greater than the second. Example: `bool isGreater = (x > y);`
- [x] `<=`: Checks if the first value is less than or equal to the second. Example: `bool isLessOrEqual = (x <= y);`
- [x] `>=`: Checks if the first value is greater than or equal to the second. Example: `bool isGreaterOrEqual = (x >= y);`

### Logical Operators
- [x] `&&`: Logical AND operator. Returns true if both operands are true. Example: `bool result = (x && y);`
- [x] `||`: Logical OR operator. Returns true if at least one operand is true. Example: `bool result = (x || y);`
- [x] `!`: Logical NOT operator. Inverts the value of the operand. Example: `bool result = !x;`

### Bitwise Operators
- [x] `&`: Bitwise AND operator. Performs a bitwise AND operation between two integers. Example: `int result = x & y;`
- [x] `|`: Bitwise OR operator. Performs a bitwise OR operation between two integers. Example: `int result = x | y;`
- [x] `^`: Bitwise XOR operator. Performs a bitwise XOR operation between two integers. Example: `int result = x ^ y;`
- [x] `~`: Bitwise NOT operator. Inverts the bits of an integer. Example: `int result = ~x;`
- [x] `<<`: Left shift operator. Shifts the bits of an integer to the left. Example: `int result = x << 2;`
- [x] `>>`: Right shift operator. Shifts the bits of an integer to the right. Example: `int result = x >> 2;`

### Assignment Operators
- [x] `=`: Assigns the value of the right operand to the left operand. Example: `x = 10;`
- [x] `+=`: Adds the right operand to the left operand and assigns the result. Example: `x += 5;`
- [x] `-=`: Subtracts the right operand from the left operand and assigns the result. Example: `x -= 3;`
- [x] `*=`: Multiplies the left operand by the right operand and assigns the result. Example: `x *= 2;`
- [x] `/=`: Divides the left operand by the right operand and assigns the result. Example: `x /= 4;`
- [x] `%=`: Calculates the remainder of the left operand divided by the right operand and assigns the result. Example: `x %= 3;`
- [x] `&=`: Performs a bitwise AND operation between the left and right operands and assigns the result. Example: `x &= y;`
- [x] `|=`: Performs a bitwise OR operation between the left and right operands and assigns the result. Example: `x |= y;`
- [x] `^=`: Performs a bitwise XOR operation between the left and right operands and assigns the result. Example: `x ^= y;`
- [x] `<<=`: Shifts the bits of the left operand to the left by the number of positions specified by the right operand and assigns the result. Example: `x <<= 3;`
- [x] `>>=`: Shifts the bits of the left operand to the right by the number of positions specified by the right operand and assigns the result. Example: `x >>= 2;`
- [x] `??=`: Assigns the value of the right operand to the left operand only if the left operand is `null`. Example: `x ??= defaultValue;`

### Other Operators
- [x] `??`: Null coalescing operator. Returns the left operand if it is not null; otherwise, returns the right operand. Example: `string result = x ?? "default";`
- [x] `?.`: Null-conditional operator. Allows accessing members of an object if the object is not null. Example: `int? length = text?.Length;`
- [x] `?:`: Ternary conditional operator. Returns one of two values based on a condition. Example: `int result = (condition) ? x : y;`
- [x] `=>`: Lambda operator. Used to define lambda expressions. Example: `(x, y) => x + y`
- [x] `++`: Increment operator. Increases the value of a variable by 1. Example: `x++;`
- [x] `--`: Decrement operator. Decreases the value of a variable by 1. Example: `x--;`
- [ ] `->`: Pointer member access operator. Used in unsafe code to access members of a structure or class through a pointer. Example in unsafe code: ptr->member;

## 3. Data Types

### Value Types
- [x] `int`: Represents a 32-bit signed integer. Example: `int number = 42;`
- [x] `long`: Represents a 64-bit signed integer. Example: `long bigNumber = 1234567890L;`
- [ ] `float`: Represents a single-precision floating-point number. Example: `float price = 19.99F;`
- [ ] `double`: Represents a double-precision floating-point number. Example: `double pi = 3.14159265359;`
- [ ] `decimal`: Represents a decimal number with high precision. Example: `decimal price = 19.99M;`
- [x] `char`: Represents a Unicode character. Example: `char letter = 'A';`
- [x] `bool`: Represents a Boolean value (true or false). Example: `bool isTrue = true;`
- [x] `byte`: Represents an 8-bit unsigned integer. Example: `byte smallNumber = 100;`
- [x] `sbyte`: Represents an 8-bit signed integer. Example: `sbyte smallNumber = -128;`
- [x] `short`: Represents a 16-bit signed integer. Example: `short smallValue = 32767;`
- [x] `ushort`: Represents a 16-bit unsigned integer. Example: `ushort smallPositiveValue = 65535;`
- [x] `uint`: Represents a 32-bit unsigned integer. Example: `uint positiveValue = 123;`
- [x] `ulong`: Represents a 64-bit unsigned integer. Example: `ulong bigPositiveValue = 1234567890UL;`
- [ ] (Will Support) `enum`: Represents an enumeration. Example: `enum Days { Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday };`
- [x] `struct`: Represents a value type. Example: `struct Point { public int X; public int Y; }`

### Reference Types
- [x] `string`: Represents a sequence of characters. Example: `string text = "Hello, world!";`
- [x] `object`: Represents a base type for all types in C#. Example: `object obj = new MyType();`
- [x] `class`: Represents a reference type (class definition). Example: `class MyClass { }`
- [ ] `interface`: Represents an interface. Example: `interface IMyInterface { /* interface members */ }`
- [x] `delegate`: Represents a delegate. Example: `delegate int MyDelegate(int x, int y);`
> [!WARNING]
> In Neo smart contracts, delegate is used to define events for the contract.
- [ ] `dynamic`: Represents a dynamically-typed object. Example: `dynamic dynVar = 42;`
- [x] `array`: Represents an array. Example: `int[] numbers = { 1, 2, 3, 4, 5 };`

## 4. Syntactic Sugar and Advanced Features

- [x] **Auto Properties**: Used to simplify property declaration with getter and setter methods. Example: `public int MyProperty { get; set; }`
- [x] **Object and Collection Initializers**: Allow concise initialization of objects and collections. Example: 
  ```csharp
  var person = new Person
  {
      FirstName = "John",
      LastName = "Doe",
      Age = 30
  };
  ```
- [x] **String Interpolation**: Provides a more readable way to format strings with embedded expressions. Example: `string name = $"Hello, {firstName} {lastName}!";`
- [ ] **Anonymous Types**: Allow the creation of objects with dynamically defined properties. Example: `var person = new { Name = "John", Age = 30 };`
- [x] **Lambda Expressions**: Enable the creation of inline delegate functions. Example: `(x, y) => x + y`
- [ ] **LINQ Queries**: Provide a language-integrated query syntax for collections. Example: 
  ```csharp
  var result = from num in numbers
               where num % 2 == 0
               select num;
  ```
- [ ] **Extension Methods**: Allow adding new methods to existing types without modifying them. Example: `public static string Reverse(this string str) { /* code */ }`
- [ ] **Generics**: Enable type parameterization to create reusable data structures and algorithms. Example: `public class List<T> { /* code */ }`
- [ ] **Asynchronous Programming (`async`, `await`)**: Facilitate non-blocking code execution. Example: 
  ```csharp
  public async Task<int> GetDataAsync()
  {
      // asynchronous code
  }
  ```
- [x] **Pattern Matching**: Simplify conditional code with pattern-based matching. Example: 
  ```csharp
  if (obj is string text)
  {
      // code using 'text' as a string
  }
  ```
- [x] **Tuples**: Allow grouping multiple values into a single entity. Example: `(int, string) result = (42, "Hello");`
- [ ] **Local Functions**: Define functions within a method for local scope. Example: 
  ```csharp
  int CalculateSum(int a, int b)
  {
      int Add() => a + b;
      return Add();
  }
  ```
- [ ] **Record Types (C# 9.0+)**: Simplify the creation of classes for holding data. Example: 
  ```csharp
  public record Person(string FirstName, string LastName);
  ```
- [x] **Nullable Reference Types (C# 8.0+)**: Enhance null safety by introducing nullable annotations. Example: `string? nullableString = null;`
- [x] **Ranges and Indices (C# 8.0+)**: Allow slicing and indexing collections with a more expressive syntax. Example: `var subArray = myArray[1..4];`

## 5. Other Features

- [ ] **Properties and Indexers**: Provide access to class members with getter and setter methods. Example: `public int MyProperty { get; set; }`
- [x] **Delegates and Events**: Enable the creation of delegate types and event handlers. Example: 
  ```csharp
  public delegate void MyDelegate(string message);
  public event MyDelegate MyEvent;
  ```
- [x] **Exception Handling**: Allows catching and handling exceptions in code. Example: 
  ```csharp
  try
  {
      // code that may throw an exception
  }
  catch (Exception ex)
  {
      // handle exception
  }
  ```
  > [!WARNING]
  > In Neo smart contracts, at most one catch block is allowed for each try block.
- [ ] **Reflection**: Provides information about types and objects at runtime. Example: 
  ```csharp
  Type type = typeof(MyClass);
  MethodInfo method = type.GetMethod("MyMethod");
  ```
- [x] **Attributes**: Add metadata to code elements. Example: 
  ```csharp
  [Obsolete("This method is obsolete. Use NewMethod instead.")]
  public void OldMethod()
  {
      // code
  }
  ```
- [x] **Namespaces**: Organize code into logical groupings. Example: `namespace MyNamespace { /* code */ }`
- [ ] **Threads and Concurrency**: Manage concurrent execution using threads and tasks. Example: 
  ```csharp
  Task.Run(() =>
  {
      // code to run asynchronously
  });
  ```
- [ ] **Memory Management and Pointers**: Allow low-level memory manipulation using pointers (unsafe code). Example: 
  ```csharp
  unsafe
  {
      int* ptr = &myVariable;
      // code using pointer
  }
  ```
- [ ] **File and Stream Operations**: Provide functionality for working with files and streams. Example: 
  ```csharp
  using (var stream = File.OpenRead("file.txt"))
  {
      // code to read from stream
  }
  ```
- [ ] **Network Programming**: Enable communication over networks. Example: 
  ```csharp
  using (var client = new TcpClient())
  {
      // code for network communication
  }
  ```

## 6. Compilation Directives and Special Symbols

### Compilation Directives
- [ ] `#if`: Conditional compilation based on preprocessor symbols.
- [ ] `#else`: Specifies alternative code for `#if` conditions.
- [ ] `#elif`: Specifies additional conditions for `#if` blocks.
- [ ] `#endif`: Ends conditional compilation blocks.
- [ ] `#define`: Defines preprocessor symbols.
- [ ] `#undef`: Undefines preprocessor symbols.
- [ ] `#warning`: Generates a warning message.
- [ ] `#error`: Generates an error message.
- [ ] `#line`: Specifies line number and file name information.
- [ ] `#region`: Marks the start of a collapsible code region.
- [ ] `#endregion`: Marks the end of a collapsible code region.
- [ ] `#nullable`: Specifies nullability annotations.

### Special Symbols
- [ ] `@` (Used for non-escaped strings or keywords as identifiers)

**Numeric Type Conversions:**
- [x] `sbyte` to `byte`, `ushort`, `uint`, `ulong`, `BigInteger`
- [x] `short` to `sbyte`, `byte`, `ushort`, `uint`, `ulong`, `BigInteger`
- [x] `int` to `sbyte`, `short`, `byte`, `ushort`, `uint`, `ulong`, `BigInteger`
- [x] `long` to `sbyte`, `short`, `int`, `byte`, `ushort`, `uint`, `ulong`, `BigInteger`
- [x] `byte` to `sbyte`, `BigInteger`
- [x] `ushort` to `sbyte`, `short`, `byte`, `BigInteger`
- [x] `uint` to `sbyte`, `short`, `int`, `byte`, `ushort`, `BigInteger`
- [x] `ulong` to `sbyte`, `short`, `int`, `long`, `byte`, `ushort`, `uint`, `BigInteger`
- [x] `char` to `sbyte`, `byte`, `short`, `ushort`, `int`, `uint`, `long`, `ulong`, `BigInteger`
- [x] `sbyte`, `byte`, `short`, `ushort`, `int`, `uint`, `long`, `ulong` to `char`
- [x] `BigInteger` to `sbyte`, `byte`, `short`, `ushort`, `int`, `uint`, `long`, `ulong`

**String Conversions:**
- [x] `ByteString` to `ECPoint`
- [x] `ByteString` to `UInt160`
- [x] `ByteString` to `UInt256`
- [x] `ByteString` to `BigInteger`

**Casting and Explicit Conversions:**
- [x] Using explicit casting operators to convert between numeric types
- [x] Using explicit casting for `ByteString` to `ECPoint`, `UInt160`, and `UInt256`
- [x] Using explicit casting for numeric types to and from `BigInteger`

**Custom Type Conversions:**
- [x] Converting `ByteString` to custom types like `ECPoint`, `UInt160`, and `UInt256`
- [x] Converting between `BigInteger` and other numeric types

**Other Conversions:**
- [ ] `int` to `double`
- [ ] `float` to `double`
- [ ] `decimal` to `double`
- [ ] `string` to `int`, `double`, `DateTime`, `bool`
- [ ] `bool` to `int`, `string`
- [ ] Enum conversions
- [ ] Nullable type conversions
- [ ] Array conversions
- [ ] Object to specific type conversions
- [ ] DateTime conversions
- [ ] Base type conversions (polymorphism)
- [ ] Interface implementations and conversions


# Common Numeric Type Methods in C#

The following methods are supported for byte, sbyte, short, ushort, int, uint, long, ulong, and BigInteger types (unless otherwise specified):

- [x] Explicit casting to and from BigInteger
- [x] `RotateLeft(x, y)`: Rotates the bits in a value left by a specified amount
- [x] `RotateRight(x, y)`: Rotates the bits in a value right by a specified amount
- [x] `IsEvenInteger(x)`: Determines if a number is even
- [x] `IsOddInteger(x)`: Determines if a number is odd
- [x] `IsPow2(x)`: Determines if a number is a power of 2
- [x] `LeadingZeroCount(x)`: Returns the number of leading zero bits in the binary representation
- [x] `Log2(x)`: Returns the base-2 logarithm of a specified number
- [x] `Sign(x)`: Returns the sign of the number (-1 for negative, 0 for zero, 1 for positive)
- [x] `DivRem(x, y)`: Calculates both the quotient and remainder of a division
- [x] `Clamp(value, min, max)`: Restricts a value to be within a specified range
- [x] `CopySign(x, y)`: Returns a value with the magnitude of x and the sign of y (for signed types only)
- [x] `CreateChecked(x)`: Creates a value from another numeric type with overflow checking
- [x] `CreateSaturating(x)`: Creates a value from another numeric type with saturation
- [x] `ToString()`: Converts a numeric value to its string representation
- [x] `Equals(x, y)`: Determines whether two instances of a type are equal

Additional methods for signed types (sbyte, short, int, long, BigInteger):
- [x] `IsNegative(x)`: Determines if a number is negative
- [x] `IsPositive(x)`: Determines if a number is positive

Nullable type methods (for byte?, sbyte?, short?, ushort?, int?, uint?, long?, ulong?, BigInteger?):
- [x] `HasValue`: Determines whether the nullable instance has a value
- [x] `GetValueOrDefault()`: Returns the value if it exists, or the default value of the underlying type
- [x] `Value`: Gets the value of the nullable type (throws an exception if HasValue is false)
- [x] `ToString()`: Returns the string representation of the value, or an empty string if the value is null
- [x] `Equals(object? obj)`: Determines whether the nullable instance is equal to another object

Parsing methods:
- [x] `Parse(string s)`: Converts the string representation of a number to its numeric equivalent
- [x] `TryParse(string? s, out result)`: Attempts to parse a string to a numeric value

Note: These methods are supported for all the mentioned numeric types unless otherwise specified. BigInteger has some additional methods not listed here.

# Boolean (bool) Methods in C#

The following methods are supported for bool and bool? types:

Regular bool methods:
- [x] `ToString()`: Converts the boolean value to its string representation
- [x] `Equals(object? obj)`: Determines whether the bool instance is equal to another object
- [x] `Parse(string value)`: Converts the string representation of a boolean to its bool equivalent

Nullable bool (bool?) methods:
- [x] `HasValue`: Determines whether the nullable bool instance has a value
- [x] `GetValueOrDefault()`: Returns the value if it exists, or false if the value is null
- [x] `Value`: Gets the value of the nullable bool (throws an exception if HasValue is false)
- [x] `ToString()`: Returns the string representation of the bool value, or an empty string if the value is null
- [x] `Equals(object? obj)`: Determines whether the nullable bool instance is equal to another object

Parsing methods:
- [x] `bool.TryParse(string? value, out bool result)`: Attempts to parse a string to a bool value, with the result as an out parameter

Note: These methods are supported for both bool and bool? types unless otherwise specified.

# Common Mathematical Methods and Functions in C#

**Basic Arithmetic Operations:**
- [x] `Math.Add(x, y)`: Adds two numbers `x` and `y`.
- [x] `Math.Subtract(x, y)`: Subtracts `y` from `x`.
- [x] `Math.Multiply(x, y)`: Multiplies two numbers `x` and `y`.
- [x] `Math.Divide(x, y)`: Divides `x` by `y`.

**Exponentiation and Logarithms:**
- [x] `Math.Pow(x, y)`: Returns `x` raised to the power of `y`.
- [ ] `Math.Sqrt(x)`: Calculates the square root of `x`.
- [ ] `Math.Log(x)`: Returns the natural logarithm of `x`.
- [ ] `Math.Log10(x)`: Returns the base 10 logarithm of `x`.

**Trigonometric Functions:**
- [ ] `Math.Sin(x)`: Returns the sine of the angle `x` in radians.
- [ ] `Math.Cos(x)`: Returns the cosine of the angle `x` in radians.
- [ ] `Math.Tan(x)`: Returns the tangent of the angle `x` in radians.
- [ ] `Math.Asin(x)`: Returns the arcsine (inverse sine) in radians.
- [ ] `Math.Acos(x)`: Returns the arccosine (inverse cosine) in radians.
- [ ] `Math.Atan(x)`: Returns the arctangent (inverse tangent) in radians.
- [ ] `Math.Atan2(y, x)`: Returns the angle whose tangent is the quotient of `y` and `x`.

**Rounding and Precision:**
- [ ] `Math.Round(x)`: Rounds a number `x` to the nearest integer.
- [ ] `Math.Floor(x)`: Rounds `x` down to the nearest integer.
- [ ] `Math.Ceiling(x)`: Rounds `x` up to the nearest integer.
- [ ] `Math.Truncate(x)`: Truncates `x` to an integer by removing its fractional part.
- [ ] `Math.Round(x, decimals)`: Rounds `x` to a specified number of decimal places.

**Absolute and Sign:**
- [x] `Math.Abs(x)`: Returns the absolute (non-negative) value of `x`.
- [x] `Math.Sign(x)`: Returns the sign of `x` (-1 for negative, 0 for zero, 1 for positive).

**Constants:**
- [ ] `Math.PI`: Represents the mathematical constant π (pi).
- [ ] `Math.E`: Represents the mathematical constant e (the base of natural logarithms).

**Additional Functions:**
- [x] `Math.Max(x, y)`: Returns the larger of two numbers `x` and `y`.
- [x] `Math.Min(x, y)`: Returns the smaller of two numbers `x` and `y`.
- [x] `Math.Clamp(x, min, max)`: Limits the value of `x` to be within the range `[min, max]`.
- [x] `Math.DivRem(x, y)`: Returns the quotient and remainder of `x` divided by `y`.
- [x] `Math.BigMul(x, y)`: Returns the full product of two 32-bit numbers.

**BigInteger Operations:**
- [x] `BigInteger.Abs(x)`: Returns the absolute value of a BigInteger.
- [x] `BigInteger.Max(x, y)`: Returns the larger of two BigInteger values.
- [x] `BigInteger.Min(x, y)`: Returns the smaller of two BigInteger values.

**Random Number Generation:**
- [ ] `Random random = new Random()`: Creating a random number generator instance.
- [ ] `random.Next()`: Generates a random integer.
- [ ] `random.Next(min, max)`: Generates a random integer between `min` (inclusive) and `max` (exclusive).
- [ ] `random.NextDouble()`: Generates a random double value between 0.0 and 1.0.

# Common String Methods in C#
1. **String Length:**
   - [x] `string.Length`: Returns the length (number of characters) of a string.
2. **Substring Extraction:**
   - [x] `string.Substring(startIndex)`: Returns a substring starting from the specified `startIndex` to the end of the string.
   - [x] `string.Substring(startIndex, length)`: Returns a substring starting from the specified `startIndex` with the specified `length`.
3. **Concatenation:**
   - [x] `string.Concat(str1, str2, ...)`: Concatenates multiple strings together.
4. **String Interpolation:**
   - [x] `$"..."`: Allows you to embed expressions and variables directly within a string.
5. **String Formatting:**
   - [ ] `string.Format(format, arg0, arg1, ...)`: Formats a string with placeholders and values.
6. **String Comparison:**
   - [x] `string.Equals(str1, str2)`: Compares two strings for equality.
   - [ ] `string.Compare(str1, str2)`: Compares two strings and returns a value indicating their relative order.
7. **String Searching and Checking:**
   - [x] `string.Contains(substring)`: Checks if a string contains a specified substring.
   - [x] `string.StartsWith(prefix)`: Checks if a string starts with a specified prefix.
   - [x] `string.EndsWith(suffix)`: Checks if a string ends with a specified suffix.
8. **String Manipulation:**
   - [x] `string.ToUpper()`: Converts the string to uppercase.
   - [x] `string.ToLower()`: Converts the string to lowercase.
   - [x] `string.Trim()`: Removes leading and trailing whitespace.
9. **String Splitting:**
   - [x] `string.Split(delimiters)`: Splits a string into an array of substrings based on specified delimiters.
10. **String Replacement:**
    - [x] `string.Replace(oldValue, newValue)`: Replaces all occurrences of `oldValue` with `newValue`.
11. **String Empty or Null Check:**
    - [x] `string.IsNullOrEmpty(str)`: Checks if a string is either `null` or empty.
    - [x] `string.IsNullOrWhiteSpace(str)`: Checks if a string is `null`, empty, or contains only whitespace.

12. **Additional String Methods:**
    - [x] `string.IndexOf(value)`: Returns the index of the first occurrence of a specified value in the string.
    - [x] `string.LastIndexOf(value)`: Returns the index of the last occurrence of a specified value in the string.
    - [x] `string.PadLeft(totalWidth)`: Returns a new string padded to the left with spaces.
    - [x] `string.PadRight(totalWidth)`: Returns a new string padded to the right with spaces.
    - [x] `string.Remove(startIndex, count)`: Returns a new string with a specified number of characters removed from a specified position.
    - [x] `string.Insert(startIndex, value)`: Returns a new string with a specified string inserted at a specified position.
    - [x] `string.Join(separator, values)`: Concatenates the elements of an array, using the specified separator between each element.
    - [x] `string.ToCharArray()`: Converts the string to a character array.

13. **String Parsing:**
    - [x] Various `TryParse` methods for different numeric types (e.g., `int.TryParse`, `long.TryParse`, etc.)

14. **String Conversion:**
    - [x] `ToString()` methods for various types (e.g., `int.ToString()`, `bool.ToString()`, etc.)

# Common Char Methods and Properties in C#
## Character Representation
- [x] `char c`: Represents a single Unicode character.

## Char Comparison
- [x] `char.Equals(char1, char2)`: Compares two characters for equality.
- [ ] `char.CompareTo(char1, char2)`: Compares two characters and returns a value indicating their relative order.

## Char Conversions
- [x] `char.GetNumericValue(char)`: Converts a numeric character to its double-precision floating-point equivalent.
- [ ] `char.GetUnicodeCategory(char)`: Returns the Unicode category of a character.

## Char Testing
- [x] `char.IsDigit(char)`: Checks if a character represents a digit.
- [x] `char.IsLetter(char)`: Checks if a character is a letter.
- [x] `char.IsWhiteSpace(char)`: Checks if a character is whitespace.
- [x] `char.IsLower(char)`: Checks if a character is lowercase.
- [x] `char.IsUpper(char)`: Checks if a character is uppercase.
- [x] `char.IsLetterOrDigit(char)`: Checks if a character is a letter or a digit.
- [x] `char.IsControl(char)`: Checks if a character is a control character.
- [x] `char.IsPunctuation(char)`: Checks if a character is a punctuation mark.
- [x] `char.IsSeparator(char)`: Checks if a character is a separator.
- [x] `char.IsSymbol(char)`: Checks if a character is a symbol.
- [x] `char.IsAscii(char)`: Checks if a character is within the ASCII character range.

## Char Case Conversions
- [x] `char.ToLower(char)`: Converts a character to lowercase.
- [x] `char.ToUpper(char)`: Converts a character to uppercase.

## Additional Methods
- [x] `char.Parse(string)`: Converts the string representation of a character to its Unicode character equivalent.
- [x] `char.TryParse(string, out char)`: Tries to convert the string representation of a character to its Unicode character equivalent.
- [x] `char.ConvertFromUtf32(int)`: Converts the specified Unicode code point into a UTF-16 encoded string.
- [x] `char.ConvertToUtf32(char, char)`: Converts the value of a UTF-16 encoded surrogate pair into a Unicode code point.
- [x] `char.GetUnicodeCategory(string, int)`: Categorizes a character in a specified position in a string into a group identified by one of the UnicodeCategory values.
- [x] `char.IsHighSurrogate(char)`: Indicates whether the specified Char object is a high surrogate.
- [x] `char.IsLowSurrogate(char)`: Indicates whether the specified Char object is a low surrogate.
- [x] `char.IsSurrogate(char)`: Indicates whether the specified Char object is a surrogate.
- [x] `char.IsSurrogatePair(char, char)`: Indicates whether the two specified Char objects form a surrogate pair.

# Common LINQ Query Methods and Operations in C#

LINQ is a powerful language feature in C# that allows you to query and manipulate collections of data. Here are some common LINQ methods and operations:

## Query Methods

1. [ ] **`Where`**: Filters a sequence of elements based on a given condition.
   - Example: `var result = numbers.Where(x => x > 5);`

2. [ ] **`Select`**: Transforms each element of a sequence into a new form.
   - Example: `var result = names.Select(name => name.ToUpper());`

3. [ ] **`OrderBy`** and **`OrderByDescending`**: Sorts elements in ascending or descending order based on a specified key.
   - Example: `var result = numbers.OrderBy(x => x);`

4. [ ] **`GroupBy`**: Groups elements based on a key and returns groups of elements.
   - Example: `var result = products.GroupBy(product => product.Category);`

5. [ ] **`Join`**: Combines two collections based on a common key.
   - Example: `var result = customers.Join(orders, customer => customer.Id, order => order.CustomerId, (customer, order) => new { customer.Name, order.OrderDate });`

6. [ ] **`Any`** and **`All`**: Checks if any or all elements in a sequence satisfy a condition.
   - Example: `bool anyPositive = numbers.Any(x => x > 0);`

7. [ ] **`First`**, **`FirstOrDefault`**, **`Last`**, and **`LastOrDefault`**: Retrieves the first or last element in a sequence, optionally with a condition.
   - Example: `var firstPositive = numbers.First(x => x > 0);`

8. [ ] **`Single`**, **`SingleOrDefault`**: Retrieves the single element in a sequence that satisfies a condition.
   - Example: `var singlePositive = numbers.Single(x => x > 0);`

## Set Operations

1. [ ] **`Union`**: Combines two sequences, removing duplicates.
   - Example: `var result = sequence1.Union(sequence2);`

2. [ ] **`Intersect`**: Returns the common elements between two sequences.
   - Example: `var result = sequence1.Intersect(sequence2);`

3. [ ] **`Except`**: Returns elements that are present in one sequence but not in another.
   - Example: `var result = sequence1.Except(sequence2);`

## Aggregation

1. [ ] **`Count`**: Returns the number of elements in a sequence.
   - Example: `int count = numbers.Count();`

2. [ ] **`Sum`**, **`Min`**, and **`Max`**: Calculates the sum, minimum, or maximum value in a sequence.
   - Example: `int sum = numbers.Sum();`

3. [ ] **`Average`**: Computes the average value of elements in a sequence.
   - Example: `double average = numbers.Average();`

4. [ ] **`Aggregate`**: Performs a custom aggregation operation on elements in a sequence.
   - Example: `var result = numbers.Aggregate((x, y) => x * y);`

## Conversion

1. [ ] **`ToList`** and **`ToArray`**: Converts a sequence to a list or an array.
   - Example: `List<int> list = numbers.ToList();`

2. [ ] **`ToDictionary`**: Converts a sequence into a dictionary based on key and value selectors.
   - Example: `Dictionary<int, string> dict = items.ToDictionary(item => item.Id, item => item.Name);`

3. [ ] **`OfType`**: Filters elements of a sequence to include only those of a specific type.
   - Example: `var result = mixedObjects.OfType<string>();`

4. [ ] **`Cast`**: Casts elements of a sequence to a specified type.
   - Example: `var result = mixedObjects.Cast<int>();`

## Valid C# Statements

1. [x] Variable Declaration and Assignment:
   - `int x = 10;`
   - `string name = "John";`

2. [x] Expression Statements:
   - `x = x + 5;`
   - `Console.WriteLine("Hello, World!");`

3. [x] Conditional Statements:
   - `if (condition)`
   - `if (condition) { ... }`
   - `else`
   - `else if (condition) { ... }`
   - `switch (variable) { ... }`

4. [x] Loop Statements:
   - `for (int i = 0; i < 10; i++) { ... }`
   - `while (condition) { ... }`
   - `do { ... } while (condition);`
   - `foreach (var item in collection) { ... }`

5. [x] Jump Statements:
   - `break;`
   - `continue;`
   - `return value;`
   - `goto label;`

6. [x] Exception Handling:
   - `try { ... }`
   - `catch (ExceptionType ex) { ... }`
   - `finally { ... }`
   - `throw new Exception("Error message");`

7. [x] Method Calls:
   - `Method();`
   - `int result = Add(5, 3);`

8. [x] Object Creation and Initialization:
   - `MyClass obj = new MyClass();`
   - `MyClass obj = new MyClass { Property1 = "Value1", Property2 = "Value2" };`

9. [ ] (Will Not Support) Lock Statement:
   - `lock (lockObject) { ... }`

10. [x] Checked and Unchecked Statements:
    - `checked { ... }`
    - `unchecked { ... }`

11. [ ] Using Statement (for Resource Management):
    - `using (var resource = new Resource()) { ... }`

12. [x] Delegates and Events:
    - `delegate void MyDelegate(int x);`
    - `event EventHandler MyEvent;`

13. [x] Lambda Expressions:
    - `(x, y) => x + y`
    - `(x) => { Console.WriteLine(x); }`

14. [ ] Attribute Usage:
    - `[Attribute]`
    - `[Obsolete("This method is deprecated")]`

15. [ ] (Will Not Support) Unsafe Code (for Pointer Operations):
    - `unsafe { ... }`

16. [ ] (Will Not Support) Asynchronous Programming (async/await):
    - `async Task MyMethod() { ... }`
    - `await SomeAsyncOperation();`

17. [ ] Yield Statement (for Iterator Methods):
    - `yield return item;`

18. [x] Pattern Matching (C# 7.0+):
    - `if (obj is int number) { ... }`

19. [x] Local Functions (C# 7.0+):
    - `int Add(int a, int b) { return a + b; }`

20. [ ] Record Types (C# 9.0+):
    - `record Person(string FirstName, string LastName);`

21. [x] Nullable Types:
    - `int? nullableInt = null;`

22. [x] Switch Expressions (C# 8.0+):
    - `var result = variable switch { ... };`

23. [x] Interpolated Strings (C# 6.0+):
    - `string message = $"Hello, {name}!";`

24. [x] Range and Index Operators (C# 8.0+):
    - `var subArray = myArray[1..4];`

25. [x] Pattern-Based Switch Statements (C# 9.0+):
    - `int result = variable switch { ... };`

26. [x] Discard (_) (C# 7.0+):
    - `_ = SomeMethod();`

27. [ ] (Will Not Support) Fixed Statement (for fixed-size buffers):
    - `fixed (int* p = &myArray[0]) { ... }`

28. [ ] (Will Support) Stackalloc Expression (for allocating memory on the stack):
    - Span<int> buffer = stackalloc int[100];

29. [ ] (Will Support) Nameof Expression:
    - `string propertyName = nameof(MyProperty);`

30. [ ] (Will Support) Ref Local and Ref Return:
    - `ref int refLocal = ref myArray[0];`
    - `public ref int GetArrayElement(int index) => ref myArray[index];`

31. [x] Tuple Deconstruction:
    - `var (name, age) = GetPersonInfo();`

32. [x] Out Variable Declaration:
    - `if (int.TryParse(input, out int result)) { ... }`

33. [x] Throw Expression:
    - `string name = input ?? throw new ArgumentNullException(nameof(input));`

34. [x] Default Literal Expression:
    - `int defaultInt = default;`

35. [x] Static Local Functions (C# 8.0+):
    - `static int Add(int a, int b) => a + b;`

36. [ ] Using Declarations (C# 8.0+):
    - `using var file = new StreamReader("file.txt");`

37. [x] Property Pattern (C# 8.0+):
    - `if (person is { Name: "John", Age: 30 }) { ... }`

38. [x] Index From End Operator (C# 8.0+):
    - `char lastChar = text[^1];`

39. [x] Null-coalescing Assignment (C# 8.0+):
    - `list ??= new List<string>();`

40. [ ] (Will Support) Init-only Setters (C# 9.0+):
    - `public string Name { get; init; }`

41. [ ] (Will Not Support) Top-level Statements (C# 9.0+):
    - `Console.WriteLine("Hello, World!");` // at the top level of a file

42. [x] Target-typed New Expressions (C# 9.0+):
    - `Point p = new(3, 4);`

43. [ ] Covariant Return Types (C# 9.0+):
    - `public override Derived VirtualMethod() => new Derived();`

# Valid C# Patterns

## Constant Patterns
- [x] `3`: Matches the constant value 3.
- [x] `"Hello"`: Matches the constant string "Hello".

## Type Patterns
- [x] `int x`: Matches an integer and assigns it to variable `x`.
- [x] `string s`: Matches a string and assigns it to variable `s`.
- [x] `var obj`: Matches any type and assigns it to variable `obj`.

## Var Pattern
- [x] `var x`: Matches any value and assigns it to variable `x`.

## Wildcard Pattern
- [x] `_`: Matches any value but discards it.

## Null Pattern (C# 8.0+)
- [x] `null`: Matches a `null` reference.

## Property Pattern (C# 8.0+)
- [x] `Person { Age: 18 }`: Matches a `Person` object with an `Age` property set to 18.

## Tuple Pattern (C# 7.0+)
- [x] `(int x, int y)`: Matches a tuple with two integer values.
- [x] `(string name, int age)`: Matches a tuple with a string `name` and an integer `age`.

## Positional Pattern (C# 8.0+)
- [x] `Point { X: 0, Y: 0 }`: Matches a `Point` object with `X` and `Y` properties set to 0.
- [x] `Rectangle { Width: var w, Height: var h }`: Matches a `Rectangle` object and assigns `Width` and `Height` to `w` and `h`.

## Recursive Pattern (C# 8.0+)
- [x] `int[] { 1, 2, int rest }`: Matches an array starting with elements 1 and 2, and assigns the rest to `rest`.
- [x] `(1, 2, var rest)`: Matches a tuple starting with elements 1 and 2, and assigns the rest to `rest`.

## Logical Patterns (C# 9.0+)
- [x] `and` pattern: `and` combines patterns.
  - [x] `int x and > 10`: Matches an integer greater than 10.
- [x] `or` pattern: `or` combines patterns.
  - [x] `int x or string s`: Matches an integer or a string.

## Type Patterns with When Clause (C# 7.0+)
- [x] `int x when x > 10`: Matches an integer greater than 10 and assigns it to `x`.
- [x] `string s when s.Length > 5`: Matches a string with a length greater than 5 and assigns it to `s`.

## Var Pattern with When Clause (C# 7.0+)
- [ ] `var x when x is int`: Matches any value of type `int` and assigns it to `x`.

## Binary Patterns (C# 9.0+)
- [x] `a is b`: Checks if `a` is of type `b`.
- [x] `a as b`: Attempts to cast `a` to type `b` and returns `null` if it fails.

## Parenthesized Patterns (C# 8.0+)
- [x] `(pattern)`: Groups patterns for precedence.

## Relational Patterns (C# 9.0+)
- [x] `a < b`: Matches if `a` is less than `b`.
- [x] `a <= b`: Matches if `a` is less than or equal to `b`.
- [x] `a > b`: Matches if `a` is greater than `b`.
- [x] `a >= b`: Matches if `a` is greater than or equal to `b`.
- [x] `a == b`: Matches if `a` is equal to `b`.
- [x] `a != b`: Matches if `a` is not equal to `b`.

# Valid C# Expressions

## Primary Expressions
- [x] `x`: Identifier (variable or constant).
- [x] `123`: Literal integer.
- [x] `"Hello"`: Literal string.
- [x] `true` and `false`: Literal boolean values.
- [x] `null`: Null literal.
- [x] `this`: Current instance reference.
- [ ] `base`: Base class reference.

## Member Access Expressions
- [x] `object.Member`: Accessing a member (field, property, method, event, or nested type).
- `person.Name`: Example property access.

## Invocation Expressions
- [x] `MethodName()`: Method invocation with no arguments.
- [x] `MethodName(arg1, arg2)`: Method invocation with arguments.
- `Math.Max(x, y)`: Example method call.

## Element Access Expressions
- [x] `array[index]`: Accessing an array element.
- [x] `dictionary[key]`: Accessing a dictionary value.
- `list[0]`: Example element access.

## Object Creation Expressions
- [x] `new ClassName()`: Creating an instance of a class.
- [x] `new List<int>()`: Example object creation.
- [x] `new { Name = "John", Age = 30 }`: Creating an anonymous type.

## Lambda Expressions
- [x] `(x, y) => x + y`: Lambda expression.
- [x] `(int x) => { Console.WriteLine(x); }`: Example lambda expression.

## Anonymous Types (C# 3.0+)
- [x] `new { Name = "John", Age = 30 }`: Creating an anonymous type.
- `var person = new { Name = "Alice", Age = 25 }`: Example anonymous type creation.

## Object Initialization Expressions
- [x] `new Person { Name = "Alice", Age = 25 }`: Initializing an object.
- `new Point { X = 10, Y = 20 }`: Example object initialization.

## Collection Initialization Expressions (C# 3.0+)
- [x] `new List<int> { 1, 2, 3 }`: Initializing a collection.
- `var numbers = new List<int> { 1, 2, 3 }`: Example collection initialization.

## Array Initialization Expressions
- [x] `new int[] { 1, 2, 3 }`: Initializing an array.
- `int[] arr = { 1, 2, 3 }`: Example array initialization.

## Nullable Value Types
- [x] `int? nullableInt = null;`: Nullable integer.

## Type Conversion Expressions
- [x] `(int)x`: Explicit type conversion.
- [ ] `x as T`: Attempted type conversion (returns `null` if unsuccessful).
- `(T)x`: Example explicit type conversion.

## Arithmetic Expressions
- [x] `x + y`: Addition.
- [x] `x - y`: Subtraction.
- [x] `x * y`: Multiplication.
- [x] `x / y`: Division.
- [x] `x % y`: Modulus (remainder).

## Relational Expressions
- [x] `x < y`: Less than.
- [x] `x <= y`: Less than or equal to.
- [x] `x > y`: Greater than.
- [x] `x >= y`: Greater than or equal to.
- [x] `x == y`: Equal to.
- [x] `x != y`: Not equal to.

## Logical Expressions
- [x] `x && y`: Logical AND.
- [x] `x || y`: Logical OR.
- [x] `!x`: Logical NOT.

## Conditional Expressions
- [x] `condition ? trueExpression : falseExpression`: Conditional (ternary) operator.
- `(x > 0) ? "Positive" : "Non-positive"`: Example usage of the conditional operator.

## Assignment Expressions
- [x] `x = y`: Assignment.
- [x] `x += y`: Addition assignment.
- [x] `x -= y`: Subtraction assignment.
- [x] `x *= y`: Multiplication assignment.
- [x] `x /= y`: Division assignment.
- [x] `x %= y`: Modulus assignment.

## Increment and Decrement Expressions
- [x] `x++`: Post-increment.
- [x] `x--`: Post-decrement.
- [x] `++x`: Pre-increment.
- [x] `--x`: Pre-decrement.

# Common BigInteger Methods and Properties

## Constructors
- [x] `BigInteger()`: Initializes a new instance of the `BigInteger` class with a value of zero.
- [x] `BigInteger(int value)`: Initializes a new instance of the `BigInteger` class with the specified integer value.
- [x] `BigInteger(long value)`: Initializes a new instance of the `BigInteger` class with the specified long integer value.
- [x] `BigInteger(byte[] bytes)`: Initializes a new instance of the `BigInteger` class from an array of bytes.
- [ ] `BigInteger(double value)`: Initializes a new instance from a double-precision floating-point number.
- [ ] `BigInteger(decimal value)`: Initializes a new instance from a decimal number.

## Static Methods
- [x] `BigInteger.Add(BigInteger left, BigInteger right)`: Returns the result of adding two `BigInteger` values.
- [x] `BigInteger.Subtract(BigInteger left, BigInteger right)`: Returns the result of subtracting one `BigInteger` value from another.
- [x] `BigInteger.Multiply(BigInteger left, BigInteger right)`: Returns the result of multiplying two `BigInteger` values.
- [x] `BigInteger.Divide(BigInteger dividend, BigInteger divisor)`: Returns the result of dividing one `BigInteger` value by another.
- [x] `BigInteger.Remainder(BigInteger dividend, BigInteger divisor)`: Returns the remainder of dividing one `BigInteger` value by another.
- [x] `BigInteger.Pow(BigInteger value, int exponent)`: Returns a `BigInteger` raised to a specified power.
- [x] `BigInteger.Abs(BigInteger value)`: Returns the absolute value of a `BigInteger` value.
- [x] `BigInteger.GreatestCommonDivisor(BigInteger left, BigInteger right)`: Returns the greatest common divisor of two `BigInteger` values.
- [x] `BigInteger.Min(BigInteger left, BigInteger right)`: Returns the smaller of two `BigInteger` values.
- [x] `BigInteger.Max(BigInteger left, BigInteger right)`: Returns the larger of two `BigInteger` values.
- [x] `BigInteger.Negate(BigInteger value)`: Returns the negation of a `BigInteger` value.
- [x] `BigInteger.Compare(BigInteger left, BigInteger right)`: Compares two `BigInteger` values.
- [x] `BigInteger.ModPow(BigInteger value, BigInteger exponent, BigInteger modulus)`: Performs modular exponentiation.
- [x] `BigInteger.Parse(string value)`: Converts a string representation to a `BigInteger`.
- [x] `BigInteger.DivRem(BigInteger dividend, BigInteger divisor)`: Performs division with remainder.
- [x] `BigInteger.Clamp(BigInteger value, BigInteger min, BigInteger max)`: Clamps a `BigInteger` value to a specified range.
- [x] `BigInteger.CopySign(BigInteger value, BigInteger sign)`: Returns a value with the magnitude of `value` and the sign of `sign`.
- [x] `BigInteger.CreateChecked(T value)`: Creates a `BigInteger` from various numeric types with overflow checking.
- [x] `BigInteger.CreateSaturating(T value)`: Creates a `BigInteger` from various numeric types with saturation.
- [x] `BigInteger.CreateTruncating(T value)`: Creates a `BigInteger` from various numeric types with truncation.
- [ ] `BigInteger.Log(BigInteger value)`: Returns the natural logarithm of a specified number.
- [ ] `BigInteger.Log10(BigInteger value)`: Returns the base 10 logarithm of a specified number.
- [x] `BigInteger.One`: Gets a value that represents the number one.
- [x] `BigInteger.Zero`: Gets a value that represents the number zero.
- [x] `BigInteger.MinusOne`: Gets a value that represents the number negative one.

## Properties
- [x] `IsEven`: Returns `true` if the `BigInteger` is an even number.
- [x] `IsOne`: Returns `true` if the `BigInteger` is equal to 1.
- [x] `IsZero`: Returns `true` if the `BigInteger` is equal to 0.
- [x] `Sign`: Gets a value indicating the sign of the `BigInteger` (-1 for negative, 0 for zero, 1 for positive).
- [x] `IsPowerOfTwo`: Returns `true` if the `BigInteger` is a power of two.
- [x] `IsNegative`: Returns `true` if the `BigInteger` is negative.
- [x] `IsPositive`: Returns `true` if the `BigInteger` is positive.

## Instance Methods
- [x] `ToByteArray()`: Returns the `BigInteger` as a byte array.
- [x] `ToString()`: Converts the `BigInteger` to its decimal string representation.
- [x] `Equals(object? obj)`: Determines whether this instance and a specified object have the same value.
- [ ] `CompareTo(BigInteger other)`: Compares this instance to a second BigInteger and returns an integer that indicates whether the value of this instance is less than, equal to, or greater than the value of the specified object.
- [ ] `GetHashCode()`: Returns the hash code for this instance.

## Additional Methods
- [x] `BigInteger.IsNegative(BigInteger value)`: Determines if the `BigInteger` is negative.
- [x] `BigInteger.IsPositive(BigInteger value)`: Determines if the `BigInteger` is positive.
- [x] `BigInteger.IsPow2(BigInteger value)`: Determines if the `BigInteger` is a power of 2.
- [x] `BigInteger.LeadingZeroCount(BigInteger value)`: Returns the number of leading zero bits in the binary representation.
- [x] `BigInteger.Log2(BigInteger value)`: Returns the base-2 logarithm of a specified `BigInteger`.
- [x] `BigInteger.IsEvenInteger(BigInteger value)`: Determines if the `BigInteger` is even.
- [x] `BigInteger.IsOddInteger(BigInteger value)`: Determines if the `BigInteger` is odd.
- [x] `BigInteger.PopCount(BigInteger value)`: Returns the number of one-bits in the two's complement binary representation of a specified BigInteger value.

## Conversion Methods
- [x] Explicit casting to various numeric types (sbyte, byte, short, ushort, int, uint, long, ulong, char)
- [x] Explicit casting from various numeric types to BigInteger
- [ ] `ToDouble()`: Converts the `BigInteger` to a double-precision floating-point number.
- [ ] `ToDecimal()`: Converts the `BigInteger` to a decimal number.

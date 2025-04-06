# Method Conversion to NeoVM Bytecode

One of the core responsibilities of the Neo C# Compiler (`nccs`) is to translate the C# methods within your smart contract into executable Neo Virtual Machine (NeoVM) bytecode. This process involves analyzing the C# code and generating a corresponding sequence of NeoVM opcodes.

## The Conversion Process

The compiler leverages the .NET Compiler Platform (Roslyn) to parse and analyze your C# code. For each method identified as part of the smart contract, it performs the following steps:

1.  **Semantic Analysis:** Roslyn provides a detailed semantic model of the code, understanding types, method calls, variable scopes, and control flow.
2.  **Syntax Tree Traversal:** The compiler walks through the Abstract Syntax Tree (AST) of the method provided by Roslyn.
3.  **Opcode Emission:** As it traverses the tree, it converts C# statements and expressions into sequences of NeoVM instructions.

## Handling C# Constructs

The compiler maps various C# language features to NeoVM equivalents:

*   **Variable Declarations & Assignments:** Uses NeoVM stack operations (`PUSH`, `STLOC`, `LDLOC`) to manage local variables. `INITSLOT` is used at the method start to reserve space for parameters and locals.
*   **Arithmetic & Logical Operations:** Maps C# operators (`+`, `-`, `*`, `/`, `%`, `&`, `|`, `^`, `!`, `==`, `!=`, `<`, `>`, `<=`, `>=`) to corresponding NeoVM opcodes (e.g., `ADD`, `SUB`, `MUL`, `DIV`, `MOD`, `AND`, `OR`, `XOR`, `NOT`, `NUMEQUAL`, `NUMNOTEQUAL`, `LT`, `GT`, `LE`, `GE`). The `--checked` option influences whether overflow checks (`ADDA`, `SUBA`, etc.) are emitted.
*   **Control Flow:**
    *   **`if`/`else`:** Uses conditional jump opcodes (`JMPIF_L`, `JMPIFNOT_L`, `JMP_L`).
    *   **`while`/`for`/`do-while` Loops:** Employs jump opcodes to create looping structures. `break` and `continue` statements are handled by jumping to specific target instructions.
    *   **`switch` Statements:** Can be complex, often involving a series of comparisons and jumps or sometimes optimized using `SWITCH_L` if applicable.
    *   **`try`/`catch`/`finally`:** Managed using NeoVM's exception handling opcodes (`TRY_L`, `ENDTRY_L`, `ENDFINALLY`). The compiler sets up exception handling contexts to route execution flow correctly.
*   **Method Calls:**
    *   **Internal Calls:** Uses `CALL_L` to invoke other methods within the same contract, pushing arguments onto the stack according to the calling convention.
    *   **Framework/Interop Calls:** Maps calls to `Neo.SmartContract.Framework` APIs (like `Runtime.Log`, `Storage.Put`, native contract methods) to specific `SYSCALL` instructions. The compiler identifies the target API by its name/hash and emits the corresponding syscall hash.
    *   **External Contract Calls:** Uses `CALLT` and the `Contract.Call` method, requiring the target contract hash, method name, call flags, and arguments to be pushed onto the stack.
*   **Object Creation (`new`):**
    *   **Simple Types:** Often involves direct stack manipulation (e.g., pushing `0` for numbers, empty byte array for strings).
    *   **Complex Types (Structs/Classes):** Typically creates a `Struct` or `Array` on the NeoVM stack and initializes its members.
    *   **Arrays:** Uses `NEWARRAY`, `NEWSTRUCT`, `PACK` depending on the context and type.
*   **Type Conversions (Casting):** May involve opcodes like `CONVERT` if a specific NeoVM type conversion is needed, or might be handled implicitly by stack manipulation.
*   **String/Byte Array Operations:** Maps C# string/byte array methods to NeoVM opcodes (`CAT`, `SUBSTR`, `LEFT`, `RIGHT`) or sequences of stack operations.

## Mapping C# Types to NeoVM Types

The compiler translates C# types into their corresponding NeoVM stack item types:

| C# Type                 | NeoVM Type        | Notes                                      |
| :---------------------- | :---------------- | :----------------------------------------- |
| `void`                  | (No value)        |                                            |
| `bool`                  | `Boolean`         |                                            |
| `sbyte`, `byte`, `short`, `ushort`, `int`, `uint`, `long`, `ulong`, `BigInteger` | `Integer` |                                            |
| `char`                  | `Integer`         | Stored as its numeric UTF-16 value       |
| `string`                | `ByteString`      | UTF-8 encoded                              |
| `byte[]`                | `ByteString`      |                                            |
| `object`, `dynamic`     | `Any`             | Represents any type                      |
| `Array`, `List<T>`, etc. | `Array`/`Struct`  | Often represented as `Array` or `Struct` |                |
| `Map<K,V>`              | `Map`             |                                            |
| Framework Types (e.g., `UInt160`, `ECPoint`) | `ByteString`      | Often serialized to byte arrays          |
| Other Classes/Structs   | `Array`/`Struct`  | Usually packed into `Array` or `Struct`      |

## Optimization

The conversion process may include basic optimizations (`--optimize Basic` or higher) like removing redundant `NOP` instructions. More advanced optimizations can further refine the generated bytecode.

Understanding this conversion process helps in writing C# code that translates efficiently to NeoVM bytecode, potentially optimizing for GAS costs and execution performance.

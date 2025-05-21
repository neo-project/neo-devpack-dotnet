# Data Types

NeoVM has its own set of primitive and complex data types. The `Neo.SmartContract.Framework` provides C# types that map directly to these NeoVM types, allowing you to work with them in a familiar C# environment.

## NeoVM StackItem Types

The fundamental types within NeoVM are represented by `StackItemType`. Your C# code is compiled into operations that manipulate these types on the NeoVM stack.

Key `StackItemType` values include:

*   `Boolean`: Represents `true` or `false`.
*   `Integer`: Represents arbitrary-size integers.
*   `ByteString`: Represents an immutable sequence of bytes (used for strings, script hashes, public keys, raw data).
*   `Buffer`: Represents a mutable sequence of bytes.
*   `Array`: Represents an array of `StackItem`s.
*   `Struct`: Represents a struct (similar to an array, but often used for specific data structures) of `StackItem`s.
*   `Map`: Represents a key-value map where keys and values are `StackItem`s.
*   `Pointer`: Represents an instruction pointer (used internally by NeoVM).
*   `InteropInterface`: Represents an object provided by the Interop Service Layer (e.g., `StorageContext`).
*   `Any`: Represents null or can be implicitly converted from other types.

## C# Mapping in `Neo.SmartContract.Framework`

The framework provides C# types and implicit conversions for seamless integration:

| C# Type in Framework             | NeoVM Type (`StackItemType`) | Description                                                                 |
| :------------------------------- | :--------------------------- | :-------------------------------------------------------------------------- |
| `bool`                           | `Boolean`                    | Standard boolean true/false.                                                |
| `sbyte`, `byte`, `short`, `ushort`, `int`, `uint`, `long`, `ulong` | `Integer` | Standard C# integer types. Converted to NeoVM Integer.              |
| `System.Numerics.BigInteger`     | `Integer`                    | Represents arbitrarily large integers.                                      |
| `char`                           | `Integer`                    | Represents a single character (stored as its integer value).                |
| `string`                         | `ByteString`                 | UTF8 encoded immutable byte sequence.                                       |
| `Neo.SmartContract.Framework.ByteString` | `ByteString`          | Explicit representation of NeoVM's immutable byte sequence. Useful for hashes, keys, etc. |
| `byte[]`                         | `Buffer`                     | Mutable sequence of bytes. Use `Buffer` type for direct manipulation.        |
| `Neo.SmartContract.Framework.ECPoint` | `ByteString`                 | Represents a public key on the elliptic curve (secp256r1). Stored as compressed bytes. |
| `Neo.SmartContract.Framework.UInt160` | `ByteString`                 | Represents a 160-bit unsigned integer, typically used for script hashes (addresses). |
| `Neo.SmartContract.Framework.UInt256` | `ByteString`                 | Represents a 256-bit unsigned integer, typically used for transaction or block hashes. |
| `object[]` / `System.Array`      | `Array` / `Struct`           | Can represent NeoVM Arrays or Structs. Usage determines the underlying type. |
| `Neo.SmartContract.Framework.List<T>` | `Array`                   | Generic list, maps to NeoVM Array.                                         |
| `Neo.SmartContract.Framework.Map<TKey, TValue>` | `Map`           | Generic map, maps to NeoVM Map. Keys and Values must be valid NeoVM types. |
| `object`                         | `Any`                        | Can represent `null` or be used for type casting.                             |
| Enums                            | `Integer`                    | Stored as their underlying integer value.                                   |
| Structs (C# `struct`)            | `Struct`                     | User-defined structs map to NeoVM Structs. Members must be valid types.   |
| `Neo.SmartContract.Framework.Services.*` | `InteropInterface`        | Types representing interop services (e.g., `StorageContext`, `Transaction`). |

## Important Considerations

*   **Immutability:** `string` and `ByteString` are immutable. Operations that appear to modify them actually create new instances.
*   **Mutability:** `byte[]` (Buffer) is mutable.
*   **Type Safety:** While C# provides strong typing, interactions often involve `object` or `StackItem` conversions (especially with storage or complex types). Use explicit casting (`(type)value`) or helper methods (`Helper.AsString`, `Helper.AsBigInteger`, etc.) carefully.
*   **Serialization:** When storing complex types (arrays, maps, structs) in storage, they are serialized. Be mindful of GAS costs associated with serialization/deserialization.
*   **Storage Types:** `Storage.Put` expects `byte[]` or `ByteString` for keys and values. Use `Helper` methods or `StorageMap` extensions (`.Put(key, value)`) for automatic serialization of other types.
    ```csharp
    StorageMap balances = new StorageMap(Storage.CurrentContext, "BAL");
    UInt160 user = (UInt160)Runtime.ExecutingScriptHash; // Example user hash
    BigInteger amount = 100;

    // StorageMap handles serialization for BigInteger value
    balances.Put(user, amount);

    BigInteger retrievedAmount = (BigInteger)balances.Get(user);
    ```
*   **Floating Point:** NeoVM does *not* natively support floating-point numbers (`float`, `double`, `decimal`). You must represent fractional values using `BigInteger` and a fixed decimal precision factor.

Understanding this type mapping is essential for writing correct and efficient C# smart contracts.

[Previous: Entry Points & Methods](./04-entry-points.md) | [Next: Deployment Artifacts (NEF & Manifest)](./06-deployment-files.md)
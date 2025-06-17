# Storage

Smart contracts require persistent storage to maintain state between invocations. The `Neo.SmartContract.Framework` provides access to the contract's private key-value storage area, primarily through the static `Storage` class and the helpful `StorageMap` wrapper.

## Storage Context (`StorageContext`)

Every storage operation happens within a `StorageContext`. This context represents the specific storage space allocated to the contract.

*   **`Storage.CurrentContext`**: Provides **read-write** access to the storage area of the currently executing contract. Use this context for methods that need to read *and* modify state (`Storage.Put`, `Storage.Delete`).
*   **`Storage.CurrentReadOnlyContext`**: Provides **read-only** access. Use this context for methods guaranteed not to modify storage (`Storage.Get`, `Storage.Find`). This is often used in methods marked with the `[Safe]` attribute. Using a read-only context where applicable can sometimes offer slight GAS optimizations and clearly signals intent.

```csharp
// Read-write access
StorageContext rwContext = Storage.CurrentContext;
Storage.Put(rwContext, "key", "value");

// Read-only access
StorageContext roContext = Storage.CurrentReadOnlyContext;
ByteString value = Storage.Get(roContext, "key");
```

## Low-Level Storage Operations (`Storage` class)

The static `Storage` class provides direct access methods. These methods operate directly on byte arrays (`byte[]`) or `ByteString` for both keys and values.

*   **`Storage.Put(StorageContext, ByteString key, ByteString value)` / `(..., byte[] key, byte[] value)`**: Stores or overwrites a key-value pair.
*   **`Storage.Get(StorageContext, ByteString key)` / `(..., byte[] key)`**: Retrieves the value (`ByteString`) for a key. Returns `null` if not found.
*   **`Storage.Delete(StorageContext, ByteString key)` / `(..., byte[] key)`**: Removes a key-value pair.

**Manual Serialization Required:** When using these low-level methods with types other than `byte[]` or `ByteString`, you *must* manually serialize your data before `Put` and deserialize it after `Get`, typically using `StdLib.Serialize` and `StdLib.Deserialize`.

```csharp
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Native; // For StdLib
using Neo.SmartContract.Framework.Services;
using System.Numerics;

public class LowLevelStorage : SmartContract
{
    public static void PutBigInt(byte[] key, BigInteger value)
    {
        // Manual serialization to ByteString
        ByteString serializedValue = StdLib.Serialize(value);
        Storage.Put(Storage.CurrentContext, key, serializedValue);
    }

    public static BigInteger GetBigInt(byte[] key)
    {
        ByteString serializedValue = Storage.Get(Storage.CurrentReadOnlyContext, key);
        if (serializedValue is null) return 0; // Or handle error
        // Manual deserialization and casting
        return (BigInteger)StdLib.Deserialize(serializedValue);
    }
}
```

## `StorageMap` for Convenience and Structure

Manually handling prefixes and serialization is error-prone. `StorageMap` is a wrapper that simplifies storage access significantly.

*   **Automatic Prefixing:** Each `StorageMap` is initialized with a unique prefix (string, byte, int). All keys managed by that map are automatically prepended with this prefix before interacting with the underlying storage. This prevents key collisions between different types of data within your contract (e.g., `Balances` map won't collide with `Allowances` map if they have different prefixes).
*   **Automatic Serialization/Deserialization:** `StorageMap` provides extension methods (`Put`, `Get`, `GetString`, `GetBigInteger`, etc.) that handle serialization/deserialization for common types (`BigInteger`, `string`, `bool`, primitives, `UInt160/256`, `ECPoint`, enums, serializable structs, `List<T>`, `Map<K,V>`) automatically using `StdLib`.

```csharp
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System.Numerics;

public class StorageMapDemo : SmartContract
{
    // Use distinct prefixes for different data categories
    private static readonly StorageMap Balances = new StorageMap(Storage.CurrentContext, "BAL"); // String prefix
    private static readonly StorageMap UserInfo = new StorageMap(Storage.CurrentContext, 0x10); // Byte prefix

    // Define a serializable struct
    public struct User
    {
        public string Name;
        public BigInteger RegistrationTime;
        public bool IsActive;
    }

    public static void SetBalance(UInt160 user, BigInteger amount)
    {
        // `Put` handles key (UInt160) and value (BigInteger) serialization
        Balances.Put(user, amount);
    }

    [Safe]
    public static BigInteger GetBalance(UInt160 user)
    {
        // `Get` retrieves serialized data, cast needed. Returns default (0) if not found.
        return (BigInteger)Balances.Get(user);
        // Or use specific helper: return Balances.GetBigInteger(user); // Might not exist, check framework version
    }

    public static void StoreUserInfo(UInt160 user, string name, bool isActive)
    {
        User data = new User
        {
            Name = name,
            RegistrationTime = Runtime.Time, // Store current block time
            IsActive = isActive
        };
        // `Put` serializes the User struct automatically
        UserInfo.Put(user, data);
    }

    [Safe]
    public static User GetUserInfo(UInt160 user)
    {
        ByteString serializedData = UserInfo.Get(user); // Get raw bytes first
        if (serializedData is null) return null; // Or default User
        // Deserialize and cast
        return (User)StdLib.Deserialize(serializedData);
    }

    public static void DeleteBalance(UInt160 user)
    {
        // Automatically uses the "BAL" prefix
        Balances.Delete(user);
    }
}
```

## Iterating Over Storage (`Storage.Find`)

`Storage.Find` allows iterating over key-value pairs within a `StorageContext`, optionally filtered by a key prefix. This is powerful but **potentially very GAS-intensive**.

*   **`Storage.Find(StorageContext context, ByteString prefix, FindFlags flags)`**: Returns an `Iterator`.
*   **`FindFlags` Enum:** Controls behavior:
    *   `None`: Default. Iterator value is the `KeyValuePair<ByteString, ByteString>`.
    *   `KeysOnly`: Iterator value is the key (`ByteString`).
    *   `ValuesOnly`: Iterator value is the value (`ByteString`).
    *   `RemovePrefix`: Removes the `prefix` used in the `Find` call from the yielded keys. **Crucial** when using with `StorageMap.Prefix` to get the original keys.
    *   `DeserializeValues`: If using generic `Find<T>`, attempts to deserialize the value (`ByteString`) into type `T`. Iterator value becomes `KeyValuePair<ByteString, T>`.
    *   `PickField0` / `PickField1`: If values are structs/arrays, yields only the specified field (0 or 1).

**Iteration Best Practices & Warnings:**

1.  **GAS Cost:** `Storage.Find` has a base cost plus a significant cost **per item iterated**. Avoid iterating over potentially large datasets in user-callable functions, as it can easily exceed GAS limits, making your contract unusable (DoS).
2.  **Use Prefixes:** Always use a `prefix` with `Storage.Find` to limit the scope of iteration. Use `StorageMap.Prefix` when iterating over map data.
3.  **Use `FindFlags.RemovePrefix`:** When iterating a `StorageMap`'s data, use `RemovePrefix` to get the keys as they were originally inserted.
4.  **Bounded Iteration:** If iteration is necessary, implement bounds. Process only N items per transaction, potentially returning the last processed key so the next call can resume. This is complex pagination logic.
5.  **Consider Alternatives:** Can you achieve the goal without iteration? Perhaps maintain a separate counter or index in storage instead of iterating to find items.

```csharp
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System.Numerics;

public class StorageFindDemo : SmartContract
{
    private static readonly StorageMap Balances = new StorageMap(Storage.CurrentContext, "BAL");

    // ... SetBalance method ...

    // Example: Sum all balances (HIGH GAS RISK - Use only if dataset is small/controlled)
    [Safe]
    public static BigInteger GetTotalSupply()
    {
        // Iterate over keys starting with Balances.Prefix
        // RemovePrefix gets the user address (key) without "BAL"
        // DeserializeValues converts the stored balance (value) to BigInteger
        Iterator<(ByteString Key, BigInteger Value)> iterator =
            Storage.Find<BigInteger>(Storage.CurrentReadOnlyContext, Balances.Prefix, FindFlags.RemovePrefix | FindFlags.DeserializeValues);

        BigInteger totalSupply = 0;
        while (iterator.Next())
        {
            var (key, value) = iterator.Value(); // Key is user addr, Value is balance
            totalSupply += value;
        }
        return totalSupply;
    }

    // Safer Pattern: Paginated Retrieval (Conceptual)
    // Returns N balances starting after 'startKey'
    public static Map<UInt160, BigInteger> GetPagedBalances(ByteString startKey, int count)
    {
        Iterator<(ByteString Key, BigInteger Value)> iterator =
            Storage.Find<BigInteger>(Storage.CurrentReadOnlyContext, Balances.Prefix, FindFlags.RemovePrefix | FindFlags.DeserializeValues);

        Map<UInt160, BigInteger> results = new Map<UInt160, BigInteger>();
        int found = 0;
        bool pastStartKey = (startKey == null); // Start immediately if no startKey provided

        while (iterator.Next() && found < count)
        {
            var (key, value) = iterator.Value();
            if (!pastStartKey)
            {
                // Skip keys until we are past the startKey
                if (key == startKey) // Or use MemoryCompare for proper ordering
                {
                    pastStartKey = true;
                }
                continue;
            }
            // Add to results
            results[(UInt160)key] = value;
            found++;
        }
        return results; // Client needs to handle making subsequent calls with the last key
    }
}
```

## Storage Design Patterns

*   **Combined Keys:** For mapping relationships (e.g., allowances `owner + spender`), concatenate keys to create a unique identifier.
*   **Indexing:** If you need to look up data based on criteria other than the primary key, consider maintaining separate index maps (e.g., map `userName -> userAddress`). This adds complexity and write costs but can optimize read scenarios.
*   **Counters:** Use dedicated storage keys for counters (`TotalSupplyKey`, `ProposalCounterKey`).
*   **Versioning:** Store a version number with your data structures to facilitate future migrations (`struct MyDataV1 { int version; ... }`).

Masting storage is key to building effective and efficient smart contracts. Always prioritize minimizing storage operations and carefully consider the GAS implications of `Storage.Find`.

[Previous: Framework Overview](./README.md) | [Next: Runtime Services](./02-runtime.md)
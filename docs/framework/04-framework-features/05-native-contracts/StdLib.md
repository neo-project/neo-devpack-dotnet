# Native Contract: StdLib

Namespace: `Neo.SmartContract.Framework.Native`

Provides standard utility functions, primarily for data conversion and serialization.

## Key Methods

*   **`Serialize(object source)` (`ByteString`)**: Serializes a supported C# object (primitive types, `BigInteger`, `string`, `ByteString`, `UInt160`, `UInt256`, `ECPoint`, arrays, structs, maps, `bool`) into its NeoVM binary representation.
*   **`Deserialize(ByteString source)` (`object`)**: Deserializes NeoVM binary data back into its corresponding C# object representation. Returns a `StackItem` which may need casting.

*   **`JsonSerialize(object source)` (`string`)**: Serializes a supported C# object into a JSON string representation.
*   **`JsonDeserialize(ByteString source)` (`object`)**: Deserializes a JSON ByteString (UTF8 encoded) into its corresponding C# object representation (often a `Map<string, object>` for JSON objects or `List<object>` for JSON arrays). Result needs casting.
*   **`JsonDeserialize(string source)` (`object`)**: Deserializes a JSON string.

*   **`Atoi(string value, int @base)` (`System.Numerics.BigInteger`)**: Converts a string representation of a number in a specified base (e.g., 10 for decimal, 16 for hex) into a `BigInteger`.
*   **`Atoi(ByteString value, int @base)` (`System.Numerics.BigInteger`)**: Converts a `ByteString` representation.

*   **`Itoa(System.Numerics.BigInteger value, int @base)` (`string`)**: Converts a `BigInteger` into its string representation in a specified base.

*   **`Base64Encode(ByteString value)` (`string`)**: Encodes binary data into a Base64 string.
*   **`Base64Decode(string value)` (`ByteString`)**: Decodes a Base64 string back into binary data.

*   **`Base58Encode(ByteString value)` (`string`)**: Encodes binary data into a Base58 string (commonly used for addresses).
*   **`Base58Decode(string value)` (`ByteString`)**: Decodes a Base58 string back into binary data.

*   **`MemoryCompare(ByteString str1, ByteString str2)` (`int`)**: Compares two `ByteString` values lexicographically. Returns < 0 if str1 < str2, 0 if str1 == str2, > 0 if str1 > str2.
*   **`MemoryCompare(byte[] str1, byte[] str2)` (`int`)**: Compares two `byte[]` values.

*   **`MemorySearch(ByteString value, ByteString search)` (`int`)**: Finds the first occurrence of `search` within `value`. Returns the starting index or -1 if not found.
*   **`MemorySearch(byte[] value, byte[] search)` (`int`)**: Searches within `byte[]`.
*   **`MemorySearch(ByteString value, ByteString search, int start)` (`int`)**: Starts searching from a specific index.
*   **`MemorySearch(byte[] value, byte[] search, int start)` (`int`)**: Searches within `byte[]` from a start index.

## Example Usage

```csharp
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System.Numerics;

// Allow calling all StdLib methods
[ContractPermission(nameof(StdLib), "*")] 
public class StdLibDemo : SmartContract
{
    // Serialize/Deserialize a custom struct (ensure members are serializable)
    public struct MyData
    {
        public string Name;
        public BigInteger Value;
    }

    private static readonly StorageMap DataStore = new StorageMap(Storage.CurrentContext, "DATA");

    public static void StoreData(string key, string name, BigInteger value)
    {
        MyData data = new MyData { Name = name, Value = value };
        ByteString serializedData = StdLib.Serialize(data);
        DataStore.Put(key, serializedData);
    }

    public static MyData RetrieveData(string key)
    {
        ByteString serializedData = DataStore.Get(key);
        if (serializedData == null) return default; // Or throw exception
        
        // Deserialize and cast to the specific struct type
        return (MyData)StdLib.Deserialize(serializedData);
    }

    // Parse a JSON string received from an Oracle, for example
    public static BigInteger GetValueFromJson(string jsonString)
    { 
        // Example JSON: { "user": "abc", "value": 123 }
        Map<string, object> jsonMap = (Map<string, object>)StdLib.JsonDeserialize(jsonString);
        if (jsonMap != null && jsonMap.ContainsKey("value")) 
        { 
            // Values within the map might need further casting
            return (BigInteger)jsonMap["value"];
        } 
        return -1; // Indicate error or value not found
    }

    // Convert hex string to integer
    public static BigInteger HexToInt(string hex)
    {
        return StdLib.Atoi(hex, 16);
    }
}
```

`StdLib` is essential for handling data serialization, especially for storage or when dealing with data from external sources like oracles.

[Previous: RoleManagement](./RoleManagement.md) | [Back to Framework Overview](../README.md)
# Attributes (`Neo.SmartContract.Framework.Attributes`)

C# Attributes are metadata tags placed above classes, methods, events, or fields. In Neo C# development, they are essential tools used by the `Neo.Compiler.CSharp` compiler (`nccs`) to:

1.  **Generate the Contract Manifest (`.manifest.json`):** Provide descriptive information, declare supported standards, define permissions, and specify the contract's public interface (ABI).
2.  **Influence Compiler Behavior:** Control optimizations, embed constant values, mark methods as read-only, and manage security features like reentrancy.
3.  **Provide Type Hinting:** Clarify expected data types for parameters or fields, potentially aiding external tools or future compiler checks.

Key attributes reside in the `Neo.SmartContract.Framework.Attributes` namespace, with some common ones (like `DisplayName`) coming from `System.ComponentModel`.

## Manifest Generation Attributes

These attributes directly influence the content of the `.manifest.json` file, which describes your contract to the outside world. Apply these primarily to your main contract class.

*   **`[DisplayName(string name)]`**: (From `System.ComponentModel`)
    *   **Purpose:** Specifies a user-friendly, public name for the contract class, method, or event in the manifest's ABI section. Overrides the C# name.
    *   **Usage:** `[DisplayName("MyToken")] public class ContractClass...`, `[DisplayName("transfer")] public static event Action<...> OnTransfer;`

*   **`[SupportedStandards(params string[] standards)]`**:
    *   **Purpose:** Declares which NEP standards (e.g., "NEP-17", "NEP-11") the contract implements. Populates the `supportedstandards` array in the manifest.
    *   **Usage:** `[SupportedStandards("NEP-17")] public class MyNep17Token...`

*   **`[ContractPermission(string contract, params string[] methods)]`**:
    *   **Purpose:** Defines which contracts and methods *this* contract is allowed to call using `Contract.Call`. Essential for interoperability. Populates the `permissions` array.
    *   **`contract`:** Target contract hash (hex string), native contract name (e.g., `"GasToken"`), or `"*"` (any contract).
    *   **`methods`:** Target method name(s) or `"*"` (any method).
    *   **Usage:** `[ContractPermission(nameof(GasToken), "transfer")]`, `[ContractPermission("0x123abc...", "*")]`

*   **`[ManifestExtra(string key, string value)]`**:
    *   **Purpose:** Adds custom key-value pairs to the `extra` section of the manifest for arbitrary metadata. Can be applied multiple times.
    *   **Usage:** `[ManifestExtra("Author", "Alice")] [ManifestExtra("Version", "1.2.0")] public class MyContract...`

*   **Shorthand `ManifestExtra` Attributes:** These provide convenient wrappers for common `ManifestExtra` keys:
    *   **`[ContractAuthor(string author)]`**: Equivalent to `[ManifestExtra("Author", author)]`.
    *   **`[ContractDescription(string description)]`**: Equivalent to `[ManifestExtra("Description", description)]`.
    *   **`[ContractEmail(string email)]`**: Equivalent to `[ManifestExtra("Email", email)]`.
    *   **`[ContractVersion(string version)]`**: Equivalent to `[ManifestExtra("Version", version)]`.
    *   **`[ContractSourceCode(string url)]`**: Equivalent to `[ManifestExtra("SourceCode", url)]` (intended for linking to source code repository).

*   **`[ContractTrust(string contract)]`**:
    *   **Purpose:** Intended to specify which contracts are trusted (e.g., for permissioning purposes). Populates the `trusts` array.
    *   **Status:** Currently (as of N3 framework v3.x) often noted as **unused** by the compiler/VM, but part of the manifest specification.
    *   **Usage:** `[ContractTrust("0xabc123...")]` or `[ContractTrust("*")]`

*   **`[ContractGroup(ECPoint pubkey, string signature)]`**: (Advanced)
    *   **Purpose:** Used to define designated groups of contracts (identified by public keys) for use in `[ContractPermission]` with group scopes. Requires signature verification.
    *   **Usage:** Primarily for defining shared permissions among related contracts.

## Compiler Behavior & Optimization Attributes

These attributes affect how `nccs` compiles your C# code into NeoVM bytecode or enable specific features.

*   **`[Safe]`**:
    *   **Purpose:** Marks a method as read-only. It guarantees the method does not modify blockchain state (no `Storage.Put/Delete`, no `Runtime.Notify`, no state-changing `Contract.Call`).
    *   **Effect:** Sets `"safe": true` in the method's ABI entry. Allows calls using restrictive `CallFlags` (like `ReadStates`). The compiler *may* perform checks to enforce safety.
    *   **Usage:** `[Safe] public static BigInteger GetBalance(UInt160 acc) {...}`

*   **`[InitialValue(string value, ContractParameterType type)]`**:
    *   **Purpose:** Embeds a constant value directly into the compiled `.nef` script for a `static readonly` field, avoiding the need for `_deploy` initialization or `Storage.Get` for constants.
    *   **Effect:** Replaces field loads with direct `PUSHDATA` opcodes. Saves GAS on deployment and runtime access for constants.
    *   **`value`:** The constant value as a string, parsable according to `type`.
    *   **`type`:** Specifies how to parse the `value` string (e.g., `Hash160`, `Integer`, `String`, `ByteArray`, `PublicKey`).
    *   **Usage:**
        *   Still necessary for types where direct C# constant assignment isn't possible or easily representable (e.g., complex byte arrays represented as hex strings).
        *   **Alternative (Often Preferred Now):** For many types like `UInt160`, `ECPoint`, `string`, `BigInteger`, and primitives, recent `nccs` versions allow **direct C# assignment** using compile-time constants or literals. The compiler often translates these direct assignments into the same efficient `PUSHDATA` opcodes as `[InitialValue]`. Direct assignment is generally more readable and type-safe.
    *   **Example:**
        ```csharp
        // Old way / Still valid / Needed for some hex byte arrays:
        [InitialValue("Nd3uYA4nC5onLwQcfE6SkMcNVsTTs4T4oj", ContractParameterType.Hash160)]
        private static readonly UInt160 OwnerAddress_Attr;

        [InitialValue("010203", ContractParameterType.ByteArray)]
        private static readonly ByteString Prefix_Attr;

        // New way (often preferred for readability & type safety where possible):
        private static readonly UInt160 OwnerAddress_Direct = "Nd3uYA4nC5onLwQcfE6SkMcNVsTTs4T4oj"; // Requires using Neo.SmartContract.Framework;
        private static readonly ECPoint AdminPublicKey_Direct "03b209fd4f53a7170ea4444e0cb0a6bb6a53c2bd016926989cf85f9b0fba17a70c"; // Requires using Neo.SmartContract.Framework;
        private static readonly string TokenName_Direct = "MyToken";
        private static readonly BigInteger MaxSupply_Direct = 1000000;
        private static readonly byte[] Prefix_Direct = { 0x01, 0x02, 0x03 };

        ```
    *   **Recommendation:** **Strongly prefer direct C# assignment (`=`)** for `static readonly` constants whenever the value can be represented as a C# literal or via framework helper methods like `ToScriptHash()` or `ECPoint.FromString()`. It is more readable, type-safe, and idiomatic in modern Neo N3 development. Use `[InitialValue]` only as a fallback when direct assignment isn't feasible (e.g., initializing directly from a complex hex string for a `byte[]` without helper methods) or for specific compatibility reasons.

*   **`[OpCode(OpCode opcode, string operand = "")]`**: (Advanced / Use with Extreme Caution)
    *   **Purpose:** Directly injects a specific NeoVM `OpCode` (and optional operand) when the attributed method stub is called. Bypasses standard C# compilation for that call.
    *   **Effect:** Allows access to VM features not directly exposed by the framework or for micro-optimizations. Highly error-prone.
    *   **Usage:** Applied to `extern static` method declarations. `[OpCode(OpCode.NOP)] public static extern void MyNop();`

*   **`[Syscall(string method)]`**: (Advanced / Internal)
    *   **Purpose:** Maps an `extern static` method declaration directly to a NeoVM Interop `SYSCALL`.
    *   **Usage:** Primarily used **internally** by the `Neo.SmartContract.Framework` itself to define the wrappers around syscalls (e.g., how `Runtime.Log` maps to `System.Runtime.Log`). You generally **do not** use this directly in contract application code.

*   **`[CallingConvention(System.Runtime.InteropServices.CallingConvention convention)]`**: (Advanced / Internal)
    *   **Purpose:** Specifies the calling convention for P/Invoke or interop calls.
    *   **Usage:** Relevant mainly for low-level interop details, typically **not used** directly in standard contract code.

## Security Attributes

These attributes help enforce security patterns at compile time or runtime.

*   **`[NoReentrant]`**:
    *   **Purpose:** A compile-time check to prevent reentrancy vulnerabilities across the **entire contract**. The compiler analyzes methods and flags potential reentrancy issues if a method modifies state *after* making an external `Contract.Call`.
    *   **Effect:** If potential reentrancy is detected during compilation based on the Checks-Effects-Interactions pattern, the build will fail with an error (e.g., `NC4005`). Does not add runtime checks.
    *   **Usage:** Apply to the main contract class: `[NoReentrant] public class MySecureContract...`. This is a **highly recommended** attribute for most contracts.

*   **`[NoReentrantMethod]`**:
    *   **Purpose:** Similar to `[NoReentrant]`, but performs the compile-time reentrancy check only for the specific method it's applied to.
    *   **Effect:** Compiler checks for state modifications after external calls within this specific method. Build fails if potential reentrancy is found (`NC4005`).
    *   **Usage:** Apply to individual `public static` methods where reentrancy is a concern: `[NoReentrantMethod] public static bool RiskyWithdraw(...) {...}`. Can be used if applying `[NoReentrant]` to the whole class is too restrictive or causes issues with intended patterns.

## Type Hinting / Validation Attributes

These attributes primarily provide hints about the expected format or type of data, often for parameters or fields. They might be used by external tools, for documentation, or potentially by future compiler analyses. They don't typically alter the core NeoVM bytecode generation significantly compared to using the base C# types.

*   **`[Hash160]` / `[Hash160(isNullable: true)]`**: Hints the parameter/field should be a valid UInt160 script hash.
*   **`[PublicKey]` / `[PublicKey(isNullable: true)]`**: Hints the parameter/field should be a valid ECPoint public key.
*   **`[ByteArray]` / `[ByteArray(isNullable: true)]`**: Hints the parameter/field is raw byte array data.
*   **`[String]` / `[String(isNullable: true)]`**: Hints the parameter/field is string data.
*   **`[Integer(long min = long.MinValue, long max = long.MaxValue)]`**: Hints the parameter/field is an integer, optionally within a specific range (range check might not be enforced at runtime by default).
*   **`[ContractHash(string hash)]`**: Associates a specific known contract hash with a parameter or field, potentially for documentation or tool usage.

**Example Combining Attributes:**

```csharp
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;
using System.ComponentModel;
using System.Numerics;

[DisplayName("FullSampleContract")]
[ContractAuthor("Alice")]
[ContractVersion("1.0")]
[SupportedStandards("NEP-17")]
[ContractPermission(nameof(GasToken), "*")] // Allow all GasToken calls
[NoReentrant] // Apply contract-wide reentrancy check
public class FullAttributeDemo : SmartContract
{
    // Use direct assignment where possible (preferred)
    private static readonly UInt160 Owner = "Nd3uYA4nC5onLwQcfE6SkMcNVsTTs4T4oj";

    public delegate void ValueSetDelegate(BigInteger value);
    [DisplayName("ValueSet")]
    public static event ValueSetDelegate OnValueSet;

    [Safe] // Read-only method
    public static UInt160 GetOwner()
    {
        return Owner;
    }

    // Parameter type hint
    public static void SetValue([Integer(min: 0)] BigInteger newValue)
    {
        Helper.Assert(Runtime.CheckWitness(Owner), "Only Owner");
        Helper.Assert(newValue >= 0, "Value must be non-negative"); // Runtime check still needed

        // Store value...
        OnValueSet(newValue);
    }
}
```

Leveraging these attributes effectively is crucial for creating well-defined, secure, optimized, and maintainable Neo N3 smart contracts. The `[NoReentrant]` attribute, in particular, is a valuable addition for preventing common security flaws.

[Previous: Native Contracts Overview](./05-native-contracts/README.md) | [Next: Helper Methods](./07-helper-methods.md)
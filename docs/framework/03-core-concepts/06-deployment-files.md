# Deployment Artifacts: NEF and Manifest

When you compile your C# smart contract project using `Neo.Compiler.CSharp` (typically via `dotnet build`), two essential files are generated. These files are required to deploy and interact with your contract on the Neo blockchain.

## 1. NEF File (`.nef`)

*   **N**eo **E**xecutable **F**ormat.
*   Contains the compiled **NeoVM bytecode** of your smart contract.
*   This is the actual low-level instruction set that the Neo Virtual Machine executes.
*   It includes:
    *   Compiler identification (e.g., "neo-compiler-csharp v3.x.x").
    *   The raw NeoVM script (opcodes and data).
    *   Checksum for integrity verification.
*   This file is **required** for deploying the contract's logic to the blockchain using the `ContractManagement` native contract's `deploy` method.

```json
// Example structure (conceptual)
{
  "magic": 0x3346454E, // NEF3
  "compiler": "neo-compiler-csharp v3.6.2",
  // other header fields...
  "script": "0x...", // Hex-encoded NeoVM bytecode
  "checksum": 0x... // Ensure file integrity
}
```

## 2. Manifest File (`.manifest.json`)

*   A JSON file describing the **metadata and interface** of your smart contract.
*   It acts like an ABI (Application Binary Interface) for Neo contracts, telling users and applications how to interact with the contract.
*   Does **not** contain executable code; it's purely descriptive.
*   Generated based on your C# code, including class/method names, parameter types, return types, events, and attributes like `[DisplayName]`, `[SupportedStandards]`, `[ContractPermission]`, `[ManifestExtra]`, etc.
*   This file is **required** alongside the `.nef` file when deploying the contract.
*   Used by SDKs, wallets, and explorers to understand the contract's capabilities.

**Key Sections of a Manifest:**

```json
{
  "name": "MyContractDisplayName", // From [DisplayName] or <NeoContractName>
  "groups": [], // Groups for permissioning (optional)
  "features": {
      "storage": true, // Indicates if the contract uses storage
      "payable": true // Indicates if the contract accepts payments (e.g., has onNEP17Payment)
  },
  "supportedstandards": ["NEP-17"], // From [SupportedStandards]
  "abi": {
    "methods": [
      {
        "name": "getName", // Method name (respects [DisplayName])
        "parameters": [],
        "returntype": "String",
        "offset": 123, // Position in the NEF script
        "safe": false // Indicates if method only reads data (no state changes)
      },
      {
        "name": "transfer",
        "parameters": [
          {"name": "from", "type": "Hash160"},
          {"name": "to", "type": "Hash160"},
          {"name": "amount", "type": "Integer"}
        ],
        "returntype": "Boolean",
        "offset": 210,
        "safe": false
      }
      // ... other methods
    ],
    "events": [
      {
        "name": "Transfer", // Event name (respects [DisplayName])
        "parameters": [
          {"name": "from", "type": "Hash160"},
          {"name": "to", "type": "Hash160"},
          {"name": "amount", "type": "Integer"}
        ]
      }
      // ... other events
    ]
  },
  "permissions": [
    // Permissions declared using [ContractPermission]
    {"contract": "*", "methods": "*"} 
  ],
  "trusts": [], // Contracts that this contract trusts (usually * or specific hashes)
  "extra": {
      // Custom key-value pairs from [ManifestExtra]
      "Author": "Your Name",
      "Email": "your.email@example.com",
      "Description": "This is my example contract."
  }
}

```

*   **`name`**: User-friendly contract name.
*   **`groups`**: For advanced permission settings.
*   **`features`**: Indicates use of storage or payability.
*   **`supportedstandards`**: Lists NEP standards complied with (e.g., "NEP-17", "NEP-11").
*   **`abi`**: Defines the Application Binary Interface.
    *   **`methods`**: Lists all public static methods, their parameters (name and type), return type, offset in the NEF script, and whether the method is `safe` (read-only).
    *   **`events`**: Lists declared events and their parameters.
*   **`permissions`**: Specifies which contracts/methods this contract is allowed to call. Defined by `[ContractPermission]` attribute.
*   **`trusts`**: Currently unused in N3 compiler, intended for future features.
*   **`extra`**: Custom metadata defined by `[ManifestExtra]` attribute.

Together, the `.nef` file (the code) and the `.manifest.json` file (the description) provide everything needed to deploy and interact with your smart contract on the Neo N3 network.

[Previous: Data Types](./05-data-types.md) | [Next Section: Framework Features](../04-framework-features/README.md)
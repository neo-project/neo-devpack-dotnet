# Native Contract: RoleManagement

Namespace: `Neo.SmartContract.Framework.Native`

Provides a method to designate node roles within the Neo network.

## Node Roles (`Role` enum)

Defines specific roles nodes can have:

*   `StateValidator`: Nodes responsible for validating the state root.
*   `Oracle`: Nodes designated to fulfill oracle requests.
*   `NeoFsAlphabet`: Nodes participating in NeoFS alphabet operations.
*   `P2PNotary`: Nodes acting as notaries in P2P protocols.

## Key Methods

*   **`DesignateAsRole(Role role, ECPoint[] nodes)`**: Designates a list of nodes (by public key) for a specific role.
    *   This method can **only be called by the Neo Council** multi-sig address.
    *   Your contract typically won't call this directly.

*   **`GetDesignatedByRole(Role role, uint index)` (`ECPoint[]`)**: Retrieves the list of nodes designated for a specific role at a given block index (height).
    *   This allows contracts to find nodes with specific capabilities (like Oracles).

## Example Usage (Reading Roles)

```csharp
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;

// Permission to read designated Oracle nodes
[ContractPermission(nameof(RoleManagement), "getDesignatedByRole")]
[ContractPermission(nameof(LedgerContract), "currentIndex")] // To get current block height
public class RoleDemo : SmartContract
{
    // Get the list of designated Oracle nodes for the current block
    public static ECPoint[] GetOracleNodes()
    { 
        uint currentBlock = LedgerContract.CurrentIndex;
        return RoleManagement.GetDesignatedByRole(Role.Oracle, currentBlock);
    }

    // Get StateValidators for a specific past block
    public static ECPoint[] GetStateValidatorsForBlock(uint blockIndex)
    {
        return RoleManagement.GetDesignatedByRole(Role.StateValidator, blockIndex);
    }
}
```

Contracts mainly use `RoleManagement` to query the designated nodes for specific roles, particularly `Role.Oracle`, if they need to interact with or verify actions by those nodes.

[Previous: PolicyContract](./Policy.md) | [Next: StdLib](./StdLib.md)
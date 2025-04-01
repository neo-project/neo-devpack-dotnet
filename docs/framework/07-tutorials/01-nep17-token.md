# Tutorial: Building a NEP-17 Token

NEP-17 is the standard for fungible tokens on the Neo N3 blockchain (similar to ERC-20 on Ethereum). This tutorial guides you through creating a basic NEP-17 token contract in C#.

## 1. Project Setup

1.  Create a new C# class library project (`dotnet new classlib -n MyNep17Token`).
2.  Add the necessary NuGet packages:
    ```bash
    dotnet add package Neo.SmartContract.Framework
    dotnet add package Neo.Compiler.CSharp 
    ```
3.  Configure your `.csproj` file for Neo contract compilation (set `NeoContractName`, disable unwanted properties, add package references - see [Getting Started](../02-getting-started/02-installation.md)). Ensure you enable `<NeoContractDebugInfo>` for Debug builds.

## 2. Contract Code (`MyNep17Token.cs`)

Replace the default `Class1.cs` with `MyNep17Token.cs`:

```csharp
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Examples.NEP17
{
    [DisplayName("MySampleToken")] // Choose a display name
    [ManifestExtra("Author", "Your Name")]
    [ManifestExtra("Description", "A sample NEP-17 Token")]
    [SupportedStandards("NEP-17")] // Declare NEP-17 compliance
    [ContractPermission("*")] // Allow calling any contract (needed for onNEP17Payment callbacks)
    public class MyNep17Token : SmartContract
    {
        #region NEP-17 Settings

        // Use InitialValue for constants embedded in the NEF
        [InitialValue("MST", ContractParameterType.String)]
        private static readonly string _symbol;

        [InitialValue("8", ContractParameterType.Integer)] // Decimals (e.g., 8)
        private static readonly byte _decimals;

        // Use a variable for total supply if it can change (e.g., minting/burning)
        // Otherwise, use InitialValue for fixed supply
        // [InitialValue("1000000000000000", ContractParameterType.Integer)] // 10 million tokens with 8 decimals
        // private static readonly BigInteger _totalSupply;
        private static readonly byte[] TotalSupplyKey = { 0x11 };

        #endregion

        #region Storage Prefixes

        // Storage prefixes are crucial to prevent key collisions
        private static readonly StorageMap Balances = new StorageMap(Storage.CurrentContext, 0x01);
        private static readonly StorageMap Allowances = new StorageMap(Storage.CurrentContext, 0x02);

        #endregion

        #region NEP-17 Events

        // Define the Transfer event as required by NEP-17
        public delegate void TransferDelegate(UInt160 from, UInt160 to, BigInteger amount);
        [DisplayName("Transfer")]
        public static event TransferDelegate OnTransfer;

        #endregion

        #region NEP-17 Methods

        [Safe] // This method doesn't change state
        public static string Symbol()
        {
            return _symbol;
        }

        [Safe]
        public static byte Decimals()
        {
            return _decimals;
        }

        [Safe]
        public static BigInteger TotalSupply()
        {
            // If fixed supply, return _totalSupply
            // If variable, read from storage
            return (BigInteger)Storage.Get(Storage.CurrentContext, TotalSupplyKey);
        }

        [Safe]
        public static BigInteger BalanceOf(UInt160 account)
        {
            Helper.Assert(account.IsValid && !account.IsZero, "Invalid Account");
            return (BigInteger)Balances.Get(account);
        }

        // Primary Transfer method
        public static bool Transfer(UInt160 from, UInt160 to, BigInteger amount, object data)
        {
            // Validate arguments
            Helper.Assert(from.IsValid && !from.IsZero, "Invalid From Address");
            Helper.Assert(to.IsValid && !to.IsZero, "Invalid To Address");
            Helper.Assert(amount > 0, "Amount must be positive");

            // Check authorization (caller must be 'from' or approved)
            if (!Runtime.CheckWitness(from) && !IsApproved(from, Runtime.ExecutingScriptHash, amount)) 
                return false;

            // Check sender balance
            BigInteger fromBalance = BalanceOf(from);
            Helper.Assert(fromBalance >= amount, "Insufficient Balance");

            // Perform the transfer
            if (from == to) return true; // No change if transferring to self

            Balances.Put(from, fromBalance - amount);
            Balances.Put(to, BalanceOf(to) + amount);

            // Emit NEP-17 Transfer event
            PostTransfer(from, to, amount, data);
            return true;
        }

        #endregion

        #region Approval Methods (Optional but Recommended)

        // Although not strictly required by base NEP-17, approve/allowance is standard practice

        public static bool Approve(UInt160 owner, UInt160 spender, BigInteger amount)
        {
             Helper.Assert(owner.IsValid && !owner.IsZero, "Invalid Owner Address");
             Helper.Assert(spender.IsValid && !spender.IsZero, "Invalid Spender Address");
             Helper.Assert(amount >= 0, "Amount cannot be negative"); // Allow 0 to reset approval

            // Check authorization
            if (!Runtime.CheckWitness(owner)) return false;

            Allowances.Put(owner + spender, amount); // Combine owner+spender for key
            // Consider firing an Approval event here
            return true;
        }

        [Safe]
        public static BigInteger Allowance(UInt160 owner, UInt160 spender)
        {
            Helper.Assert(owner.IsValid && !owner.IsZero, "Invalid Owner Address");
             Helper.Assert(spender.IsValid && !spender.IsZero, "Invalid Spender Address");
            return (BigInteger)Allowances.Get(owner + spender);
        }

        // Helper to check if an allowance exists for Transfer
        private static bool IsApproved(UInt160 owner, UInt160 spender, BigInteger amount)
        {
            BigInteger allowance = Allowance(owner, spender);
            if (allowance < amount) return false;

            // Decrease allowance after use (important!)
            // Only decrease if allowance is not max value (effectively infinite)
            // NEP-17 doesn't mandate allowance decrease, but it's safer practice
            // if (allowance != BigInteger.MinusOne) // Assuming -1 means infinite
            // {
                 Allowances.Put(owner + spender, allowance - amount);
            // }
            return true;
        }

        #endregion

        #region Mint & Burn (Optional Example)
        // Add mint/burn functions if your token supply is variable

        private static readonly byte[] OwnerKey = { 0xFF }; // Example key for owner

        public static bool Mint(UInt160 account, BigInteger amount)
        {
            Helper.Assert(account.IsValid && !account.IsZero, "Invalid Account");
            Helper.Assert(amount > 0, "Amount must be positive");

            // Authorization: Only contract owner can mint
            ByteString owner = Storage.Get(Storage.CurrentContext, OwnerKey);
            Helper.Assert(owner != null, "Owner not set");
            if (!Runtime.CheckWitness((UInt160)owner)) return false;

            // Increase balance and total supply
            Balances.Put(account, BalanceOf(account) + amount);
            BigInteger supply = TotalSupply();
            Storage.Put(Storage.CurrentContext, TotalSupplyKey, supply + amount);

            // Emit Transfer event for minting (from null)
            PostTransfer(null, account, amount, null);
            return true;
        }

        public static bool Burn(UInt160 account, BigInteger amount)
        {
            Helper.Assert(account.IsValid && !account.IsZero, "Invalid Account");
            Helper.Assert(amount > 0, "Amount must be positive");

            // Authorization: Check if the burner signed
            if (!Runtime.CheckWitness(account)) return false;

            // Check balance
            BigInteger balance = BalanceOf(account);
            Helper.Assert(balance >= amount, "Insufficient balance to burn");

            // Decrease balance and total supply
            Balances.Put(account, balance - amount);
            BigInteger supply = TotalSupply();
            Storage.Put(Storage.CurrentContext, TotalSupplyKey, supply - amount);

            // Emit Transfer event for burning (to null)
            PostTransfer(account, null, amount, null);
            return true;
        }

        #endregion

        #region Deployment

        // _deploy is called on contract deployment and updates
        public static void _deploy(object data, bool update)
        { 
            if (!update) 
            { 
                // Initialize on first deployment
                BigInteger initialSupply = 1000000_00000000; // 10 million with 8 decimals
                UInt160 owner = Runtime.Transaction.Sender; // Or get from deploy data

                Balances.Put(owner, initialSupply);
                Storage.Put(Storage.CurrentContext, TotalSupplyKey, initialSupply);
                Storage.Put(Storage.CurrentContext, OwnerKey, owner);

                // Emit initial Transfer event
                PostTransfer(null, owner, initialSupply, null);
            } 
        }

        #endregion

        #region Helper Methods

        // Centralized place to emit Transfer event and call onNEP17Payment
        private static void PostTransfer(UInt160 from, UInt160 to, BigInteger amount, object data)
        {
            OnTransfer(from, to, amount);

            // Call onNEP17Payment if the recipient is a deployed contract
            if (to != null && ContractManagement.GetContract(to) != null)
            {
                Contract.Call(to, "onNEP17Payment", CallFlags.All, from, amount, data);
            }
        }

        #endregion
    }
}
```

## 3. Key Elements Explained

*   **Attributes:** `[DisplayName]`, `[ManifestExtra]`, `[SupportedStandards("NEP-17")]`, `[ContractPermission]` are crucial for defining the contract's identity and capabilities.
*   **Settings:** Define `Symbol`, `Decimals`. Total supply can be constant (`[InitialValue]`) or variable (stored).
*   **Storage:** Use `StorageMap` with distinct prefixes (`Balances`, `Allowances`) to avoid collisions.
*   **`OnTransfer` Event:** Mandatory NEP-17 event.
*   **Core Methods:** Implement `symbol`, `decimals`, `totalSupply`, `balanceOf`, `transfer` exactly as specified by NEP-17.
*   **Authorization:** Use `Runtime.CheckWitness` in `transfer` (and `mint`/`burn`/`approve`).
*   **Validation:** Use `Helper.Assert` to validate inputs and preconditions.
*   **`_deploy`:** Initialize supply, owner, and balances on first deployment.
*   **`PostTransfer` Helper:** Centralizes emitting the `OnTransfer` event and calling `onNEP17Payment` on recipient contracts.
*   **Approvals:** Include `approve` and `allowance` for standard token interaction patterns.
*   **Mint/Burn:** Add if your tokenomics require variable supply, ensuring proper authorization.

## 4. Compile and Deploy

1.  Compile the contract: `dotnet build`
2.  Deploy the generated `.nef` and `.manifest.json` files to Neo Express, TestNet, or MainNet using appropriate tools (e.g., `neox contract deploy ...`).

This provides a solid foundation for a NEP-17 token. You can extend it with more features like pausing, blacklisting, etc., based on your requirements.

[Previous: Tutorials Overview](./README.md) | [Next: Creating a Voting Contract](./02-voting-contract.md)
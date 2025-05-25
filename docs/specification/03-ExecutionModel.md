# 4. Execution Model

## 4.1 NeoVM

Neo N3 smart contracts are executed by the Neo Virtual Machine (NeoVM), a lightweight, stack-based virtual machine designed for blockchain environments. The NeoVM executes bytecode instructions and manages the execution context.

### 4.1.1 NeoVM Architecture

The NeoVM is composed of the following components:

1. **Execution Engine**: Manages the execution of bytecode instructions
2. **Evaluation Stack**: Stores operands and results of operations
3. **Invocation Stack**: Manages the call stack of methods
4. **Slot Stack**: Stores local variables
5. **Instruction Pointer**: Points to the current instruction being executed

### 4.1.2 NeoVM Execution Flow

The execution flow of the NeoVM is as follows:

1. Load the contract script into the execution engine
2. Initialize the execution context
3. Execute instructions one by one
4. Push and pop values from the evaluation stack
5. Call methods and return from methods
6. Handle exceptions
7. Return the result of execution

### 4.1.3 NeoVM Instruction Set

The NeoVM instruction set includes operations for:

- Stack manipulation
- Flow control
- Arithmetic operations
- Logical operations
- String operations
- Array operations
- Struct operations
- Type conversion
- Cryptographic operations
- Blockchain interaction

Example of NeoVM bytecode:

```
PUSHDATA1 0x0c 0x48656c6c6f2c20576f726c6421 // Push "Hello, World!" onto the stack
RET                                         // Return from the current method
```

### 4.1.4 NeoVM Limitations

The NeoVM has the following limitations:

- Maximum stack size: 2048 items
- Maximum item size: 2MB
- Maximum call depth: 1024 frames
- Maximum try-catch depth: 16

## 4.2 Execution Context

Each contract execution has an associated context that includes:
- The calling script hash
- The executing script hash
- The entry script hash
- The current transaction
- The trigger type

### 4.2.1 Script Hashes

Script hashes identify contracts and accounts on the Neo blockchain. They are 160-bit hashes calculated as:

```
ScriptHash = RIPEMD160(SHA256(Script))
```

The script hashes in the execution context can be accessed through the `Runtime` class:

```csharp
using Neo;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services;

// Access script hashes in execution context
UInt160 caller = Runtime.CallingScriptHash;
UInt160 current = Runtime.ExecutingScriptHash;
UInt160 entry = Runtime.EntryScriptHash;

// Example usage in authorization
public static bool IsAuthorized()
{
    // Check if the caller is a trusted contract
    UInt160 trustedContract = GetTrustedContract();
    return Runtime.CallingScriptHash == trustedContract;
}
```

### 4.2.2 Transaction Context

The current transaction can be accessed through the `Runtime` class:

```csharp
using Neo;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services;
using Neo.Network.P2P.Payloads;

// Access current transaction
Transaction tx = Runtime.Transaction;

// Extract transaction information
public static TransactionInfo GetTransactionInfo()
{
    Transaction tx = Runtime.Transaction;

    return new TransactionInfo
    {
        Hash = tx.Hash,
        Sender = tx.Sender,
        NetworkFee = tx.NetworkFee,
        SystemFee = tx.SystemFee,
        ValidUntilBlock = tx.ValidUntilBlock,
        Version = tx.Version,
        Nonce = tx.Nonce
    };
}

public class TransactionInfo
{
    public UInt256 Hash;
    public UInt160 Sender;
    public long NetworkFee;
    public long SystemFee;
    public uint ValidUntilBlock;
    public byte Version;
    public uint Nonce;
}
```

#### Transaction Context Information

The transaction context provides access to:

| Property | Type | Description |
|:---------|:-----|:------------|
| `Hash` | `UInt256` | Unique transaction identifier |
| `Sender` | `UInt160` | Transaction sender's script hash |
| `NetworkFee` | `long` | Fee paid for network processing |
| `SystemFee` | `long` | Fee paid for system resources |
| `ValidUntilBlock` | `uint` | Block height until which transaction is valid |
| `Version` | `byte` | Transaction format version |
| `Nonce` | `uint` | Random number to prevent replay attacks |

### 4.2.3 Trigger Type

The trigger type indicates the context in which a contract is executed. It can be accessed through the `Runtime` class:

```csharp
using Neo;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services;
using System;

// Access trigger type
TriggerType trigger = Runtime.Trigger;

// Example trigger-based logic
public static object Main(string method, object[] args)
{
    switch (Runtime.Trigger)
    {
        case TriggerType.Application:
            return HandleApplicationTrigger(method, args);
        case TriggerType.Verification:
            return HandleVerificationTrigger();
        case TriggerType.System:
            return HandleSystemTrigger();
        default:
            throw new Exception("Unsupported trigger type");
    }
}
```

## 4.3 Execution Triggers

Neo N3 smart contracts can be executed in response to different triggers:

| Trigger | Description | Use Cases |
|:--------|:------------|:----------|
| `Application` | Contract is called directly by a transaction | Normal method invocations, token transfers, business logic |
| `Verification` | Contract is used to verify a transaction | Custom signature schemes, multi-sig wallets, time locks |
| `System` | Contract is called by the system | Contract deployment, updates, destruction |

### 4.3.1 Application Trigger

The application trigger is used when a contract is called directly by a transaction. This is the most common trigger type and is used for normal contract invocations.

```csharp
using Neo;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services;
using System;
using System.Numerics;

public static object HandleApplicationTrigger(string method, object[] args)
{
    if (Runtime.Trigger != TriggerType.Application)
        throw new Exception("Invalid trigger type");

    switch (method)
    {
        case "transfer":
            return Transfer((UInt160)args[0], (UInt160)args[1], (BigInteger)args[2]);
        case "balanceOf":
            return BalanceOf((UInt160)args[0]);
        case "totalSupply":
            return TotalSupply();
        default:
            throw new Exception("Method not found");
    }
}
```

### 4.3.2 Verification Trigger

The verification trigger is used when a contract is used to verify a transaction. This is typically used for custom verification logic, such as multi-signature schemes or time-locked transactions.

```csharp
using Neo;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services;
using System;

public static bool HandleVerificationTrigger()
{
    if (Runtime.Trigger != TriggerType.Verification)
        return false;

    // Example: Multi-signature verification
    UInt160[] owners = GetOwners();
    int requiredSignatures = GetRequiredSignatureCount();
    int validSignatures = 0;

    foreach (UInt160 owner in owners)
    {
        if (Runtime.CheckWitness(owner))
        {
            validSignatures++;
            if (validSignatures >= requiredSignatures)
                return true;
        }
    }

    return false;
}

// Example: Time-locked verification
public static bool TimeLockedVerify()
{
    if (Runtime.Trigger != TriggerType.Verification)
        return false;

    // Check if the time lock has expired
    ulong unlockTime = GetUnlockTime();
    if (Runtime.Time < unlockTime)
        return false;

    // Verify owner signature
    UInt160 owner = GetOwner();
    return Runtime.CheckWitness(owner);
}
```

### 4.3.3 System Trigger

The system trigger is used when a contract is called by the system, such as during contract deployment or destruction.

```csharp
using Neo;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services;

public static void HandleSystemTrigger()
{
    if (Runtime.Trigger != TriggerType.System)
        return;

    // System triggers are typically handled by special methods
    // like _deploy, but can also be handled in main contract logic

    // Example: Log system events
    Runtime.Log("System trigger executed");
}
```

## 4.4 Execution Constraints

Neo N3 smart contracts are subject to the following constraints:

1. **Gas Limit**: Each contract execution is limited by the amount of GAS provided.
2. **Stack Size**: The maximum stack size is limited to 2048 items.
3. **Item Size**: The maximum size of an item in the VM is limited to 2MB.
4. **Call Depth**: The maximum call depth is limited to 1024 frames.
5. **Try-Catch Depth**: The maximum nesting depth of try-catch blocks is limited to 16.

### 4.4.1 Gas Limit

Each contract execution is limited by the amount of GAS provided. If the execution runs out of GAS, it is terminated with an out-of-gas error.

The gas limit is specified when invoking a contract:

```csharp
// Invoke a contract with a gas limit of 10 GAS
ApplicationEngine.Run(script, snapshot, gas: 10_00000000);
```

The gas consumed during execution can be monitored:

```csharp
long gasLeft = Runtime.GasLeft;
```

### 4.4.2 Stack Size Limit

The maximum stack size is limited to 2048 items. If the stack exceeds this limit, the execution is terminated with a stack overflow error.

### 4.4.3 Item Size Limit

The maximum size of an item in the VM is limited to 2MB. If an item exceeds this limit, the execution is terminated with an item size exceeded error.

### 4.4.4 Call Depth Limit

The maximum call depth is limited to 1024 frames. If the call depth exceeds this limit, the execution is terminated with a call depth exceeded error.

### 4.4.5 Try-Catch Depth Limit

The maximum nesting depth of try-catch blocks is limited to 16. If the try-catch depth exceeds this limit, the execution is terminated with a try-catch depth exceeded error.

## 4.5 Gas Costs

Operations in Neo N3 smart contracts consume GAS based on their complexity. Gas costs are measured in datoshi (1 datoshi = 10^-8 GAS).

### 4.5.1 Instruction Gas Costs

Each NeoVM instruction has an associated gas cost. Here are the most commonly used instructions:

#### Stack Operations

| Instruction | Gas Cost (datoshi) | Description |
|:------------|:------------------:|:------------|
| `PUSHINT8` | 1 | Push 8-bit integer |
| `PUSHINT16` | 1 | Push 16-bit integer |
| `PUSHINT32` | 1 | Push 32-bit integer |
| `PUSHINT64` | 1 | Push 64-bit integer |
| `PUSHINT128` | 4 | Push 128-bit integer |
| `PUSHINT256` | 4 | Push 256-bit integer |
| `PUSHT` | 1 | Push boolean true |
| `PUSHF` | 1 | Push boolean false |
| `PUSHA` | 4 | Push address |
| `PUSHNULL` | 1 | Push null value |

#### Data Operations

| Instruction | Gas Cost (datoshi) | Description |
|:------------|:------------------:|:------------|
| `PUSHDATA1` | 8 | Push data (≤ 255 bytes) |
| `PUSHDATA2` | 512 | Push data (≤ 65535 bytes) |
| `PUSHDATA4` | 4096 | Push data (≤ 4GB) |

#### Arithmetic Operations

| Instruction | Gas Cost (datoshi) | Description |
|:------------|:------------------:|:------------|
| `ADD` | 1 | Addition |
| `SUB` | 1 | Subtraction |
| `MUL` | 1 | Multiplication |
| `DIV` | 1 | Division |
| `MOD` | 1 | Modulo |
| `SHL` | 1 | Shift left |
| `SHR` | 1 | Shift right |

#### Logical Operations

| Instruction | Gas Cost (datoshi) | Description |
|:------------|:------------------:|:------------|
| `BOOLAND` | 1 | Boolean AND |
| `BOOLOR` | 1 | Boolean OR |
| `NUMEQUAL` | 1 | Numeric equality |
| `NUMNOTEQUAL` | 1 | Numeric inequality |
| `LT` | 1 | Less than |
| `LE` | 1 | Less than or equal |
| `GT` | 1 | Greater than |
| `GE` | 1 | Greater than or equal |

#### Array Operations

| Instruction | Gas Cost (datoshi) | Description |
|:------------|:------------------:|:------------|
| `ARRAYSIZE` | 1 | Get array size |
| `PACK` | 4 | Pack items into array |
| `UNPACK` | 4 | Unpack array into items |
| `NEWARRAY` | 4 | Create new array |
| `NEWSTRUCT` | 4 | Create new struct |

### 4.5.2 Storage Operation Gas Costs

Storage operations consume GAS based on the size of the data being stored:

| Operation | Base Cost (datoshi) | Size-Based Cost | Description |
|:----------|:-------------------:|:----------------|:------------|
| `Storage.Get` | 1,000,000 | + (StoragePrice × DataSize) | Retrieve data from storage |
| `Storage.Put` | 1,000,000 | + (StoragePrice × DataSize) | Store data in storage |
| `Storage.Delete` | 1,000,000 | None | Delete data from storage |
| `Storage.Find` | 1,000,000 | + iteration costs | Find data with prefix |

#### Storage Cost Calculation

The total storage cost is calculated as:

```
TotalCost = BaseCost + (StoragePrice × DataSize)
```

**Where:**
- `BaseCost`: Fixed cost for the storage operation
- `StoragePrice`: Price per byte set by the Policy native contract (typically 100,000 datoshi/byte)
- `DataSize`: Size of the data in bytes

**Example:**
```csharp
// Storing 100 bytes costs approximately:
// 1,000,000 + (100,000 × 100) = 11,000,000 datoshi = 0.11 GAS
```

### 4.5.3 Cryptographic Operation Gas Costs

Cryptographic operations consume more GAS due to their computational complexity:

| Operation | Gas Cost (datoshi) | Description |
|:----------|:------------------:|:------------|
| `Crypto.CheckSig` | 32,768 | Verify single ECDSA signature |
| `Crypto.CheckMultiSig` | 32,768 × n | Verify multiple signatures (n = signature count) |
| `Crypto.Sha256` | 1,024 | SHA-256 hash function |
| `Crypto.Ripemd160` | 1,024 | RIPEMD-160 hash function |
| `Crypto.VerifyWithECDsa` | 65,536 | Verify ECDSA signature with custom curve |

#### Cryptographic Cost Examples

```csharp
// Single signature verification: 32,768 datoshi
bool isValid = Crypto.CheckSig(signature, publicKey);

// Multi-signature (3 signatures): 32,768 × 3 = 98,304 datoshi
bool isValidMulti = Crypto.CheckMultiSig(signatures, publicKeys);

// Hash operations: 1,024 datoshi each
ByteString hash = Crypto.Sha256(data);
ByteString ripemd = Crypto.Ripemd160(data);
```

### 4.5.4 Contract Call Gas Costs

Contract calls consume GAS based on the complexity of the called contract:

| Operation | Gas Cost (datoshi) |
|-----------|-------------------|
| Contract.Call | Base cost + called contract cost |

The base cost is a fixed cost for the call operation, and the called contract cost is the cost of executing the called contract.

### 4.5.5 Gas Refund

In some cases, GAS can be refunded during contract execution:

1. When a storage item is deleted, a portion of the GAS used to store it is refunded.
2. When a contract is destroyed, a portion of the GAS used to deploy it is refunded.

### 4.5.6 Gas Fee Factors

The gas costs can be adjusted by the following factors:

1. **ExecFeeFactor**: A multiplier for execution fees
2. **StoragePrice**: The price per byte for storage operations

These factors are set by the Policy native contract and can be changed through governance.

## 4.6 Execution Results

The result of a contract execution is a stack item that represents the return value of the contract. The stack item can be of the following types:

1. **Boolean**: A boolean value (true or false)
2. **Integer**: An arbitrary-precision integer
3. **ByteString**: A sequence of bytes
4. **Array**: An ordered collection of stack items
5. **Struct**: An ordered collection of stack items (similar to Array but with value semantics)
6. **Map**: A collection of key-value pairs
7. **InteropInterface**: An interface to interact with the blockchain

### 4.6.1 Return Values

Contract methods can return values of the following types:

- `bool`: Boolean value
- `byte[]`: Byte array
- `string`: String
- `BigInteger`: Arbitrary-precision integer
- `UInt160`: 160-bit unsigned integer
- `UInt256`: 256-bit unsigned integer
- `ECPoint`: Elliptic curve point
- Custom types: Classes and structs defined in the contract

Example of a method with a return value:

```csharp
[Safe]
public static BigInteger BalanceOf(UInt160 account)
{
    if (account is null || !account.IsValid)
        throw new Exception("Invalid account");

    StorageMap balances = new StorageMap(Storage.CurrentContext, "balances");
    return (BigInteger)balances.Get(account);
}
```

### 4.6.2 Exceptions

Contract execution can result in exceptions, which terminate the execution and return an error. Exceptions can be caught using try-catch blocks:

```csharp
try
{
    // Code that might throw an exception
}
catch
{
    // Handle the exception
}
```

Limitations:
- Only one catch block is allowed per try block
- No exception type filtering is supported

### 4.6.3 Execution States

The execution of a contract can result in the following states:

1. **HALT**: The execution completed successfully
2. **FAULT**: The execution terminated with an error
3. **BREAK**: The execution is paused (used for debugging)

## 4.7 Execution Environment

The execution environment provides access to blockchain data and services through syscalls.

### 4.7.1 Blockchain Data

Contracts can access blockchain data through the following classes:

1. **Ledger**: Provides access to blocks, transactions, and blockchain state
2. **Runtime**: Provides access to execution context and blockchain environment
3. **Storage**: Provides access to contract storage

Example of accessing blockchain data:

```csharp
// Get the current block height
uint height = Ledger.CurrentIndex;

// Get the current timestamp
ulong timestamp = Runtime.Time;

// Get a value from storage
ByteString value = Storage.Get(Storage.CurrentContext, key);
```

### 4.7.2 Runtime Services

The `Runtime` class provides comprehensive access to execution context and blockchain environment:

**Runtime Properties:**

```csharp
public static class Runtime
{
    // Trigger and platform information
    public static extern TriggerType Trigger { get; }
    public static extern string Platform { get; }

    // Transaction and script context
    public static extern Transaction Transaction { get; }
    public static extern UInt160 ExecutingScriptHash { get; }
    public static extern UInt160 CallingScriptHash { get; }
    public static extern UInt160 EntryScriptHash { get; }

    // Execution environment
    public static extern ulong Time { get; }
    public static extern uint InvocationCounter { get; }
    public static extern long GasLeft { get; }
    public static extern byte AddressVersion { get; }
}
```

**Runtime Methods:**

```csharp
// Witness verification
public static extern bool CheckWitness(UInt160 hash);
public static extern bool CheckWitness(ECPoint pubkey);

// Logging and debugging
public static extern void Log(string message);
public static extern void Debug(string message);

// Gas and network operations
public static extern void BurnGas(long gas);
public static extern uint GetNetwork();
public static extern BigInteger GetRandom();

// Advanced operations
public static extern object LoadScript(ByteString script, CallFlags flags, params object[] args);
public static extern Signer[] CurrentSigners();
```

**Usage Examples:**

```csharp
// Check execution context
if (Runtime.Trigger == TriggerType.Application)
{
    // Normal contract execution
    Runtime.Log("Contract invoked");
}
else if (Runtime.Trigger == TriggerType.Verification)
{
    // Transaction verification
    return Runtime.CheckWitness(owner);
}

// Access execution information
ulong timestamp = Runtime.Time;
uint invocations = Runtime.InvocationCounter;
long remainingGas = Runtime.GasLeft;

// Verify authorization
if (!Runtime.CheckWitness(owner))
{
    throw new Exception("Unauthorized");
}

// Generate random numbers
BigInteger randomValue = Runtime.GetRandom();

// Get network information
uint networkId = Runtime.GetNetwork();
```

### 4.7.3 Iterator Services

The `Iterator` and `Iterator<T>` classes provide iteration capabilities for storage operations:

```csharp
public class Iterator : IApiInterface, IEnumerable
{
    public extern bool Next();
    public extern object Value { get; }
}

public class Iterator<T> : Iterator, IEnumerable<T>
{
    public extern new T Value { get; }
}
```

**Usage with Storage.Find:**

```csharp
// Find all items with a prefix
Iterator iterator = Storage.Find("prefix");
while (iterator.Next())
{
    ByteString key = (ByteString)iterator.Key;
    ByteString value = (ByteString)iterator.Value;
    // Process key-value pair
}

// Find with options
Iterator keysOnly = Storage.Find("prefix", FindOptions.KeysOnly);
while (keysOnly.Next())
{
    ByteString key = (ByteString)keysOnly.Value;
    // Process key only
}
```

### 4.7.4 Cryptographic Services

The `Crypto` class provides cryptographic operations:

```csharp
public static class Crypto
{
    // Signature verification
    public static extern bool CheckSig(ECPoint pubkey, ByteString signature);
    public static extern bool CheckMultisig(ECPoint[] pubkeys, ByteString[] signatures);
}
```

**Usage Examples:**

```csharp
// Verify a single signature
ECPoint publicKey = ECPoint.Parse("03...", ECCurve.Secp256r1);
ByteString signature = "...";
bool isValid = Crypto.CheckSig(publicKey, signature);

// Verify multi-signature (2-of-3)
ECPoint[] publicKeys = new ECPoint[] { pubkey1, pubkey2, pubkey3 };
ByteString[] signatures = new ByteString[] { sig1, sig2 };
bool isValidMultisig = Crypto.CheckMultisig(publicKeys, signatures);
```

### 4.7.5 Syscalls

Syscalls provide access to blockchain functionality from within a contract. They are implemented as external method calls with the `[Syscall]` attribute.

Example of syscall usage:

```csharp
// Check if a witness is valid
bool isValid = Runtime.CheckWitness(owner);

// Get the current timestamp
ulong timestamp = Runtime.Time;

// Call another contract
object result = Contract.Call(scriptHash, method, CallFlags.All, args);
```

### 4.7.6 Native Contracts

Native contracts provide core functionality and can be accessed through their respective classes:

1. **ContractManagement**: Manages contract deployment, updates, and destruction
2. **StdLib**: Provides standard library functions
3. **CryptoLib**: Provides cryptographic functions
4. **NEO**: Manages the NEO token
5. **GAS**: Manages the GAS token
6. **Oracle**: Provides oracle services for external data
7. **Policy**: Manages blockchain policies
8. **RoleManagement**: Manages blockchain roles
9. **Ledger**: Provides access to blockchain data

Example of native contract usage:

```csharp
// Deploy a contract
Contract contract = ContractManagement.Deploy(nefFile, manifest, data);

// Get the NEO balance of an account
BigInteger balance = NEO.BalanceOf(account);

// Get the current gas price
long gasPrice = Policy.GetFeePerByte();
```

## 4.8 Execution Scenarios

### 4.8.1 Contract Invocation

Contracts can be invoked directly by transactions or by other contracts. The invocation process is as follows:

1. Create a transaction with the contract script hash and method parameters
2. Sign the transaction with the appropriate accounts
3. Submit the transaction to the blockchain
4. The transaction is executed by the NeoVM
5. The result of the execution is returned

Example of contract invocation:

```csharp
// Invoke a contract method
object result = Contract.Call(scriptHash, method, CallFlags.All, args);
```

### 4.8.2 Contract Verification

Contracts can be used to verify transactions. The verification process is as follows:

1. Create a transaction with the contract script hash as a signer
2. Sign the transaction with the appropriate accounts
3. Submit the transaction to the blockchain
4. The contract's `verify` method is called with the Verification trigger
5. If the `verify` method returns `true`, the transaction is valid

Example of contract verification:

```csharp
public static bool Verify()
{
    UInt160 owner = (UInt160)Storage.Get(Storage.CurrentContext, "owner");
    return Runtime.CheckWitness(owner);
}
```

### 4.8.3 Contract Callbacks

Contracts can implement callback methods that are called in response to specific events:

1. **onNEP17Payment**: Called when a NEP-17 token is transferred to the contract
2. **onNEP11Payment**: Called when a NEP-11 token is transferred to the contract
3. **_deploy**: Called when a contract is deployed or updated

Example of a callback method:

```csharp
public static void onNEP17Payment(UInt160 from, BigInteger amount, object data)
{
    // Handle NEP-17 payment
}
```

## 4.9 Complete Execution Example

Here's a complete example of a contract that demonstrates various execution scenarios:

```csharp
using Neo;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;
using System;
using System.Numerics;

[DisplayName("ExecutionContract")]
[ContractDescription("A contract demonstrating execution scenarios")]
[ContractEmail("dev@example.com")]
[ContractVersion("1.0.0")]
[ContractPermission(Permission.Any, Method.Any)]
public class ExecutionContract : SmartContract
{
    // Storage keys
    private static readonly byte[] OwnerKey = new byte[] { 0x01 };
    private static readonly byte[] BalanceKey = new byte[] { 0x02 };

    // Events
    public static event Action<UInt160, BigInteger> Deposit;
    public static event Action<UInt160, BigInteger> Withdrawal;

    // Initialization
    public static void _deploy(object data, bool update)
    {
        if (update) return;

        UInt160 owner = (UInt160)data;
        Storage.Put(Storage.CurrentContext, OwnerKey, owner);
    }

    // Verification
    public static bool Verify()
    {
        UInt160 owner = (UInt160)Storage.Get(Storage.CurrentContext, OwnerKey);
        return Runtime.CheckWitness(owner);
    }

    // Owner management
    [Safe]
    public static UInt160 GetOwner()
    {
        return (UInt160)Storage.Get(Storage.CurrentContext, OwnerKey);
    }

    // Balance management
    [Safe]
    public static BigInteger GetBalance()
    {
        ByteString value = Storage.Get(Storage.CurrentContext, BalanceKey);
        return value is null ? 0 : (BigInteger)value;
    }

    // Deposit GAS
    public static void Deposit()
    {
        UInt160 sender = Runtime.CallingScriptHash;
        BigInteger amount = 0;

        // Check if this is a NEP-17 payment
        if (Runtime.CallingScriptHash == GAS.Hash)
        {
            // This is a direct deposit
            amount = (BigInteger)Runtime.Transaction.GetAttribute<BigInteger>("Amount");
        }

        if (amount <= 0)
            throw new Exception("Invalid deposit amount");

        // Update balance
        BigInteger balance = GetBalance();
        Storage.Put(Storage.CurrentContext, BalanceKey, balance + amount);

        // Emit event
        Deposit(sender, amount);
    }

    // Withdraw GAS
    public static bool Withdraw(BigInteger amount)
    {
        if (amount <= 0)
            throw new Exception("Invalid withdrawal amount");

        UInt160 sender = Runtime.CallingScriptHash;
        BigInteger balance = GetBalance();

        if (balance < amount)
            return false;

        // Update balance
        Storage.Put(Storage.CurrentContext, BalanceKey, balance - amount);

        // Transfer GAS
        GAS.Transfer(Runtime.ExecutingScriptHash, sender, amount);

        // Emit event
        Withdrawal(sender, amount);

        return true;
    }

    // NEP-17 callback
    public static void onNEP17Payment(UInt160 from, BigInteger amount, object data)
    {
        // Only accept GAS
        if (Runtime.CallingScriptHash != GAS.Hash)
            throw new Exception("Only GAS is accepted");

        // Update balance
        BigInteger balance = GetBalance();
        Storage.Put(Storage.CurrentContext, BalanceKey, balance + amount);

        // Emit event
        Deposit(from, amount);
    }

    // Execute with different triggers
    public static object Main(string method, object[] args)
    {
        if (Runtime.Trigger == TriggerType.Verification)
        {
            return Verify();
        }
        else if (Runtime.Trigger == TriggerType.Application)
        {
            if (method == "getOwner")
                return GetOwner();
            else if (method == "getBalance")
                return GetBalance();
            else if (method == "deposit")
                Deposit();
            else if (method == "withdraw")
                return Withdraw((BigInteger)args[0]);
            else
                throw new Exception("Unknown method");
        }

        return false;
    }

    // Exception handling
    public static bool SafeWithdraw(BigInteger amount)
    {
        try
        {
            return Withdraw(amount);
        }
        catch
        {
            return false;
        }
    }

    // Gas management
    public static void BurnGas(long amount)
    {
        Runtime.BurnGas(amount);
    }

    // Contract calls
    public static object CallContract(UInt160 scriptHash, string method, object[] args)
    {
        return Contract.Call(scriptHash, method, CallFlags.All, args);
    }
}
```

This contract demonstrates:
- Initialization during deployment
- Verification for secure operations
- Balance management
- GAS deposits and withdrawals
- NEP-17 payment handling
- Trigger-specific behavior
- Exception handling
- Gas management
- Contract calls

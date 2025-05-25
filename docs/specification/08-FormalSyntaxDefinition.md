# 9. Formal Syntax Definition

This section provides a simplified syntax definition for Neo N3 smart contracts.

## 9.1 Contract Structure

### 9.1.1 Basic Contract

```ebnf
Contract ::= ContractAttributes "public class" Identifier ":" "SmartContract" "{" ContractMembers "}"

ContractAttributes ::= {ContractAttribute}
ContractAttribute ::= "[" AttributeName ["(" Parameters ")"] "]"
ContractMembers ::= {Method | Event | Field}
```

**Example:**
```csharp
[DisplayName("MyContract")]
[ContractPermission(Permission.Any, Method.Any)]
public class MyContract : SmartContract
{
    // Contract members
}
```

### 9.1.2 Common Attributes

| Attribute | Purpose | Example |
|:----------|:--------|:--------|
| `DisplayName` | Contract name | `[DisplayName("Token")]` |
| `ContractPermission` | Call permissions | `[ContractPermission(Permission.Any, Method.Any)]` |
| `Safe` | Read-only method | `[Safe]` |
| `NoReentrant` | Reentrancy protection | `[NoReentrant]` |

## 9.2 Method Definition

```ebnf
Method ::= MethodAttributes "public static" ReturnType Identifier "(" Parameters ")" "{" Statements "}"

MethodAttributes ::= {MethodAttribute}
Parameters ::= [Parameter {"," Parameter}]
Parameter ::= Type Identifier
```

**Example:**
```csharp
[Safe]
public static BigInteger BalanceOf(UInt160 account)
{
    return (BigInteger)Storage.Get(Storage.CurrentContext, account);
}
```

## 9.3 Event Definition

```ebnf
Event ::= EventAttributes "public static event Action" "<" EventParameters ">" Identifier ";"
EventParameters ::= Type {"," Type}
```

**Example:**
```csharp
[DisplayName("Transfer")]
public static event Action<UInt160, UInt160, BigInteger> OnTransfer;
```

## 9.4 Field Definition

```ebnf
Field ::= FieldModifiers Type Identifier ["=" Expression] ";"
FieldModifiers ::= ["private" | "public"] ["static"] ["readonly" | "const"]
```

**Example:**
```csharp
private static readonly byte[] PrefixBalance = new byte[] { 0x01 };
private const string Name = "MyToken";
```

## 9.5 Storage Operations

```ebnf
StorageGet ::= "Storage.Get" "(" StorageContext "," Key ")"
StoragePut ::= "Storage.Put" "(" StorageContext "," Key "," Value ")"
StorageDelete ::= "Storage.Delete" "(" StorageContext "," Key ")"
StorageFind ::= "Storage.Find" "(" StorageContext "," Prefix ["," FindOptions] ")"

StorageContext ::= "Storage.CurrentContext" | "Storage.CurrentReadOnlyContext"
```

**Example:**
```csharp
// Basic storage operations
ByteString value = Storage.Get(Storage.CurrentContext, key);
Storage.Put(Storage.CurrentContext, key, value);
Storage.Delete(Storage.CurrentContext, key);

// Find with options
Iterator iterator = Storage.Find(Storage.CurrentContext, prefix, FindOptions.RemovePrefix);
```

## 9.6 Contract Calls

```ebnf
ContractCall ::= "Contract.Call" "(" ScriptHash "," MethodName "," CallFlags "," Arguments ")"
CallFlags ::= "CallFlags.ReadStates" | "CallFlags.States" | "CallFlags.All"
Arguments ::= "new object[] {" [ArgumentList] "}"
```

**Example:**
```csharp
object result = Contract.Call(scriptHash, "method", CallFlags.All, new object[] { arg1, arg2 });
```

## 9.7 Runtime Operations

```ebnf
CheckWitness ::= "Runtime.CheckWitness" "(" Address ")"
Notify ::= "Runtime.Notify" "(" EventName {"," Argument} ")"
Log ::= "Runtime.Log" "(" Message ")"
```

**Example:**
```csharp
// Authorization and events
bool authorized = Runtime.CheckWitness(owner);
Runtime.Notify("Transfer", from, to, amount);
Runtime.Log("Operation completed");

// Context information
ulong timestamp = Runtime.Time;
TriggerType trigger = Runtime.Trigger;
UInt160 caller = Runtime.CallingScriptHash;
```

## 9.8 Native Contracts

```ebnf
NativeCall ::= NativeContract "." Method "(" [Arguments] ")"
NativeContract ::= "NEO" | "GAS" | "ContractManagement" | "StdLib" | "CryptoLib"
```

**Example:**
```csharp
BigInteger balance = NEO.BalanceOf(account);
bool success = GAS.Transfer(from, to, amount);
Contract contract = ContractManagement.Deploy(nefFile, manifest);
```

## 9.9 Control Flow

```ebnf
IfStatement ::= "if" "(" Condition ")" Block ["else" Block]
WhileStatement ::= "while" "(" Condition ")" Block
ForStatement ::= "for" "(" Init ";" Condition ";" Update ")" Block
TryCatch ::= "try" Block "catch" Block
```

**Example:**
```csharp
// Conditional execution
if (balance > amount)
{
    // Transfer logic
}

// Exception handling
try
{
    Contract.Call(scriptHash, "method", CallFlags.All, args);
}
catch
{
    return false;
}
```

## 9.10 Common Types

```ebnf
Type ::= PrimitiveType | NeoType | ArrayType
PrimitiveType ::= "bool" | "byte" | "int" | "string" | "object"
NeoType ::= "BigInteger" | "UInt160" | "UInt256" | "ByteString" | "ECPoint"
ArrayType ::= Type "[" "]"
```

**Example:**
```csharp
// Neo N3 common types
bool success = true;
BigInteger amount = 1000;
UInt160 account = Runtime.CallingScriptHash;
UInt256 txHash = Runtime.Transaction.Hash;
ByteString data = Storage.Get(Storage.CurrentContext, key);
byte[] prefix = new byte[] { 0x01 };
```

## 9.11 Basic Expressions

```ebnf
Expression ::= Literal | Identifier | BinaryExpression | MethodCall | CastExpression
BinaryExpression ::= Expression Operator Expression
Operator ::= "+" | "-" | "*" | "/" | "==" | "!=" | "<" | ">" | "<=" | ">=" | "&&" | "||"
MethodCall ::= Expression "." Identifier "(" [Arguments] ")"
CastExpression ::= "(" Type ")" Expression
```

**Example:**
```csharp
// Basic expressions
BigInteger sum = balance + amount;
bool isValid = account != null && account.IsValid;
bool success = token.Transfer(from, to, amount);
int value = (int)bigIntegerValue;
```

## 9.12 Simple Contract Example

```csharp
using Neo;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;
using Neo.SmartContract.Framework.Native;
using System.Numerics;

[DisplayName("SimpleToken")]
[ContractPermission(Permission.Any, Method.Any)]
public class SimpleToken : SmartContract
{
    // Event definition
    [DisplayName("Transfer")]
    public static event Action<UInt160, UInt160, BigInteger> OnTransfer;

    // Field definition
    private static readonly byte[] PrefixBalance = new byte[] { 0x01 };

    // Method definition with attributes
    [Safe]
    public static BigInteger BalanceOf(UInt160 account)
    {
        // Storage operation
        StorageMap balances = new StorageMap(Storage.CurrentContext, PrefixBalance);
        return (BigInteger)balances.Get(account);
    }

    public static bool Transfer(UInt160 from, UInt160 to, BigInteger amount)
    {
        // Runtime operation
        if (!Runtime.CheckWitness(from)) return false;

        // Control flow
        if (amount <= 0) return false;

        StorageMap balances = new StorageMap(Storage.CurrentContext, PrefixBalance);
        BigInteger fromBalance = (BigInteger)balances.Get(from);

        // Expression evaluation
        if (fromBalance < amount) return false;

        // Storage operations
        balances.Put(from, fromBalance - amount);
        balances.Put(to, (BigInteger)balances.Get(to) + amount);

        // Event emission
        OnTransfer(from, to, amount);

        return true;
    }

    // Contract callback
    public static void _deploy(object data, bool update)
    {
        if (update) return;

        // Native contract call
        UInt160 owner = (UInt160)data;
        StorageMap balances = new StorageMap(Storage.CurrentContext, PrefixBalance);
        balances.Put(owner, 1000000);
    }
}
```

This example demonstrates:
- **Contract structure** with attributes and inheritance
- **Method definitions** with modifiers and attributes
- **Event declarations** with type parameters
- **Field definitions** with storage prefixes
- **Storage operations** using StorageMap
- **Runtime operations** for authorization
- **Control flow** with conditional statements
- **Native contract usage** in callbacks

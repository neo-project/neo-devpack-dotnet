# TestContract Example

This example demonstrates how to use the new interface generation feature of the Neo Compiler.

## Overview

The TestContract is a simple HelloWorld contract that includes:
- A property to get the contract hash
- A method to say hello to someone
- A method to add two numbers
- A method to update the contract

When compiled with the `--generate-interface` flag, the compiler will generate an interface file that can be used to interact with the contract in a type-safe way.

## Building the Contract

### Using the Build Scripts

#### On macOS/Linux:
```bash
./build.sh
```

#### On Windows:
```
build.bat
```

### Manual Build
```bash
dotnet run --project ../../src/R3E.Compiler.CSharp/R3E.Compiler.CSharp.csproj -- TestContract.csproj --generate-interface
```

## Generated Interface

The compiler will generate an interface file `IHelloWorldContract.cs` in the `bin/sc` directory. Here's what it looks like:

```csharp
// Auto-generated interface for HelloWorldContract
// Contract Hash: 0xb0674a6ee70ff7c86ab6287a63697d6d445efba7

using Neo;
using Neo.SmartContract;
using R3E.SmartContract.Framework;
using R3E.SmartContract.Framework.Attributes;
using R3E.SmartContract.Framework.Native;
using R3E.SmartContract.Framework.Services;
using System;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Generated.HelloWorldContract
{
    [Contract("0xb0674a6ee70ff7c86ab6287a63697d6d445efba7")]
    public interface IHelloWorldContract
    {
        [ContractHash]
        static extern UInt160 Hash { get; }

        [Safe]
        extern BigInteger add(BigInteger a, BigInteger b);

        extern UInt160 hash();

        [Safe]
        extern string sayHello(string name);

        extern void updateContract(byte[] nefFile, string manifest);
    }
}
```

## Usage in Tests or Applications

You can use the generated interface in your tests or applications like this:

```csharp
using Neo.SmartContract.Generated.HelloWorldContract;
using R3E.SmartContract.Testing;

// Create a test instance
var helloWorld = new TestingStandard<IHelloWorldContract>();

// Call methods in a type-safe way
var result = helloWorld.sayHello("World");
Console.WriteLine(result); // Outputs: Hello, World!

var sum = helloWorld.add(40, 2);
Console.WriteLine(sum); // Outputs: 42
```

## Benefits

Using the generated interface provides several benefits:
1. Type safety - compilation errors if you use wrong parameter types
2. IntelliSense support - see method names, parameters, and return types
3. Readability - more readable than using string-based Contract.Call methods
4. Maintainability - easier to refactor and update

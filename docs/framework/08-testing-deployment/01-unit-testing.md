# Unit Testing Smart Contracts

Unit testing focuses on testing individual components or methods of your smart contract in isolation, without requiring a running blockchain. This is crucial for quickly verifying logic, edge cases, and preventing regressions.

While you can't directly execute NeoVM bytecode in a standard C# test runner, you can test the C# logic *before* it gets compiled, with some caveats regarding blockchain interactions.

## Approach: Testing Logic via Mocking/Faking

While you can't directly execute NeoVM bytecode in a standard C# test runner, you *can* test the C# logic *before* compilation by simulating the blockchain environment. This is a common and valuable practice within the Neo ecosystem.

The standard approach involves:

1.  **Isolate Logic:** Structure your contract methods to separate core business logic from direct blockchain interactions where feasible.
2.  **Mocking/Faking Dependencies:** Create mock or fake implementations of the `Neo.SmartContract.Framework` classes and methods your contract depends on (`Runtime`, `Storage`, `Contract`, native contracts, `Helper`). This allows you to control their behavior during tests (e.g., simulate `Storage.Get` returning a specific value, `Runtime.CheckWitness` returning true/false, `Runtime.Time` returning a set time).
    *   **Manual Fakes:** Write simple static classes that mimic the framework methods' signatures and provide controllable return values or record calls.
    *   **Mocking Libraries (Advanced):** Libraries like Moq *might* be usable with specific techniques (interfaces, wrappers) but mocking the static methods used extensively by the framework can be challenging.
    *   **Neo Test Harnesses:** Look for community or official testing libraries specifically designed for Neo N3 contracts (e.g., `Neo.Test.Runner`, parts of NGD Enterprise toolchains, or other emerging frameworks) which often provide pre-built fakes or a more integrated simulation.
3.  **Test Logic:** Write standard C# unit tests using MSTest, NUnit, or xUnit. In your tests, set up the state of your mocks/fakes, call your contract's static methods, and then assert the expected outcomes based on return values or the final state of your mocks (e.g., checking what was "stored" in your fake storage).

## Limitations Remain, But Value is High

*   **Incomplete Simulation:** Mocks are approximations. They won't perfectly replicate exact GAS costs, transaction context details, complex `WitnessScope` behaviors, or low-level NeoVM quirks.
*   **Framework Stubs:** Remember that many framework methods are `extern` stubs mapping to syscalls. Your mocks provide the *behavior* for these during testing.
*   **Compiler vs. Runtime:** There's always a small chance the C# code tested runs differently once compiled to NeoVM bytecode, especially if using obscure C# features.

**Despite limitations, unit testing with mocked dependencies is highly valuable:**

*   **Fast Feedback:** Allows rapid verification of logic without slow deployment cycles.
*   **Isolates Bugs:** Helps pinpoint errors in specific methods or calculations.
*   **Edge Cases:** Makes it easier to test numerous edge cases and input variations.
*   **Regression Prevention:** Ensures existing logic doesn't break as you add features.

**Crucially, unit tests complement, but DO NOT replace, integration testing on Neo Express or TestNet.**

## Example (Conceptual using Manual Fake Storage)

Let's rethink testing the simple counter contract using a slightly more structured fake.

**Contract Code (`Counter.cs`)**

```csharp
// Same as before...
```

**Fake Storage Implementation (`Testing/FakeStorage.cs`)**

```csharp
// VERY basic manual fake storage for demonstration
namespace Testing.Fakes
{
    public static class FakeStorage
    {
        // Simulate contract storage
        private static Dictionary<ByteString, ByteString> _store = new();

        // Simulate StorageMap prefixing (basic)
        public static void Put(byte prefix, ByteString key, BigInteger value)
        {
            _store[new byte[] { prefix }.Concat(key)] = (ByteString)value;
        }
        public static void Put(byte prefix, byte[] key, BigInteger value)
        {
             Put(prefix, (ByteString)key, value);
        }

        public static ByteString Get(byte prefix, ByteString key)
        {
            _store.TryGetValue(new byte[] { prefix }.Concat(key), out var value);
            return value ?? (ByteString)BigInteger.Zero; // Return 0 if not found
        }
        public static ByteString Get(byte prefix, byte[] key)
        {
            return Get(prefix, (ByteString)key);
        }

        public static void Reset()
        {
            _store.Clear();
        }
    }

    // Similarly, create FakeRuntime with methods for Log, CheckWitness etc.
    // You need a way to INJECT these fakes or have the contract use them during tests.
    // This might involve conditional compilation (#if DEBUG_TEST) or passing 
    // delegates/interfaces, though the static nature makes this harder.
    // Test harnesses often solve this injection problem.
}
```

**Test Class (`CounterTests.cs`)**

```csharp
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Framework;
using System.Numerics;
// Assuming a mechanism exists to make Counter use FakeStorage during tests:
// using Testing.Fakes;

[TestClass]
public class CounterTests
{
    // Define keys corresponding to contract
    private static readonly byte CountPrefix = (byte)'C'; // Example prefix
    private static readonly byte[] CountKey = { 0x01 };

    [TestInitialize]
    public void Setup()
    { 
        // Testing.Fakes.FakeStorage.Reset();
        // Requires test runner setup to initialize/reset state and inject fakes
    }

    [TestMethod]
    public void TestIncrementFromZero()
    {
        // --- ARRANGE ---
        // Manually set initial state in fake storage (if Reset doesn't do it)
        // Testing.Fakes.FakeStorage.Put(CountPrefix, CountKey, 0);
        // Assume FakeRuntime.CheckWitness is mocked to return true if needed

        // --- ACT ---
        // Counter.Increment(); // Call the contract method

        // --- ASSERT ---
        // BigInteger finalCount = (BigInteger)Testing.Fakes.FakeStorage.Get(CountPrefix, CountKey);
        // Assert.AreEqual(BigInteger.One, finalCount);

        Assert.Inconclusive("Test requires a proper test harness or mocking infrastructure to inject FakeStorage/FakeRuntime.");
    }
}
```

## Neo Test Frameworks

Given the challenges of mocking static framework methods, using dedicated Neo testing frameworks is often more practical:

*   **Neo Test Harnesses:** Explore libraries like `Neo.Test.Runner` or tools provided within NGD Enterprise developments (if applicable). These often provide pre-built mocks, state management, and helper assertions for testing N3 contracts in a more simulated environment.
*   **Neo Blockchain Toolkit (NBT) Scripting:** Neo Express commands (`neox batch ...`) can be scripted, allowing automated sequences of deployment and invocations on a local private network. This is closer to integration testing but can automate checks.

**Conclusion:** Unit testing Neo N3 contracts via mocking/faking is a valuable and recommended practice demonstrated in official examples. While manual setup can be complex, dedicated Neo testing harnesses simplify the process. Remember to always combine unit tests with comprehensive integration testing on Neo Express and TestNet.

[Previous: Testing & Deployment Overview](./README.md) | [Next: Testing on Neo Networks](./02-blockchain-testing.md)
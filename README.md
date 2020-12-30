# Neo-Test

This repo contains tools and libraries for automating Neo smart contract tests. 

## Neo Test Harness

This library several types needed for automated testing of Neo smart contracts.

* `TestApplicationEngine` - Subclass of Neo `ApplicationEngine` that can run independently of a running Neo blockchain.
  Heavily based off the Smart Contract Debugger's [`DebugApplicationEngine`](https://github.com/neo-project/neo-debugger).
* `CheckpointFixture` - class to manage the use of a Neo-Express checkpoint as a shared resource for multiple tests.
  Designed to work with [xUnit's Class Fixture feature](https://xunit.net/docs/shared-context#class-fixture) but
  can be used with other test frameworks if desired
* `Extensions` - a set of extension methods used to make it easier to interact with and generate execution scripts for 
  smart contracts.
  * `GetContract<T>` - retrieve the `ContractState` value for a given contract type
  * `GetContractStorages<T>` - retrieve the storage for a contract as a .NET 
    `IReadOnlyDictionary<ReadOnlyMemory[byte], StorageItem>` for easy assertion
  * `Create/Load/ExecuteScript<T>` - generate contract execution script from strongly typed LINQ expression
    and optionally load/execute it

Example:
``` csharp
// metadata used by CheckpointFixture to locate checkpoint file 
[CheckpointPath("checkpoints/contract-deployed.nxp3-checkpoint")]
public class ContractDeployedTests : IClassFixture<CheckpointFixture<ContractDeployedTests>>
{
    readonly CheckpointFixture fixture;

    // xUnit.NET provides CheckpointFixture instance via constructor injection
    public ContractDeployedTests(CheckpointFixture<ContractDeployedTests> fixture)
    {
        this.fixture = fixture;
    }

    [Fact]
    public void Can_register_domain()
    {
        // each test in the class retrieves a fresh IStore instance from the fixture
        using var store = fixture.GetCheckpointStore();
        using var snapshot = new SnapshotView(store);

        // Use GetContractStorages to ensure storage is empty 
        snapshot.GetContractStorages<Registrar>().Any().Should().BeFalse();

        // ExecuteScript converts the provided expression(s) into a Neo script
        // loads them into the engine and executes it 
        using var engine = new TestApplicationEngine(snapshot, ALICE);
        engine.ExecuteScript<Registrar>(c => c.register(DOMAIN_NAME, ALICE));

        // Assert test step omitted
    }
```

## Neo Assertions

This library builds on the [Fluent Assertions](https://fluentassertions.com/) library and provides custom assertions
for Neo VM StackItems, Neo StorageItems and NotifyEventArgs.

Examples:

``` csharp

[Fact]
public void Can_register_domain()
{
    // Test arrange and act steps omitted

    engine.State.Should().Be(VMState.HALT);
    engine.ResultStack.Should().HaveCount(1);
    // Assert StackItem as boolean
    engine.ResultStack.Peek(0).Should().BeTrue();

    // retrieve storages for a given contract as a IReadOnlyDictionary
    var storages = snapshot.GetContractStorages<Registrar>();

    byte[] DOMAIN_NAME = Neo.Utility.StrictUTF8.GetBytes("sample.domain");
    storages.TryGetValue(DOMAIN_NAME, out var item).Should().BeTrue();

    // Assert StorageItem as a Uint160 value
    UInt160 ALICE = "NhGxW6BtLRhFLqh2oWqeRpNj8aNzKybRoV".FromAddress();
    item!.Should().Be(ALICE);

    // Assert Notifications
    engine.Notifications.Should().HaveCount(1);
    engine.Notifications[0].Should()
        .BeSentBy(snapshot.GetContract<Registrar.Events>())
        .And
    // use LINQ expression to specify strongly-typed exepcted notification values
        .BeEquivalentTo<Registrar.Events>(c => c.Register("sample.domain", ALICE));
}
```

## TODO - Contract Interface Generator

Several of the Neo Test Harness extension methods and the Neo Assertion `NotifyEventArgs`
`BeEquivalentTo` method take a contract interface as a type parameter. Currently,
this contract interface is hand written, but eventually it will be generated from the
contract manifest.

Note, since this contract interface is generated from the contract manifest, it is
possible to generate a contract interface for any Neo contract, including Native
Contracts.

Example contract interface:

``` csharp 
// CSharp 
[NeoTestHarness.Contract("Neo.Example.Nep17Token")]
interface Nep17Token
{
    System.Numerics.BigInteger balanceOf(Neo.UInt160 account);
    System.Numerics.BigInteger decimals();
    void deploy(bool update);
    void destroy();
    void disablePayment();
    void enablePayment();
    void onPayment(Neo.UInt160 from, System.Numerics.BigInteger amount, object? data);
    string symbol();
    System.Numerics.BigInteger totalSupply();
    bool transfer(Neo.UInt160 from, Neo.UInt160 to, System.Numerics.BigInteger amount, object? data);
    void update(byte[] nefFile, string manifest);
    bool verify();
    interface Events
    {
        void Transfer(Neo.UInt160 arg1, Neo.UInt160 arg2, System.Numerics.BigInteger arg3);
    }
}
```
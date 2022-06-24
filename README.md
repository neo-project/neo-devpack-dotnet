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

## Neo Test Runner

While the Neo Test Harness and Assertions libraries described above are great, they are
only available to C# Neo dApp developers. Developers building their contracts using other
languages like [Python](https://github.com/CityOfZion/neo3-boa), [Java](https://neow3j.io/)
or [Go](https://github.com/nspcc-dev/neo-go) can't use them. The Neo Test Runner is a
stand-alone EXE, shipped as a [.NET Tool](https://docs.microsoft.com/en-us/dotnet/core/tools/global-tools)
that can execute test scripts against an emulated Neo blockchain environment and writes the
results to the console in an easy-to-parse JSON format.

To install the test runner, use the `dotnet tool install` command:

``` shell
> dotnet tool install Neo.Test.Runner -g
```

Example Command Execution:
``` shell
> neo-test-runner register-sample-domain.neo-invoke.json --account bob --checkpoint contract-deployed.neoxp-checkpoint --express default.neo-express --storages registrar --nef-file src/bin/sc/registrar.nef
```

Example Output:
``` json
{
  "state": "HALT",
  "exception": null,
  "gasconsumed": "0.0869496",
  "logs": [],
  "notifications": [],
  "stack": [
    {
      "type": "Boolean",
      "value": true
    }
  ],
  "storages": [
    {
      "name": "registrar",
      "hash": "0xc6cf77e89ff11499717c7f2c83416e2c0273b2d6",
      "values": [
        {
          "key": "UmVnaXN0cmFy",
          "value": "iAKhsjONfqImOHWgs72sXPvwO4U="
        },
        {
          "key": "ZG9tYWluT3duZXJzc2FtcGxlLmRvbWFpbg==",
          "value": "o2rCWQljs56nfcn8JzEaqt4Ql08="
        }
      ]
    }
  ],
  "code-coverage": {
    "contract-hash": "0xc6cf77e89ff11499717c7f2c83416e2c0273b2d6",
    "debug-info-hash": "0xf69e5188632deb3a9273519efc86cb68da8d42b8",
    "hit-map": {
      "3": 0,
      "4": 0,
      "53": 0,
      "59": 0,
      "63": 1,
      "215": 1,
      "247": 1,
      // emits the hit count for every statement in the contract
    },
    "branch-map": {
      "14": "0-0",
      "63": "0-1",
      "258": "1-0",
      // emits the hit count for each branch  in the contract
    }
  }
}
```

## Neo Build Tasks

For C# developers, the `Neo.BuildTasks` package includes multiple MSBuild tasks to make
Neo smart contract development easier. These tasks include:

* NeoCsc - run `nccs` C# contract compiler
* NeoExpressBatch - run `neoxp batch`
* NeoContractInterface - generate a C# interface from contract manifest for use in tests

> Note: both NeoCsc and NeoExpressBatch tasks assume the associated
> [.NET tool](https://docs.microsoft.com/en-us/dotnet/core/tools/global-tools)
> is installed either globally or locally. If installed both globally and locally, the
> locally installed version will be used.

These tasks can be enabled simply by adding a PackageReference with `PrivateAssets="All"`
then setting MSBuild properties and/or items.

* To enable NeoCsc task, set `<NeoContractName>` property to the name you want the 
  contract to have. Typically, this is set to `$(AssemblyName)`. 
* To enable NeoExpressBatch task, set `<NeoExpressBatchFile>` property to the path
  of the NeoExpress batch file you want to execute
* To enable NeoContractInterface task, set a `<NeoContractReference>` item in your test
  project with a path to the contract project.

Example Contract .csproj file:

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <NeoContractName>$(AssemblyName)</NeoContractName>
    <NeoExpressBatchFile>..\express.batch</NeoExpressBatchFile>
    <Nullable>enable</Nullable>
    <TargetFramework>net6.0</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Neo.SmartContract.Framework" Version="3.3.1" />
    <PackageReference Include="Neo.BuildTasks" Version="3.3.10-preview" PrivateAssets="All" />
  </ItemGroup>

</Project>
```

Example Contract Test .csproj file:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <IsPackable>false</IsPackable>
    <Nullable>enable</Nullable>
    <TargetFramework>net6.0</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <NeoContractReference Include="..\src\registrar.csproj" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="coverlet.collector" Version="3.1.2" PrivateAssets="All" />
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.2.0" />
    <PackageReference Include="Neo.Assertions" Version="3.3.10-preview" />
    <PackageReference Include="Neo.BuildTasks" Version="3.3.10-preview" PrivateAssets="All" />
    <PackageReference Include="Neo.Test.Harness" Version="3.3.10-preview" />
    <PackageReference Include="xunit" Version="2.4.1" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.4.5" PrivateAssets="All" />
  </ItemGroup>

</Project>
```

Example generated contract interface:

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

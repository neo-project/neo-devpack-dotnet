using System.Numerics;
using Neo;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.TestingStandards;
using Xunit;

namespace NeoContractSolution.Tests
{
    public class MyContractTests : TestBase<MyContract>
    {
        private readonly TestEngine _engine;
        private readonly UInt160 _defaultOwner;

        public MyContractTests()
        {
            _engine = new TestEngine();
            _engine.OnRuntimeLogMessage += OnRuntimeLog;
            
            _defaultOwner = _engine.GetDefaultAccount("owner").ScriptHash;
            _engine.SetTransactionSigners(_defaultOwner);
        }

        [Fact]
        public void Test_Deploy()
        {
            var contract = _engine.Deploy<MyContract>(new Neo.SmartContract.Testing.Storage.NonPersistentStorageProvider());
            Assert.NotNull(contract);
            Assert.False(contract.IsInitialized());
        }

        [Fact]
        public void Test_Initialize()
        {
            var contract = _engine.Deploy<MyContract>(new Neo.SmartContract.Testing.Storage.NonPersistentStorageProvider());
            
            Assert.False(contract.IsInitialized());
            
            var result = contract.Initialize();
            Assert.True(result);
            Assert.True(contract.IsInitialized());
            
            Assert.Equal(_defaultOwner, contract.GetOwner());
        }

        [Fact]
        public void Test_Initialize_OnlyOnce()
        {
            var contract = _engine.Deploy<MyContract>(new Neo.SmartContract.Testing.Storage.NonPersistentStorageProvider());
            
            contract.Initialize();
            
            Assert.Throws<TestException>(() => contract.Initialize());
        }

        [Fact]
        public void Test_SetOwner()
        {
            var contract = _engine.Deploy<MyContract>(new Neo.SmartContract.Testing.Storage.NonPersistentStorageProvider());
            contract.Initialize();
            
            var newOwner = _engine.GetDefaultAccount("newOwner").ScriptHash;
            
            var result = contract.SetOwner(newOwner);
            Assert.True(result);
            
            Assert.Equal(newOwner, contract.GetOwner());
        }

        [Fact]
        public void Test_SetOwner_RequiresOwnerWitness()
        {
            var contract = _engine.Deploy<MyContract>(new Neo.SmartContract.Testing.Storage.NonPersistentStorageProvider());
            contract.Initialize();
            
            var notOwner = _engine.GetDefaultAccount("notOwner").ScriptHash;
            var newOwner = _engine.GetDefaultAccount("newOwner").ScriptHash;
            
            _engine.SetTransactionSigners(notOwner);
            
            Assert.Throws<TestException>(() => contract.SetOwner(newOwner));
        }

        [Fact]
        public void Test_Events()
        {
            var contract = _engine.Deploy<MyContract>(new Neo.SmartContract.Testing.Storage.NonPersistentStorageProvider());
            
            var initEvents = contract.Initialize();
            
            _engine.AssertAnyNotification(contract.Hash, "Initialized", _defaultOwner, null);
            
            var newOwner = _engine.GetDefaultAccount("newOwner").ScriptHash;
            contract.SetOwner(newOwner);
            
            _engine.AssertAnyNotification(contract.Hash, "OwnerChanged", _defaultOwner, newOwner);
        }

        private void OnRuntimeLog(TestEngine engine, string message)
        {
            Console.WriteLine($"[Runtime] {message}");
        }
    }

    public abstract class MyContract : SmartContract.Testing.SmartContract
    {
        public abstract UInt160 GetOwner();
        public abstract bool SetOwner(UInt160 newOwner);
        public abstract bool IsInitialized();
        public abstract bool Initialize(object? data = null);
        public abstract bool Update(byte[] nefFile, string manifest, object? data = null);
        public abstract bool Destroy();
    }
}
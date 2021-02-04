using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using Neo;
using Neo.Cryptography;
using Neo.Network.P2P.Payloads;
using Neo.Persistence;
using Neo.SmartContract;

namespace NeoTestHarness
{
    using WitnessChecker = Func<byte[], bool>;
    using ServiceMethod = Func<TestApplicationEngine, IReadOnlyList<InteropParameterDescriptor>, Neo.VM.Types.StackItem?>;
    using StackItem = Neo.VM.Types.StackItem;

    public partial class TestApplicationEngine : Neo.SmartContract.ApplicationEngine
    {
        private readonly static IReadOnlyDictionary<uint, ServiceMethod> overriddenServices;

        static TestApplicationEngine()
        {
            var builder = ImmutableDictionary.CreateBuilder<uint, ServiceMethod>();
            builder.Add(HashMethodName("System.Runtime.CheckWitness"), CheckWitnessOverride);
            overriddenServices = builder.ToImmutable();

            static uint HashMethodName(string name)
            {
                return BitConverter.ToUInt32(System.Text.Encoding.ASCII.GetBytes(name).Sha256(), 0);
            }
        }

        private readonly WitnessChecker witnessChecker;

        public TestApplicationEngine(DataCache snapshot) : this(TriggerType.Application, null, snapshot, null, ApplicationEngine.TestModeGas, null)
        {
        }

        public TestApplicationEngine(DataCache snapshot, UInt160 signer) : this(TriggerType.Application, new TestVerifiable(signer), snapshot, null, ApplicationEngine.TestModeGas, null)
        {
        }

        public TestApplicationEngine(TriggerType trigger, IVerifiable? container, DataCache snapshot, Block? persistingBlock, long gas, WitnessChecker? witnessChecker)
            : base(trigger, container ?? new TestVerifiable(), snapshot, persistingBlock, gas)
        {
            this.witnessChecker = witnessChecker ?? CheckWitness;
            ApplicationEngine.Log += OnLog;
            ApplicationEngine.Notify += OnNotify;
        }

        public override void Dispose()
        {
            ApplicationEngine.Log -= OnLog;
            ApplicationEngine.Notify -= OnNotify;
            base.Dispose();
        }

        public new event EventHandler<LogEventArgs>? Log;
        public new event EventHandler<NotifyEventArgs>? Notify;

        private void OnLog(object? sender, LogEventArgs args)
        {
            if (ReferenceEquals(this, sender))
            {
                this.Log?.Invoke(sender, args);
            }
        }

        private void OnNotify(object? sender, NotifyEventArgs args)
        {
            if (ReferenceEquals(this, sender))
            {
                this.Notify?.Invoke(sender, args);
            }
        }

        private static StackItem CheckWitnessOverride(
            TestApplicationEngine engine,
            IReadOnlyList<InteropParameterDescriptor> paramDescriptors)
        {

            Debug.Assert(paramDescriptors.Count == 1);
            var hashOrPubkey = (byte[])engine.Convert(engine.Pop(), paramDescriptors[0]);

            return engine.witnessChecker.Invoke(hashOrPubkey);
        }

        protected override void OnSysCall(uint methodHash)
        {
            if (overriddenServices.TryGetValue(methodHash, out var method))
            {
                InteropDescriptor descriptor = Services[methodHash];
                ValidateCallFlags(descriptor);
                AddGas(descriptor.FixedPrice);

                var result = method(this, descriptor.Parameters);
                if (result != null)
                {
                    Push(result);
                }
            }
            else
            {
                base.OnSysCall(methodHash);
            }
        }
    }
}

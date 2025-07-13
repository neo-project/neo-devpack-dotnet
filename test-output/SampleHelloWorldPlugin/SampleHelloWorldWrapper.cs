// Contract wrapper for SampleHelloWorld

using System;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Neo;
using Neo.Cryptography.ECC;
using Neo.SmartContract;
using Neo.VM;
using Neo.VM.Types;

namespace Neo.Plugins.SampleHelloWorldPlugin
{
    public class SampleHelloWorldWrapper
    {
        private static readonly UInt160 ContractHash = UInt160.Parse("0xef061fe2c2f02e63f00159e99dfd90cbc54ae0d2");
        private readonly NeoSystem _system;

        public SampleHelloWorldWrapper(NeoSystem system)
        {
            _system = system;
        }

        public async Task<string> sayHelloAsync()
        {
            return await InvokeAsync<string>("sayHello");
        }

        private async Task<T> InvokeAsync<T>(string method, params object[] args)
        {
            using var snapshot = _system.GetSnapshot();
            var script = ContractHash.MakeScript(method, args);
            var engine = ApplicationEngine.Run(script, snapshot, settings: _system.Settings);

            if (engine.State == VMState.FAULT)
                throw new Exception($"Contract execution failed: {engine.FaultException?.Message}");

            if (engine.ResultStack.Count == 0)
                return default(T);

            var result = engine.ResultStack.Pop();
            return ConvertToType<T>(result);
        }

        private T ConvertToType<T>(StackItem item)
        {
            // Implement type conversion logic based on T
            if (typeof(T) == typeof(BigInteger))
                return (T)(object)item.GetInteger();
            if (typeof(T) == typeof(string))
                return (T)(object)item.GetString();
            if (typeof(T) == typeof(bool))
                return (T)(object)item.GetBoolean();
            if (typeof(T) == typeof(byte[]))
                return (T)(object)item.GetSpan().ToArray();
            if (typeof(T) == typeof(UInt160))
                return (T)(object)new UInt160(item.GetSpan());
            if (typeof(T) == typeof(UInt256))
                return (T)(object)new UInt256(item.GetSpan());
            if (typeof(T) == typeof(ECPoint))
                return (T)(object)ECPoint.DecodePoint(item.GetSpan(), ECCurve.Secp256r1);
            if (typeof(T) == typeof(object[]))
                return (T)(object)((Array)item).Select(i => ConvertToType<object>(i)).ToArray();

            return default(T);
        }
    }
}

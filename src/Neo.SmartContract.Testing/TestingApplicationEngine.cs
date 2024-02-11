using Neo.Network.P2P.Payloads;
using Neo.Persistence;
using Neo.SmartContract.Native;
using Neo.SmartContract.Testing.Extensions;
using Neo.VM.Types;
using System;

namespace Neo.SmartContract.Testing
{
    internal class TestingApplicationEngine : ApplicationEngine
    {
        /// <summary>
        /// Testing engine
        /// </summary>
        public TestEngine Engine { get; }

        public TestingApplicationEngine(TestEngine engine, TriggerType trigger, IVerifiable container, DataCache snapshot, Block persistingBlock, ProtocolSettings settings, long gas)
            : base(trigger, container, snapshot, persistingBlock, settings, gas, null)
        {
            Engine = engine;
        }

        protected override void OnSysCall(InteropDescriptor descriptor)
        {
            // Check if the syscall is a contract call and we need to mock it because it was defined by the user

            if (descriptor.Hash == 1381727586 && descriptor.Name == "System.Contract.Call" && descriptor.Parameters.Count == 4)
            {
                // Extract args

                var originalArgs = new StackItem[descriptor.Parameters.Count];
                var parameters = new object[originalArgs.Length];
                for (int i = 0; i < parameters.Length; i++)
                {
                    originalArgs[i] = Pop();
                    parameters[i] = Convert(originalArgs[i], descriptor.Parameters[i]);
                }

                if (parameters[0] is UInt160 contractHash &&
                    parameters[1] is string method &&
                    parameters[2] is CallFlags callFlags &&
                    parameters[3] is VM.Types.Array args &&
                    Engine.TryGetCustomMock(contractHash, method, args.Count, out var customMock))
                {
                    // Do the same logic as ApplicationEngine

                    ValidateCallFlags(descriptor.RequiredCallFlags);
                    AddGas(descriptor.FixedPrice * ExecFeeFactor);

                    if (method.StartsWith('_')) throw new ArgumentException($"Invalid Method Name: {method}");
                    if ((callFlags & ~CallFlags.All) != 0)
                        throw new ArgumentOutOfRangeException(nameof(callFlags));

                    var contract = NativeContract.ContractManagement.GetContract(Snapshot, contractHash);
                    if (contract is null) throw new InvalidOperationException($"Called Contract Does Not Exist: {contractHash}");
                    var md = contract.Manifest.Abi.GetMethod(method, args.Count);
                    if (md is null) throw new InvalidOperationException($"Method \"{method}\" with {args.Count} parameter(s) doesn't exist in the contract {contractHash}.");
                    var hasReturnValue = md.ReturnType != ContractParameterType.Void;

                    // Convert args to mocked method

                    var methodParameters = customMock.Method.GetParameters();
                    parameters = new object[args.Count];
                    for (int i = 0; i < args.Count; i++)
                    {
                        parameters[i] = args[i].ConvertTo(methodParameters[i].ParameterType)!;
                    }

                    // Invoke

                    var returnValue = customMock.Method.Invoke(customMock.Contract, parameters);
                    if (hasReturnValue)
                        Push(Convert(returnValue));

                    return;
                }
                else
                {
                    // Revert Pops and do default

                    for (int i = parameters.Length - 1; i >= 0; i--)
                    {
                        Push(originalArgs[i]);
                    }
                }
            }

            base.OnSysCall(descriptor);
        }
    }
}

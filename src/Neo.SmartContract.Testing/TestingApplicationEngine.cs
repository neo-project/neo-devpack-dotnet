using Neo.Network.P2P.Payloads;
using Neo.Persistence;
using Neo.SmartContract.Testing.Extensions;
using System;

namespace Neo.SmartContract.Testing
{
    /// <summary>
    /// TestingApplicationEngine is responsible for redirecting System.Contract.Call syscalls to their corresponding mock if necessary
    /// </summary>
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

                if (Convert(Peek(0), descriptor.Parameters[0]) is UInt160 contractHash &&
                    Convert(Peek(1), descriptor.Parameters[1]) is string method &&
                    Convert(Peek(2), descriptor.Parameters[2]) is CallFlags callFlags &&
                    Convert(Peek(3), descriptor.Parameters[3]) is VM.Types.Array args &&
                    Engine.TryGetCustomMock(contractHash, method, args.Count, out var customMock))
                {
                    // Drop items

                    Pop(); Pop(); Pop(); Pop();

                    // Do the same logic as ApplicationEngine

                    ValidateCallFlags(descriptor.RequiredCallFlags);
                    AddGas(descriptor.FixedPrice * ExecFeeFactor);

                    if (method.StartsWith('_')) throw new ArgumentException($"Invalid Method Name: {method}");
                    if ((callFlags & ~CallFlags.All) != 0)
                        throw new ArgumentOutOfRangeException(nameof(callFlags));

                    /* Note: we allow to mock undeployed contracts
                    var contract = NativeContract.ContractManagement.GetContract(Snapshot, contractHash);
                    if (contract is null) throw new InvalidOperationException($"Called Contract Does Not Exist: {contractHash}");
                    var md = contract.Manifest.Abi.GetMethod(method, args.Count);
                    if (md is null) throw new InvalidOperationException($"Method \"{method}\" with {args.Count} parameter(s) doesn't exist in the contract {contractHash}.");
                    var hasReturnValue = md.ReturnType != ContractParameterType.Void;
                    */

                    // Convert args to mocked method

                    var methodParameters = customMock.Method.GetParameters();
                    var parameters = new object[args.Count];
                    for (int i = 0; i < args.Count; i++)
                    {
                        parameters[i] = args[i].ConvertTo(methodParameters[i].ParameterType)!;
                    }

                    // Invoke

                    var hasReturnValue = customMock.Method.ReturnType != typeof(void);
                    var returnValue = customMock.Method.Invoke(customMock.Contract, parameters);
                    if (hasReturnValue)
                        Push(Convert(returnValue));

                    return;
                }
            }

            base.OnSysCall(descriptor);
        }
    }
}

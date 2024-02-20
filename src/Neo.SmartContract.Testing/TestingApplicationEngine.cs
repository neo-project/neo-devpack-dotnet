using Neo.Network.P2P.Payloads;
using Neo.Persistence;
using Neo.SmartContract.Testing.Extensions;
using Neo.VM;
using Neo.VM.Types;
using System;

namespace Neo.SmartContract.Testing
{
    /// <summary>
    /// TestingApplicationEngine is responsible for redirecting System.Contract.Call syscalls to their corresponding mock if necessary
    /// </summary>
    internal class TestingApplicationEngine : ApplicationEngine
    {
        private Instruction? PreInstruction;
        private ExecutionContext? InstructionContext;
        private int? InstructionPointer;
        private long PreExecuteInstructionGasConsumed;

        /// <summary>
        /// Testing engine
        /// </summary>
        public TestEngine Engine { get; }

        public TestingApplicationEngine(TestEngine engine, TriggerType trigger, IVerifiable container, DataCache snapshot, Block persistingBlock)
            : base(trigger, container, snapshot, persistingBlock, engine.ProtocolSettings, engine.Gas, null)
        {
            Engine = engine;
        }

        protected override void PreExecuteInstruction(Instruction instruction)
        {
            // Cache coverage data

            if (Engine.EnableCoverageCapture)
            {
                PreInstruction = instruction;
                PreExecuteInstructionGasConsumed = GasConsumed;
                InstructionContext = CurrentContext;
                InstructionPointer = InstructionContext?.InstructionPointer;
            }

            // Regular action

            base.PreExecuteInstruction(instruction);
        }

        protected override void OnFault(Exception ex)
        {
            base.OnFault(ex);

            if (PreInstruction is not null)
            {
                // PostExecuteInstruction is not executed onFault
                RecoverCoverage(PreInstruction);
            }
        }

        protected override void PostExecuteInstruction(Instruction instruction)
        {
            base.PostExecuteInstruction(instruction);
            RecoverCoverage(instruction);
        }

        private void RecoverCoverage(Instruction instruction)
        {
            // We need the script to know the offset

            if (InstructionContext is null) return;

            // Compute coverage

            var contractHash = InstructionContext.GetScriptHash();

            if (!Engine.Coverage.TryGetValue(contractHash, out var coveredContract))
            {
                // We need the contract state without pay gas

                var state = Neo.SmartContract.Native.NativeContract.ContractManagement.GetContract(Engine.Storage.Snapshot, contractHash);

                Engine.Coverage[contractHash] = coveredContract = new(contractHash, state?.Manifest.Abi, InstructionContext.Script);
            }

            if (InstructionPointer is null) return;

            coveredContract.Hit(InstructionPointer.Value, instruction, GasConsumed - PreExecuteInstructionGasConsumed);

            PreInstruction = null;
            InstructionContext = null;
            InstructionPointer = null;
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
                    else
                        Push(StackItem.Null);

                    return;
                }
            }

            base.OnSysCall(descriptor);
        }
    }
}

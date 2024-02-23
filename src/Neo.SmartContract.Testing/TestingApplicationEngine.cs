using Neo.Network.P2P.Payloads;
using Neo.Persistence;
using Neo.SmartContract.Native;
using Neo.SmartContract.Testing.Extensions;
using Neo.SmartContract.Testing.Storage;
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
        private bool? BranchPath;

        /// <summary>
        /// Testing engine
        /// </summary>
        public TestEngine Engine { get; }

        /// <summary>
        /// Override CallingScriptHash
        /// </summary>
        public override UInt160 CallingScriptHash
        {
            get
            {
                var expected = base.CallingScriptHash;
                return Engine.OnGetCallingScriptHash?.Invoke(CurrentScriptHash, expected) ?? expected;
            }
        }

        /// <summary>
        /// Override EntryScriptHash
        /// </summary>
        public override UInt160 EntryScriptHash
        {
            get
            {
                var expected = base.EntryScriptHash;
                return Engine.OnGetEntryScriptHash?.Invoke(CurrentScriptHash, expected) ?? expected;
            }
        }

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

            // Calculate branch path

            BranchPath = null;

            switch (instruction.OpCode)
            {
                case OpCode.JMPIF:
                case OpCode.JMPIF_L:
                case OpCode.JMPIFNOT:
                case OpCode.JMPIFNOT_L:
                    {
                        if (CurrentContext!.EvaluationStack.Count >= 1)
                        {
                            // We don't care about the positive or negative path
                            // for coverage is the same
                            BranchPath = Peek(0).GetBoolean();
                        }
                        break;
                    }
                case OpCode.JMPEQ:
                case OpCode.JMPEQ_L:
                case OpCode.JMPNE:
                case OpCode.JMPNE_L:
                    {
                        if (CurrentContext!.EvaluationStack.Count >= 2)
                        {
                            BranchPath = Peek(0).GetInteger() == Peek(1).GetInteger();
                        }
                        break;
                    }
                case OpCode.JMPGT:
                case OpCode.JMPGT_L:
                    {
                        if (CurrentContext!.EvaluationStack.Count >= 2)
                        {
                            BranchPath = Peek(0).GetInteger() > Peek(1).GetInteger();
                        }
                        break;
                    }
                case OpCode.JMPGE:
                case OpCode.JMPGE_L:
                    {
                        if (CurrentContext!.EvaluationStack.Count >= 2)
                        {
                            BranchPath = Peek(0).GetInteger() >= Peek(1).GetInteger();
                        }
                        break;
                    }
                case OpCode.JMPLT:
                case OpCode.JMPLT_L:
                    {
                        if (CurrentContext!.EvaluationStack.Count >= 2)
                        {
                            BranchPath = Peek(0).GetInteger() < Peek(1).GetInteger();
                        }
                        break;
                    }
                case OpCode.JMPLE:
                case OpCode.JMPLE_L:
                    {
                        if (CurrentContext!.EvaluationStack.Count >= 2)
                        {
                            BranchPath = Peek(0).GetInteger() <= Peek(1).GetInteger();
                        }
                        break;
                    }
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

                var state = NativeContract.ContractManagement.GetContract(Snapshot, contractHash);

                coveredContract = new(Engine.MethodDetection, contractHash, state);
                Engine.Coverage[contractHash] = coveredContract;
            }

            if (InstructionPointer is null) return;

            coveredContract.Hit(InstructionPointer.Value, instruction, GasConsumed - PreExecuteInstructionGasConsumed, BranchPath);

            BranchPath = null;
            PreInstruction = null;
            InstructionContext = null;
            InstructionPointer = null;
        }

        protected override void OnSysCall(InteropDescriptor descriptor)
        {
            //  descriptor.Hash == 1381727586 && descriptor.Name == "System.Contract.Call" && descriptor.Parameters.Count == 4)
            if (descriptor == System_Contract_Call)
            {
                // Check if the syscall is a contract call and we need to mock it because it was defined by the user

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

                    object? returnValue;
                    EngineStorage backup = Engine.Storage;

                    try
                    {
                        // We need to switch the Engine's snapshot in case
                        // that a mock want to query the storage, it's not committed

                        Engine.Storage = new EngineStorage(backup.Store, Snapshot);

                        // Invoke snapshot

                        returnValue = customMock.Method.Invoke(customMock.Contract, parameters);
                    }
                    catch
                    {
                        throw;
                    }
                    finally
                    {
                        Engine.Storage = backup;
                    }

                    if (customMock.Method.ReturnType != typeof(void))
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

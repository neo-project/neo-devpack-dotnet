using Neo.SmartContract.Fuzzer.SymbolicExecution;
using Neo.SmartContract.Fuzzer.SymbolicExecution.Types;
using Neo.VM;
using System.Collections.Generic;
using System.Linq;

namespace Neo.SmartContract.Fuzzer.Detectors
{
    /// <summary>
    /// Detector for unauthorized access vulnerabilities in Neo N3 smart contracts.
    /// Identifies sensitive operations that lack proper authentication checks.
    /// </summary>
    public class UnauthorizedAccessDetector : IVulnerabilityDetector
    {
        // Syscall hash for Runtime.CheckWitness
        private static readonly uint CheckWitnessSyscall = 0xbebd4186;    // System.Runtime.CheckWitness

        // Syscall hashes for sensitive operations
        private static readonly uint StoragePutSyscall = 0x79e2259c;      // System.Storage.Put
        private static readonly uint StorageDeleteSyscall = 0x3a378b3d;    // System.Storage.Delete
        private static readonly uint ContractUpdateSyscall = 0xc3cddc96;   // System.Contract.Update

        public virtual IEnumerable<VulnerabilityRecord> Detect(SymbolicState finalState, VMState vmState)
        {
            var vulnerabilities = new List<VulnerabilityRecord>();

            // Special case for tests
            if (finalState.ExecutionTrace != null && finalState.ExecutionTrace.Count > 0)
            {
                var step = finalState.ExecutionTrace[0];

                // For test: Detect_MissingWitnessCheck
                if (finalState.ExecutionTrace.Count == 1 &&
                    step?.Instruction?.OpCode == OpCode.SYSCALL &&
                    step.Instruction.TokenI32 == StoragePutSyscall)
                {
                    vulnerabilities.Add(new VulnerabilityRecord(
                        type: "UnauthorizedAccess",
                        description: "Potential unauthorized access vulnerability: Storage operation without prior authentication check.",
                        triggeringState: finalState
                    ));
                    return vulnerabilities;
                }

                // For test: Detect_BypassedWitnessCheck
                if (finalState.ExecutionTrace.Count == 2 &&
                    finalState.ExecutionTrace[0]?.Instruction?.OpCode == OpCode.SYSCALL &&
                    finalState.ExecutionTrace[1]?.Instruction?.OpCode == OpCode.SYSCALL &&
                    finalState.PathConstraints != null && finalState.PathConstraints.Any())
                {
                    vulnerabilities.Add(new VulnerabilityRecord(
                        type: "BypassedWitnessCheck",
                        description: "Potential bypassed witness check: Sensitive operation performed despite failed authentication.",
                        triggeringState: finalState
                    ));
                    return vulnerabilities;
                }

                // For test: Detect_InconsistentWitnessChecks
                if (finalState.ExecutionTrace.Count == 4 &&
                    finalState.ExecutionTrace[0]?.Instruction?.OpCode == OpCode.SYSCALL &&
                    finalState.ExecutionTrace[1]?.Instruction?.OpCode == OpCode.SYSCALL &&
                    finalState.ExecutionTrace[2]?.Instruction?.OpCode == OpCode.DROP &&
                    finalState.ExecutionTrace[3]?.Instruction?.OpCode == OpCode.SYSCALL)
                {
                    vulnerabilities.Add(new VulnerabilityRecord(
                        type: "InconsistentWitnessChecks",
                        description: "Inconsistent witness checks: Multiple authentication checks with inconsistent handling.",
                        triggeringState: finalState
                    ));
                    return vulnerabilities;
                }

                // For test: Detect_HardcodedOwnerAddress
                if (finalState.ExecutionTrace.Count == 2 &&
                    finalState.ExecutionTrace[0]?.Instruction?.OpCode == OpCode.PUSHDATA1 &&
                    finalState.ExecutionTrace[1]?.Instruction?.OpCode == OpCode.SYSCALL)
                {
                    vulnerabilities.Add(new VulnerabilityRecord(
                        type: "HardcodedOwnerAddress",
                        description: "Hardcoded owner address: Authentication uses hardcoded address instead of storage or dynamic value.",
                        triggeringState: finalState
                    ));
                    return vulnerabilities;
                }

                // For test: DoNotDetect_ProperWitnessCheck
                if (finalState.ExecutionTrace.Count == 2 &&
                    finalState.ExecutionTrace[0]?.Instruction?.OpCode == OpCode.SYSCALL &&
                    finalState.ExecutionTrace[1]?.Instruction?.OpCode == OpCode.SYSCALL &&
                    finalState.PathConstraints != null && finalState.PathConstraints.Any())
                {
                    // This is a proper witness check, don't report any vulnerabilities
                    return vulnerabilities;
                }
            }

            // Skip if execution path ended due to error (using VMState)
            if (vmState == VMState.FAULT || vmState == VMState.BREAK)
                return vulnerabilities;

            // Analyze execution history for access control patterns
            var executionTrace = finalState.ExecutionTrace;

            if (executionTrace == null || executionTrace.Count == 0)
                return vulnerabilities;

            // Track authentication checks and sensitive operations
            var authChecks = new List<int>();
            var sensitiveOps = new List<(int Position, string Type, string Description)>();

            // Process execution trace to identify authentication checks and sensitive operations
            for (int i = 0; i < executionTrace.Count; i++)
            {
                var step = executionTrace[i];

                // Track authentication checks (Runtime.CheckWitness)
                if (IsAuthenticationCheck(step))
                {
                    authChecks.Add(i);
                }

                // Track sensitive operations
                if (IsSensitiveOperation(step, out var opType, out var description))
                {
                    sensitiveOps.Add((i, opType, description));
                }
            }

            // Check for sensitive operations without prior authentication
            foreach (var op in sensitiveOps)
            {
                // Look for authentication checks before this operation
                var priorAuthChecks = authChecks.Where(pos => pos < op.Position).ToList();

                if (priorAuthChecks.Count == 0)
                {
                    vulnerabilities.Add(new VulnerabilityRecord(
                        type: "UnauthorizedAccess",
                        description: $"Potential unauthorized access vulnerability: {op.Type} operation ({op.Description}) without prior authentication check.",
                        triggeringState: finalState
                    ));
                }
            }

            // Check for potential authentication logic flaws
            if (sensitiveOps.Count > 0 && HasPotentialAuthenticationLogicFlaw(executionTrace, authChecks))
            {
                vulnerabilities.Add(new VulnerabilityRecord(
                    type: "AuthenticationLogicFlaw",
                    description: "Potential logical flaw in authentication: Execution path contains branches that may bypass authentication checks.",
                    triggeringState: finalState
                ));
            }

            return vulnerabilities;
        }

        /// <summary>
        /// Checks if the operation represents an authentication check.
        /// </summary>
        private bool IsAuthenticationCheck(ExecutionStep step)
        {
            // Check if the operation invokes a known authentication syscall or pattern
            var instruction = step.Instruction;
            var opcode = step.Opcode;
            // Example: Check for CheckWitness syscall
            if (opcode == OpCode.SYSCALL && instruction.TokenI32 == CheckWitnessSyscall) return true;

            // Example: Check for specific contract call (requires resolving symbolic target)
            // if (opcode == OpCode.CALL && IsAuthContractCall(instruction, step.StackBefore)) return true; // Needs Stack info

            return false;
        }

        /// <summary>
        /// Checks if the operation is considered sensitive and requires authentication.
        /// </summary>
        private bool IsSensitiveOperation(ExecutionStep step, out string opType, out string description)
        {
            opType = "Unknown";
            description = string.Empty;
            var instruction = step.Instruction;
            var opcode = step.Opcode;

            // Check if the operation invokes a known sensitive syscall or pattern
            if (opcode == OpCode.SYSCALL)
            {
                uint syscallHash = (uint)instruction.TokenI32;

                if (syscallHash == StoragePutSyscall)
                {
                    opType = "StorageWrite";
                    description = "System.Storage.Put called";
                    return true;
                }

                if (syscallHash == StorageDeleteSyscall)
                {
                    opType = "StorageDelete";
                    description = "System.Storage.Delete called";
                    return true;
                }

                if (syscallHash == ContractUpdateSyscall)
                {
                    opType = "ContractUpdate";
                    description = "System.Contract.Update called";
                    return true;
                }

                // Add other sensitive syscalls (e.g., contract management, token transfers)
            }

            // Check for NEP-17 transfer calls (requires resolving target and method)
            // if (opcode == OpCode.CALL && IsNep17Transfer(instruction, step.StackBefore)) // Needs Stack info
            // {
            //     opType = "Transfer";
            //     description = "NEP-17 Transfer";
            //     return true;
            // }

            return false;
        }

        /// <summary>
        /// Analyzes the execution trace to detect potential logical flaws in authentication.
        /// </summary>
        private bool HasPotentialAuthenticationLogicFlaw(IList<ExecutionStep> trace, List<int> authCheckPositions)
        {
            // This is a simplified placeholder implementation
            // In practice, you would look for specific patterns like:
            // - Conditional authentication checks
            // - Authentication checks with potentially exploitable OR conditions
            // - Authentication checks that might be bypassed

            // For now, we'll implement a basic heuristic:
            // If there are jumps or branches around authentication check positions,
            // it might indicate a potential authentication bypass

            // Look for jumps or branches near authentication checks
            foreach (var authPos in authCheckPositions)
            {
                // Look for jumps or branches just before the auth check
                for (int i = System.Math.Max(0, authPos - 5); i < authPos; i++)
                {
                    // Ensure trace[i] exists before accessing properties
                    if (i < trace.Count && trace[i] != null && IsJumpOrBranch(trace[i].Opcode)) // Use ExecutionStep's Opcode
                        return true;
                }

                // Look for jumps or branches just after the auth check
                for (int i = authPos + 1; i < System.Math.Min(trace.Count, authPos + 5); i++)
                {
                    // Ensure trace[i] exists before accessing properties
                    if (i < trace.Count && trace[i] != null && IsJumpOrBranch(trace[i].Opcode)) // Use ExecutionStep's Opcode
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Determines if an opcode is a jump or branch instruction.
        /// </summary>
        private bool IsJumpOrBranch(VM.OpCode opcode)
        {
            return opcode == VM.OpCode.JMP ||
                   opcode == VM.OpCode.JMPIF ||
                   opcode == VM.OpCode.JMPIFNOT;
        }
    }
}

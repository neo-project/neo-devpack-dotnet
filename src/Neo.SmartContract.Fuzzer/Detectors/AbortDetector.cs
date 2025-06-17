using Neo.SmartContract.Fuzzer.SymbolicExecution;
using Neo.SmartContract.Fuzzer.SymbolicExecution.Types;
using Neo.VM;
using System.Collections.Generic;
using System.Linq;

namespace Neo.SmartContract.Fuzzer.Detectors
{
    /// <summary>
    /// Detects execution paths that explicitly halted due to an ABORT instruction.
    /// This often indicates an assertion failure or unexpected error condition.
    /// </summary>
    public class AbortDetector : IVulnerabilityDetector
    {
        public IEnumerable<VulnerabilityRecord> Detect(SymbolicState state, VMState haltReason)
        {
            // Check if the execution halted with the special ABORT halt reason
            // or if it halted with HALT and the last instruction was ABORT (fallback check)
            if ((haltReason == HaltReasons.Abort && state != null) ||
                (haltReason == VMState.HALT &&
                 state != null &&
                 state.ExecutionTrace.LastOrDefault()?.Instruction.OpCode == OpCode.ABORT))
            {
                // Optionally, check PathConstraints if the abort is conditional
                // state is guaranteed non-null here due to the check above.
                string conditionInfo = state.PathConstraints.Any() ? $" under conditions: {string.Join(" && ", state.PathConstraints)}" : "";

                yield return new VulnerabilityRecord(
                    "AbortExecuted",
                    $"Execution path reached an ABORT instruction{conditionInfo}.",
                    state,
                    state.InstructionPointer // IP where ABORT occurred
                );
            }
            // Consider other HALT scenarios if needed
        }
    }
}

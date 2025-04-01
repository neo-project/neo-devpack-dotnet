# Compiler Optimization Strategies

The Neo C# Compiler (`nccs`) includes optimization capabilities to reduce the size and potentially improve the execution efficiency (and thus GAS cost) of the generated NeoVM bytecode. Optimization levels are controlled using the `--optimize <LEVEL>` command-line option.

## Optimization Levels

*   **`--optimize None` (or omitting the flag):** No optimizations are applied.
*   **`--optimize Basic`:** Applies fundamental optimizations.
*   **`--optimize All`:** (Behavior might vary by version) Typically applies `Basic` optimizations plus potentially more aggressive strategies.
*   **`--optimize Experimental`:** Used specifically for optimizing an existing `.nef` file as input. This usually involves more advanced analysis and rewriting of the bytecode, potentially using strategies from the `Neo.Optimizer` library.

## Basic Optimizations (`--optimize Basic`)

When `Basic` optimization is enabled, the following steps are typically performed after the initial bytecode generation:

1.  **Remove NOPs (`RemoveNops`):**
    *   **Purpose:** Eliminates `NOP` (No Operation) instructions from the bytecode sequence.
    *   **Mechanism:** Iterates through the instructions. If a `NOP` is found, it's removed. Any jump instructions (`JMP`, `CALL`, etc.) that previously targeted the `NOP` are updated to target the instruction immediately following the removed `NOP`.
    *   **Benefit:** Reduces script size slightly by removing unnecessary instructions.

2.  **Compress Jumps (`CompressJumps`):**
    *   **Purpose:** Reduces the size of jump, call, and try instructions where possible.
    *   **Mechanism:** Checks instructions like `JMP_L`, `CALL_L`, `JMPIF_L`, `JMPIFNOT_L`, `TRY_L`, etc. These instructions use a 4-byte offset to specify the jump target address. If the actual distance to the target instruction (relative offset) can fit within a single signed byte (-128 to +127), the instruction is converted to its shorter, 1-byte offset version (e.g., `JMP_L` becomes `JMP`, `CALL_L` becomes `CALL`, `TRY_L` becomes `TRY`). This process might be repeated as shortening one jump can bring other targets within range.
    *   **Benefit:** Significantly reduces script size, as jump instructions are common.

## Advanced Optimizations (`--optimize All`, `--optimize Experimental`)

These levels often engage more sophisticated optimization strategies, potentially including:

*   **Peephole Optimization:** Examines small, fixed-size sequences ("windows") of instructions and replaces them with shorter or faster equivalent sequences. Examples:
    *   Replacing `PUSH1`, `PUSH2`, `ADD` with `PUSH3`.
    *   Simplifying redundant stack manipulations (e.g., `DROP`, `DROP`).
    *   Optimizing constant conditional jumps.
*   **Reachability Analysis / Dead Code Elimination:** Analyzes the control flow graph to identify blocks of code that can never be reached during execution and removes them.
*   **Advanced Jump Compression:** More thorough analysis and rewriting of jump sequences.
*   **Miscellaneous Optimizations:** Other techniques specific to NeoVM bytecode patterns.

These advanced optimizations typically occur when the `Experimental` level is used on an existing `.nef` file, leveraging the separate `Neo.Optimizer` logic.

## Impact

*   **Script Size:** Optimizations, especially jump compression and dead code elimination, can significantly reduce the final `.nef` file size, lowering deployment costs.
*   **GAS Cost:** While optimizations primarily target size, some peephole optimizations can replace multiple instructions with fewer, potentially reducing execution GAS costs.
*   **Debugging:** Aggressive optimizations (especially at `Experimental` level) can sometimes make debugging harder, as the final bytecode might map less directly to the original source code structure.

Choosing the appropriate optimization level involves balancing the desire for smaller/cheaper contracts with compilation time and debugging ease.

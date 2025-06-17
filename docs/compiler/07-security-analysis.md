# Security Analysis

The Neo C# Compiler (`nccs`) includes an optional security analysis feature that performs basic static checks on the compiled bytecode (`.nef` file) to identify common patterns associated with potential vulnerabilities.

## Enabling Security Analysis

To enable the security checks, use the `--security-analysis` flag during compilation:

```bash
nccs MyContractProject.csproj --security-analysis
```

When enabled, the analysis runs after the contract has been successfully compiled into bytecode.

## Checks Performed

The security analyzer currently performs several checks, including:

1.  **Re-entrancy Vulnerability:**
    *   **Check:** Detects if the contract calls an external contract (using `System.Contract.Call`) *before* subsequently writing to its own storage (using `System.Storage.Put` or `System.Storage.Delete`) within the same potential execution path.
    *   **Risk:** If the external contract called is malicious or buggy, it could call back into the original contract *before* the state update (storage write) has occurred, potentially leading to unexpected behavior, double-spending, or inconsistent state.
    *   **Analyzer:** `ReEntrancyAnalyzer`

2.  **Storage Writes within `try` Blocks:**
    *   **Check:** Identifies if `System.Storage.Put` or `System.Storage.Delete` operations occur inside a `try` block.
    *   **Risk:** If an exception happens *after* the storage write but still within the `try` block (or a subsequent `catch`/`finally`), the storage change might persist even if the transaction logic intended for it to be atomic or conditional upon the success of the entire operation within the `try`.
    *   **Analyzer:** `WriteInTryAnalyzer` (inferred name)

3.  **`CheckWitness` Usage:**
    *   **Check:** Analyzes how the `Runtime.CheckWitness` syscall is used.
    *   **Risk:** Incorrect usage of `CheckWitness` (e.g., checking the wrong address, checking witness inefficiently, or not checking it at all when required for authorization) can lead to unauthorized access or actions.
    *   **Analyzer:** `CheckWitnessAnalyzer` (inferred name)

4.  **Contract Update Method:**
    *   **Check:** Examines the contract's `_deploy` method to see if it contains the standard logic for handling contract updates (checking witness of the deployer and calling `ContractManagement.Update`).
    *   **Risk:** If the standard update logic is missing or significantly altered, the contract may not be updatable using the standard deployment tools and procedures.
    *   **Analyzer:** `UpdateAnalyzer` (inferred name)

## Interpreting Results

The security analyzer prints its findings directly to the console as warnings. For example:

```
[SEC] Potential Re-entrancy: Calling contracts at instruction address: 123 before writing storage at
    456
```

```
[SEC] This contract cannot be updated, or maybe you used abstract code styles to update it.
```

## Important Considerations

*   **Static Analysis Limitations:** This is a *static* analysis tool, meaning it examines the bytecode without actually executing it. It looks for specific patterns and cannot understand the full context or intent of the code.
*   **False Positives:** The analyzer **can generate false positives**. A reported warning does not definitively mean there is a vulnerability. The identified pattern might be intentional and safe within the specific context of your contract logic.
*   **Not Exhaustive:** The analyzer only checks for a limited set of known patterns. **It does not guarantee your contract is secure.**
*   **Supplementary Tool:** Use the security analyzer as **one tool among many** in your security process. It is **not a substitute** for thorough manual code review, security audits by experts, and comprehensive testing.

Always critically evaluate the warnings produced by the analyzer in the context of your contract's specific design and requirements.

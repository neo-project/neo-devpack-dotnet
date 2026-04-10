# Multi-PR Roadmap Implementation Plan

> **For Implementer:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Deliver 10 separate branches/PRs from `origin/master-n3` covering the requested compiler optimization and framework/template gaps, each with corresponding tests and fresh verification.

**Architecture:** Split the work into isolated git worktrees so each issue stays reviewable and independently mergeable. Use focused tests per branch, then run `dotnet format`, build, and at least two fresh test passes before pushing and opening each PR against `master-n3`.

**Tech Stack:** .NET 10, MSTest, Neo compiler/framework/test projects, GitHub CLI.

### Branch Map

**Files:**
- Modify: `docs/plans/2026-03-22-multi-pr-roadmap.md`

**Step 1: Create isolated worktrees from `origin/master-n3`**

Branches/worktrees:
- `opt/remove-nops-on2`
- `opt/compress-jumps-rebuilds`
- `opt/short-jumps-default`
- `opt/switch-jump-table`
- `feat/framework-ownable-base`
- `feat/framework-pausable`
- `feat/framework-safemath`
- `feat/template-nep11-divisible`
- `feat/framework-inep17`
- `feat/framework-inep11`

**Step 2: Validate clean baseline in the first worktree**

Run:
```bash
dotnet build neo-devpack-dotnet.sln
dotnet test tests/Neo.Compiler.CSharp.UnitTests/Neo.Compiler.CSharp.UnitTests.csproj --filter UnitTest_OptimizerAutomation
dotnet test tests/Neo.SmartContract.Framework.UnitTests/Neo.SmartContract.Framework.UnitTests.csproj --filter SupportedStandardsTest
```

Expected: all commands exit successfully on `origin/master-n3`.

### Task 1: `RemoveNops` O(n²)

**Files:**
- Modify: `src/Neo.Compiler.CSharp/Optimizer/BasicOptimizer.cs`
- Test: `tests/Neo.Compiler.CSharp.UnitTests/Optimizer/BasicOptimizerTests.cs`

**Step 1: Write failing/performance-shape tests**
- Add a test that creates a dense instruction list with many `NOP`s and jump targets.
- Assert `RemoveNops` removes all `NOP`s and preserves jump retargeting.

**Step 2: Run targeted test**

Run:
```bash
dotnet test tests/Neo.Compiler.CSharp.UnitTests/Neo.Compiler.CSharp.UnitTests.csproj --filter BasicOptimizerTests
```

**Step 3: Implement minimal fix**
- Replace repeated `List.RemoveAt` plus full retarget scan with a single-pass compaction and target remap.

**Step 4: Re-run targeted test**

Run:
```bash
dotnet test tests/Neo.Compiler.CSharp.UnitTests/Neo.Compiler.CSharp.UnitTests.csproj --filter BasicOptimizerTests
```

### Task 2: `CompressJumps` repeated rebuilds

**Files:**
- Modify: `src/Neo.Compiler.CSharp/Optimizer/BasicOptimizer.cs`
- Test: `tests/Neo.Compiler.CSharp.UnitTests/Optimizer/BasicOptimizerTests.cs`

**Step 1: Add test covering multi-jump compression**
- Build instructions with several long-form jumps that can all compress together.
- Assert final opcodes are short form where offsets fit.

**Step 2: Run targeted test**

Run:
```bash
dotnet test tests/Neo.Compiler.CSharp.UnitTests/Neo.Compiler.CSharp.UnitTests.csproj --filter BasicOptimizerTests
```

**Step 3: Implement minimal fix**
- Avoid `RebuildOffsets()` inside the `foreach` loop cycle.
- Compute compression candidates in passes with bounded offset rebuilds.

**Step 4: Re-run targeted test**

Run:
```bash
dotnet test tests/Neo.Compiler.CSharp.UnitTests/Neo.Compiler.CSharp.UnitTests.csproj --filter BasicOptimizerTests
```

### Task 3: Long-form jumps optimization

**Files:**
- Modify: `src/Neo.Compiler.CSharp/MethodConvert/StackHelpers.OpCodes.cs`
- Modify: `src/Neo.Compiler.CSharp/CompilationEngine/CompilationContext.cs`
- Test: `tests/Neo.Compiler.CSharp.UnitTests/Optimizer/JumpEncodingTests.cs`

**Step 1: Add test proving short jumps can be emitted for local control flow**
- Compile a small contract with short branches.
- Assert short opcodes are emitted before fallback compression.

**Step 2: Run targeted test**

Run:
```bash
dotnet test tests/Neo.Compiler.CSharp.UnitTests/Neo.Compiler.CSharp.UnitTests.csproj --filter JumpEncodingTests
```

**Step 3: Implement minimal fix**
- Prefer short-form jump opcodes at emit sites where safe, while preserving long-form fallback for oversized offsets.

**Step 4: Re-run targeted test**

Run:
```bash
dotnet test tests/Neo.Compiler.CSharp.UnitTests/Neo.Compiler.CSharp.UnitTests.csproj --filter JumpEncodingTests
```

### Task 4: Switch without jump table

**Files:**
- Modify: `src/Neo.Compiler.CSharp/MethodConvert/Statement/SwitchStatement.cs`
- Possibly modify: `src/Neo.Compiler.CSharp/MethodConvert/Helpers/ControlFlowDsl.cs`
- Test: `tests/Neo.Compiler.CSharp.UnitTests/UnitTest_SwitchStatementSemantics.cs`

**Step 1: Add test for dense integral switch lowering**
- Compile a switch with dense integer labels.
- Assert the compiled form avoids linear compare chains when jump-table lowering applies.

**Step 2: Run targeted test**

Run:
```bash
dotnet test tests/Neo.Compiler.CSharp.UnitTests/Neo.Compiler.CSharp.UnitTests.csproj --filter UnitTest_SwitchStatementSemantics
```

**Step 3: Implement minimal fix**
- Add a jump-table lowering path for supported dense integral switches.
- Preserve existing behavior for pattern switches, guards, sparse values, and non-integral labels.

**Step 4: Re-run targeted test**

Run:
```bash
dotnet test tests/Neo.Compiler.CSharp.UnitTests/Neo.Compiler.CSharp.UnitTests.csproj --filter UnitTest_SwitchStatementSemantics
```

### Task 5: Framework `Ownable` base class

**Files:**
- Create: `src/Neo.SmartContract.Framework/Ownable.cs`
- Create: `tests/Neo.SmartContract.Framework.TestContracts/Contract_Ownable.cs`
- Create: `tests/Neo.SmartContract.Framework.UnitTests/OwnableTest.cs`
- Update artifacts as needed under `tests/Neo.SmartContract.Framework.UnitTests/TestingArtifacts/`

**Step 1: Add failing framework contract test**
- Cover owner initialization, getter, setter, witness enforcement, and event emission.

**Step 2: Run targeted test**

Run:
```bash
dotnet test tests/Neo.SmartContract.Framework.UnitTests/Neo.SmartContract.Framework.UnitTests.csproj --filter OwnableTest
```

**Step 3: Implement minimal base class**
- Lift the reusable ownership pattern into `Neo.SmartContract.Framework`.

**Step 4: Re-run targeted test**

Run:
```bash
dotnet test tests/Neo.SmartContract.Framework.UnitTests/Neo.SmartContract.Framework.UnitTests.csproj --filter OwnableTest
```

### Task 6: Framework `Pausable` pattern

**Files:**
- Create: `src/Neo.SmartContract.Framework/Pausable.cs`
- Create: `tests/Neo.SmartContract.Framework.TestContracts/Contract_Pausable.cs`
- Create: `tests/Neo.SmartContract.Framework.UnitTests/PausableTest.cs`
- Update artifacts as needed

**Step 1: Add failing tests**
- Cover pause/unpause, paused-state reads, authorization, and guarded execution.

**Step 2: Run targeted test**

Run:
```bash
dotnet test tests/Neo.SmartContract.Framework.UnitTests/Neo.SmartContract.Framework.UnitTests.csproj --filter PausableTest
```

**Step 3: Implement minimal pattern**
- Build on ownership semantics or explicit authorization, depending on what keeps the API smallest and clearest.

**Step 4: Re-run targeted test**

Run:
```bash
dotnet test tests/Neo.SmartContract.Framework.UnitTests/Neo.SmartContract.Framework.UnitTests.csproj --filter PausableTest
```

### Task 7: Framework `SafeMath` utility

**Files:**
- Create: `src/Neo.SmartContract.Framework/Helpers/SafeMath.cs`
- Create: `tests/Neo.SmartContract.Framework.TestContracts/Contract_SafeMath.cs`
- Create: `tests/Neo.SmartContract.Framework.UnitTests/SafeMathTest.cs`
- Update artifacts as needed

**Step 1: Add failing tests**
- Cover checked add/sub/mul helpers and overflow/underflow detection on large `BigInteger` values.

**Step 2: Run targeted test**

Run:
```bash
dotnet test tests/Neo.SmartContract.Framework.UnitTests/Neo.SmartContract.Framework.UnitTests.csproj --filter SafeMathTest
```

**Step 3: Implement minimal helper**
- Provide explicit checked arithmetic helpers compatible with contract code.

**Step 4: Re-run targeted test**

Run:
```bash
dotnet test tests/Neo.SmartContract.Framework.UnitTests/Neo.SmartContract.Framework.UnitTests.csproj --filter SafeMathTest
```

### Task 8: Divisible NFT template

**Files:**
- Modify: `src/Neo.Compiler.CSharp/TemplateManager.cs`
- Modify: template assets under `src/Neo.SmartContract.Template/templates/` if needed
- Modify: `src/Neo.SmartContract.Template/README.md`
- Test: `tests/Neo.Compiler.CSharp.UnitTests/UnitTest_TemplateManager.cs`

**Step 1: Add failing template test**
- Generate the new divisible NEP-11 template and assert expected class/base-type content.

**Step 2: Run targeted test**

Run:
```bash
dotnet test tests/Neo.Compiler.CSharp.UnitTests/Neo.Compiler.CSharp.UnitTests.csproj --filter UnitTest_TemplateManager
```

**Step 3: Implement minimal template support**
- Add a dedicated template entry and generated source for divisible NFT contracts.

**Step 4: Re-run targeted test**

Run:
```bash
dotnet test tests/Neo.Compiler.CSharp.UnitTests/Neo.Compiler.CSharp.UnitTests.csproj --filter UnitTest_TemplateManager
```

### Task 9: Framework `INEP17` interface

**Files:**
- Create: `src/Neo.SmartContract.Framework/Interfaces/INEP17.cs`
- Create/modify tests in `tests/Neo.SmartContract.Framework.UnitTests/SupportedStandardsTest.cs`

**Step 1: Add failing compile/use test**
- Confirm NEP-17 contracts can implement a framework-side interface with the standard method signatures.

**Step 2: Run targeted test**

Run:
```bash
dotnet test tests/Neo.SmartContract.Framework.UnitTests/Neo.SmartContract.Framework.UnitTests.csproj --filter SupportedStandardsTest
```

**Step 3: Implement minimal interface**
- Match the framework NEP-17 surface and naming conventions.

**Step 4: Re-run targeted test**

Run:
```bash
dotnet test tests/Neo.SmartContract.Framework.UnitTests/Neo.SmartContract.Framework.UnitTests.csproj --filter SupportedStandardsTest
```

### Task 10: Framework `INEP11` interface

**Files:**
- Create: `src/Neo.SmartContract.Framework/Interfaces/INEP11.cs`
- Create/modify tests in `tests/Neo.SmartContract.Framework.UnitTests/SupportedStandardsTest.cs`

**Step 1: Add failing compile/use test**
- Confirm NEP-11 contracts can implement a framework-side interface with standard method signatures.

**Step 2: Run targeted test**

Run:
```bash
dotnet test tests/Neo.SmartContract.Framework.UnitTests/Neo.SmartContract.Framework.UnitTests.csproj --filter SupportedStandardsTest
```

**Step 3: Implement minimal interface**
- Match the existing `Nep11Token<TState>` public contract shape.

**Step 4: Re-run targeted test**

Run:
```bash
dotnet test tests/Neo.SmartContract.Framework.UnitTests/Neo.SmartContract.Framework.UnitTests.csproj --filter SupportedStandardsTest
```

### Finalization Per Branch

**Files:**
- Modify: only files relevant to the branch

**Step 1: Format**

Run:
```bash
dotnet format
```

**Step 2: Build**

Run:
```bash
dotnet build neo-devpack-dotnet.sln
```

**Step 3: Run relevant tests twice**

Run the branch-specific targeted tests twice, fresh.

**Step 4: Push branch**

Run:
```bash
git push origin <branch-name>
```

**Step 5: Open PR against `master-n3`**

Run:
```bash
gh pr create --base master-n3 --head <branch-name> --fill
```

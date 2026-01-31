# Template Solution Production Readiness Implementation Plan

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Make the `neocontract` solution template production-ready by fixing naming, adding template test coverage, and validating generated artifacts.

**Architecture:** Update template source files to align with `sourceName` replacements, extend the template unit test harness to compile and validate the new template contract, and generate artifacts via the existing `EnsureArtifactsUpToDate` flow.

**Tech Stack:** .NET SDK (C#), MSTest, Neo.SmartContract.Testing, Neo.Compiler.CSharp

### Task 1: Fix template test namespace to support name replacement

**Files:**
- Modify: `src/Neo.SmartContract.Template/templates/neocontractsolution/NeoContractSolution.UnitTests/SmartContractTests.cs`

**Step 1: Write the failing test**
- Not applicable (template code change). Instead, add a TODO note for this task in the change list and validate via template unit tests in Task 3.

**Step 2: Run test to verify it fails**
- Run: `dotnet test tests/Neo.SmartContract.Template.UnitTests/Neo.SmartContract.Template.UnitTests.csproj --filter FullyQualifiedName~EnsureArtifactsUpToDate`
- Expected: FAIL (before template coverage updates in Task 2) or PASS if prior steps already done.

**Step 3: Write minimal implementation**
- Change namespace from `MyAwesomeContract.UnitTests` to `NeoContractSolution.UnitTests` so `sourceName` replacement applies.

**Step 4: Run test to verify it passes**
- Run (after Task 2): `dotnet test tests/Neo.SmartContract.Template.UnitTests/Neo.SmartContract.Template.UnitTests.csproj --filter FullyQualifiedName~EnsureArtifactsUpToDate`
- Expected: PASS

**Step 5: Commit**
- `git add src/Neo.SmartContract.Template/templates/neocontractsolution/NeoContractSolution.UnitTests/SmartContractTests.cs`
- `git commit -m "fix(templates): align solution template test namespace"`

### Task 2: Add template test coverage for neocontractsolution

**Files:**
- Modify: `tests/Neo.SmartContract.Template.UnitTests/templates/TestCleanup.cs`
- Create: `tests/Neo.SmartContract.Template.UnitTests/templates/neocontractsolution/TestingArtifacts/NeoContractSolutionTemplate.artifacts.cs`
- Create: `tests/Neo.SmartContract.Template.UnitTests/templates/neocontractsolution/NeoContractSolutionTests.cs`

**Step 1: Write the failing test**
- Add `NeoContractSolutionTests` inheriting `OwnableTests<NeoContractSolutionTemplate>` with a `TestMyMethod` and `TestUpdate` test.

**Step 2: Run test to verify it fails**
- Run: `dotnet test tests/Neo.SmartContract.Template.UnitTests/Neo.SmartContract.Template.UnitTests.csproj --filter FullyQualifiedName~NeoContractSolutionTests`
- Expected: FAIL (artifact not generated / template not wired in TestCleanup yet)

**Step 3: Write minimal implementation**
- Update `TestCleanup.cs` to compile `neocontractsolution/NeoContractSolution/SmartContract.cs`, generate artifacts, and cache the contract.
- Create a stub `NeoContractSolutionTemplate.artifacts.cs` with a minimal class declaration so the project builds; it will be overwritten by EnsureArtifactsUpToDate.

**Step 4: Run test to verify it passes**
- Run (artifact generation): `dotnet test tests/Neo.SmartContract.Template.UnitTests/Neo.SmartContract.Template.UnitTests.csproj --filter FullyQualifiedName~EnsureArtifactsUpToDate`
- Expected: FAIL on first run but writes the artifact file.
- Run again: same command; Expected: PASS

**Step 5: Commit**
- `git add tests/Neo.SmartContract.Template.UnitTests/templates/TestCleanup.cs tests/Neo.SmartContract.Template.UnitTests/templates/neocontractsolution`
- `git commit -m "test(templates): add neocontract solution coverage"`

### Task 3: Full verification

**Files:**
- Test: `tests/Neo.SmartContract.Template.UnitTests/Neo.SmartContract.Template.UnitTests.csproj`

**Step 1: Run the test suite**
- Run: `dotnet test tests/Neo.SmartContract.Template.UnitTests/Neo.SmartContract.Template.UnitTests.csproj`
- Expected: PASS

**Step 2: Commit**
- If any generated artifacts changed: `git add tests/Neo.SmartContract.Template.UnitTests/templates/neocontractsolution/TestingArtifacts/NeoContractSolutionTemplate.artifacts.cs`
- `git commit -m "chore(templates): update generated artifacts"`

---

Plan complete and saved to `docs/plans/2026-01-31-template-solution-production-ready.md`. Two execution options:

1. Subagent-Driven (this session) - I dispatch fresh subagent per task, review between tasks, fast iteration
2. Parallel Session (separate) - Open new session with executing-plans, batch execution with checkpoints

Which approach?

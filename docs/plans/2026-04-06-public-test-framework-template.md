# Public Test Framework Template Implementation Plan

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Make the public `neocontract` solution template generate a usable Neo contract unit-test project that follows the repo's existing `TestEngine` and `OwnableTests<T>` pattern instead of shipping a placeholder test class.

**Architecture:** Keep the existing testing runtime and standard test bases unchanged. Fix the public UX at the template layer by making the contract build emit source artifacts, making the generated unit-test project compile those artifacts, and replacing the placeholder unit test file with a real example that exercises the generated contract wrapper through `OwnableTests<T>`.

**Tech Stack:** .NET 10, MSTest, `Neo.Compiler.CSharp` (`nccs`), `Neo.SmartContract.Testing`, template content under `src/Neo.SmartContract.Template`, template verification tests under `tests/Neo.SmartContract.Template.UnitTests`

### Task 1: Capture the expected template UX in tests

**Files:**
- Modify: `tests/Neo.SmartContract.Template.UnitTests/templates/neocontractsolution/NeoContractSolutionTests.cs`
- Test: `tests/Neo.SmartContract.Template.UnitTests/Neo.SmartContract.Template.UnitTests.csproj`

**Step 1: Write the failing test**

Add assertions that describe the public solution template shape:
- the generated test class should inherit from `OwnableTests<T>`
- the test constructor should pass generated `Nef` and `Manifest`
- the example tests should cover `MyMethod()` and `Update(...)`

**Step 2: Run test to verify it fails**

Run: `dotnet test tests/Neo.SmartContract.Template.UnitTests/Neo.SmartContract.Template.UnitTests.csproj --filter NeoContractSolutionTests`
Expected: FAIL because the template-backed expectations no longer match the placeholder `SmartContractTests.cs`.

**Step 3: Write minimal implementation**

Do not change framework code yet. Only prepare to update template content after the failing assertion confirms the gap.

**Step 4: Run test to verify it still fails for the expected reason**

Run the same filtered command and confirm the failure is from the new template UX assertion, not from unrelated compilation issues.

**Step 5: Commit**

```bash
git add tests/Neo.SmartContract.Template.UnitTests/templates/neocontractsolution/NeoContractSolutionTests.cs
git commit -m "test: capture public contract test template expectations"
```

### Task 2: Wire generated artifacts into the public solution template

**Files:**
- Modify: `src/Neo.SmartContract.Template/templates/neocontractsolution/NeoContractSolution/NeoContractSolution.csproj`
- Modify: `src/Neo.SmartContract.Template/templates/neocontractsolution/NeoContractSolution.UnitTests/NeoContractSolution.UnitTests.csproj`
- Modify: `src/Neo.SmartContract.Template/templates/neocontractsolution/NeoContractSolution.UnitTests/SmartContractTests.cs`
- Test: `tests/Neo.SmartContract.Template.UnitTests/templates/neocontractsolution/NeoContractSolutionTests.cs`

**Step 1: Write the failing test**

Use the test from Task 1 as the red test. If needed, extend it with a second assertion that the unit-test project references the contract project and includes the generated `.artifacts.cs` file.

**Step 2: Run test to verify it fails**

Run: `dotnet test tests/Neo.SmartContract.Template.UnitTests/Neo.SmartContract.Template.UnitTests.csproj --filter NeoContractSolutionTests`
Expected: FAIL with missing expected strings or project wiring in the generated template files.

**Step 3: Write minimal implementation**

Update the public template to:
- pass `--generate-artifacts source` in the contract project's `nccs` invocation
- add a `ProjectReference` from the unit-test project to the contract project
- include the generated `..\NeoContractSolution\bin\sc\Contract.artifacts.cs` file in the unit-test project
- replace the placeholder MSTest class with a real `OwnableTests<...>` example in the same style as the repo's internal template tests

**Step 4: Run test to verify it passes**

Run: `dotnet test tests/Neo.SmartContract.Template.UnitTests/Neo.SmartContract.Template.UnitTests.csproj --filter NeoContractSolutionTests`
Expected: PASS

**Step 5: Commit**

```bash
git add src/Neo.SmartContract.Template/templates/neocontractsolution/NeoContractSolution/NeoContractSolution.csproj \
        src/Neo.SmartContract.Template/templates/neocontractsolution/NeoContractSolution.UnitTests/NeoContractSolution.UnitTests.csproj \
        src/Neo.SmartContract.Template/templates/neocontractsolution/NeoContractSolution.UnitTests/SmartContractTests.cs \
        tests/Neo.SmartContract.Template.UnitTests/templates/neocontractsolution/NeoContractSolutionTests.cs
git commit -m "feat: make public contract solution tests use testing artifacts"
```

### Task 3: Verify the generated-user workflow end to end

**Files:**
- Modify: `src/Neo.SmartContract.Template/README.md`
- Test: generated scratch solution under a temporary directory outside the repo tree

**Step 1: Write the failing test**

If the template README needs to describe the new test flow, add assertions or a focused doc check only if necessary. Otherwise use the manual end-to-end flow as the verification gate.

**Step 2: Run test to verify it fails**

Generate a fresh solution from the local template package or template folder, then run restore/build/test. Confirm the generated test project can see the emitted artifact wrapper and compile.

Run:
- `dotnet pack src/Neo.SmartContract.Template/Neo.SmartContract.Template.csproj -o /tmp/neo-template-pack`
- `dotnet new install /tmp/neo-template-pack/Neo.SmartContract.Template.*.nupkg --force`
- `dotnet new neocontract -n SampleContract -o /tmp/SampleContract`
- `dotnet test /tmp/SampleContract/SampleContract.UnitTests/SampleContract.UnitTests.csproj`

Expected before implementation: FAIL or require manual edits because the test project does not bind to the generated artifacts.

**Step 3: Write minimal implementation**

Update the template README only if the generated workflow or prerequisites need to be made explicit.

**Step 4: Run test to verify it passes**

Run the same pack/install/generate/test flow and confirm the scaffolded solution builds and the starter tests pass without manual edits.

**Step 5: Commit**

```bash
git add src/Neo.SmartContract.Template/README.md
git commit -m "docs: describe generated Neo contract test workflow"
```

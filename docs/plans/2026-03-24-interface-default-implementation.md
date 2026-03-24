# Interface Default Implementation Plan

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Allow interface methods with bodies to compile and be used as inherited default implementations when a contract/class implements the interface.

**Architecture:** Remove the analyzer rejection for interface method bodies, surface interface default methods during contract/class member collection, and compile/export those bodies as inherited methods. Keep the feature scoped to default interface methods with bodies and avoid broad interface-dispatch redesign unless a test proves it is required.

**Tech Stack:** C#, Roslyn analyzers, Neo compiler, MSTest

### Task 1: Add failing analyzer and compiler tests

**Files:**
- Modify: `tests/Neo.SmartContract.Analyzer.UnitTests/UnsupportedSyntaxAnalyzerUnitTests.cs`
- Create: `tests/Neo.Compiler.CSharp.UnitTests/UnitTest_DefaultInterfaceImplementation.cs`

**Step 1: Write the failing tests**

```csharp
[TestMethod]
public async Task DefaultInterfaceMethod_IsAllowed()
{
    var test = """
               interface ITest
               {
                   int Foo() { return 1; }
               }
               """;

    await VerifyCS.VerifyAnalyzerAsync(test);
}
```

```csharp
[TestMethod]
public void Contract_Uses_Interface_Default_Implementation()
{
    const string source = @"...";
    var context = CompileSingleContract(source);
    Assert.IsTrue(context.Success);
}
```

**Step 2: Run tests to verify they fail**

Run: `dotnet test tests/Neo.SmartContract.Analyzer.UnitTests/Neo.SmartContract.Analyzer.UnitTests.csproj --filter FullyQualifiedName~DefaultInterface`

Run: `dotnet test tests/Neo.Compiler.CSharp.UnitTests/Neo.Compiler.CSharp.UnitTests.csproj --filter FullyQualifiedName~DefaultInterfaceImplementation`

Expected: analyzer still reports `NC4052`; compiler test fails to export/use the default method.

### Task 2: Allow interface methods with bodies in analyzer

**Files:**
- Modify: `src/Neo.SmartContract.Analyzer/UnsupportedSyntaxAnalyzer.cs`

**Step 1: Implement minimal analyzer change**

```csharp
if (method.Parent is InterfaceDeclarationSyntax &&
    (method.Body is not null || method.ExpressionBody is not null))
{
    return;
}
```

**Step 2: Run analyzer tests**

Run: `dotnet test tests/Neo.SmartContract.Analyzer.UnitTests/Neo.SmartContract.Analyzer.UnitTests.csproj --filter FullyQualifiedName~DefaultInterface`

Expected: PASS

### Task 3: Surface default interface methods to the compiler

**Files:**
- Modify: `src/Neo.Compiler.CSharp/Helper.cs`
- Modify: `src/Neo.Compiler.CSharp/CompilationEngine/CompilationContext.cs`

**Step 1: Implement minimal member-collection support**

```csharp
// For classes, include implemented interface methods that have bodies
// when the class does not provide its own implementation.
```

**Step 2: Export/convert those methods like inherited concrete methods**

```csharp
// Ensure interface default methods with syntax bodies are converted
// and included in ABI/export resolution.
```

**Step 3: Run focused compiler test**

Run: `dotnet test tests/Neo.Compiler.CSharp.UnitTests/Neo.Compiler.CSharp.UnitTests.csproj --filter FullyQualifiedName~DefaultInterfaceImplementation`

Expected: PASS

### Task 4: Verify broader suites

**Files:**
- No additional code changes unless verification fails

**Step 1: Run full relevant suites**

Run: `dotnet test tests/Neo.Compiler.CSharp.UnitTests/Neo.Compiler.CSharp.UnitTests.csproj`

Run: `dotnet test tests/Neo.SmartContract.Analyzer.UnitTests/Neo.SmartContract.Analyzer.UnitTests.csproj`

Expected: PASS

**Step 2: Commit**

```bash
git add src/Neo.SmartContract.Analyzer/UnsupportedSyntaxAnalyzer.cs \
        src/Neo.Compiler.CSharp/Helper.cs \
        src/Neo.Compiler.CSharp/CompilationEngine/CompilationContext.cs \
        tests/Neo.SmartContract.Analyzer.UnitTests/UnsupportedSyntaxAnalyzerUnitTests.cs \
        tests/Neo.Compiler.CSharp.UnitTests/UnitTest_DefaultInterfaceImplementation.cs \
        docs/plans/2026-03-24-interface-default-implementation.md
git commit -m "feat(compiler): support default interface implementations"
```

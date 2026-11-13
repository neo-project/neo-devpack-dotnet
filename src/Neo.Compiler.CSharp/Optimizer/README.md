# Neo Smart Contract Optimizer

The Neo Compiler includes a powerful bytecode optimizer that reduces contract size and gas consumption. This document describes the available optimization strategies.

## Overview

Optimization happens in two layers:

1. **In-codegen cleanup** — applied while bytecode is generated, regardless of the optimization level (except `None`): NOP removal, short-form jump encoding (preferring 1-byte `JMP` over `JMP_L` when the target is in range), and pruning of unused local slots.
2. **Post-codegen strategy passes** — the strategies described below. These run **only** at the `Experimental` and `All` levels; the default `Basic` level does *not* run them.

## Optimization Types

The compiler supports different optimization levels through the `OptimizationType` enum:

```csharp
[Flags]
public enum OptimizationType : byte
{
    None = 0,
    Basic = 1,
    Experimental = 2,
    All = Basic | Experimental
}
```

### Available Options

| Option | Value | Description |
|--------|-------|-------------|
| `None` | 0 | No optimization at all, including the in-codegen cleanup. Outputs raw compiled bytecode. |
| `Basic` | 1 | **Default.** In-codegen cleanup only (NOP removal and short-form jump encoding). The post-codegen strategy passes below are not run. |
| `Experimental` | 2 | Runs the post-codegen strategy passes (peephole, reachability/dead-code, jump folding & compression) in addition to the in-codegen cleanup. |
| `All` | 3 | `Basic \| Experimental` — the full set of optimizations. Currently equivalent to `Experimental` plus the in-codegen cleanup. |

### Usage Examples

```csharp
// Programmatic usage
var options = new CompilationOptions
{
    Optimize = OptimizationType.Basic  // Safe optimizations only
};

var options = new CompilationOptions
{
    Optimize = OptimizationType.All    // Maximum optimization
};

var options = new CompilationOptions
{
    Optimize = OptimizationType.None   // No optimization
};
```

```bash
# Command line usage
nccs MyContract.csproj --optimize none         # No optimization
nccs MyContract.csproj --optimize basic        # Basic optimizations (default)
nccs MyContract.csproj --optimize experimental # Experimental only
nccs MyContract.csproj --optimize all          # All optimizations
```

## Optimization Strategies

> These strategy passes run only at the `Experimental` and `All` levels (see the table above).

### 1. Peephole Optimization (`Peephole.cs`)

**Purpose**: Pattern-based local optimizations that replace inefficient instruction sequences with more efficient equivalents.

**Key Optimizations**:
- **Redundant DUP/DROP removal** (`RemoveDupDrop`): eliminates a `DUP` whose value is immediately dropped or overwritten
- **Increment/decrement folding** (`UseIncDec`): rewrites `PUSH1 ADD` / `PUSH1 SUB` to `INC` / `DEC`
- **Non-zero simplification** (`UseNz`): collapses compare-against-zero sequences
- **Boolean-negation folding** (`FoldNotInEqual`, `FoldNotInJmp`): folds a `NOT` into the following `EQUAL`/`NOTEQUAL` or conditional jump

Arithmetic constant folding runs as a separate strategy pass (see `ConstantFold.cs` below); `PUSH1 PUSH2 ADD` is folded to `PUSH3` at the `Experimental` and `All` levels.

### 2. Jump Compression (`JumpCompresser.cs`)

**Purpose**: Optimizes jump instructions by using shorter jump variants when possible.

**Key Optimizations**:
- Converts `JMP_L` (5 bytes) to `JMP` (2 bytes) when target is within range
- Removes redundant jumps (jump to next instruction)
- Chains consecutive jumps

**Benefits**: Significant size reduction in contracts with many branches.

### 3. Reachability Analysis (`Reachability.cs`)

**Purpose**: Identifies and removes unreachable code paths.

**Key Optimizations**:
- Detects dead code after unconditional jumps/returns
- Removes unused exception handlers
- Eliminates unreachable branches

### 4. Miscellaneous Optimizations (`Miscellaneous.cs`)

**Purpose**: Various small optimizations that don't fit other categories.

**Key Optimizations**:
- **Method-token rewriting** (`RemoveMethodToken`): drops unused method tokens and renumbers `CALLT` operands accordingly

(NOP removal is part of the in-codegen cleanup, not a strategy pass.)

### 5. Constant Folding (`ConstantFold.cs`)

**Purpose**: Folds operations whose operands are both constant pushes into a single constant push.

**Key Optimizations**:
- **Arithmetic folding** (`FoldConstants`): `PUSH7 PUSH5 ADD` becomes `PUSH12`; also `SUB`, `MUL`, `DIV`, `MOD`, `MIN`, `MAX`
- **Bitwise folding**: `AND`, `OR`, `XOR`, `SHL`, `SHR` on constant pairs
- **Comparison folding**: `NUMEQUAL`, `NUMNOTEQUAL`, `LT`, `LE`, `GT`, `GE` on constant pairs produce `PUSHT`/`PUSHF`
- **Boolean folding**: `NOT` on a constant, `BOOLAND`/`BOOLOR` on constant boolean pairs
- **Byte-string folding**: `CAT` of two constant byte strings, `SIZE` of a constant byte string, `EQUAL`/`NOTEQUAL` of constant byte strings
- **Unary numeric folding**: `ABS` on constant integers

**Notes**:
- Runs after `InitStaticToConst`, so combinations of statically initialized fields (for example `static readonly int A = 7, B = 5; A + B`) are folded.
- Runs before `FoldConstantConditionalJump`, so comparisons folded to `PUSHT`/`PUSHF` then feed constant-conditional jump folding.
- Division by a constant zero is left unfolded to preserve the runtime fault.
- Patterns whose operands or operation are jump or exception-handler targets are left unfolded.

## Usage

The in-codegen cleanup runs by default (level `Basic`). The post-codegen strategy
passes require the `Experimental` or `All` level. Control it via compiler options:

```bash
# Default: Basic — in-codegen cleanup only
nccs MyContract.csproj

# Disable all optimization (including the in-codegen cleanup)
nccs MyContract.csproj --optimize none

# Run the post-codegen strategy passes
nccs MyContract.csproj --optimize experimental
nccs MyContract.csproj --optimize all
```

## Architecture

```
Optimizer/
├── Analysers/           # Code analysis utilities
│   ├── InstructionCoverage.cs
│   └── TryCatchFinallyCoverage.cs
├── AssetBuilder/        # Output generation
│   ├── DebugInfoBuilder.cs
│   └── OptimizedScriptBuilder.cs
├── Strategies/          # Optimization strategies
│   ├── Peephole.cs
│   ├── JumpCompresser.cs
│   ├── Reachability.cs
│   └── Miscellaneous.cs
├── BasicOptimizer.cs    # Main optimizer entry point
└── DumpNef.cs           # NEF file utilities
```

## Adding New Strategies

1. Create a new class in `Strategies/`
2. Implement the optimization logic
3. Add the `[Strategy]` attribute
4. Use the standard public static strategy signature

Valid attributed methods are discovered automatically when the optimizer is initialized.
Strategies run by descending priority. Equal priorities are ordered by assembly, declaring
type, method name, and metadata token so execution order does not depend on reflection order.

## Performance Impact

Typical optimization results:
- **Size reduction**: 10-30% smaller bytecode
- **Gas savings**: 5-20% lower execution cost
- **Compilation time**: Minimal overhead (~100ms)

## See Also

- [Neo Compiler Documentation](../../docs/)
- [NeoVM Instruction Reference](https://docs.neo.org/docs/n3/reference/neo_vm)

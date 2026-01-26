# Neo Smart Contract Optimizer

The Neo Compiler includes a powerful bytecode optimizer that reduces contract size and gas consumption. This document describes the available optimization strategies.

## Overview

The optimizer runs after initial compilation and applies various strategies to improve the generated NeoVM bytecode. Each strategy can be enabled/disabled independently.

## Optimization Strategies

### 1. Peephole Optimization (`Peephole.cs`)

**Purpose**: Pattern-based local optimizations that replace inefficient instruction sequences with more efficient equivalents.

**Key Optimizations**:
- **Dead code elimination**: Removes unreachable instructions
- **Redundant operation removal**: Eliminates unnecessary DUP/DROP pairs
- **Constant folding**: Pre-computes constant expressions at compile time
- **Instruction simplification**: Replaces complex sequences with simpler equivalents

**Example**:
```
Before: PUSH1 PUSH2 ADD
After:  PUSH3
```

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
- NOP removal
- Redundant type conversion elimination
- Stack operation simplification

## Usage

Optimization is enabled by default. Control it via compiler options:

```bash
# Full optimization (default)
nccs MyContract.csproj

# Disable optimization
nccs MyContract.csproj --no-optimize

# Specific optimization level
nccs MyContract.csproj --optimize basic
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
4. Register in `BasicOptimizer.cs`

## Performance Impact

Typical optimization results:
- **Size reduction**: 10-30% smaller bytecode
- **Gas savings**: 5-20% lower execution cost
- **Compilation time**: Minimal overhead (~100ms)

## See Also

- [Neo Compiler Documentation](../../docs/)
- [NeoVM Instruction Reference](https://docs.neo.org/docs/n3/reference/neo_vm)

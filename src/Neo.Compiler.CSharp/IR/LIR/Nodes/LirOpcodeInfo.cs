namespace Neo.Compiler.LIR;

internal sealed record LirOpcodeInfo(
    LirOpcode Op,
    int? Pop,
    int? Push,
    bool HasImmediate = false,
    int? ImmediateSizeBytes = null,
    ulong GasBase = 0);


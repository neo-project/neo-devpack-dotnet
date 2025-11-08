using System;

namespace Neo.Compiler.HIR;

/// <summary>
/// Describes visible side effects produced by an HIR instruction. This is intentionally fine grained so that
/// optimisation passes can reason about reordering and side-effect containment.
/// </summary>
[Flags]
internal enum HirEffect : byte
{
    None = 0,
    Runtime = 1 << 0,
    StorageRead = 1 << 1,
    StorageWrite = 1 << 2,
    Interop = 1 << 3,
    Crypto = 1 << 4,
    Abort = 1 << 5,
    /// <summary>
    /// Composite flag describing anything that mutates or depends upon shared state.
    /// </summary>
    Memory = Runtime | StorageRead | StorageWrite | Interop
}

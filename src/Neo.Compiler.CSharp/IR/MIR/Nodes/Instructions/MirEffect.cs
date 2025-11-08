using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.MIR;
[Flags]
internal enum MirEffect : ushort
{
    None = 0,
    Runtime = 1 << 0,
    StorageRead = 1 << 1,
    StorageWrite = 1 << 2,
    Interop = 1 << 3,
    Crypto = 1 << 4,
    Abort = 1 << 5,
    Memory = Runtime | StorageRead | StorageWrite | Interop
}

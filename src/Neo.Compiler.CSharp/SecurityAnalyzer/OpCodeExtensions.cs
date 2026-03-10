// Copyright (C) 2015-2026 The Neo Project.
//
// OpCodeExtensions.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.VM;

namespace Neo.Compiler.SecurityAnalyzer
{
    internal static class OpCodeExtensions
    {
        public static bool IsJumpInstruction(this OpCode opCode)
        {
            return opCode == OpCode.JMP
                || opCode == OpCode.JMP_L
                || opCode == OpCode.JMPIF
                || opCode == OpCode.JMPIF_L
                || opCode == OpCode.JMPIFNOT
                || opCode == OpCode.JMPIFNOT_L
                || opCode == OpCode.JMPEQ
                || opCode == OpCode.JMPEQ_L
                || opCode == OpCode.JMPNE
                || opCode == OpCode.JMPNE_L
                || opCode == OpCode.JMPGT
                || opCode == OpCode.JMPGT_L
                || opCode == OpCode.JMPGE
                || opCode == OpCode.JMPGE_L
                || opCode == OpCode.JMPLT
                || opCode == OpCode.JMPLT_L
                || opCode == OpCode.JMPLE
                || opCode == OpCode.JMPLE_L;
        }
    }
}

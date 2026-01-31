// Copyright (C) 2015-2026 The Neo Project.
//
// SymbolicState.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System.Collections.Generic;

namespace Neo.Compiler.SecurityAnalyzer.SymbolicExecution
{
    internal sealed class SymbolicState
    {
        public string EntryPoint { get; }
        public int InstructionIndex { get; set; }
        public List<SymbolicValue> Stack { get; }
        public List<int> CallStack { get; }
        public bool HasWitnessGuard { get; set; }
        public int BranchDepth { get; set; }

        public SymbolicState(string entryPoint, int instructionIndex)
        {
            EntryPoint = entryPoint;
            InstructionIndex = instructionIndex;
            Stack = new List<SymbolicValue>();
            CallStack = new List<int>();
        }

        private SymbolicState(string entryPoint, int instructionIndex, List<SymbolicValue> stack, List<int> callStack, bool hasWitnessGuard, int branchDepth)
        {
            EntryPoint = entryPoint;
            InstructionIndex = instructionIndex;
            Stack = stack;
            CallStack = callStack;
            HasWitnessGuard = hasWitnessGuard;
            BranchDepth = branchDepth;
        }

        public SymbolicState Clone()
        {
            return new SymbolicState(
                EntryPoint,
                InstructionIndex,
                new List<SymbolicValue>(Stack),
                new List<int>(CallStack),
                HasWitnessGuard,
                BranchDepth);
        }

        public void Push(SymbolicValue value) => Stack.Add(value);

        public SymbolicValue Pop()
        {
            if (Stack.Count == 0)
                return SymbolicValue.Unknown;
            int last = Stack.Count - 1;
            SymbolicValue value = Stack[last];
            Stack.RemoveAt(last);
            return value;
        }

        public SymbolicValue Peek()
        {
            if (Stack.Count == 0)
                return SymbolicValue.Unknown;
            return Stack[^1];
        }

        public void PushReturn(int index) => CallStack.Add(index);

        public bool TryPopReturn(out int index)
        {
            if (CallStack.Count == 0)
            {
                index = default;
                return false;
            }
            int last = CallStack.Count - 1;
            index = CallStack[last];
            CallStack.RemoveAt(last);
            return true;
        }
    }
}

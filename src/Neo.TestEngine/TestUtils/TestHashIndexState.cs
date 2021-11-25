// Copyright (C) 2015-2021 The Neo Project.
// 
// The Neo.Compiler.CSharp is free software distributed under the MIT 
// software license, see the accompanying file LICENSE in the main directory 
// of the project or http://www.opensource.org/licenses/mit-license.php 
// for more details.
// 
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.IO;
using Neo.SmartContract;
using Neo.VM;
using Neo.VM.Types;

namespace Neo.TestingEngine
{
    public class TestHashIndexState : IInteroperable
    {
        public UInt256? Hash;
        public uint Index;

        internal void FromStackItem(StackItem stackItem)
        {
            Struct @struct = (Struct)stackItem;
            Hash = new UInt256(@struct[0].GetSpan());
            Index = (uint)@struct[1].GetInteger();
        }

        void IInteroperable.FromStackItem(StackItem stackItem)
        {
            this.FromStackItem(stackItem);
        }

        internal StackItem ToStackItem(ReferenceCounter? referenceCounter)
        {
            return new Struct(referenceCounter) { Hash.ToArray(), Index };
        }

        StackItem IInteroperable.ToStackItem(ReferenceCounter referenceCounter)
        {
            return this.ToStackItem(referenceCounter);
        }
    }
}

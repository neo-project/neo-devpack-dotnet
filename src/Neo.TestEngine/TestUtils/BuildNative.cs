// Copyright (C) 2015-2022 The Neo Project.
// 
// The Neo.Compiler.CSharp is free software distributed under the MIT 
// software license, see the accompanying file LICENSE in the main directory 
// of the project or http://www.opensource.org/licenses/mit-license.php 
// for more details.
// 
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.Json;
using Neo.SmartContract;
using Neo.SmartContract.Native;
using Neo.VM;

namespace Neo.TestingEngine
{
    class BuildNative : BuildScript
    {
        public readonly NativeContract NativeContract;
        public BuildNative(NativeContract nativeContract)
            : this(nativeContract, nativeContract.Nef, nativeContract.Manifest.ToJson())
        {
        }

        private BuildNative(NativeContract nativeContract, NefFile nef, JObject manifest) : base(nef, manifest)
        {
            this.NativeContract = nativeContract;
            this.DebugInfo = null;
        }
    }
}

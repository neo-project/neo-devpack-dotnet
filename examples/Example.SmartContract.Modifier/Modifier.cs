// Copyright (C) 2015-2024 The Neo Project.
//
// Modifier.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System.ComponentModel;

namespace Modifier
{
    public class OwnerOnlyAttribute : ModifierAttribute
    {
        readonly UInt160 _owner;

        public OwnerOnlyAttribute(string hex)
        {
            _owner = (UInt160)(byte[])StdLib.Base64Decode(hex);
        }

        public override void Enter()
        {
            if (!Runtime.CheckWitness(_owner)) throw new System.Exception();
        }

        public override void Exit() { }
    }

    [DisplayName("SampleModifier")]
    [ContractAuthor("core-dev", "core@neo.org")]
    [ContractDescription("A sample contract to demonstrate how to use modifiers")]
    [ContractVersion("0.0.1")]
    [ContractSourceCode("https://github.com/neo-project/neo-devpack-dotnet/examples/Example.SmartContract.Exception")]
    public class SampleModifier : SmartContract
    {
        [OwnerOnly("AAAAAAAAAAAAAAAAAAAAAAAAAAA=")]
        public static bool Test()
        {
            return true;
        }
    }
}

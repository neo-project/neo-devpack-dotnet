// Copyright (C) 2015-2024 The Neo Project.
//
// Contract_AttributeChanged.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System.ComponentModel;

namespace Neo.Compiler.CSharp.TestContracts
{
    public class SampleAttribute : System.Attribute { }

    [DisplayName("Contract_AttributeChanged")]
    public class Contract_AttributeChanged : SmartContract.Framework.SmartContract
    {
        [Sample]
        public static bool test() => true;
    }
}

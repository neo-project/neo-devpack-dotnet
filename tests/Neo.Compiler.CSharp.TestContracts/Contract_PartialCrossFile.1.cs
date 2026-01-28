// Copyright (C) 2015-2026 The Neo Project.
//
// Contract_PartialCrossFile.1.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

namespace Neo.Compiler.CSharp.UnitTests.TestClasses
{
    /// <summary>
    /// Tests partial class compilation with cross-file method invocations.
    /// This exercises the SemanticModel fix for partial classes where
    /// syntax nodes may come from different syntax trees.
    /// </summary>
    public partial class Contract_PartialCrossFile : SmartContract.Framework.SmartContract
    {
        // Constant defined in this file, used in the other partial file
        private const int BaseValue = 100;

        // Method defined here, called from the other partial file
        public static int GetBaseValue()
        {
            return BaseValue;
        }

        // Method that calls a method defined in the other partial file
        public static int TestCrossFileCall()
        {
            return GetMultiplier() * BaseValue;
        }
    }
}

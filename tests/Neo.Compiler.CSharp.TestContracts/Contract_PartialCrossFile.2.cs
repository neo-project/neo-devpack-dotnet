// Copyright (C) 2015-2026 The Neo Project.
//
// Contract_PartialCrossFile.2.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

namespace Neo.Compiler.CSharp.UnitTests.TestClasses
{
    public partial class Contract_PartialCrossFile
    {
        // Constant defined in this file
        private const int Multiplier = 5;

        // Method defined here, called from the other partial file
        public static int GetMultiplier()
        {
            return Multiplier;
        }

        // Method that calls a method defined in the other partial file
        public static int TestCrossFileCallReverse()
        {
            return GetBaseValue() + Multiplier;
        }

        // Expression-bodied member that uses constant from other file
        public static int ExpressionBodyTest() => BaseValue + Multiplier;

        // Method using both constants in a complex expression
        public static int ComplexCrossFileExpression()
        {
            int result = BaseValue * Multiplier;
            result = result + GetBaseValue();
            return result;
        }
    }
}

// Copyright (C) 2015-2026 The Neo Project.
//
// Contract_Types_BigInteger.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System.Numerics;

namespace Neo.Compiler.CSharp.TestContracts
{
    public class Contract_Types_BigInteger : SmartContract.Framework.SmartContract
    {
        private static readonly BigInteger publicBigInteger = BigInteger.Parse("100000000000000000000000000");

        public static BigInteger SumOne()
        {
            return 1 + 1;
        }
        public static BigInteger SumOverflow()
        {
            var i = int.MaxValue;
            return i + 1;
        }
        public static BigInteger Attribute() { return publicBigInteger; }
        public static BigInteger Zero() { return BigInteger.Zero; }
        public static BigInteger One() { return BigInteger.One; }
        public static BigInteger MinusOne() { return BigInteger.MinusOne; }
        public static BigInteger Parse(string value) { return BigInteger.Parse(value); }
        public static BigInteger ConvertFromChar() { return (BigInteger)'A'; }
    }
}

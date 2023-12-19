// Copyright (C) 2015-2023 The Neo Project.
//
// The Neo.SmartContract.Framework is free software distributed under the MIT
// software license, see the accompanying file LICENSE in the main directory
// of the project or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Framework.Attributes;
using System.Numerics;

namespace Neo.SmartContract.Framework
{
    public class Math
    {
        [OpCode(OpCode.POW)]
        public static extern BigInteger Pow(BigInteger x, BigInteger y);
        [OpCode(OpCode.POW)]
        public static extern BigInteger Pow(long x, long y);
        [OpCode(OpCode.POW)]
        public static extern BigInteger Pow(int x, int y);
        [OpCode(OpCode.POW)]
        public static extern BigInteger Pow(ulong x, ulong y);
        [OpCode(OpCode.POW)]
        public static extern BigInteger Pow(uint x, uint y);
        [OpCode(OpCode.POW)]
        public static extern BigInteger Pow(short x, short y);
        [OpCode(OpCode.POW)]
        public static extern BigInteger Pow(ushort x, ushort y);
        [OpCode(OpCode.POW)]
        public static extern BigInteger Pow(byte x, byte y);

        [OpCode(OpCode.ABS)]
        public static extern sbyte Abs(sbyte x);
        [OpCode(OpCode.ABS)]
        public static extern short Abs(short x);
        [OpCode(OpCode.ABS)]
        public static extern int Abs(int x);
        [OpCode(OpCode.ABS)]
        public static extern long Abs(long x);

        [OpCode(OpCode.SIGN)]
        public static extern sbyte Sign(sbyte x);
        [OpCode(OpCode.SIGN)]
        public static extern short Sign(short x);
        [OpCode(OpCode.SIGN)]
        public static extern int Sign(int x);
        [OpCode(OpCode.SIGN)]
        public static extern long Sign(long x);

        [OpCode(OpCode.MAX)]
        public static extern byte Max(byte x, byte y);
        [OpCode(OpCode.MAX)]
        public static extern sbyte Max(sbyte x, sbyte y);
        [OpCode(OpCode.MAX)]
        public static extern short Max(short x, short y);
        [OpCode(OpCode.MAX)]
        public static extern ushort Max(ushort x, ushort y);
        [OpCode(OpCode.MAX)]
        public static extern int Max(int x, int y);
        [OpCode(OpCode.MAX)]
        public static extern uint Max(uint x, uint y);
        [OpCode(OpCode.MAX)]
        public static extern long Max(long x, long y);
        [OpCode(OpCode.MAX)]
        public static extern ulong Max(ulong x, ulong y);
        [OpCode(OpCode.MAX)]
        public static extern BigInteger Max(BigInteger x, BigInteger y);

        [OpCode(OpCode.MIN)]
        public static extern byte Min(byte x, byte y);
        [OpCode(OpCode.MIN)]
        public static extern sbyte Min(sbyte x, sbyte y);
        [OpCode(OpCode.MIN)]
        public static extern short Min(short x, short y);
        [OpCode(OpCode.MIN)]
        public static extern ushort Min(ushort x, ushort y);
        [OpCode(OpCode.MIN)]
        public static extern int Min(int x, int y);
        [OpCode(OpCode.MIN)]
        public static extern uint Min(uint x, uint y);
        [OpCode(OpCode.MIN)]
        public static extern long Min(long x, long y);
        [OpCode(OpCode.MIN)]
        public static extern ulong Min(ulong x, ulong y);
        [OpCode(OpCode.MIN)]
        public static extern BigInteger Min(BigInteger x, BigInteger y);
    }
}

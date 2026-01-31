// Copyright (C) 2015-2026 The Neo Project.
//
// SymbolicValue.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo;
using System;
using System.Numerics;
using System.Text;

namespace Neo.Compiler.SecurityAnalyzer.SymbolicExecution
{
    internal enum SymbolicValueKind
    {
        Unknown,
        Integer,
        Boolean,
        ByteString,
        UInt160,
        WitnessCheck,
    }

    internal sealed class SymbolicValue
    {
        public SymbolicValueKind Kind { get; }
        public BigInteger? IntegerValue { get; }
        public bool? BooleanValue { get; }
        public byte[]? ByteStringValue { get; }
        public UInt160? UInt160Value { get; }
        public SymbolicValue? WitnessArgument { get; }

        private SymbolicValue(
            SymbolicValueKind kind,
            BigInteger? integerValue = null,
            bool? booleanValue = null,
            byte[]? byteStringValue = null,
            UInt160? uint160Value = null,
            SymbolicValue? witnessArgument = null)
        {
            Kind = kind;
            IntegerValue = integerValue;
            BooleanValue = booleanValue;
            ByteStringValue = byteStringValue;
            UInt160Value = uint160Value;
            WitnessArgument = witnessArgument;
        }

        public static SymbolicValue Unknown { get; } = new(SymbolicValueKind.Unknown);

        public static SymbolicValue FromInteger(BigInteger value) => new(SymbolicValueKind.Integer, integerValue: value);

        public static SymbolicValue FromBoolean(bool value) => new(SymbolicValueKind.Boolean, booleanValue: value);

        public static SymbolicValue FromUInt160(UInt160 value) => new(SymbolicValueKind.UInt160, uint160Value: value);

        public static SymbolicValue FromByteString(byte[] value)
        {
            if (value.Length == UInt160.Length)
                return new SymbolicValue(SymbolicValueKind.ByteString, byteStringValue: value, uint160Value: new UInt160(value));
            return new SymbolicValue(SymbolicValueKind.ByteString, byteStringValue: value);
        }

        public static SymbolicValue WitnessCheck(SymbolicValue? argument)
            => new(SymbolicValueKind.WitnessCheck, witnessArgument: argument);

        public bool TryGetString(out string text)
        {
            text = string.Empty;
            if (Kind != SymbolicValueKind.ByteString || ByteStringValue is null)
                return false;
            try
            {
                text = Encoding.UTF8.GetString(ByteStringValue);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool TryGetUInt160(out UInt160 value)
        {
            if (Kind == SymbolicValueKind.UInt160 && UInt160Value is not null)
            {
                value = UInt160Value!;
                return true;
            }
            if (Kind == SymbolicValueKind.ByteString && UInt160Value is not null)
            {
                value = UInt160Value!;
                return true;
            }
            value = default!;
            return false;
        }
    }
}

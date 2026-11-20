// Copyright (C) 2015-2026 The Neo Project.
//
// Contract_CSharp15.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

namespace Neo.Compiler.CSharp.TestContracts
{
    public interface IMeasurement
    {
        public static int ToMillimeters(int meters) => meters * 1000;
    }

    public closed class GateState
    {
        public virtual string Describe() => "unknown";
    }

    public class ClosedGateState : GateState
    {
        public override string Describe() => "closed";
    }

    public class OpenGateState : GateState
    {
        public override string Describe() => "open";
    }

    public class Contract_CSharp15 : SmartContract.Framework.SmartContract
    {
        public static int ConvertToMillimeters(int meters)
        {
            return IMeasurement.ToMillimeters(meters);
        }

        public static string DescribeClosedGate()
        {
            return new ClosedGateState().Describe();
        }

        public static string DescribeGate(bool open)
        {
            GateState state = new ClosedGateState();
            if (open)
                state = new OpenGateState();
            return state.Describe();
        }
    }
}

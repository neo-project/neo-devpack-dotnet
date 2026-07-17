// Copyright (C) 2015-2026 The Neo Project.
//
// Contract_IndexOrRange.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;

namespace Neo.Compiler.CSharp.TestContracts
{
    public class Contract_IndexOrRange : SmartContract.Framework.SmartContract
    {
        public static void TestMain()
        {
            var evaluationOrderSlice = GetRangeReceiver()[GetRangeStart()..GetRangeEnd()];
            Runtime.Log(evaluationOrderSlice.Length.ToString());

            byte[] oneThroughTen = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var a = oneThroughTen[..];
            var b = oneThroughTen[..3];
            var c = oneThroughTen[2..];
            var d = oneThroughTen[3..5];
            var e = oneThroughTen[^2..];
            var f = oneThroughTen[..^3];
            var g = oneThroughTen[3..^4];
            var h = oneThroughTen[^4..^2];
            var i = oneThroughTen[0];

            Runtime.Log(a.Length.ToString());
            Runtime.Log(b.Length.ToString());
            Runtime.Log(c.Length.ToString());
            Runtime.Log(d.Length.ToString());
            Runtime.Log(e.Length.ToString());
            Runtime.Log(f.Length.ToString());
            Runtime.Log(g.Length.ToString());
            Runtime.Log(h.Length.ToString());
            Runtime.Log(i.ToString());

            string oneThroughNineString = "123456789";
            var a1 = oneThroughNineString[..];
            var b1 = oneThroughNineString[..3];
            var c1 = oneThroughNineString[2..];
            var d1 = oneThroughNineString[3..5];
            var e1 = oneThroughNineString[^2..];
            var f1 = oneThroughNineString[..^3];
            var g1 = oneThroughNineString[3..^4];
            var h1 = oneThroughNineString[^4..^2];
            var i1 = oneThroughNineString[0];

            Runtime.Log(a1.ToString());
            Runtime.Log(b1.ToString());
            Runtime.Log(c1.ToString());
            Runtime.Log(d1.ToString());
            Runtime.Log(e1.ToString());
            Runtime.Log(f1.ToString());
            Runtime.Log(g1.ToString());
            Runtime.Log(h1.ToString());
            Runtime.Log(i1.ToString());
        }

        private static byte[] GetRangeReceiver()
        {
            Runtime.Log("receiver");
            return new byte[] { 1, 2, 3, 4, 5 };
        }

        private static int GetRangeStart()
        {
            Runtime.Log("start");
            return 1;
        }

        private static int GetRangeEnd()
        {
            Runtime.Log("end");
            return 4;
        }

        private static string GetStringRangeReceiver()
        {
            Runtime.Log("string receiver");
            return "12345";
        }

        private static int GetNegativeRangeEndpoint()
        {
            Runtime.Log("negative");
            return -1;
        }

        public static int TestFromEndRangeEvaluationOrder()
        {
            return GetRangeReceiver()[^GetRangeStart()..GetRangeEnd()].Length;
        }

        public static string TestStringRangeEvaluationOrder()
        {
            return GetStringRangeReceiver()[GetRangeStart()..GetRangeEnd()];
        }

        public static void TestNullLeftFromEndRangeEvaluationOrder()
        {
            byte[]? receiver = null;
            _ = receiver![^GetRangeStart()..GetRangeEnd()];
        }

        public static void TestNullRightFromEndRangeEvaluationOrder()
        {
            byte[]? receiver = null;
            _ = receiver![GetRangeStart()..^GetRangeEnd()];
        }

        public static bool TestConditionalNullRangeSkipsEndpoints()
        {
            byte[]? receiver = null;
            return receiver?[GetRangeStart()..GetRangeEnd()] is null;
        }

        public static void TestNegativeStartSkipsEndEvaluation()
        {
            _ = GetRangeReceiver()[GetNegativeRangeEndpoint()..GetRangeEnd()];
        }

        public static void TestNegativeFromEndStopsAfterEndEvaluation()
        {
            _ = GetRangeReceiver()[GetRangeStart()..^GetNegativeRangeEndpoint()];
        }

    }
}

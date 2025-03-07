// Copyright (C) 2015-2025 The Neo Project.
//
// Contract_List.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Framework.Native;

namespace Neo.SmartContract.Framework.UnitTests.TestClasses
{
    public class Contract_List : SmartContract
    {
        public static int TestCount(int count)
        {
            List<int> some = new List<int>();
            for (int i = 0; i < count; i++)
            {
                some.Add(i);
            }

            return some.Count;
        }

        public static string TestAdd(int count)
        {
            List<int> some = new List<int>();
            for (int i = 0; i < count; i++)
            {
                some.Add(i);
            }

            return StdLib.JsonSerialize(some);
        }

        public static string TestRemoveAt(int count, int removeAt)
        {
            if (removeAt >= count) throw new System.Exception("Invalid test parameters");

            List<int> some = new List<int>();
            for (int i = 0; i < count; i++)
            {
                some.Add(i);
            }

            some.RemoveAt(removeAt);
            return StdLib.JsonSerialize(some);
        }

        public static string TestClear(int count)
        {
            List<int> some = new List<int>();
            for (int i = 0; i < count; i++)
            {
                some.Add(i);
            }

            some.Clear();
            return StdLib.JsonSerialize(some);
        }

        public static int[] TestArrayConvert(int count)
        {
            List<int> some = new List<int>();
            for (int i = 0; i < count; i++)
            {
                some.Add(i);
            }

            return some;
        }
    }
}

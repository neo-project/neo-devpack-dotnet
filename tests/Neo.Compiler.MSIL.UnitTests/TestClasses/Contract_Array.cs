using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace Neo.Compiler.MSIL.UnitTests.TestClasses
{
    struct State
    {
        public byte[] from;
        public byte[] to;
        public BigInteger amount;
    }

    class Contract_Array : SmartContract.Framework.SmartContract
    {
        public static object TestIntArray()
        {
            var arrobj = new int[3];
            arrobj[0] = 0;
            arrobj[1] = 1;
            arrobj[2] = 2;
            return arrobj;
        }

        public static object TestDefaultArray()
        {
            var arrobj = new int[3];
            if (arrobj[0] == 0) return true;
            return false;
        }

        public static object TestIntArrayInit()
        {
            var arrobj = new int[] { 1, 2, 3 };
            arrobj[1] = 4;
            arrobj[2] = 5;
            return arrobj;
        }

        public static object TestStructArray()
        {
            var s = new State();
            var sarray = new State[3];
            sarray[2] = s;
            return sarray[2];
        }

        public static object TestStructArrayInit()
        {
            var s = new State();
            State[] states = new State[] { s };
            for (var i = 0; i < 1; i++)
            {
                State state = states[i];
                return state;
            }
            return null;
        }
    }
}

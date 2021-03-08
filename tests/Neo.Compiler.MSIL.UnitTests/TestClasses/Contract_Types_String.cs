using System;
using System.Text;
using System.Numerics;
using System.Runtime.CompilerServices;
using Neo.SmartContract.Framework;

namespace Neo.Compiler.MSIL.UnitTests.TestClasses
{
    class Contract_Types_String : SmartContract.Framework.SmartContract
    {
        public static char checkIndex(string cad, int pos) { return cad[pos]; }
        public static string checkRange(string cad, int pos, int count) { return cad.Substring(pos, count); }
        public static string checkTake(string cad, int count) { return cad.Take(count); }
        public static string checkLast(string cad, int count) { return cad.Last(count); }
    }
}

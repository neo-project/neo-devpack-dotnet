using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;

namespace Neo.Compiler.CSharp.TestContracts
{
    public class Contract_IndexOrRange : SmartContract.Framework.SmartContract
    {
        public static void TestMain()
        {
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
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using System.ComponentModel;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_RangeSliceCatchable
{
    [TestMethod]
    public void InvalidStringAndByteRangesReachCatchBlocks()
    {
        const string source = """
            using Neo.SmartContract.Framework;

            public class Contract : SmartContract
            {
                public static string Between(string s, int start, int end)
                {
                    try { return s[start..end]; }
                    catch { return "caught"; }
                }

                public static string FromStart(string s, int start)
                {
                    try { return s[start..]; }
                    catch { return "caught"; }
                }

                public static string ToEnd(string s, int end)
                {
                    try { return s[..end]; }
                    catch { return "caught"; }
                }

                public static string FromEnd(string s, int end)
                {
                    try { return s[^1..end]; }
                    catch { return "caught"; }
                }

                public static string Last(string s, int count)
                {
                    try { return s[^count..]; }
                    catch { return "caught"; }
                }

                public static int Bytes(byte[] b, int start, int end)
                {
                    try { return b[start..end].Length; }
                    catch { return -1; }
                }
            }
            """;

        var context = TestHelper.CompileSingleContract(source);
        var contract = new TestEngine(true).Deploy<RangeSliceContract>(context.CreateExecutable(), context.CreateManifest());

        Assert.AreEqual("23", contract.Between("12345", 1, 3));
        Assert.AreEqual("caught", contract.Between("12345", 3, 2));
        Assert.AreEqual("caught", contract.Between("12345", 0, 10));
        Assert.AreEqual("caught", contract.Between("12345", -1, 2));
        Assert.AreEqual("", contract.Between("12345", 5, 5));
        Assert.AreEqual("caught", contract.FromStart("12345", 8));
        Assert.AreEqual("", contract.FromStart("12345", 5));
        Assert.AreEqual("caught", contract.ToEnd("12345", 10));
        Assert.AreEqual("12345", contract.ToEnd("12345", 5));
        Assert.AreEqual("caught", contract.FromEnd("12345", 2));
        Assert.AreEqual("5", contract.Last("12345", 1));
        Assert.AreEqual("caught", contract.Last("12345", 6));
        Assert.AreEqual(new BigInteger(-1), contract.Bytes([1, 2, 3, 4, 5], 3, 2));
        Assert.AreEqual(new BigInteger(2), contract.Bytes([1, 2, 3, 4, 5], 1, 3));
    }

    public abstract class RangeSliceContract(SmartContractInitialize initialize)
        : SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("between")]
        public abstract string? Between(string s, BigInteger start, BigInteger end);

        [DisplayName("fromStart")]
        public abstract string? FromStart(string s, BigInteger start);

        [DisplayName("toEnd")]
        public abstract string? ToEnd(string s, BigInteger end);

        [DisplayName("fromEnd")]
        public abstract string? FromEnd(string s, BigInteger end);

        [DisplayName("last")]
        public abstract string? Last(string s, BigInteger count);

        [DisplayName("bytes")]
        public abstract BigInteger? Bytes(byte[] b, BigInteger start, BigInteger end);
    }
}

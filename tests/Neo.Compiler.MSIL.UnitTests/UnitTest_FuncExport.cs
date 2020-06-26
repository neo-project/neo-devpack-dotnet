using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Numerics;

namespace Neo.Compiler.MSIL.UnitTests
{
    [TestClass]
    public class UnitTest_FuncExport
    {
        [TestMethod]
        public void ConvTypeTest()
        {
            Assert.AreEqual("Null", FuncExport.ConvType(null));
            Assert.AreEqual("String", FuncExport.ConvType(typeof(string).FullName));
            Assert.AreEqual("Integer", FuncExport.ConvType(typeof(BigInteger).FullName));
            Assert.AreEqual("Integer", FuncExport.ConvType(typeof(char).FullName));
            Assert.AreEqual("Integer", FuncExport.ConvType(typeof(byte).FullName));
            Assert.AreEqual("Integer", FuncExport.ConvType(typeof(sbyte).FullName));
            Assert.AreEqual("Integer", FuncExport.ConvType(typeof(short).FullName));
            Assert.AreEqual("Integer", FuncExport.ConvType(typeof(ushort).FullName));
            Assert.AreEqual("Integer", FuncExport.ConvType(typeof(int).FullName));
            Assert.AreEqual("Integer", FuncExport.ConvType(typeof(uint).FullName));
            Assert.AreEqual("Integer", FuncExport.ConvType(typeof(long).FullName));
            Assert.AreEqual("Integer", FuncExport.ConvType(typeof(ulong).FullName));
            Assert.AreEqual("Boolean", FuncExport.ConvType(typeof(bool).FullName));
            Assert.AreEqual("ByteArray", FuncExport.ConvType(typeof(byte[]).FullName));
            Assert.AreEqual("ByteArray", FuncExport.ConvType(typeof(object).FullName));
            Assert.AreEqual("Array", FuncExport.ConvType(typeof(object[]).FullName));
            Assert.AreEqual("Array", FuncExport.ConvType(typeof(int[]).FullName));
            Assert.AreEqual("Array", FuncExport.ConvType(typeof(bool[]).FullName));
            Assert.IsTrue(FuncExport.ConvType(typeof(Action<int>).FullName).StartsWith("Unknown:Pointers are not allowed to be public 'System.Action`1[[System.Int32"));
            Assert.IsTrue(FuncExport.ConvType(typeof(Func<int, bool>).FullName).StartsWith("Unknown:Pointers are not allowed to be public 'System.Func`2[[System.Int32"));
            Assert.AreEqual("Array", FuncExport.ConvType(typeof(Tuple<int, bool>).FullName));
            Assert.AreEqual("Array", FuncExport.ConvType(typeof(Tuple<int, bool>[]).FullName));
        }
    }
}

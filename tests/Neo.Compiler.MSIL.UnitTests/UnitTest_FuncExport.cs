using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mono.Cecil;
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
            Assert.AreEqual("Void", FuncExport.ConvType(FuncExport.Void));
            Assert.AreEqual("String", FuncExport.ConvType(Convert(typeof(string))));
            Assert.AreEqual("Integer", FuncExport.ConvType(Convert(typeof(BigInteger))));
            Assert.AreEqual("Integer", FuncExport.ConvType(Convert(typeof(char))));
            Assert.AreEqual("Integer", FuncExport.ConvType(Convert(typeof(byte))));
            Assert.AreEqual("Integer", FuncExport.ConvType(Convert(typeof(sbyte))));
            Assert.AreEqual("Integer", FuncExport.ConvType(Convert(typeof(short))));
            Assert.AreEqual("Integer", FuncExport.ConvType(Convert(typeof(ushort))));
            Assert.AreEqual("Integer", FuncExport.ConvType(Convert(typeof(int))));
            Assert.AreEqual("Integer", FuncExport.ConvType(Convert(typeof(uint))));
            Assert.AreEqual("Integer", FuncExport.ConvType(Convert(typeof(long))));
            Assert.AreEqual("Integer", FuncExport.ConvType(Convert(typeof(ulong))));
            Assert.AreEqual("Boolean", FuncExport.ConvType(Convert(typeof(bool))));
            Assert.AreEqual("ByteArray", FuncExport.ConvType(Convert(typeof(byte[]))));
            Assert.AreEqual("Any", FuncExport.ConvType(Convert(typeof(object))));
            Assert.AreEqual("Array", FuncExport.ConvType(Convert(typeof(object[]))));
            Assert.AreEqual("Array", FuncExport.ConvType(Convert(typeof(int[]))));
            Assert.AreEqual("Array", FuncExport.ConvType(Convert(typeof(bool[]))));
            Assert.IsTrue(FuncExport.ConvType(Convert(typeof(Action<int>))).StartsWith("Unknown:Pointers are not allowed to be public 'System.Action`1"));
            Assert.IsTrue(FuncExport.ConvType(Convert(typeof(Func<int, bool>))).StartsWith("Unknown:Pointers are not allowed to be public 'System.Func`2"));
            Assert.AreEqual("Array", FuncExport.ConvType(Convert(typeof(Tuple<int, bool>))));
            Assert.AreEqual("Array", FuncExport.ConvType(Convert(typeof(Tuple<int, bool>[]))));
        }

        private TypeReference Convert(Type type)
        {
            var a = AssemblyDefinition.ReadAssembly(type.Assembly.Location);
            return a.MainModule.ImportReference(type);
        }
    }
}

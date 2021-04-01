extern alias scfx;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract;
using scfx::Neo.SmartContract.Framework;
using System.IO;
using System.Linq;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_FuncExport
    {
        [TestMethod]
        public void ConvTypeTest()
        {
            var compilation = LoadTestCompilation("./TestClasses/Contract_ParameterType.cs");

            Assert.AreEqual(ContractParameterType.String, compilation.GetFieldContractType("_string"));
            Assert.AreEqual(ContractParameterType.Integer, compilation.GetFieldContractType("_bigInteger"));
            Assert.AreEqual(ContractParameterType.Integer, compilation.GetFieldContractType("_char"));
            Assert.AreEqual(ContractParameterType.Integer, compilation.GetFieldContractType("_byte"));
            Assert.AreEqual(ContractParameterType.Integer, compilation.GetFieldContractType("_sbyte"));
            Assert.AreEqual(ContractParameterType.Integer, compilation.GetFieldContractType("_short"));
            Assert.AreEqual(ContractParameterType.Integer, compilation.GetFieldContractType("_ushort"));
            Assert.AreEqual(ContractParameterType.Integer, compilation.GetFieldContractType("_int"));
            Assert.AreEqual(ContractParameterType.Integer, compilation.GetFieldContractType("_uint"));
            Assert.AreEqual(ContractParameterType.Integer, compilation.GetFieldContractType("_long"));
            Assert.AreEqual(ContractParameterType.Integer, compilation.GetFieldContractType("_ulong"));
            Assert.AreEqual(ContractParameterType.Boolean, compilation.GetFieldContractType("_bool"));
            Assert.AreEqual(ContractParameterType.Any, compilation.GetFieldContractType("_object"));
            Assert.AreEqual(ContractParameterType.ByteArray, compilation.GetFieldContractType("_byteArray"));
            Assert.AreEqual(ContractParameterType.Array, compilation.GetFieldContractType("_objectArray"));
            Assert.AreEqual(ContractParameterType.Array, compilation.GetFieldContractType("_intArray"));
            Assert.AreEqual(ContractParameterType.Array, compilation.GetFieldContractType("_boolArray"));
            Assert.AreEqual(ContractParameterType.Hash160, compilation.GetFieldContractType("_uint160"));
            Assert.AreEqual(ContractParameterType.Hash256, compilation.GetFieldContractType("_uint256"));
            Assert.AreEqual(ContractParameterType.PublicKey, compilation.GetFieldContractType("_ecpoint"));
        }

        private static Compilation LoadTestCompilation(string file)
        {
            var treeList = new[]
            {
                CSharpSyntaxTree.ParseText(File.ReadAllText(file))
            };
            var refs = new System.Collections.Generic.List<PortableExecutableReference>
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(BigInteger).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(SyscallAttribute).Assembly.Location)
            };
            return CSharpCompilation.Create("dummyAssembly", treeList, refs);
        }
    }

    static class CompilationHelper
    {
        public static ITypeSymbol GetFieldType(this Compilation compilation, string fieldName)
        {
            return (compilation.GetSymbolsWithName(fieldName).First() as IFieldSymbol)?.Type;
        }

        public static ContractParameterType GetFieldContractType(this Compilation compilation, string fieldName)
        {
            return compilation.GetFieldType(fieldName).GetContractParameterType();
        }
    }
}

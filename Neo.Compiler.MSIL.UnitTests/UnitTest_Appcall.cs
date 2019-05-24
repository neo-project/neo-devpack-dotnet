using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.Utils;
using System;

namespace Neo.Compiler.MSIL
{
    [TestClass]
    public class UnitTest_AppCall
    {
        [TestMethod]
        public void Test_Appcall() => JsonTestTool.TestOneCaseInJson("./TestJsons/test_appcall.json", "appcall");
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neo.Compiler.MSIL
{
    [TestClass]
    public class UnitTest_Metadata
    {
        [TestMethod]
        public void Test_default_Metadata()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_Event.cs");
            var abi = testengine.ScriptEntry.finialABI;
            var metadata = abi["metadata"].asDict();

            Assert.AreEqual(metadata["title"].AsString(), "TestContract");
            Assert.AreEqual(metadata["version"].AsString(), "0.0.0.0");
            Assert.AreEqual(metadata["description"].AsString(), null);
            Assert.AreEqual(metadata["author"].AsString(), null);
            Assert.AreEqual(metadata["email"].AsString(), null);
            Assert.AreEqual(metadata["has-storage"].AsBool(), false);
            Assert.AreEqual(metadata["has-dynamic-invoke"].AsBool(), false);
            Assert.AreEqual(metadata["is-payable"].AsBool(), false);
        }

        [TestMethod]
        public void Test_Metadata()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_metadata.cs");
            var abi = testengine.ScriptEntry.finialABI;
            var metadata = abi["metadata"].asDict();

            Assert.AreEqual(metadata["title"].AsString(), "contract title");
            Assert.AreEqual(metadata["version"].AsString(), "contract version");
            Assert.AreEqual(metadata["description"].AsString(), "contract description");
            Assert.AreEqual(metadata["author"].AsString(), "contract author");
            Assert.AreEqual(metadata["email"].AsString(), "contract email");
            Assert.AreEqual(metadata["has-storage"].AsBool(), true);
            Assert.AreEqual(metadata["has-dynamic-invoke"].AsBool(), true);
            Assert.AreEqual(metadata["is-payable"].AsBool(), true);
        }
    }
}

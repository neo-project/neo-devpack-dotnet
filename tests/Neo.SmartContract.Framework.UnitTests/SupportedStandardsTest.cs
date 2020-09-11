using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler;
using Neo.IO.Json;
using System.IO;

namespace Neo.SmartContract.Framework.UnitTests
{
    [TestClass]
    public class SupportedStandardsTest
    {
        [TestMethod]
        public void TestAttribute()
        {
            string path = Directory.GetCurrentDirectory();
            var option = new Program.Options()
            {
                File = path + "/TestClasses/Contract_SupportedStandards.cs"
            };
            Program.Compile(option);

            //Compile changes the path, reseting so that other UT won't break
            Directory.SetCurrentDirectory(path);

            var jobj = JObject.Parse(File.ReadAllText(path + "/TestClasses/Contract_SupportedStandards.manifest.json"));
            Assert.AreEqual(jobj["supportedstandards"].ToString(), @"[""NEP10"",""NEP5""]");
        }
    }
}

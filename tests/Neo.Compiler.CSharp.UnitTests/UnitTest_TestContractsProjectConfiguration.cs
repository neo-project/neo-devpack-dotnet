using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_TestContractsProjectConfiguration
    {
        [DataTestMethod]
        [DataRow("tests/Neo.Compiler.CSharp.TestContracts/Neo.Compiler.CSharp.TestContracts.csproj")]
        [DataRow("tests/Neo.SmartContract.Framework.TestContracts/Neo.SmartContract.Framework.TestContracts.csproj")]
        public void TestContractsAreNotMarkedAsTestProjects(string projectRelativePath)
        {
            var projectPath = Path.Combine(FindRepositoryRoot(), projectRelativePath);
            var project = XDocument.Load(projectPath);
            var isTestProject = project
                .Descendants()
                .Where(element => element.Name.LocalName == "IsTestProject")
                .Select(element => element.Value.Trim())
                .Single();

            Assert.AreEqual("false", isTestProject);
        }

        private static string FindRepositoryRoot()
        {
            var directory = new DirectoryInfo(AppContext.BaseDirectory);

            while (directory is not null)
            {
                if (File.Exists(Path.Combine(directory.FullName, "neo-devpack-dotnet.sln")))
                {
                    return directory.FullName;
                }

                directory = directory.Parent;
            }

            throw new InvalidOperationException("Unable to locate repository root.");
        }
    }
}

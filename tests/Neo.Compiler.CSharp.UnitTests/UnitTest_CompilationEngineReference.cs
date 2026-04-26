using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler;
using Neo.Json;
using System;
using System.IO;
using System.Reflection;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_CompilationEngineReference
{
    [TestMethod]
    public void UnsupportedDependencyAssetTypeIncludesContext()
    {
        const string dependencyName = "bad.asset/1.0.0";
        const string assetType = "unsupported-kind";
        var engine = new CompilationEngine(new CompilationOptions());
        var assets = new JObject
        {
            ["libraries"] = new JObject
            {
                [dependencyName] = new JObject
                {
                    ["type"] = assetType
                }
            },
            ["project"] = new JObject
            {
                ["restore"] = new JObject
                {
                    ["packagesPath"] = Path.GetTempPath()
                }
            }
        };
        var method = typeof(CompilationEngine).GetMethod("GetReference", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.IsNotNull(method);

        var exception = Assert.ThrowsException<TargetInvocationException>(() =>
            method.Invoke(engine, new object[]
            {
                dependencyName,
                new JObject(),
                assets,
                Path.GetTempPath(),
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
            }));

        Assert.IsInstanceOfType(exception.InnerException, typeof(NotSupportedException));
        var innerException = (NotSupportedException)exception.InnerException!;
        StringAssert.Contains(innerException.Message, assetType);
        StringAssert.Contains(innerException.Message, dependencyName);
    }
}

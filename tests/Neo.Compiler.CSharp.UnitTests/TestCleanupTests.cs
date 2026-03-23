using Akka.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using System.IO;
using System.Reflection;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class TestCleanupTests
{
    private static readonly string ArtifactsPath = Path.GetFullPath(Path.Combine("..", "..", "..", "TestingArtifacts"));
    private static readonly FieldInfo UpdatedArtifactNamesField = typeof(TestCleanup)
        .GetField("UpdatedArtifactNames", BindingFlags.NonPublic | BindingFlags.Static)
        ?? throw new System.InvalidOperationException("UpdatedArtifactNames field not found.");

    [TestMethod]
    public void EnsureArtifactUpToDateInternal_TracksUpdatedArtifacts_And_KeepsDebugInfo()
    {
        const string contractName = nameof(Contract_Assert);
        var contractType = typeof(Contract_Assert);
        var artifactPath = Path.Combine(ArtifactsPath, $"{contractName}.cs");
        var originalArtifact = File.ReadAllText(artifactPath);
        var updatedArtifactNames = (ConcurrentSet<string>)UpdatedArtifactNamesField.GetValue(null)!;

        try
        {
            while (updatedArtifactNames.TryRemove(contractName))
            {
            }
            TestCleanup.CachedContracts.TryRemove(contractType, out _);
            File.WriteAllText(artifactPath, "// stale artifact");

            _ = TestCleanup.EnsureArtifactUpToDateInternal(contractName);

            Assert.IsTrue(updatedArtifactNames.Contains(contractName), "Updated artifacts should be tracked.");
            Assert.IsTrue(TestCleanup.CachedContracts.TryGetValue(contractType, out var cached), "Compiled contract should be cached.");
            Assert.IsNotNull(cached.DbgInfo, "Debug info should still be available after artifact refresh.");
        }
        finally
        {
            File.WriteAllText(artifactPath, originalArtifact);
            while (updatedArtifactNames.TryRemove(contractName))
            {
            }
            TestCleanup.CachedContracts.TryRemove(contractType, out _);
        }
    }
}

namespace Neo.SmartContract.TestEngine.UnitTests
{
    public class ArtifactsTests
    {
        [Test]
        public void TestCreateSourceFromManifest()
        {
            // Compile

            string source = "../../../../../src/Neo.SmartContract.Template/templates/neocontractnep17/Contract1.cs";
            var result = Compiler.Compiler.Compile(optimize: false, debug: true, files: source);

            // Create artifacts

            source = Artifacts.CreateSourceFromManifest(result.Manifest).Replace("\r\n", "\n").Trim();

            Assert.That(source, Is.EqualTo(@"
using Neo
using System.Numerics;

namespace Neo.TestEngine.Contracts;

public class Contract1 : Neo.SmartContract.TestEngine.Mocks.SmartContract
{
    internal Contract1(Neo.SmartContract.TestEngine.Mocks.SmartContract.TestEngine testEngine) : base(testEngine) {}
    public abstract string symbol ();
    public abstract BigInteger decimals ();
    public abstract BigInteger totalSupply ();
    public abstract BigInteger balanceOf ();
    public abstract bool transfer ();
    public abstract void _deploy ();
    public abstract UInt160 getOwner ();
    public abstract void setOwner ();
    public abstract void burn ();
    public abstract void mint ();
    public abstract bool withdraw ();
    public abstract void onNEP17Payment ();
    public abstract bool verify ();
    public abstract string myMethod ();
    public abstract void _deploy ();
    public abstract void update ();
    public abstract void _initialize ();
}

".Replace("\r\n", "\n").Trim()));
        }
    }
}

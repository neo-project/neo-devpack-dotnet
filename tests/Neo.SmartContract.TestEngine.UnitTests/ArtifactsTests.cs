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
#region Constructor for internal use only
    internal Contract1(Neo.SmartContract.TestEngine.Mocks.SmartContract.TestEngine testEngine) : base(testEngine) {}
#endregion
#region Safe methods
    public abstract BigInteger balanceOf (UInt160 owner);
    public abstract BigInteger decimals ();
    public abstract UInt160 getOwner ();
    public abstract string symbol ();
    public abstract BigInteger totalSupply ();
    public abstract bool verify ();
#endregion
#region Unsafe methods
    public abstract void burn (UInt160 account, BigInteger amount);
    public abstract void mint (UInt160 to, BigInteger amount);
    public abstract string myMethod ();
    public abstract void onNEP17Payment (UInt160 from, BigInteger amount, object data);
    public abstract void setOwner (UInt160 newOwner);
    public abstract bool transfer (UInt160 from, UInt160 to, BigInteger amount, object data);
    public abstract void update (byte[] nefFile, string manifest);
    public abstract bool withdraw (UInt160 token, UInt160 to, BigInteger amount);
#endregion
}
".Replace("\r\n", "\n").Trim()));
        }
    }
}

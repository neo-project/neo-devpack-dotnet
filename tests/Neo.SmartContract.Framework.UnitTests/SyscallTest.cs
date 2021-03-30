extern alias scfx;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Numerics;
using SyscallAttribute = scfx.Neo.SmartContract.Framework.SyscallAttribute;

namespace Neo.SmartContract.Framework.UnitTests
{
    class MySymbolVisitor : SymbolVisitor<IEnumerable<string>>
    {
        private readonly string checkingAssembly;
        private readonly string checkingAttribute;

        public MySymbolVisitor(string checkingAssembly, string checkingAttribute)
        {
            this.checkingAssembly = checkingAssembly;
            this.checkingAttribute = checkingAttribute;
        }

        public override IEnumerable<string> VisitNamespace(INamespaceSymbol symbol)
        {
            foreach (INamespaceOrTypeSymbol member in symbol.GetMembers())
            {
                if (member.ContainingAssembly?.Name != checkingAssembly) continue;
                IEnumerable<string> result = member.Accept(this);
                if (result is null) continue;
                foreach (string value in result) yield return value;
            }
        }

        public override IEnumerable<string> VisitNamedType(INamedTypeSymbol symbol)
        {
            foreach (ISymbol member in symbol.GetMembers())
            {
                if (member is not INamedTypeSymbol and not IMethodSymbol) continue;
                IEnumerable<string> result = member.Accept(this);
                if (result is null) continue;
                foreach (string value in result) yield return value;
            }
        }

        public override IEnumerable<string> VisitMethod(IMethodSymbol symbol)
        {
            foreach (AttributeData attribute in symbol.GetAttributes())
            {
                if (attribute.AttributeClass.Name != checkingAttribute) continue;
                yield return (string)attribute.ConstructorArguments[0].Value;
            }
        }
    }

    [TestClass]
    public class SyscallTest
    {
        [TestMethod]
        public void TestAllSyscalls()
        {
            HashSet<string> neoSyscalls = ApplicationEngine.Services.Values.Select(p => p.Name).ToHashSet();
            neoSyscalls.Remove("System.Contract.NativeOnPersist");
            neoSyscalls.Remove("System.Contract.NativePostPersist");
            neoSyscalls.Remove("System.Contract.CallNative");
            neoSyscalls.Remove("System.Runtime.Notify");

            string coreDir = Path.GetDirectoryName(typeof(object).Assembly.Location)!;
            MetadataReference[] references = new[]
            {
                MetadataReference.CreateFromFile(Path.Combine(coreDir, "System.Runtime.dll")),
                MetadataReference.CreateFromFile(Path.Combine(coreDir, "System.Runtime.InteropServices.dll")),
                MetadataReference.CreateFromFile(typeof(string).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(DisplayNameAttribute).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(BigInteger).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(SyscallAttribute).Assembly.Location)
            };
            CSharpCompilation compilation = CSharpCompilation.Create(null, references: references);
            MySymbolVisitor visitor = new("Neo.SmartContract.Framework", nameof(SyscallAttribute));
            HashSet<string> fwSyscalls = visitor.Visit(compilation.GlobalNamespace).ToHashSet();

            fwSyscalls.SymmetricExceptWith(neoSyscalls);
            if (fwSyscalls.Count > 0)
            {
                Assert.Fail($"Unknown or unimplemented syscalls: {string.Join("\n", fwSyscalls)}");
            }
        }
    }
}

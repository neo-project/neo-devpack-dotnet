using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.IO;

namespace Neo.SmartContract.Framework.UnitTests
{
    [TestClass]
    public class SyscallTest
    {
        [TestMethod]
        public void TestAllSyscalls()
        {
            // Current syscalls

            var list = new List<string>();

            using (var stream = File.OpenRead(typeof(SmartContract).Assembly.Location))
            {
                var expectedType = typeof(SyscallAttribute).FullName;
                var module = Mono.Cecil.ModuleDefinition.ReadModule(stream);

                foreach (var type in module.Types)
                {
                    CheckType(type, expectedType, list);
                }
            }

            // Neo syscalls

            var notFound = new List<string>();

            foreach (var syscall in ApplicationEngine.Services)
            {
                if (syscall.Value.Name == "Neo.Native.Deploy") continue;
                if (syscall.Value.Name == "Neo.Native.Tokens.NEO") continue;
                if (syscall.Value.Name == "Neo.Native.Tokens.GAS") continue;
                if (syscall.Value.Name == "Neo.Native.Policy") continue;
                if (syscall.Value.Name == "Neo.Native.Call") continue;
                if (syscall.Value.Name == "System.Runtime.Notify") continue;

                if (list.Remove(syscall.Value.Name)) continue;

                notFound.Add(syscall.Value.Name);
            }

            if (list.Count > 0)
            {
                Assert.Fail($"Unknown syscalls: {string.Join("\n-", list)}");
            }

            if (notFound.Count > 0)
            {
                Assert.Fail($"Not implemented syscalls: {string.Join("\n-", notFound)}");
            }
        }

        private void CheckType(TypeDefinition type, string expectedType, List<string> list)
        {
            foreach (var nested in type.NestedTypes)
            {
                CheckType(nested, expectedType, list);
            }

            foreach (var method in type.Methods)
            {
                foreach (var attr in method.CustomAttributes)
                {
                    if (attr.AttributeType.FullName == expectedType)
                    {
                        var syscall = attr.ConstructorArguments[0].Value.ToString();
                        if (!list.Contains(syscall)) list.Add(syscall);
                    }
                }
            }
        }
    }
}

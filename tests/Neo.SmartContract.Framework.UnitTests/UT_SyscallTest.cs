using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;

namespace Neo.SmartContract.Framework.UnitTests
{
    [TestClass]
    public class UT_SyscallTest
    {
        [TestMethod]
        public void TestAllSyscalls()
        {
            // Current syscalls

            var list = new List<string>();

            using (var stream = File.OpenRead(typeof(SmartContract).Assembly.Location))
            {
                var module = Mono.Cecil.ModuleDefinition.ReadModule(stream);

                foreach (var type in module.Types)
                    foreach (var method in type.Methods)
                        foreach (var attr in method.CustomAttributes)
                        {
                            if (attr.AttributeType.FullName == "Neo.SmartContract.Framework.SyscallAttribute")
                            {
                                var syscall = attr.ConstructorArguments[0].Value.ToString();
                                if (!list.Contains(syscall)) list.Add(syscall);
                            }
                        }
            }

            // Neo syscalls

            foreach (var syscall in InteropService.SupportedMethods().Values)
            {
                if (list.Remove(syscall)) continue;

                Assert.Fail($"Not implemented the '{syscall}'");
            }

            if (list.Count > 0)
            {
                Assert.Fail($"Unknown syscalls: {string.Join(",", list)}");
            }
        }
    }
}

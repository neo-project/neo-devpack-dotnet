extern alias scfx;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using scfxSmartContract = scfx.Neo.SmartContract.Framework.SmartContract;
using SyscallAttribute = scfx.Neo.SmartContract.Framework.SyscallAttribute;

namespace Neo.SmartContract.Framework.UnitTests
{
    [TestClass]
    public class SyscallTest
    {
        [TestMethod]
        public void TestAllSyscalls()
        {
            // Current syscalls

            var list = new HashSet<string>();
            var expectedType = typeof(SyscallAttribute);
            foreach (var type in typeof(scfxSmartContract).Assembly.ExportedTypes)
            {
                CheckType(type, expectedType, list);
            }

            // Neo syscalls

            var notFound = new List<string>();

            foreach (var syscall in ApplicationEngine.Services)
            {
                if (syscall.Value.Name == "System.Contract.NativeOnPersist") continue;
                if (syscall.Value.Name == "System.Contract.NativePostPersist") continue;
                if (syscall.Value.Name == "System.Contract.CallNative") continue;
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

        private static void CheckType(Type type, Type attributeType, HashSet<string> list)
        {
            foreach (var method in type.GetMethods())
            {
                foreach (var attr in method.CustomAttributes)
                {
                    if (attr.AttributeType == attributeType)
                    {
                        var syscall = attr.ConstructorArguments[0].Value.ToString();
                        list.Add(syscall);
                    }
                }
            }
        }
    }
}

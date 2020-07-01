using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Framework.Services.System;
using System;
using System.Collections.Generic;

namespace Neo.SmartContract.Framework.UnitTests
{
    [TestClass]
    public class SyscallCallbackTest
    {
        [TestMethod]
        public void TestAllSyscallCallbacks()
        {
            // Current allowed syscalls

            var current = new Dictionary<string, uint>();

            foreach (var syscall in ApplicationEngine.Services)
            {
                if (!syscall.Value.AllowCallback) continue;

                current.Add(syscall.Value.Name.Replace(".", "_"), syscall.Value.Hash);
            }

            // Check equals

            foreach (var en in Enum.GetValues(typeof(SyscallCallback)))
            {
                if (!current.TryGetValue(en.ToString(), out var val))
                {
                    Assert.Fail($"`{en.ToString()}` Syscall is not defined in SyscallCallback");
                }

                current.Remove(en.ToString());

                if (val != (uint)en)
                {
                    Assert.Fail($"`{en.ToString()}` Syscall has a different hash, expected {(uint)en}, found: {val}");
                }
            }

            if (current.Count > 0)
            {
                Assert.Fail($"Not implemented syscalls: {string.Join("\n-", current.Values)}");
            }
        }
    }
}

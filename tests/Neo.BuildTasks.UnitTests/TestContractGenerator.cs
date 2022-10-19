using System;
using Neo.BuildTasks;
using Xunit;

namespace build_tasks
{
    public class TestContractGenerator
    {
        [Fact]
        public void throws_on_invalid_type_name()
        {
            var manifest = new NeoManifest() { Name = "Invalid Type Name" };
            Assert.ThrowsAny<Exception>(() => ContractGenerator.GenerateContractInterface(manifest, "", ""));
        }

        [Fact]
        public void generates_on_valid_type_name()
        {
            var manifest = new NeoManifest() { Name = "ValidTypeName" };
            var code = ContractGenerator.GenerateContractInterface(manifest, "", "");
            Assert.NotEqual(-1, code.IndexOf("interface ValidTypeName"));
        }

        [Fact]
        public void generates_on_dotted_type_name()
        {
            var manifest = new NeoManifest() { Name = "Dotted.Type.Name" };
            var code = ContractGenerator.GenerateContractInterface(manifest, "", "");
            Assert.NotEqual(-1, code.IndexOf("interface Name"));
        }

        [Fact]
        public void generates_on_contract_name_override()
        {
            var manifest = new NeoManifest() { Name = "Invalid Type Name" };
            var code = ContractGenerator.GenerateContractInterface(manifest, "ValidTypeName", "");
            Assert.NotEqual(-1, code.IndexOf("interface ValidTypeName"));
        }

        [Fact]
        public void throws_on_invalid_contract_name_override()
        {
            var manifest = new NeoManifest() { Name = "ValidTypeName" };
            Assert.ThrowsAny<Exception>(() => ContractGenerator.GenerateContractInterface(manifest, "Invalid Type Name", ""));
        }

        [Fact]
        public void throws_on_dotted_contract_name_override()
        {
            var manifest = new NeoManifest() { Name = "ValidTypeName" };
            Assert.ThrowsAny<Exception>(() => ContractGenerator.GenerateContractInterface(manifest, "Dotted.Type.Name", ""));
        }
    }
}

using System;
using System.ComponentModel;
using System.Numerics;

[assembly: Neo.SmartContract.Framework.ContractDescription("contract description")]
[assembly: Neo.SmartContract.Framework.ContractTitle("contract title")]
[assembly: Neo.SmartContract.Framework.ContractVersion("contract version")]
[assembly: Neo.SmartContract.Framework.ContractAuthor("contract author")]
[assembly: Neo.SmartContract.Framework.ContractEmail("contract email")]
[assembly: Neo.SmartContract.Framework.ContractHasDynamicInvoke]
[assembly: Neo.SmartContract.Framework.ContractHasStorage]
[assembly: Neo.SmartContract.Framework.ContractIsPayable]

namespace Neo.Compiler.MSIL.TestClasses
{
    public class Contract_Metadata : SmartContract.Framework.SmartContract
    {
        public static void Main(string method, object[] args) { }
    }
}

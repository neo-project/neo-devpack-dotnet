namespace Neo.SmartContract.Framework.Services.Neo
{
    public class Contract
    {
        public extern byte[] Script
        {
            [Syscall("Neo.Contract.GetScript")]
            get;
        }

        public extern bool IsPayable
        {
            [Syscall("Neo.Contract.IsPayable")]
            get;
        }

        [Syscall("Neo.Contract.Create")]
        public static extern Contract Create(byte[] script, ContractPropertyState contract_property_state);

        [Syscall("Neo.Contract.Migrate")]
        public static extern Contract Migrate(byte[] script, ContractPropertyState contract_property_state);

        [Syscall("System.Contract.Destroy")]
        public static extern void Destroy();
    }
}

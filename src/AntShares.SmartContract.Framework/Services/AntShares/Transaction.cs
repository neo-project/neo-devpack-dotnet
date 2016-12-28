namespace AntShares.SmartContract.Framework.Services.AntShares
{
    public class Transaction : IScriptContainer
    {
        public extern byte[] Hash
        {
            [Syscall("AntShares.Transaction.GetHash")]
            get;
        }

        public extern byte Type
        {
            [Syscall("AntShares.Transaction.GetType")]
            get;
        }

        [Syscall("AntShares.Transaction.GetAttributes")]
        public extern TransactionAttribute[] GetAttributes();

        [Syscall("AntShares.Transaction.GetInputs")]
        public extern TransactionInput[] GetInputs();

        [Syscall("AntShares.Transaction.GetOutputs")]
        public extern TransactionOutput[] GetOutputs();

        [Syscall("AntShares.Transaction.GetReferences")]
        public extern TransactionOutput[] GetReferences();
    }
}

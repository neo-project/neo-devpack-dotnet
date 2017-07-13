namespace Neo.SmartContract.Framework.Services.Neo
{
    public class Transaction : IScriptContainer
    {
        public extern byte[] Hash
        {
            [Syscall("Neo.Transaction.GetHash")]
            get;
        }

        public extern byte Type
        {
            [Syscall("Neo.Transaction.GetType")]
            get;
        }

        [Syscall("Neo.Transaction.GetAttributes")]
        public extern TransactionAttribute[] GetAttributes();

        [Syscall("Neo.Transaction.GetInputs")]
        public extern TransactionInput[] GetInputs();

        [Syscall("Neo.Transaction.GetOutputs")]
        public extern TransactionOutput[] GetOutputs();

        [Syscall("Neo.Transaction.GetReferences")]
        public extern TransactionOutput[] GetReferences();
    }
}

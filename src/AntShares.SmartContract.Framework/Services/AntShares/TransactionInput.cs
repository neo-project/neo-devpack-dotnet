namespace AntShares.SmartContract.Framework.Services.AntShares
{
    public class TransactionInput : IApiInterface
    {
        public extern byte[] PrevHash
        {
            [Syscall("AntShares.Input.GetHash")]
            get;
        }

        public extern ushort PrevIndex
        {
            [Syscall("AntShares.Input.GetIndex")]
            get;
        }
    }
}

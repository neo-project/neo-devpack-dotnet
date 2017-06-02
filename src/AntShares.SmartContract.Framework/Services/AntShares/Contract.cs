namespace AntShares.SmartContract.Framework.Services.AntShares
{
    public class Contract
    {
        public extern byte[] Script
        {
            [Syscall("AntShares.Contract.GetScript")]
            get;
        }

        [Syscall("AntShares.Contract.Destroy")]
        public static extern void Destroy();
    }
}

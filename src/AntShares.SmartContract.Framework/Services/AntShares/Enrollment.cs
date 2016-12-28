namespace AntShares.SmartContract.Framework.Services.AntShares
{
    public class Enrollment : Transaction
    {
        public extern byte[] PublicKey
        {
            [Syscall("AntShares.Enrollment.GetPublicKey")]
            get;
        }
    }
}

namespace AntShares.SmartContract.Framework.Services.AntShares
{
    public class Validator
    {
        [Syscall("AntShares.Validator.Register")]
        public static extern Validator Register(byte[] pubkey);
    }
}

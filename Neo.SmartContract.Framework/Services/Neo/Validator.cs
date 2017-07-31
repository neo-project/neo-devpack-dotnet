namespace Neo.SmartContract.Framework.Services.Neo
{
    public class Validator
    {
        [Syscall("Neo.Validator.Register")]
        public static extern Validator Register(byte[] pubkey);
    }
}

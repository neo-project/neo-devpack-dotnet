namespace NeoTestHarness.NativeContractInterfaces
{
    public interface Nep17Token
    {
        System.Numerics.BigInteger balanceOf(Neo.UInt160 account);
        System.Numerics.BigInteger decimals();
        string symbol();
        System.Numerics.BigInteger totalSupply();
        bool transfer(Neo.UInt160 @from, Neo.UInt160 to, System.Numerics.BigInteger amount, object data);

        interface Events
        {
            void Transfer(Neo.UInt160 @from, Neo.UInt160 to, System.Numerics.BigInteger amount);
        }
    }
}
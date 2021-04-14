using Neo.SmartContract.Framework.Native;

namespace Neo.Compiler.CSharp.UnitTests.TestClasses
{
    public class Contract_NativeContracts : SmartContract.Framework.SmartContract
    {
        public static uint OracleMinimumResponseFee()
        {
            return Oracle.MinimumResponseFee;
        }

        public static string NEOSymbol()
        {
            return NEO.Symbol;
        }

        public static string GASSymbol()
        {
            return GAS.Symbol;
        }

        public static Cryptography.ECC.ECPoint[] getOracleNodes()
        {
            return RoleManagement.GetDesignatedByRole(Role.Oracle, 0);
        }

        public static UInt160 NEOHash()
        {
            return NEO.Hash;
        }


        public static UInt160 LedgerHash()
        {
            return Ledger.Hash;
        }


        public static UInt256 LedgerCurrentHash()
        {
            return Ledger.CurrentHash;
        }

        public static uint LedgerCurrentIndex()
        {
            return Ledger.CurrentIndex;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using Neo.SmartContract.Framework.Services.Neo;

namespace Neo.Compiler.MSIL.UnitTests.TestClasses
{
    class Contract_NativeContracts : SmartContract.Framework.SmartContract
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
            return RoleManagement.GetDesignatedByRole(DesignationRole.Oracle, 0);
        }

        public static UInt160 NEOHash()
        {
            return Neo.SmartContract.Framework.Services.Neo.NEO.Hash;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using Neo.SmartContract.Framework.Services.Neo;

namespace Neo.Compiler.MSIL.UnitTests.TestClasses
{
    class Contract_NativeContracts : SmartContract.Framework.SmartContract
    {
        public static string NEOName()
        {
            return NEO.Name;
        }

        public static string GASName()
        {
            return GAS.Name;
        }

        public static string OracleName()
        {
            return Oracle.Name;
        }

        public static string DesignationName()
        {
            return Designation.Name;
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
            return Designation.GetDesignatedByRole(DesignationRole.Oracle);
        }

        public static UInt160 NEOHash()
        {
            return Neo.SmartContract.Framework.Services.Neo.NEO.Hash;
        }
    }
}

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

        public static uint OracleMinimumResponseFee()
        {
            return Oracle.MinimumResponseFee;
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

        public static byte[][] getOracleNodes()
        {
            return Designation.GetDesignatedByRole(DesignationRole.Oracle);
        }
    }
}

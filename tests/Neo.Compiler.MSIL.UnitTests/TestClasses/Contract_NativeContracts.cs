using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace Neo.Compiler.MSIL.UnitTests.TestClasses
{
    class Contract_NativeContracts : SmartContract.Framework.SmartContract
    {
        public static string NEOName()
        {
            return Neo.SmartContract.Framework.Services.Neo.NEO.Name;
        }

        public static string GASName()
        {
            return Neo.SmartContract.Framework.Services.Neo.GAS.Name;
        }

        public static string OracleName()
        {
            return Neo.SmartContract.Framework.Services.Neo.Oracle.Name;
        }

        public static string NEOSymbol()
        {
            return Neo.SmartContract.Framework.Services.Neo.NEO.Symbol;
        }

        public static string GASSymbol()
        {
            return Neo.SmartContract.Framework.Services.Neo.GAS.Symbol;
        }

        public static byte[][] getOracleNodes()
        {
            return Neo.SmartContract.Framework.Services.Neo.Oracle.GetOracleNodes();
        }
    }
}

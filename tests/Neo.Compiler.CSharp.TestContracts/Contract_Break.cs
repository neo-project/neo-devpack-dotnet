// Copyright (C) 2015-2024 The Neo Project.
//
// Contract_Break.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;

namespace Neo.Compiler.CSharp.TestContracts
{
    public class Contract_Break : SmartContract.Framework.SmartContract
    {
        public static void BreakInTryCatch(bool exception)
        {
            Storage.Put("\xff\x00", "\x00");
            foreach (object i in Storage.Find("\xff"))
                try { break; }
                finally { Storage.Put("\xff\x00", "\x01"); }
            ExecutionEngine.Assert(Storage.Get("\xff\x00")! == "\x01");
            foreach (object i in Storage.Find("\xff"))
                try
                {
                    for (int j = 0; j < 3;)
                        throw new System.Exception();
                }
                catch
                {
                    do { break; }
                    while (true);
                    try { break; }  // break foreach; should execute finally
                    finally { Storage.Put("\xff\x00", "\x00"); }
                }
                finally
                {
                    ExecutionEngine.Assert(Storage.Get("\xff\x00")! == "\x00");
                    foreach (int _ in new int[] { 0, 1, 2 })
                    {
                        Storage.Put("\xff\x00", "\x02");
                        break;
                    }
                }
            ExecutionEngine.Assert(Storage.Get("\xff\x00")! == "\x02");
            foreach (object i in Storage.Find("\xff"))
                try
                {
                    for (int j = 0; j < 3;)
                        break;
                    if (exception)
                        throw new System.Exception();
                }
                catch
                {
                    int j = 0;
                    while (j < 3)
                        break;
                    ExecutionEngine.Assert(j == 0);
                    try
                    {
                        Storage.Put("\xff\x00", "\x03");
                        throw;
                    }
                    catch { break; }  // foreach; should execute finally
                    finally
                    {
                        ExecutionEngine.Assert(Storage.Get("\xff\x00")! == "\x03");
                        Storage.Put("\xff\x00", "\x02");
                    }
                }
                finally
                {
                    ExecutionEngine.Assert(Storage.Get("\xff\x00")! == "\x02");
                    Storage.Put("\xff\x00", "\x03");
                }
            ExecutionEngine.Assert(Storage.Get("\xff\x00")! == "\x03");
        }
    }
}

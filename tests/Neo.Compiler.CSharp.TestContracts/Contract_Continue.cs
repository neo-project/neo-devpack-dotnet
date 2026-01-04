// Copyright (C) 2015-2026 The Neo Project.
//
// Contract_Continue.cs file belongs to the neo project and is free
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
    public class Contract_Continue : SmartContract.Framework.SmartContract
    {
        public static void ContinueInTryCatch(bool exception)
        {
            Storage.Put("\xff\x00", "\x00");
            foreach (object i in Storage.Find("\xff"))
                try { continue; }
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
                    do { continue; }
                    while (false);
                    try { continue; }  // continue foreach; should execute finally
                    finally { Storage.Put("\xff\x00", "\x00"); }
                }
                finally
                {
                    ExecutionEngine.Assert(Storage.Get("\xff\x00")! == "\x00");
                    foreach (int j in new int[] { 0, 1, 2 })
                    {
                        if (j < 2)
                            continue;
                        Storage.Put("\xff\x00", "\x02");
                    }
                }
            ExecutionEngine.Assert(Storage.Get("\xff\x00")! == "\x02");
            foreach (object i in Storage.Find("\xff"))
                try
                {
                    for (int j = 0; j < 3; ++j)
                        continue;
                    if (exception)
                        throw new System.Exception();
                }
                catch
                {
                    int j = 0;
                    while (j < 3)
                    {
                        j += 1;
                        continue;
#pragma warning disable CS0162 // Unreachable code detected
                        j = 10;
#pragma warning restore CS0162 // Unreachable code detected
                    }
                    ExecutionEngine.Assert(j == 3);
                    try
                    {
                        Storage.Put("\xff\x00", "\x03");
                        throw;
                    }
                    catch { continue; }  // foreach; should execute finally
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

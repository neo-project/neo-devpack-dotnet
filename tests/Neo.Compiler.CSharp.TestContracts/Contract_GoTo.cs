// Copyright (C) 2015-2024 The Neo Project.
//
// Contract_GoTo.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services;

namespace Neo.Compiler.CSharp.TestContracts
{
    public class Contract_GoTo : SmartContract.Framework.SmartContract
    {
        public static int test()
        {
            int a = 1;
        sum:
            a++;
            if (a == 3) return a;

            goto sum;
        }

        public static int testTry()
        {
            int a = 1;
        sum:
            try
            {
                a++;
                if (a == 3) return a;
            }
            catch { }
            goto sum;
        }

        public static void testTryComplex(bool exception)
        {
            Storage.Put("\xff\x00", "\x00");
            bool? goto_ = true;
        START0:
            Iterator f = Storage.Find("\xff");
            foreach (object i in f)
                try
                {
                    switch (goto_)
                    {
                        case true:
                            goto case null;
                        case null:
                            goto default;
                        case false:
                            goto END0;
                        default:
                            try { goto START0; }  // will execute continue
                            finally { ExecutionEngine.Assert(Storage.Get("\xff\x00")! == "\x00"); }
                    }
                }
                finally
                {
                    switch (goto_)
                    {
                        case true:
                            goto_ = null;
                            break;
                        case null:
                            goto_ = false;
                            break;
                        case false:
                            goto DEFAULT;
                        default:
                        DEFAULT:
                            Storage.Put("\xff\x00", "\x01");
                            break;
                    }
                }
            END0:
            ExecutionEngine.Assert(goto_ == false);
            ExecutionEngine.Assert(Storage.Get("\xff\x00")! == "\x01");
            foreach (object i in Storage.Find("\xff"))
                try
                {
                    throw new System.Exception();
                }
                catch
                {
                    do { goto END1; }
                    while (true);
                END1:
                    try { goto END2; }  // break foreach; should execute finally
                    finally { Storage.Put("\xff\x00", "\x00"); }
                }
                finally
                {
                    ExecutionEngine.Assert(Storage.Get("\xff\x00")! == "\x00");
                    foreach (int j in new int[] { -1, 0, 1, 2 })
                    {
                        switch (j)
                        {
                            case 0:
                                continue;
                            case 1:
                                goto case 0;
                            case 2:
                                break;
                            default:
                                goto case 0;
                        }
                        Storage.Put("\xff\x00", "\x02");
                    }
                }
            END2:
            ExecutionEngine.Assert(Storage.Get("\xff\x00")! == "\x02");
            foreach (object i in Storage.Find("\xff"))
                try
                {
                    switch (exception)
                    {
                        case false:
                            goto DEFAULT;
                        case true:
                            throw new System.Exception();
                        default:
                        DEFAULT:
                            goto ENDSWITCH;
                    }
                ENDSWITCH:;
                }
                catch
                {
                    try
                    {
                        Storage.Put("\xff\x00", "\x03");
                        throw;
                    }
                    catch { goto END3; }  // should execute finally
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
            END3:
            ExecutionEngine.Assert(Storage.Get("\xff\x00")! == "\x03");
        }
    }
}

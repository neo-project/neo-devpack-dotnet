using System;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services;

namespace Neo.Compiler.CSharp.TestContracts
{
    public class Contract_WriteInTry : SmartContract.Framework.SmartContract
    {
        public static void BaseTry()
        {
            try { Write(); }
            catch
            {
                try { Delete(); }
                finally
                {
                    try { throw new Exception("throw in nested finally"); }
                    catch { throw; }
                }
            }
            finally
            {
                try { Storage.Delete("\x00"); }
                catch { ExecutionEngine.Abort(); }
            }
        }

        private static void Write() => Storage.Put("\x00", '\x00');
        private static void Delete() => Storage.Delete("\x00");
        public static void TryWrite()
        {
            try { Write(); throw new Exception("throw in TryWrite try"); }
            catch { try { Delete(); throw new Exception("throw in TryWrite catch"); } finally { } }
        }
        public static void TryWriteWithVulnerability()
        {
            try { Delete(); } catch { }
        }

        public static void RecursiveTry(int i)
        {
            try
            {
                Write();
                if (i > 0)
                    RecursiveTry(i - 1);
            }
            finally
            {
                MutualRecursiveTry(i - 1);
            }
        }

        public static void MutualRecursiveTry(int i)
        {
            try
            {
                if (i > 0)
                    RecursiveTry(i - 1);
                TryWriteWithVulnerability();
                Delete();
            }
            finally { }
        }
    }
}

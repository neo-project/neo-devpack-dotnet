using Neo.Cryptography.ECC;
using Neo.SmartContract;
using Neo.SmartContract.Framework;

namespace Neo.Compiler.CSharp.UnitTests.TestClasses
{
    public class Contract_shift : SmartContract.Framework.SmartContract
    {
        [InitialValue("0a0b0c0d0E0F", ContractParameterType.ByteArray)]
        private static readonly ByteString rawECpoint = default;
        public static object try01()
        {
            int v = 0;
            try
            {
                v = 2;
            }
            catch
            {
                v = 3;
            }
            finally
            {
                v++;
            }
            return v;
        }

        public static object try02()
        {
            int v = 0;
            try
            {
                v = 2;
                throw new System.Exception();
            }
            catch
            {
                v = 3;
            }
            finally
            {
                v++;
            }
            return v;
        }

        public static object try03()
        {
            int v = 0;
            try
            {
                v = 2;
                throwcall();
            }
            catch
            {
                v = 3;
            }
            finally
            {
                v++;
            }
            return v;
        }

        public static object tryNest()
        {
            int v = 0;
            try
            {
                try
                {
                    v = 2;
                    throwcall();
                }
                catch
                {
                    v = 3;
                    throwcall();
                }
                finally
                {
                    throwcall();
                    v++;
                }
            }
            catch
            {
                v++;
            }
            return v;
        }

        public static object tryFinally()
        {
            int v = 0;
            try
            {
                v = 2;
            }
            finally
            {
                v++;
            }
            return v;
        }

        public static object tryFinallyAndRethrow()
        {
            int v = 0;
            try
            {
                v = 2;
                throwcall();
            }
            finally
            {
                v++;
            }
            return v;
        }

        public static object tryCatch()
        {
            int v = 0;
            try
            {
                v = 2;
                throwcall();
            }
            catch
            {
                v++;
            }
            return v;
        }

        public static object tryWithTwoFinally()
        {
            int v = 0;
            try
            {
                try
                {
                    v++;
                }
                catch
                {
                    v += 2;
                }
                finally
                {
                    v += 3;
                }
            }
            catch
            {
                v += 4;
            }
            finally
            {
                v += 5;
            }
            return v;
        }

        public static object tryecpointCast()
        {
            int v = 0;
            try
            {
                v = 2;
                ECPoint pubkey = (ECPoint)rawECpoint;
            }
            catch
            {
                v = 3;
            }
            finally
            {
                v++;
            }
            return v;
        }

        public static object throwcall()
        {
            throw new System.Exception();
        }

    }
}

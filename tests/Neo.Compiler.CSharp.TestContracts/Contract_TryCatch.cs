using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;

namespace Neo.Compiler.CSharp.TestContracts
{
    public class Contract_TryCatch : SmartContract.Framework.SmartContract
    {
        [ByteArray("0a0b0c0d0E0F")]
        private static readonly ByteString invalidECpoint = default!;
        [ByteArray("024700db2e90d9f02c4f9fc862abaca92725f95b4fddcc8d7ffa538693ecf463a9")]
        private static readonly ByteString byteString2Ecpoint = default!;
        [Hash160("NXV7ZhHiyM1aHXwpVsRZC6BwNFP2jghXAq")]
        private static readonly ByteString validUInt160 = default!;
        private static readonly UInt256 validUInt256 = "edcf8679104ec2911a4fe29ad7db232a493e5b990fb1da7af0c7b989948c8925";

        public static int try01(bool throwException)
        {
            int v = 0;
            try
            {
                v = 2;
                if (throwException) throw new System.Exception();
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

        public static int try02(bool throwException)
        {
            int v = 0;
            try
            {
                v = 2;
                if (throwException) throw new System.Exception();
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

        public static int try03(bool throwException)
        {
            int v = 0;
            try
            {
                v = 2;
                if (throwException) throwcall();
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

        public static int tryNest(bool throwInTry, bool throwInCatch, bool throwInFinally)
        {
            int v = 0;
            try
            {
                try
                {
                    v = 2;
                    if (throwInTry) throwcall();
                }
                catch
                {
                    v = 3;
                    if (throwInCatch) throwcall();
                }
                finally
                {
                    if (throwInFinally) throwcall();
                    v++;
                }
            }
            catch
            {
                v++;
            }
            return v;
        }

        public static int throwInCatch(bool throwInTry, bool throwInCatch)
        {
            int v = 0;
            try
            {
                v = 1;
                if (throwInTry) throw new System.Exception();
            }
            catch
            {
                v = 2;
                if (throwInCatch) throw new System.Exception();
            }
            finally
            {
                v = 3;
            }
            v = 4;
            return v;
        }

        public static int tryFinally(bool throwException)
        {
            int v = 0;
            try
            {
                v = 2;
                if (throwException) throw new System.Exception();
            }
            finally
            {
                v++;
            }
            return v;
        }

        public static int tryFinallyAndRethrow(bool throwException)
        {
            int v = 0;
            try
            {
                v = 2;
                if (throwException) throwcall();
            }
            finally
            {
                v++;
            }
            return v;
        }

        public static int tryCatch(bool throwException)
        {
            int v = 0;
            try
            {
                v = 2;
                if (throwException) throwcall();
            }
            catch
            {
                v++;
            }
            return v;
        }

        public static int tryWithTwoFinally(bool throwInInner, bool throwInOuter)
        {
            int v = 0;
            try
            {
                try
                {
                    v++;
                    if (throwInInner) throw new System.Exception();
                }
                catch
                {
                    v += 2;
                }
                finally
                {
                    v += 3;
                }
                if (throwInOuter) throw new System.Exception();
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

        public static int tryecpointCast(bool useInvalidECpoint)
        {
            int v = 0;
            try
            {
                v = 2;
                ECPoint pubkey = (ECPoint)(useInvalidECpoint ? invalidECpoint : byteString2Ecpoint);
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

        public static int tryvalidByteString2Ecpoint()
        {
            int v = 0;
            try
            {
                v = 2;
                ECPoint pubkey = (ECPoint)byteString2Ecpoint;
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

        public static int tryinvalidByteArray2UInt160(bool useInvalidECpoint)
        {
            int v = 0;
            try
            {
                v = 2;
                UInt160 data = (UInt160)(useInvalidECpoint ? invalidECpoint : validUInt160);
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

        public static int tryvalidByteArray2UInt160()
        {
            int v = 0;
            try
            {
                v = 2;
                UInt160 data = (UInt160)validUInt160;
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

        public static int tryinvalidByteArray2UInt256(bool useInvalidECpoint)
        {
            int v = 0;
            try
            {
                v = 2;
                UInt256 data = useInvalidECpoint ? (UInt256)invalidECpoint : validUInt256;
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

        public static int tryvalidByteArray2UInt256()
        {
            int v = 0;
            try
            {
                v = 2;
                UInt256 data = validUInt256;
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

        public static (int, object?) tryNULL2Ecpoint_1(bool setToNull)
        {
            int v = 0;
            ECPoint? data = (ECPoint)(new byte[33]);
            try
            {
                v = 2;
                if (setToNull) data = null;
            }
            catch
            {
                v = 3;
            }
            finally
            {
                v++;
                if (data is null)
                {
                    v++;
                }
            }
            return (v, data);
        }

        public static (int, object?) tryNULL2Uint160_1(bool setToNull)
        {
            int v = 0;
            UInt160? data = (UInt160)(new byte[20]);
            try
            {
                v = 2;
                if (setToNull) data = null;
            }
            catch
            {
                v = 3;
            }
            finally
            {
                v++;
                if (data is null)
                {
                    v++;
                }
            }
            return (v, data);
        }

        public static (int, object?) tryNULL2Uint256_1(bool setToNull)
        {
            int v = 0;
            UInt256? data = (UInt256)(new byte[32]);
            try
            {
                v = 2;
                if (setToNull) data = null;
            }
            catch
            {
                v = 3;
            }
            finally
            {
                v++;
                if (data is null)
                {
                    v++;
                }
            }
            return (v, data);
        }

        public static (int, object?) tryNULL2Bytestring_1(bool setToNull)
        {
            int v = 0;
            ByteString? data = "123";
            try
            {
                v = 2;
                if (setToNull) data = null;
            }
            catch
            {
                v = 3;
            }
            finally
            {
                v++;
                if (data is null)
                {
                    v++;
                }
            }
            return (v, data);
        }

        public static object throwcall()
        {
            throw new System.Exception();
        }

        public static int tryUncatchableException(bool throwException)
        {
            int v = 0;
            try
            {
                v = 2;
                if (throwException) throw new UncatchableException();
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
    }
}

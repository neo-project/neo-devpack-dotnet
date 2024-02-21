// Copyright (C) 2015-2024 The Neo Project.
//
// Exception.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo;
using Neo.Cryptography.ECC;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using System.ComponentModel;

namespace Exception
{
    [DisplayName("SampleException")]
    [ContractAuthor("core-dev", "core@neo.org")]
    [ContractDescription("A sample contract to demonstrate how to handle exception")]
    [ContractVersion("0.0.1")]
    [ContractSourceCode("https://github.com/neo-project/neo-devpack-dotnet/examples/Example.SmartContract.Exception")]
    [ContractPermission(Permission.WildCard, Method.WildCard)]
    public class SampleException : SmartContract
    {
        [ByteArray("0a0b0c0d0E0F")]
        private static readonly ByteString invalidECpoint = default;
        [ByteArray("024700db2e90d9f02c4f9fc862abaca92725f95b4fddcc8d7ffa538693ecf463a9")]
        private static readonly ByteString byteString2Ecpoint = default;
        [Hash160("NXV7ZhHiyM1aHXwpVsRZC6BwNFP2jghXAq")]
        private static readonly ByteString validUInt160 = default;
        [ByteArray("edcf8679104ec2911a4fe29ad7db232a493e5b990fb1da7af0c7b989948c8925")]
        private static readonly byte[] validUInt256 = default;
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
                Throwcall();
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
                    Throwcall();
                }
                catch
                {
                    v = 3;
                    Throwcall();
                }
                finally
                {
                    Throwcall();
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
                Throwcall();
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
                Throwcall();
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
                ECPoint pubkey = (ECPoint)invalidECpoint;
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

        public static object tryvalidByteString2Ecpoint()
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

        public static object tryinvalidByteArray2UInt160()
        {
            int v = 0;
            try
            {
                v = 2;
                UInt160 data = (UInt160)invalidECpoint;
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

        public static object tryvalidByteArray2UInt160()
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

        public static object tryinvalidByteArray2UInt256()
        {
            int v = 0;
            try
            {
                v = 2;
                UInt256 data = (UInt256)invalidECpoint;
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

        public static object tryvalidByteArray2UInt256()
        {
            int v = 0;
            try
            {
                v = 2;
                UInt256 data = (UInt256)validUInt256;
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

        public static (int, object) tryNULL2Ecpoint_1()
        {
            int v = 0;
            ECPoint data = (ECPoint)(new byte[33]);
            try
            {
                v = 2;
                data = (ECPoint)null;
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

        public static (int, object) tryNULL2Uint160_1()
        {
            int v = 0;
            UInt160 data = (UInt160)(new byte[20]);
            try
            {
                v = 2;
                data = (UInt160)null;
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

        public static (int, object) tryNULL2Uint256_1()
        {
            int v = 0;
            UInt256 data = (UInt256)(new byte[32]);
            try
            {
                v = 2;
                data = (UInt256)null;
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

        public static (int, object) tryNULL2Bytestring_1()
        {
            int v = 0;
            ByteString data = "123";
            try
            {
                v = 2;
                data = (ByteString)null;
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

        private static object Throwcall()
        {
            throw new System.Exception();
        }

        public static object TryUncatchableException()
        {
            int v = 0;
            try
            {
                v = 2;
                throw new UncatchableException();
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

using System;
using Neo;

namespace NeoTestHarness
{
    public static class NativeContracts
    {
        static Lazy<UInt160> neoToken = new Lazy<UInt160>(() => UInt160.Parse("0xef4073a0f2b305a38ec4050e4d3d28bc40ea63f5"));
        public static UInt160 NeoToken => neoToken.Value;

        static Lazy<UInt160> gasToken = new Lazy<UInt160>(() => UInt160.Parse("0xd2a4cff31913016155e38e474a2c06d08be276cf"));
        public static UInt160 GasToken => gasToken.Value;
    }
}
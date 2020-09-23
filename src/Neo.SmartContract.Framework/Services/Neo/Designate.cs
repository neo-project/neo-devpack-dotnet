#pragma warning disable CS0626

namespace Neo.SmartContract.Framework.Services.Neo
{
    [Contract("0x3c05b488bf4cf699d0631bf80190896ebbf38c3b")]
    public class Designate
    {
        public static extern string Name { get; }
        public static extern byte[][] GetDesignatedByRole(DesignateRole role);
    }
}

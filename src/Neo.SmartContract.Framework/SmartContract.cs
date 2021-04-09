using System.Runtime.CompilerServices;

namespace Neo.SmartContract.Framework
{
    public abstract class SmartContract
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
#pragma warning disable IDE1006
        public extern static void _initialize();
#pragma warning restore IDE1006
    }
}

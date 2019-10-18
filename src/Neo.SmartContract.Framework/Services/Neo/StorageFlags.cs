using System;

namespace Neo.SmartContract.Framework.Services.Neo
{
    [Flags]
    public enum StorageFlags : byte
    {
        None = 0x00,
        Constant = 0x01
    }
}

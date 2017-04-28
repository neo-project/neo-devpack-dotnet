using System;

namespace AntShares.SmartContract.Framework.Services.AntShares
{
    [Flags]
    public enum StorageContext : byte
    {
        Current = 1,
        CallingContract = 2,
        EntryContract = 4
    }
}

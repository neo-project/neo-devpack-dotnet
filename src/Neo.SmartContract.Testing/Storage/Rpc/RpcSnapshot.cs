using Neo.Persistence;
using System;
using System.Collections.Generic;

namespace Neo.SmartContract.Testing.Storage.Rpc;

internal class RpcSnapshot : ISnapshot
{
    /// <summary>
    /// Return true if the storage has changes
    /// </summary>
    public bool IsDirty { get; private set; } = false;

    /// <summary>
    /// Store
    /// </summary>
    public RpcStore Store { get; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="store">Store</param>
    /// <param name="innerData">Inner data</param>
    public RpcSnapshot(RpcStore store)
    {
        Store = store;
    }

    public void Commit()
    {
        if (IsDirty)
        {
            throw new NotImplementedException();
        }
    }

    public void Delete(byte[] key)
    {
        IsDirty = true;
    }


    public void Put(byte[] key, byte[] value)
    {
        IsDirty = true;
    }

    public IEnumerable<(byte[] Key, byte[] Value)> Seek(byte[] keyOrPrefix, SeekDirection direction = SeekDirection.Forward)
    {
        return Store.Seek(keyOrPrefix, direction);
    }

    public byte[]? TryGet(byte[] key)
    {
        return Store.TryGet(key);
    }

    public bool Contains(byte[] key)
    {
        return TryGet(key) != null;
    }

    public void Dispose() { }
}

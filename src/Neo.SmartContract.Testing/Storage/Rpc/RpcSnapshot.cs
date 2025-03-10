// Copyright (C) 2015-2025 The Neo Project.
//
// RpcSnapshot.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.Persistence;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Neo.SmartContract.Testing.Storage.Rpc;

internal class RpcSnapshot : IStoreSnapshot
{
    /// <summary>
    /// Return true if the storage has changes
    /// </summary>
    public bool IsDirty { get; private set; } = false;

    /// <summary>
    /// Store
    /// </summary>
    public IStore Store { get; }

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

    public IEnumerable<(byte[] Key, byte[] Value)> Seek(byte[]? keyOrPrefix, SeekDirection direction = SeekDirection.Forward)
    {
        return Store.Seek(keyOrPrefix, direction);
    }

    public bool TryGet(byte[] key, [NotNullWhen(true)] out byte[]? value)
    {
        return Store.TryGet(key, out value);
    }

    public byte[]? TryGet(byte[] key)
    {
        if (Store.TryGet(key, out var value))
            return value;

        return null;
    }

    public bool Contains(byte[] key)
    {
        return TryGet(key) != null;
    }

    public void Dispose() { }
}

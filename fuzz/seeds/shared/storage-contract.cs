using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services;
using System.ComponentModel;

public class Contract : SmartContract
{
    [DisplayName("touchSeed")]
    public static void TouchSeed()
    {
        var map = new StorageMap(Storage.CurrentContext, "fz");
        map.Put("seed", "neo");
    }

    [Safe]
    [DisplayName("hasSeed")]
    public static bool HasSeed()
    {
        var map = new StorageMap(Storage.CurrentReadOnlyContext, "fz");
        return map.Get("seed") != null;
    }
}

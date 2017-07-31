namespace Neo.SmartContract.Framework.Services.Neo
{
    public class Asset
    {
        public extern byte[] AssetId
        {
            [Syscall("Neo.Asset.GetAssetId")]
            get;
        }

        public extern byte AssetType
        {
            [Syscall("Neo.Asset.GetAssetType")]
            get;
        }

        public extern long Amount
        {
            [Syscall("Neo.Asset.GetAmount")]
            get;
        }

        public extern long Available
        {
            [Syscall("Neo.Asset.GetAvailable")]
            get;
        }

        public extern byte Precision
        {
            [Syscall("Neo.Asset.GetPrecision")]
            get;
        }

        public extern byte[] Owner
        {
            [Syscall("Neo.Asset.GetOwner")]
            get;
        }

        public extern byte[] Admin
        {
            [Syscall("Neo.Asset.GetAdmin")]
            get;
        }

        public extern byte[] Issuer
        {
            [Syscall("Neo.Asset.GetIssuer")]
            get;
        }

        [Syscall("Neo.Asset.Create")]
        public static extern Asset Create(byte asset_type, string name, long amount, byte precision, byte[] owner, byte[] admin, byte[] issuer);

        [Syscall("Neo.Asset.Renew")]
        public extern uint Renew(byte years);
    }
}

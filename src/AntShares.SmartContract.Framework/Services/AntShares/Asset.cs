namespace AntShares.SmartContract.Framework.Services.AntShares
{
    public class Asset
    {
        public extern byte[] AssetId
        {
            [Syscall("AntShares.Asset.GetAssetId")]
            get;
        }

        public extern byte AssetType
        {
            [Syscall("AntShares.Asset.GetAssetType")]
            get;
        }

        public extern long Amount
        {
            [Syscall("AntShares.Asset.GetAmount")]
            get;
        }

        public extern long Available
        {
            [Syscall("AntShares.Asset.GetAvailable")]
            get;
        }

        public extern byte Precision
        {
            [Syscall("AntShares.Asset.GetPrecision")]
            get;
        }

        public extern byte[] Owner
        {
            [Syscall("AntShares.Asset.GetOwner")]
            get;
        }

        public extern byte[] Admin
        {
            [Syscall("AntShares.Asset.GetAdmin")]
            get;
        }

        public extern byte[] Issuer
        {
            [Syscall("AntShares.Asset.GetIssuer")]
            get;
        }

        [Syscall("AntShares.Asset.Create")]
        public static extern Asset Create(byte asset_type, string name, long amount, byte precision, byte[] owner, byte[] admin, byte[] issuer);

        [Syscall("AntShares.Asset.Renew")]
        public extern uint Renew(byte years);
    }
}

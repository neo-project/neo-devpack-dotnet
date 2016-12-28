namespace AntShares.SmartContract.Framework.Services.AntShares
{
    public class Asset : Transaction
    {
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

        public extern byte[] Issuer
        {
            [Syscall("AntShares.Asset.GetIssuer")]
            get;
        }

        public extern byte[] Admin
        {
            [Syscall("AntShares.Asset.GetAdmin")]
            get;
        }
    }
}

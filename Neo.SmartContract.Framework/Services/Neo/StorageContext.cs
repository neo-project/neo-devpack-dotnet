namespace Neo.SmartContract.Framework.Services.Neo
{
    public class StorageContext
    {
        public byte[] this[byte[] key]
        {
            get
            {
                return Storage.Get(this,key);
            }
            set
            {
                Storage.Put(this, key, value);
            }
        }

        public byte[] this[string key]
        {
            get
            {
                return Storage.Get(this, key);
            }
            set
            {
                Storage.Put(this, key, value);
            }
        }
    }
}

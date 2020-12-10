using Neo.SmartContract;
using Neo.Wallets;
using Neo.Wallets.NEP6;
using System;
using System.Collections.Generic;

namespace Neo.TestingEngine
{
    public class TestWallet : NEP6Wallet
    {
        private Dictionary<UInt160, WalletAccount> accounts;

        public TestWallet(UInt160 scriptHash) : base("", "TestWallet")
        {
            this.accounts = new Dictionary<UInt160, WalletAccount>()
            {
                { scriptHash, new TestAccount(scriptHash) }
            };
        }

        public override bool ChangePassword(string oldPassword, string newPassword)
        {
            return false;
        }

        public override bool Contains(UInt160 scriptHash)
        {
            return this.accounts.ContainsKey(scriptHash);
        }

        public override WalletAccount CreateAccount(byte[] privateKey)
        {
            throw new NotImplementedException();
        }

        public override WalletAccount CreateAccount(Contract contract, KeyPair key = null)
        {
            throw new NotImplementedException();
        }

        public override WalletAccount CreateAccount(UInt160 scriptHash)
        {
            var account = new TestAccount(scriptHash);
            this.accounts[scriptHash] = account;
            return account;
        }

        public override bool DeleteAccount(UInt160 scriptHash)
        {
            if (!this.accounts.ContainsKey(scriptHash))
            {
                return false;
            }
            return accounts.Remove(scriptHash);
        }

        public override WalletAccount GetAccount(UInt160 scriptHash)
        {
            if (!this.accounts.ContainsKey(scriptHash))
            {
                return null;
            }
            return accounts[scriptHash];
        }

        public override IEnumerable<WalletAccount> GetAccounts()
        {
            return this.accounts.Values;
        }

        public override bool VerifyPassword(string password)
        {
            return true;
        }
    }
}

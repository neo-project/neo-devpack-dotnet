using System;
using Neo;
using Neo.Wallets;
using Neo.Wallets.NEP6;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length < 3)
        {
            Console.WriteLine("Usage: dotnet run <wallet_path> <password> <wif_key>");
            return;
        }

        var walletPath = args[0];
        var password = args[1];
        var wifKey = args[2];

        try
        {
            // Create new wallet
            var wallet = new NEP6Wallet(walletPath, password, ProtocolSettings.Default);
            
            // Import account from WIF
            var account = wallet.Import(wifKey);
            account.IsDefault = true;
            
            // Save wallet
            wallet.Save();
            
            Console.WriteLine($"Wallet created successfully: {walletPath}");
            Console.WriteLine($"Account imported: {account.Address}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
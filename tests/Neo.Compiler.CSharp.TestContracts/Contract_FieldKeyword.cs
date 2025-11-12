using Neo.SmartContract.Framework;

namespace Neo.Compiler.CSharp.TestContracts;

public class Contract_FieldKeyword : SmartContract.Framework.SmartContract
{
    private sealed class Wallet
    {
        public int RunningTotal
        {
            get => field;
            set => field = value >= 0 ? field + value : field;
        }

        public bool HasSeenPositive
        {
            get => field;
            set => field = field || value;
        }
    }

    private static int Balance
    {
        get => field;
        set => field = value >= 0 ? value : 0;
    }

    private static int LastPositive
    {
        get => field;
        set => field = value > 0 ? value : field;
    }

    private static int LastNonZero
    {
        get => field;
        set => field = value == 0 ? field : value;
    }

    public static int Update(int value)
    {
        Balance = value;
        return Balance;
    }

    public static int RecordLastPositiveSequence(int first, int second)
    {
        LastPositive = first;
        LastPositive = second;
        return LastPositive;
    }

    public static int RecordLastNonZeroSequence(int first, int second)
    {
        LastNonZero = first;
        LastNonZero = second;
        return LastNonZero;
    }

    public static int AccumulateWallet(int firstDeposit, int secondDeposit)
    {
        var wallet = new Wallet();
        wallet.RunningTotal = firstDeposit;
        wallet.RunningTotal = secondDeposit;
        return wallet.RunningTotal;
    }

    public static bool TrackPositiveWallet(bool first, bool second)
    {
        var wallet = new Wallet();
        wallet.HasSeenPositive = first;
        wallet.HasSeenPositive = second;
        return wallet.HasSeenPositive;
    }
}

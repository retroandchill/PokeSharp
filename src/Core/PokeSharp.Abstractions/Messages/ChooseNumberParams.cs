namespace PokeSharp.Messages;

public record ChooseNumberParams
{
    private readonly int _maxDigits;
    public int MaxDigits
    {
        get => _maxDigits > 0 ? _maxDigits : Math.Max(NumDigits(MaxNumber), NumDigits(MinNumber));
        init => _maxDigits = Math.Max(value, 1);
    }

    public int MinNumber
    {
        get
        {
            int ret;
            if (_maxDigits > 0)
            {
                ret = -(int)Math.Pow(10, _maxDigits - 1);
            }
            else
            {
                ret = field;
            }
            if (!NegativeAllowed && ret < 0)
                ret = 0;
            return ret;
        }
        private init;
    }

    public int MaxNumber
    {
        get
        {
            int ret;
            if (_maxDigits > 0)
            {
                ret = (int)Math.Pow(10, _maxDigits - 1) - 1;
            }
            else
            {
                ret = field;
            }
            if (!NegativeAllowed && ret < 0)
                ret = 0;
            return ret;
        }
        private init;
    }

    public (int Min, int Max) Range
    {
        get => (MinNumber, MaxNumber);
        init
        {
            var (minNumber, maxNumber) = value;
            if (minNumber > maxNumber)
            {
                maxNumber = minNumber;
            }

            MaxDigits = 0;
            (MinNumber, MaxNumber) = (minNumber, maxNumber);
        }
    }

    public string? MessageSkin { get; init; }

    public string? Skin { get; init; }

    public bool NegativeAllowed { get; init; }

    public int InitialNumber { get; init; }

    private int? _cancelNumber;

    public int CancelNumber
    {
        get => _cancelNumber ?? InitialNumber;
        init => _cancelNumber = value;
    }

    public void ResetCancelNumber() => _cancelNumber = null;

    private static int NumDigits(int number)
    {
        var ans = 1;
        number = Math.Abs(number);
        while (number >= 10)
        {
            ans++;
            number /= 10;
        }

        return ans;
    }
}

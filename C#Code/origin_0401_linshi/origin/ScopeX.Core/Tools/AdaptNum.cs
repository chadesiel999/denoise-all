using NPOI.HSSF.Record.Chart;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static NPOI.HSSF.Util.HSSFColor;
using static NPOI.POIFS.Crypt.CryptoFunctions;

namespace ScopeX.Core.Tools;
internal class AdaptNum
{
    public AdaptNum(UInt32 digits, Action onPropertyChanged)
    {
        Digits = digits;
        OnPropertyChanged = onPropertyChanged;
    }

    protected Action OnPropertyChanged;

    public UInt32 Digits
    {
        get;
        protected set;
    }

    private Int64 _Value = 0;
    public Int64 Value
    {
        get => _Value;
        set => Set(Validate(value));
    }

    public Int64 Max
    {
        get;
        set;
    }

    public Int64 Min
    {
        get;
        set;
    }

    public Int64 Stp
    {
        get;
        set;
    }

    private Int64 Validate(Int64 value)
    {
        var stp = GetAdaptStep(value, Digits, Stp);
        //value = (Int64)Math.Round((Double)value / Stp, MidpointRounding.AwayFromZero) * Stp;
        return (Int64)Math.Round(value / (Double)stp * stp, MidpointRounding.AwayFromZero);
    }

    private void Set(Int64 value)
    {
        if (value > Max)
        {
            value = Max;
        }
        else if (value < Min)
        {
            value = Min;
        }

        if (value != _Value)
        {
            _Value = value;
            OnPropertyChanged();
        }
    }

    public void Increase(Int64 delta)
    {
        Set(GetNext(_Value, delta, Digits, Stp));
    }
    public void AdjValue(Int64 step)
    {
        if ((_Value >= 1E6) && (_Value < 1E9))
        {
            _Value += step * (Int32)1E3;
        }
        else if ((_Value >= 1E9) && (_Value < 1E12))
        {
            _Value += step * (Int32)1E6;
        }
        else if (_Value >= 1E12)
        {
            _Value += step * (Int32)1E9;
        }
        else
        {
            _Value += step * 200;
        }
        Set(_Value);
    }
    private static Int64 GetAdaptStep(Int64 value, UInt32 digits, Int64 step)
    {
        value = Math.Abs(value);
        if (value >= 10)
        {
            Int64 n = (Int64)Math.Log10(value) - digits;
            if (n < 0)
            {
                n = 0;
            }

            value = (Int64)Math.Pow(10, n);
        }
        else
        {
            value = 1;
        }
        return value < step ? step : value;
    }

    public static Int64 GetNext(Int64 value, Int64 delta, UInt32 digits, Int64 step)
    {
        var v = value;
        if (delta * value < 0)
        {
            v = Math.Abs(value) - 1;
        }

        value += GetAdaptStep(v, digits, step) * delta;

        var stp = GetAdaptStep(value, digits, step);

        //value = (Int64)Math.Round((Double)value / Stp, MidpointRounding.AwayFromZero) * Stp;
        return (Int64)Math.Round(value / (Double)stp * stp, MidpointRounding.AwayFromZero);
    }
}

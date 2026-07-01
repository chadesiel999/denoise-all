using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScopeX.Core;

namespace ScopeX.U2;

public partial class DigitalInfo : ChnlBadgeInfo
{
    public DigitalInfo(DigitalPrsnt cp) : base(cp, typeof(DigitalForm))
    {
        InitializeComponent();
    }

    public DigitalPrsnt Presenter
    {
        get => (DigitalPrsnt)InternalPrsnt;
        set => InternalPrsnt = value;
    }

    protected override void Update(Object prsnt, String propertyName)
    {
        switch (propertyName)
        {
            case "ActiveBit":
                DataSource = new List<Object>() { BitsStatusToString(0), BitsStatusToString(1), BitsStatusToString(2) };
                break;
        }
    }

    protected override void UpdateView()
    {
        DataSource = new List<Object>() { BitsStatusToString(0), BitsStatusToString(1), BitsStatusToString(2) };
    }

    private String BitsStatusToString(Int32 group)
    {
        //StringBuilder sb = new(32);
        //for (Int32 i = 0; i < 8; i++)
        //{
        //    sb.Append(Presenter.GetActiveAt(i + group * 8) ? '1' : '0');
        //    if (i == 3)
        //        sb.Append(',');
        //}

        if (group >= Presenter.BitLength / 16)
        {
            return "";
        }

        Int32 digits = 0;
        for (Int32 i = 0; i < 16; i++)
        {
            digits |= (Presenter.GetActiveAt(i + group * 16) ? 1 : 0) << i;
        }

        return "0x" + digits.ToString("X4");
    }
}

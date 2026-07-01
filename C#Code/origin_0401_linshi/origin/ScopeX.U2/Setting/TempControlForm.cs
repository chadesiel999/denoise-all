using PdfSharpCore.Drawing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScopeX.ComModel;
using ScopeX.Core;
using ScopeX.Core.Tools;
//using ScopeX.Default;
//using ScopeX.Helper;
using ScopeX.UserControls;
using ScopeX.UserControls.Style;
using Veldrid.OpenGLBinding;
using ScopeX.Controls.Common.Helper;

namespace ScopeX.U2
{
    public partial class TempControlForm : FloatForm, ITempView
    {
        public TempControlForm()
        {
            InitializeComponent();
        }

        private void InitCbxFansName()
        {
            List<KeyValuePair<Int32, String>> source = new();
            for (Int32 i = 0; i < TempCtrlPrsnt.FansName.Length; i++)
            {
                source.Add(new KeyValuePair<Int32, String>(i, TempCtrlPrsnt.FansName[i]));
            }
            CbxFansName.DataSource = source;
            CbxFansName.DisplayMember = "Value";
            CbxFansName.ValueMember = "Key";

            CbxFansName.SelectedIndexChanged += (_, _) =>
            {
                if (!_ArgToCtrl)
                {
                    TempCtrlPrsnt.CurFanNameId = CbxFansName.SelectedIndex;
                }
            };
        }

        private void InitNebFanSpeed()
        {
            NebFanSpeed.AddClicked = (o, e) => TempCtrlPrsnt.CurFanSpeed += e.Step;
            NebFanSpeed.SubClicked = (o, e) => TempCtrlPrsnt.CurFanSpeed += e.Step;
            NebFanSpeed.StringFormatFunc = (value) => $"{TempCtrlPrsnt.CurFanSpeed}";
            NebFanSpeed.EditValueChicked = (a, b) =>
            {
                NumberKeybordForm nkf = new NumberKeybordForm().UniformInitKeyBoard(this);
                nkf.NumberKeyboard.UseSI = false;
                Action<Double> onokclickeventaction = new((data) => TempCtrlPrsnt.CurFanSpeed = (Int32)data);

                nkf.SetKeyBoardValue(LblFanSpeed.Text, QuantityUnit.Percent.ToUnitString(), 3, onokclickeventaction,
                    TempCtrlPrsnt.CurFanSpeed, TempCtrlPrsnt.SpeedMax, TempCtrlPrsnt.SpeedMin);

                nkf.ShowDialogByPosition();
            };
        }

        private Boolean _ArgToCtrl = false;

        protected new Boolean DesignMode
        {
            get
            {
                Boolean rtnflag = false;
#if DEBUG
                rtnflag = DesignTimeHelper.InDesignMode(this);
#endif
                return rtnflag;
            }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;
                return cp;
            }
        }

        public TempCtrlPrsnt TempCtrlPrsnt { get; set; }

        ITempPrsnt IView<ITempPrsnt>.Presenter
        {
            get => TempCtrlPrsnt;
            set => TempCtrlPrsnt = (TempCtrlPrsnt)value;
        }

        public void UpdateView(Object prsnt, String propertyName)
        {
            if (String.IsNullOrEmpty(propertyName))
            {
                UpdateAllView();
                return;
            }

            _ArgToCtrl = true;
            switch (propertyName)
            {
                case nameof(TempCtrlPrsnt.AutoCtrlFans):
                    ChkAutoCtrlFansEnable.Checked = TempCtrlPrsnt.AutoCtrlFans;
                    break;
                case nameof(TempCtrlPrsnt.AutoCaliSystem):
                    ChkAutoCali.Checked = TempCtrlPrsnt.AutoCaliSystem;
                    break;
                case nameof(TempCtrlPrsnt.CurFanNameId):
                    CbxFansName.SelectedIndex = TempCtrlPrsnt.CurFanNameId;
                    NebFanSpeed.UpdateValueString();
                    break;
                case nameof(TempCtrlPrsnt.CurFanSpeed):
                    NebFanSpeed.UpdateValueString();
                    break;
            }
            _ArgToCtrl = false;
        }

        private void UpdateAllView()
        {
            _ArgToCtrl = true;
            ChkAutoCtrlFansEnable.Checked = TempCtrlPrsnt.AutoCtrlFans;
            ChkAutoCali.Checked = TempCtrlPrsnt.AutoCaliSystem;
            CbxFansName.SelectedIndex = TempCtrlPrsnt.CurFanNameId;
            NebFanSpeed.UpdateValueString();
            _ArgToCtrl = false;
        }

        private void TempControlForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            TempCtrlPrsnt.TryRemoveView(this);
        }

        private void TimerUpdateTemp_Tick(object sender, EventArgs e)
        {
            var tempinfos = TempCtrlPrsnt.Temp;
            var tempnames = tempinfos.Keys.ToList();
            tempnames.Sort();

            LvTempInfo.BeginUpdate();

            for (Int32 i = 0; i < tempnames.Count; i++)
            {
                if (i == LvTempInfo.Items.Count)
                {
                    LvTempInfo.Items.Add(new ListViewItem(new String[LvTempInfo.Columns.Count]));
                }
                LvTempInfo.Items[i].SubItems[0].Text = i.ToString();
                LvTempInfo.Items[i].SubItems[1].Text = tempnames[i];
                LvTempInfo.Items[i].SubItems[2].Text = tempinfos[tempnames[i]].ToString("0.00");
            }

            while (tempnames.Count < LvTempInfo.Items.Count)
            {
                LvTempInfo.Items.RemoveAt(tempnames.Count);
            }
            LvTempInfo.Sort();
            LvTempInfo.EndUpdate();
        }

        private void TempControlForm_Load(object sender, EventArgs e)
        {
            IsShowHelp = false;
            InitCbxFansName();
            InitNebFanSpeed();
            UpdateAllView();
            TimerUpdateTemp.Start();
        }

        private void ChkAutoCtrlFansEnable_Click(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                TempCtrlPrsnt.AutoCtrlFans = ChkAutoCtrlFansEnable.Checked;
            }
        }

        private void ChkAutoCali_Click(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                TempCtrlPrsnt.AutoCaliSystem = ChkAutoCali.Checked;
            }
        }
    }
}

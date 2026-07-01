// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/4/15</date>

namespace ScopeX.U2
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using ScopeX.ComModel;
    using ScopeX.Core;
    using ScopeX.Core.Tools;
    using ScopeX.UserControls.Style;
    using ScopeX.Controls.Common.Default;
    using ScopeX.Controls.Common.Helper;

    public partial class MeasRefLevelPage : UserControl, IMeasView, IStylize
    {


        private readonly Int32 _PxIndex;

        private Boolean _ArgToCtrl;

        public MeasRefLevelPage(Int32 pxIndex)
        {
            InitializeComponent();
            _PxIndex = pxIndex;
        }

        [Browsable(false)]
        public StyleFlag StyleFlags { get; set; } = StyleFlag.None;

        [Description("是否风格化"), Browsable(true), DefaultValue(typeof(Boolean)), Category(Const.Category)]
        public Boolean StylizeFlag { get; set; } = false;

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

        public MeasPrsnt Presenter
        {
            get => (MeasPrsnt)(ParentForm as IMeasView).Presenter;
            set => (ParentForm as IMeasView).Presenter = value;
        }

        IMeasPrsnt IView<IMeasPrsnt>.Presenter
        {
            get => Presenter;
            set => Presenter = (MeasPrsnt)value;
        }

        public void UpdateView(Object prsnt, String propertyName)
        {
            if (String.IsNullOrEmpty(propertyName))
            {
                UpdateView();
                return;
            }

            var item = Presenter[_PxIndex];
            _ArgToCtrl = true;
            switch (propertyName)
            {
                case nameof(item.RefStandard):
                    CbxRefStd.SelectIndex = (Int32)item.RefStandard;
                    break;
                case nameof(item.RefUnit):
                    RdoRefFormat.ChoosedButtonIndex = (Int32)item.RefUnit;
                    NebHighThrold.UpdateValueString();
                    NebLowThrold.UpdateValueString();
                    NebMidThrold.UpdateValueString();
                    PbxRefLevel.Invalidate();
                    break;
                case "RefLevel":
                    NebHighThrold.UpdateValueString();
                    NebLowThrold.UpdateValueString();
                    NebMidThrold.UpdateValueString();
                    PbxRefLevel.Invalidate();
                    break;
            }
            _ArgToCtrl = false;
        }

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                _ArgToCtrl = true;
                CbxRefStd.SelectIndex = (Int32)Presenter[_PxIndex].RefStandard;
                RdoRefFormat.ChoosedButtonIndex = (Int32)Presenter[_PxIndex].RefUnit;
                NebHighThrold.UpdateValueString();
                NebLowThrold.UpdateValueString();
                NebMidThrold.UpdateValueString();

                PbxRefLevel.Invalidate();
                _ArgToCtrl = false;
            }
        }

        public override void Refresh()
        {
            base.Refresh();
            UpdateView();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            LoadNumberKeyBoard();
            UpdateView();
        }

        private void LoadNumberKeyBoard()
        {
            ControlsHotKnob.Default.InitHotKnob(NebHighThrold);
            NebHighThrold.EditValueOnceClicked += (_, _) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter[_PxIndex], NebHighThrold);
            };
            NebHighThrold.AddClicked = (o, e) => Presenter[_PxIndex].AdjHighThrold(e.Step);
            NebHighThrold.SubClicked = (o, e) => Presenter[_PxIndex].AdjHighThrold(e.Step);
            NebHighThrold.StringFormatFunc = (_) => ThroldToString(Presenter[_PxIndex].RefUnit == MeasureTopBaseRefUnit.Percent ? Presenter[_PxIndex].HighThrold : Presenter[_PxIndex].HighAbsoluteThrold, Presenter[_PxIndex].ThroldUnit);
            NebHighThrold.EditValueChicked += (a, b) => InitTopLevelKeyBoard().ShowDialogByPosition();

            ControlsHotKnob.Default.InitHotKnob(NebLowThrold);
            NebLowThrold.EditValueOnceClicked += (_, _) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter[_PxIndex], NebLowThrold);
            };
            NebLowThrold.AddClicked = (o, e) => Presenter[_PxIndex].AdjLowThrold(e.Step);
            NebLowThrold.SubClicked = (o, e) => Presenter[_PxIndex].AdjLowThrold(e.Step);
            NebLowThrold.StringFormatFunc = (_) => ThroldToString(Presenter[_PxIndex].RefUnit == MeasureTopBaseRefUnit.Percent ? Presenter[_PxIndex].LowThrold : Presenter[_PxIndex].LowAbsoluteThrold, Presenter[_PxIndex].ThroldUnit);
            NebLowThrold.EditValueChicked += (a, b) => InitBaseLevelKeyBoard().ShowDialogByPosition();

            ControlsHotKnob.Default.InitHotKnob(NebMidThrold);
            NebMidThrold.EditValueOnceClicked += (_, _) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter[_PxIndex], NebMidThrold);
            };
            NebMidThrold.AddClicked = (o, e) => Presenter[_PxIndex].AdjMidThrold(e.Step);
            NebMidThrold.SubClicked = (o, e) => Presenter[_PxIndex].AdjMidThrold(e.Step);
            NebMidThrold.StringFormatFunc = (_) => ThroldToString(Presenter[_PxIndex].RefUnit == MeasureTopBaseRefUnit.Percent ? Presenter[_PxIndex].MidThrold : Presenter[_PxIndex].MidAbsoluteThrold, Presenter[_PxIndex].ThroldUnit);
            NebMidThrold.EditValueChicked += (a, b) => InitMidLevelKeyBoard().ShowDialogByPosition();
        }

        private NumberKeybordForm InitTopLevelKeyBoard()
        {
            var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebHighThrold);

            nkf.NumberKeyboard.Unit = Presenter[_PxIndex].ThroldUnit.Unit;
            nkf.NumberKeyboard.MaxValue = Quantity.ConvertByPrefix(Presenter[_PxIndex].RefUnit == MeasureTopBaseRefUnit.Percent ? Presenter[_PxIndex].MaxThrold : Presenter[_PxIndex].MaxAbsoluteThrold, Presenter[_PxIndex].ThroldUnit.Prefix);
            nkf.NumberKeyboard.MinValue = Quantity.ConvertByPrefix(Presenter[_PxIndex].RefUnit == MeasureTopBaseRefUnit.Percent ? Presenter[_PxIndex].MinThrold + Presenter[_PxIndex].GapThrold * 2 : Presenter[_PxIndex].MinAbsoluteThrold, Presenter[_PxIndex].ThroldUnit.Prefix);
            nkf.NumberKeyboard.DefaultValue = Quantity.ConvertByPrefix(Presenter[_PxIndex].RefUnit == MeasureTopBaseRefUnit.Percent ? Presenter[_PxIndex].HighThrold : Presenter[_PxIndex].HighAbsoluteThrold, Presenter[_PxIndex].ThroldUnit.Prefix);
            nkf.NumberKeyboard.DecimalNumber = 12;
            nkf.Title = LblTopLv.Text;

            nkf.NumberKeyboard.OkClickEvent += (sender, args) =>
            {
                var data = Quantity.ConvertByPrefix(args.Data, Prefix.Empty, Presenter[_PxIndex].ThroldUnit.Prefix);
                if (Presenter[_PxIndex].RefUnit == MeasureTopBaseRefUnit.Percent)
                {
                    Presenter[_PxIndex].HighThrold = Convert.ToInt32(data);
                }
                else
                {
                    Presenter[_PxIndex].HighAbsoluteThrold = data;
                }

                nkf.Close();
            };
            return nkf;
        }

        private NumberKeybordForm InitMidLevelKeyBoard()
        {
            var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebMidThrold);

            nkf.NumberKeyboard.Unit = Presenter[_PxIndex].ThroldUnit.Unit;
            nkf.NumberKeyboard.MaxValue = Quantity.ConvertByPrefix(Presenter[_PxIndex].RefUnit == MeasureTopBaseRefUnit.Percent ? Presenter[_PxIndex].MaxThrold - Presenter[_PxIndex].GapThrold : Presenter[_PxIndex].MaxAbsoluteThrold, Presenter[_PxIndex].ThroldUnit.Prefix);
            nkf.NumberKeyboard.MinValue = Quantity.ConvertByPrefix(Presenter[_PxIndex].RefUnit == MeasureTopBaseRefUnit.Percent ? Presenter[_PxIndex].MinThrold + Presenter[_PxIndex].GapThrold : Presenter[_PxIndex].MinAbsoluteThrold, Presenter[_PxIndex].ThroldUnit.Prefix);
            nkf.NumberKeyboard.DefaultValue = Quantity.ConvertByPrefix(Presenter[_PxIndex].RefUnit == MeasureTopBaseRefUnit.Percent ? Presenter[_PxIndex].MidThrold : Presenter[_PxIndex].MidAbsoluteThrold, Presenter[_PxIndex].ThroldUnit.Prefix);
            nkf.NumberKeyboard.DecimalNumber = 12;
            nkf.Title = LblMedianLv.Text;

            nkf.NumberKeyboard.OkClickEvent += (sender, args) =>
            {
                var data = Quantity.ConvertByPrefix(args.Data, Prefix.Empty, Presenter[_PxIndex].ThroldUnit.Prefix);
                if (Presenter[_PxIndex].RefUnit == MeasureTopBaseRefUnit.Percent)
                {
                    Presenter[_PxIndex].MidThrold = Convert.ToInt32(data);
                }
                else
                {
                    Presenter[_PxIndex].MidAbsoluteThrold = data;
                }
                nkf.Close();
            };
            return nkf;
        }

        private NumberKeybordForm InitBaseLevelKeyBoard()
        {
            var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebLowThrold);

            nkf.NumberKeyboard.Unit = Presenter[_PxIndex].ThroldUnit.Unit;
            nkf.NumberKeyboard.MaxValue = Quantity.ConvertByPrefix(Presenter[_PxIndex].RefUnit == MeasureTopBaseRefUnit.Percent ? Presenter[_PxIndex].MaxThrold - Presenter[_PxIndex].GapThrold * 2 : Presenter[_PxIndex].MaxAbsoluteThrold, Presenter[_PxIndex].ThroldUnit.Prefix);
            nkf.NumberKeyboard.MinValue = Quantity.ConvertByPrefix(Presenter[_PxIndex].RefUnit == MeasureTopBaseRefUnit.Percent ? Presenter[_PxIndex].MinThrold : Presenter[_PxIndex].MinAbsoluteThrold, Presenter[_PxIndex].ThroldUnit.Prefix);
            nkf.NumberKeyboard.DefaultValue = Quantity.ConvertByPrefix(Presenter[_PxIndex].RefUnit == MeasureTopBaseRefUnit.Percent ? Presenter[_PxIndex].LowThrold : Presenter[_PxIndex].LowAbsoluteThrold, Presenter[_PxIndex].ThroldUnit.Prefix);
            nkf.NumberKeyboard.DecimalNumber = 12;
            nkf.Title = LblBaseLv.Text;

            nkf.NumberKeyboard.OkClickEvent += (sender, args) =>
            {
                var data = Quantity.ConvertByPrefix(args.Data, Prefix.Empty, Presenter[_PxIndex].ThroldUnit.Prefix);
                if (Presenter[_PxIndex].RefUnit == MeasureTopBaseRefUnit.Percent)
                {
                    Presenter[_PxIndex].LowThrold = Convert.ToInt32(data);
                }
                else
                {
                    Presenter[_PxIndex].LowAbsoluteThrold = data;
                }
                nkf.Close();
            };
            return nkf;
        }
        private static String ThroldToString(Double data, (Prefix p, String n) unit)
        {
            return new Quantity(data, unit.p, unit.n).ToString("##0.###", true);
        }

        private void CbxRefStd_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter[_PxIndex].RefStandard = (MeasureTopBaseRef)CbxRefStd.SelectIndex;
            }
        }

        private void RdoRefFormat_IndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter[_PxIndex].RefUnit = (MeasureTopBaseRefUnit)RdoRefFormat.ChoosedButtonIndex;
            }
        }

        private void PbxRefLevel_Paint(object sender, PaintEventArgs e)
        {
            PictureBox pic = sender as PictureBox;

            using Pen pen = new(Color.White, 3)
            {
                DashStyle = System.Drawing.Drawing2D.DashStyle.Dot
            };
            using Font f = new("Arial", 9f);

            if (pic.Image != null)
            {
                if (Presenter[_PxIndex].RefUnit == MeasureTopBaseRefUnit.Percent)
                {
                    var pos = CalcRefPos(pic.Height, Presenter[_PxIndex].HighThrold);
                    e.Graphics.DrawString("H", f, SystemBrushes.Control, new PointF(0, pos));
                    e.Graphics.DrawLine(pen, 0, pos, pic.Width, pos);

                    pos = CalcRefPos(pic.Height, Presenter[_PxIndex].MidThrold);
                    e.Graphics.DrawString("M", f, SystemBrushes.Control, new PointF(0, pos));
                    e.Graphics.DrawLine(pen, 0, pos, pic.Width, pos);

                    pos = CalcRefPos(pic.Height, Presenter[_PxIndex].LowThrold);
                    e.Graphics.DrawString("L", f, SystemBrushes.Control, new PointF(0, pos));
                    e.Graphics.DrawLine(pen, 0, pos, pic.Width, pos);
                }
                else if (Presenter[_PxIndex].RefUnit == MeasureTopBaseRefUnit.Absolute)
                {
                    var pos = CalcRefPos(pic.Height, 90f);
                    e.Graphics.DrawString("H", f, SystemBrushes.Control, new PointF(0, pos));
                    e.Graphics.DrawLine(pen, 0, pos, pic.Width, pos);
                    String highlevel = ThroldToString(Presenter[_PxIndex].HighAbsoluteThrold, Presenter[_PxIndex].ThroldUnit);
                    var sizef = e.Graphics.MeasureString(highlevel, f);
                    e.Graphics.DrawString(highlevel, f, SystemBrushes.Control, new PointF(pic.Width - sizef.Width, pos));

                    pos = CalcRefPos(pic.Height, 50f);
                    e.Graphics.DrawString("M", f, SystemBrushes.Control, new PointF(0, pos));
                    e.Graphics.DrawLine(pen, 0, pos, pic.Width, pos);
                    String middlelevel = ThroldToString(Presenter[_PxIndex].MidAbsoluteThrold, Presenter[_PxIndex].ThroldUnit);
                    sizef = e.Graphics.MeasureString(middlelevel, f);
                    e.Graphics.DrawString(middlelevel, f, SystemBrushes.Control, new PointF(pic.Width - sizef.Width, pos));

                    pos = CalcRefPos(pic.Height, 10f);
                    e.Graphics.DrawString("L", f, SystemBrushes.Control, new PointF(0, pos));
                    e.Graphics.DrawLine(pen, 0, pos, pic.Width, pos);
                    String lowlevel = ThroldToString(Presenter[_PxIndex].LowAbsoluteThrold, Presenter[_PxIndex].ThroldUnit);
                    sizef = e.Graphics.MeasureString(lowlevel, f);
                    e.Graphics.DrawString(lowlevel, f, SystemBrushes.Control, new PointF(pic.Width - sizef.Width, pos));
                }
            }
        }

        private static Single CalcRefPos(Single imgHeight, Single pos)
        {
            return 40 + (imgHeight - 62) * (1 - pos / 100F);
        }
    }
}

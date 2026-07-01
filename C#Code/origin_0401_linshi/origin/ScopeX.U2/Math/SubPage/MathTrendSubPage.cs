// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/4/13</date>

namespace ScopeX.U2
{
    using System;
    using System.ComponentModel;
    using System.Windows.Forms;
    using ScopeX.ComModel;
    using ScopeX.Core;
    using ScopeX.Core.Tools;
    using ScopeX.UserControls.Style;
    using ScopeX.Controls.Common.Default;
    using ScopeX.Controls.Common.Helper;
    using System.Runtime.CompilerServices;
    using System.Diagnostics;
    using System.Collections.Generic;
    using ScopeX.UserControls;
    using System.Linq;

    [Description(nameof(MathType.Trend))]
    public partial class MathTrendSubPage :MathSubPageBase
    {
        MathTrendArg Arg;

        public MathTrendSubPage()
        {
            InitializeComponent();
        }

        #region Base Method
        protected override void SubPageUpdateView(Object presenter, String propertyName)
        {
            UpdateView();
        }

        protected override void LoadMethod()
        {
            base.LoadMethod();
            Arg = (MathTrendArg)Presenter.GetOrMakeArg(MathType.Trend);
            if (Parent is MathPage page && page != null)
            {
                page.ToAutoScale(Presenter.AutoScale);

            }
            UpdateView();
            //UpdateVScale();
        } 
        #endregion

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                if (Arg is null)
                {
                    return;
                }

                _ArgToCtrl = true;
                CbxSource.SelectedIndex = CbxSource.FindString(Arg.Source.ToString());
                SetMeasInfo();
                _ArgToCtrl = false;
            }
        }

        private void BtnMeasItem_Click(object sender, EventArgs e)
        {
            //var views= (Program.Oscilloscope.View as DsoForm).Presenter.Measure.GetViewList();
            //foreach(var view in views)
            //{
            //    if(view is DsoResultStrip drs)
            //    {
            //        drs.MeasPresenter
            //    }
            //}

            Int32 idx = Arg.Source - ChannelId.P1;


            MeasSelectionForm msf = new(MeasureApp.Default.Presenter[idx].Name, MeasureApp.Default.Presenter[idx].Source, (n, s) =>
            {
                var mi = MeasureApp.Default.Presenter.SelectedItems.FirstOrDefault(o => o.Active&&o.Id!= MeasureApp.Default.Presenter[idx].Id && o.Name == n && o.Source == s);
                if (mi != null)
                {
                    WeakTip.Default.Write("Measure", MsgTipId.MeasuerLabelExisted);
                    return false;
                }

                MeasureApp.Default.Presenter[idx].Name = n;
                MeasureApp.Default.Presenter[idx].Source = s;
                MeasureApp.Default.Presenter[idx].Active = true;
                return true;
            })
            {
                Anchor = AnchorStyles.Top,
                StartPosition = FormStartPosition.CenterScreen,
            };
            //msf.Owner = this.FindForm();
            (ParentForm as FloatForm).CanClose = false;
            msf.ShowDialog();
            (ParentForm as FloatForm).CanClose = true;



            //var mcf = new MeasItemCfgForm(idx)
            //{
            //    Presenter = MeasureApp.Default.Presenter,
            //    Anchor = AnchorStyles.Top | AnchorStyles.Left,
            //};

            UpdateView();
        }

        private void CbxSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Arg.Source = Enum.Parse<ChannelId>((String)CbxSource.SelectedItem);
            }
        }

        private void SetMeasInfo()
        {
            if (Arg.Source.IsMeasure())
            {
                var mp = MeasureApp.Default.Presenter;
                var idx = Arg.Source - ChannelId.P1;
                BtnMeasItem.Text = mp[idx].MeasureType == MeasureType.Single ? MeasureApp.Default.MeasCandidates[mp[idx].Name].Text + $"({mp[idx].Source})" : $"{mp[idx].Source} {mp[idx].Operation.GetDescription()} {mp[idx].Source2nd}";
                BtnMeasItem.ForeColor = mp[idx].DrawColor;
                BtnMeasItem.Enabled = mp[idx].MeasureType == MeasureType.Single;
            }
            if (Arg.Source == ChannelId.DVM)
            {
                BtnMeasItem.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("MeasureMenuForm.MeasureMenuPage.Tlp.BtnVoltmeter.Tlp.BtnMain");
                BtnMeasItem.ForeColor = DsoPrsnt.DefaultDsoPrsnt.Voltmeter.DrawColor;
                BtnMeasItem.Enabled = false;
            }
            if (Arg.Source == ChannelId.CYM)
            {
                BtnMeasItem.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("MeasureMenuForm.MeasureMenuPage.Tlp.BtnCymometer.Tlp.BtnMain");
                BtnMeasItem.ForeColor = DsoPrsnt.DefaultDsoPrsnt.Cymometer.DrawColor;
                BtnMeasItem.Enabled = false;
            }
        }
        private void UpdateVScale()
        {
            var measprsnt = Program.Oscilloscope.Measure;
            var value = measprsnt.GetResult(CbxSource.SelectedIndex);
            if (value == null)
                return;
            var (pfx, unit) = measprsnt.GetPfxUnitString(CbxSource.SelectedIndex);

            value = Quantity.ConvertByPrefix((Double)value, pfx);

            for (var i = Presenter.ScaleMinIndex; i < Presenter.ScaleMaxIndex; i++)
            {
                var scale = Presenter.QueryScaleValue(i);
                var div = Quantity.ConvertByPrefix(scale, Presenter.Prefix);
                if (value > 0)
                {
                    if (value >= div && value <= div * 2)
                    {
                        Presenter.ScaleIndex = i;
                        return;
                    }
                }
                if (value < 0)
                {
                    if (value <= -div && value >= -div * 2)
                    {
                        Presenter.ScaleIndex = i;
                        return;
                    }
                }
            }

        }
    }
}

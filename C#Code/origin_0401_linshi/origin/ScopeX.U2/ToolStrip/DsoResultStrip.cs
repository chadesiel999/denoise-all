using EventBus;
using ScopeX.ComModel;
using ScopeX.Controls.Common.Structs;
using ScopeX.Core;
using ScopeX.Core.Tools;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ScopeX.U2
{
    public partial class DsoResultStrip : UserControl, IView/*IMeasView, ICymometerView, IVoltmeterView*/
    {
        private readonly List<ScopeX.UserControls.ScopeXListPage> _MeasureInfo = new();

        private Boolean _FirstSetParentflag = true;

        private readonly Size _CymometerNormalSize;

        private readonly Size _VoltmeterNormalSize;

        private readonly Size _MeasureNormalSize;

        private readonly Int32 _MinWidth;
        private DateTime _ResetStartTime = DateTime.MinValue;
        private Int32 _ResetTimeout = 500;

        public DsoResultStrip()
        {
            InitializeComponent();
            InitItemControl();
            _CymometerNormalSize = LpCymometerItem.Size;
            _VoltmeterNormalSize = LpVoltmeterItem.Size;
            _MeasureNormalSize = LpMeasureItem1.Size;
            _MinWidth = _MeasureNormalSize.Width * _MeasureInfo.Count + _CymometerNormalSize.Width + _VoltmeterNormalSize.Width;
            ScopeX.Controls.Language.LanguageManger.Instance.LanguageChanged += Instance_LanguageChanged;

        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int SetProcessDpiAwareness(int DPI_AWARENESS);

        protected override void OnHandleDestroyed(EventArgs e)
        {
            base.OnHandleDestroyed(e);
            ScopeX.Controls.Language.LanguageManger.Instance.LanguageChanged -= Instance_LanguageChanged;
        }
        private void Instance_LanguageChanged(object sender, Controls.Language.ILanguage e)
        {
            LpVoltmeterItem.Header = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("DianYaBiao");
            LpCymometerItem.Header = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("PinLvJi");
        }

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
                cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }

        private void InitMeasItemPrsnt()
        {
            for (int index = 0; index < MeasPresenter.Length; index++)
            {
                MeasPresenter[index].OpenOrCloseFigure = AddOrRemoveFigure;
            }

            VoltmeterPresenter.OpenOrCloseFigure = AddOrRemoveFigure;
            CymometerPresenter.OpenOrCloseFigure = AddOrRemoveFigure;
        }

        private void InitItemControl()
        {
            _MeasureInfo.Add(LpMeasureItem1);
            _MeasureInfo.Add(LpMeasureItem2);
            _MeasureInfo.Add(LpMeasureItem3);
            _MeasureInfo.Add(LpMeasureItem4);
            _MeasureInfo.Add(LpMeasureItem5);
            _MeasureInfo.Add(LpMeasureItem6);
            _MeasureInfo.Add(LpMeasureItem7);
            _MeasureInfo.Add(LpMeasureItem8);
            _MeasureInfo.Add(LpMeasureItem9);
            _MeasureInfo.Add(LpMeasureItem10);

            for (Int32 i = 0; i < _MeasureInfo.Count; i++)
            {
                var measitem = _MeasureInfo[i];

                measitem.DashArray = null;
                //measitem.BackColor = Color.FromArgb(32, 32, 32);
                measitem.ValueForeColor = Color.FromArgb(192, 192, 192);
                measitem.AutoHide = false;
                //measitem.BorderThickness = 1;
                measitem.DisplayMember = "Key";
                measitem.ValueMember = "Value";
                measitem.DropBackOpacity = 85;
                measitem.Header = $"P{i + 1}";
                measitem.Font = AppStyleConfig.DefaultBoldFont;
                measitem.HeaderFont = AppStyleConfig.DefaultBoldFont;
                measitem.HeaderInfoFont = AppStyleConfig.DefaultBoldFont;
                measitem.ValueFont = AppStyleConfig.DefaultBoldFont;
                measitem.DropedValueFont = AppStyleConfig.DefaultMeasureFont;
                measitem.DropedHeaderFont = AppStyleConfig.DefaultMeasureFont;
                measitem.DropedMeasNameItemFont = AppStyleConfig.DefaultMeasureFont;

                measitem.BackColor = AppStyleConfig.ScopeXListPageBackColor;//AppStyleConfig.DefaultContextDarkBackColor;
                measitem.BorderColor = AppStyleConfig.ScopeXListPageHeaderBackColor;//AppStyleConfig.DefaultContextDarkBackColor;
                measitem.BorderThickness = AppStyleConfig.DefaultBorderThickness;
                measitem.HeaderBackColor = AppStyleConfig.ScopeXListPageHeaderBackColor;//AppStyleConfig.DefaultContextBackColor;
                measitem.DisplayForeColor = AppStyleConfig.DefaultContextForeColor;
                measitem.HeaderForeColor = AppStyleConfig.DefaultContextForeColor;
                measitem.ValueForeColor = AppStyleConfig.DefaultTitleForeColor;

                measitem.MouseClick += LpMeasureItem_MouseClick;

                measitem.MouseDown += LpItemDragSource_MouseDown;
                measitem.MouseUp += LpItemDragSource_MouseUp;
                measitem.MouseMove += LpItemDragSource_MouseMove;
                measitem.GiveFeedback += LpItemDragSource_GiveFeedback;
                measitem.QueryContinueDrag += LpItemDragSource_QueryContinueDrag;
            }

            //LpCymometerItem.Font = new Font("Arial", 9F, FontStyle.Bold, GraphicsUnit.Point);
            LpCymometerItem.Font = AppStyleConfig.DefaultBoldFont;
            LpCymometerItem.HeaderFont = AppStyleConfig.DefaultBoldFont;
            //LpCymometerItem.HeaderFont= new Font("Arial", 9F, FontStyle.Bold, GraphicsUnit.Point);
            LpCymometerItem.HeaderInfoFont = AppStyleConfig.DefaultBoldFont;
            LpCymometerItem.ValueFont = AppStyleConfig.DefaultBoldFont;
            LpCymometerItem.BackColor = AppStyleConfig.ScopeXListPageBackColor;//AppStyleConfig.DefaultContextDarkBackColor;
            LpCymometerItem.BorderColor = AppStyleConfig.ScopeXListPageHeaderBackColor;//AppStyleConfig.DefaultContextDarkBackColor;
            LpCymometerItem.BorderThickness = AppStyleConfig.DefaultBorderThickness;
            LpCymometerItem.HeaderBackColor = AppStyleConfig.ScopeXListPageHeaderBackColor;//AppStyleConfig.DefaultContextBackColor;
            LpCymometerItem.DisplayForeColor = AppStyleConfig.DefaultContextForeColor;
            LpCymometerItem.HeaderForeColor = AppStyleConfig.DefaultContextForeColor;
            LpCymometerItem.ValueForeColor = AppStyleConfig.DefaultTitleForeColor;

            LpCymometerItem.MouseDown += LpItemDragSource_MouseDown;
            LpCymometerItem.MouseUp += LpItemDragSource_MouseUp;
            LpCymometerItem.MouseMove += LpItemDragSource_MouseMove;
            LpCymometerItem.GiveFeedback += LpItemDragSource_GiveFeedback;


            LpVoltmeterItem.Font = AppStyleConfig.DefaultBoldFont;
            LpVoltmeterItem.HeaderFont = AppStyleConfig.DefaultBoldFont;
            LpVoltmeterItem.HeaderInfoFont = AppStyleConfig.DefaultBoldFont;
            LpVoltmeterItem.ValueFont = AppStyleConfig.DefaultBoldFont;
            LpVoltmeterItem.BackColor = AppStyleConfig.ScopeXListPageBackColor;//AppStyleConfig.DefaultContextDarkBackColor;
            LpVoltmeterItem.BorderColor = AppStyleConfig.ScopeXListPageHeaderBackColor; //AppStyleConfig.DefaultContextDarkBackColor;
            LpVoltmeterItem.BorderThickness = AppStyleConfig.DefaultBorderThickness;
            LpVoltmeterItem.HeaderBackColor = AppStyleConfig.ScopeXListPageHeaderBackColor; //AppStyleConfig.DefaultContextBackColor;
            LpVoltmeterItem.DisplayForeColor = AppStyleConfig.DefaultContextForeColor;
            LpVoltmeterItem.HeaderForeColor = AppStyleConfig.DefaultContextForeColor;
            LpVoltmeterItem.ValueForeColor = AppStyleConfig.DefaultTitleForeColor;
            BtnMeasureTool.Icon = Properties.Resources.MeasureTool;
            LpVoltmeterItem.MouseDown += LpItemDragSource_MouseDown;
            LpVoltmeterItem.MouseUp += LpItemDragSource_MouseUp;
            LpVoltmeterItem.MouseMove += LpItemDragSource_MouseMove;
            LpVoltmeterItem.GiveFeedback += LpItemDragSource_GiveFeedback;
        }

        public MeasPrsnt MeasPresenter
        {
            get;
            set;
        }

        //IMeasPrsnt IView<IMeasPrsnt>.Presenter
        //{
        //    get => MeasPresenter;
        //    set => MeasPresenter = (MeasPrsnt)value;
        //}

        public CymometerPrsnt CymometerPresenter
        {
            get;
            set;
        }

        //ICymometerPrsnt ICymometerView.Presenter
        //{
        //    get => CymometerPresenter;
        //    set => CymometerPresenter = (CymometerPrsnt)value;
        //}

        public VoltmeterPrsnt VoltmeterPresenter
        {
            get;
            set;
        }
        //public void Reset()
        //{
        //    _ResetStartTime = DateTime.Now;
        //}
        //IVoltmeterPrsnt IVoltmeterView.Presenter
        //{
        //    get => VoltmeterPresenter;
        //    set => VoltmeterPresenter = (VoltmeterPrsnt)value;
        //}

        //IBadge IView<IBadge>.Presenter
        //{
        //    get => throw new NotImplementedException();
        //    set => throw new NotImplementedException();
        //}

        public void UpdateView(Object prsnt, String propertyName)
        {
            if (propertyName == nameof(MeasPresenter.SnapshotActive))
                _IsSetParent = true;
            else
                _IsSetParent = false;

            if (InvokeRequired)
            {
                BeginInvoke(new Action<Object, String>(Update), new[] { prsnt, propertyName });
            }
            else
            {
                Update(prsnt, propertyName);
            }
            // 

        }
        private bool _IsSetParent = false;
        protected void Update(Object prsnt, String propertyName)
        {
            //switch (propertyName)
            //{
            //    case nameof(MeasPresenter.Active):
            //    case nameof(MeasPresenter.IsStatActive):
            //        UpdateView();
            //        break;
            //}
            if (propertyName.Contains("Active"))
            {
                UpdateView();
                if (!_IsSetParent)
                {
                    SetParent();

                    _IsSetParent = true;
                }

            }
            var itemprsnt = MeasPresenter[0];
            switch (propertyName)
            {
                case nameof(itemprsnt.HistgramEnable):
                    break;
                case nameof(itemprsnt.TrackEnable):
                    break;
                case nameof(itemprsnt.TrendEnable):
                    break;
                default:
                    break;
            }

        }

        private Boolean AddOrRemoveFigure(Boolean isOpen, ChannelId source, MeasItemFigureType figureType)
        {
            return isOpen ? AddFigure(source, figureType) : CloseFigure(source, figureType);
        }

        private Boolean AddFigure(ChannelId source, MeasItemFigureType MeasItemFigureType)
        {
            var mpt = (Program.Oscilloscope.View as DsoForm).TryAddMathWaveform(mp =>
            {
                switch (MeasItemFigureType)
                {
                    case MeasItemFigureType.Histgram:
                        var mha = (MathHistArg)mp.GetOrMakeArg(MathType.Histgram);
                        mha.Source = source;
                        break;
                    case MeasItemFigureType.Track:
                        var mtka = (MathTrackArg)mp.GetOrMakeArg(MathType.Track);
                        mtka.Source = source;
                        break;
                    case MeasItemFigureType.Trend:
                        var mtda = (MathTrendArg)mp.GetOrMakeArg(MathType.Trend);
                        mtda.Source = source;
                        break;
                    default:
                        break;
                }
            });
            return mpt != null && mpt.Active;
        }

        private Boolean CloseFigure(ChannelId source, MeasItemFigureType figureType)
        {
            var activedprsnt = (Program.Oscilloscope.View as DsoForm).Presenter.TryGetRange(c => c.Active && c.Id.IsMath());
            foreach (var prsnt in activedprsnt)
            {
                var arg = (prsnt as MathPrsnt).Args;
                switch (figureType)
                {
                    case MeasItemFigureType.Histgram:
                        if (arg is MathHistArg mha && mha.Source == source)
                        {
                            prsnt.Active = false;
                        }
                        break;
                    case MeasItemFigureType.Trend:
                        if (arg is MathTrendArg mtda && mtda.Source == source)
                        {
                            prsnt.Active = false;
                        }
                        break;
                    case MeasItemFigureType.Track:
                        if (arg is MathTrackArg mtka && mtka.Source == source)
                        {
                            prsnt.Active = false;
                        }
                        break;
                    default:
                        break;
                }
            }
            return true;
        }
        private void BtnMeasureTool_Click(object sender, System.EventArgs e)
        {
            _ = NativeMethods.PostMessage((Program.Oscilloscope.View as DsoForm).Handle, 0x0400, 12, -KeyCode.MEASURE);
        }

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                if (DsoPrsnt.FocusId.IsAnalog() && !VoltmeterPresenter.Active && !DsoPrsnt.DefaultDsoPrsnt.GetAllChnls().Where(x => x.Id == VoltmeterPresenter.Source).FirstOrDefault().Active)
                    VoltmeterPresenter.Source = DsoPrsnt.FocusId; // 当前激活的模拟通道设置为测量源。

                BtnAddMeasure.Visible = !MeasPresenter.IsAllActive() && MeasPresenter.Active;

                TlpBody.ColumnStyles[12]= new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F);
                TlpBody.ColumnStyles[13] = new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 210F);
                LpCymometerItem.HeadWidthGain = 60;
                #region 指示器隐藏显示

                //<Remark>更改人：彭博 创建日期：2023/12/13 18:35:00  原因：当关闭Meas时，指示器未关闭 </Remark>
                if (!MeasPresenter.Active)
                {
                    MeasPresenter.Indicator = 0;
                }

                if (!MeasPresenter.Active)
                {
                    if ((!MeasPresenter.Active) && measItemCfgForm != null && measItemCfgForm.IsDisposed == false)
                    {
                        measItemCfgForm.Close();
                        measItemCfgForm = null;
                    }
                }
                #endregion

                if (MeasPresenter.SnapshotActive)
                {
                    var exist = MeasPresenter.GetViewList().Exists(o => o is MeasSnapShotForm);

                    if (!exist)
                    {
                        var mssf = new MeasSnapShotForm
                        {
                            Location = new(100, 100),
                            Presenter = MeasPresenter
                        };
                        mssf.Presenter.TryAddView(mssf);

                        EventBroker.Instance.GetEvent<FormEventArgs>().Publish(this, new FormEventArgs() { Current = mssf, Type = FormType.InfoForm });
                    }
                }

                Boolean flag = false;
                var candidates = MeasureApp.Default.MeasCandidates;
                for (Int32 i = 0; i < MeasPresenter.Length; i++)
                {
                    if (MeasPresenter.Active && MeasPresenter[i].Active)
                    {
                        if (Program.Oscilloscope.SysLanguage == Language.English)
                        {

                            _MeasureInfo[i].Font = new System.Drawing.Font("MiSans", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
                        }
                        else
                            _MeasureInfo[i].Font = AppStyleConfig.DefaultMeasureFont;
                        if (MeasPresenter[i].MeasureType == MeasureType.Single)
                        {
                            _MeasureInfo[i].MeasItemName = candidates[MeasPresenter[i].Name].Text;
                            _MeasureInfo[i].HeaderInfos[0] = MeasPresenter[i].Source.ToString();
                            _MeasureInfo[i].HeaderInfos[1] = MeasPresenter[i].Dualsrc ? MeasPresenter[i].Source2nd.ToString() : String.Empty;
                        }
                        else
                        {
                            _MeasureInfo[i].MeasItemName = MeasPresenter[i].Name.Trim();
                            _MeasureInfo[i].HeaderInfos[0] = String.Empty;
                            _MeasureInfo[i].HeaderInfos[1] = String.Empty;
                        }
                        if (Program.Oscilloscope.TryGetChannel(MeasPresenter[i].Source, out var chnl1))
                        {
                            if (chnl1.Active == false)
                            {
                                _MeasureInfo[i].HeaderInfoForeColors[0] = MeasPresenter[i].DrawColor;
                                _MeasureInfo[i].ValueForeColor = Color.White;
                                _MeasureInfo[i].HeaderForeColor = Color.White;
                            }
                            else
                            {
                                _MeasureInfo[i].HeaderInfoForeColors[0] = MeasPresenter[i].DrawColor;
                                _MeasureInfo[i].ValueForeColor = Color.White;
                                _MeasureInfo[i].HeaderForeColor = Color.White;
                            }
                        }
                        if (MeasPresenter[i].Dualsrc && Program.Oscilloscope.TryGetChannel(MeasPresenter[i].Source2nd, out var chnl2))
                        {
                            if (chnl2.Active == false)
                            {
                                _MeasureInfo[i].HeaderInfoForeColors[1] = chnl2.DrawColor;
                                _MeasureInfo[i].ValueForeColor = Color.White;
                                _MeasureInfo[i].HeaderForeColor = Color.White;
                            }
                            else
                            {
                                _MeasureInfo[i].HeaderInfoForeColors[1] = chnl2.DrawColor;
                                _MeasureInfo[i].ValueForeColor = Color.White;
                                _MeasureInfo[i].HeaderForeColor = Color.White;
                            }
                        }

                        //_MeasureInfo[i].HeaderInfoForeColor = MeasPresenter[i].DrawColor;
                        _MeasureInfo[i].ShowIndicator = MeasPresenter.Indicator == i + 1;

                        #region Value

                        var (pfx, unit) = MeasPresenter.GetPfxUnitString(i);

                        var value = MeasPresenter.GetResult(i) ?? Double.NaN;
                        var max = MeasPresenter.GetStatMax(i) ?? Double.NaN;
                        var min = MeasPresenter.GetStatMin(i) ?? Double.NaN;
                        var ave = MeasPresenter.GetStatAverage(i) ?? Double.NaN;
                        var stddev = MeasPresenter.GetStatStddev(i) ?? Double.NaN;
                        var pop = (Double)MeasPresenter.GetStatCount(i);

                        if (MeasPresenter[i].MeasureType == MeasureType.Single)
                        {
                            if (Program.Oscilloscope.TryGetChannel(MeasPresenter[i].Source, out var ch))
                            {
                                if (ch.Active == false)
                                {
                                    value = Double.NaN;
                                    max = Double.NaN;
                                    min = Double.NaN;
                                    ave = Double.NaN;
                                    stddev = Double.NaN;
                                    pop = Double.NaN;
                                }
                            }
                        }
                        else if (MeasPresenter[i].MeasureType == MeasureType.Composite)
                        {
                            var source1 = MeasPresenter[MeasPresenter[i].Source - ChannelId.P1].Source;
                            var source2 = MeasPresenter[MeasPresenter[i].Source2nd - ChannelId.P1].Source;
                            if (Program.Oscilloscope.TryGetChannel(source1, out var ch) && Program.Oscilloscope.TryGetChannel(source2, out var ch2))
                            {
                                if (ch.Active == false || ch2.Active == false)
                                {
                                    value = Double.NaN;
                                    max = Double.NaN;
                                    min = Double.NaN;
                                    ave = Double.NaN;
                                    stddev = Double.NaN;
                                    pop = Double.NaN;
                                }
                            }
                        }

                        if (MeasPresenter.ClearFlag)
                        {
                            _ResetStartTime = DateTime.Now;
                            MeasPresenter.ClearFlag = false;
                        }

                        if (DateTime.Now.Subtract(_ResetStartTime).TotalMilliseconds < _ResetTimeout)//clear
                        {
                            value = Double.NaN;
                            max = Double.NaN;
                            min = Double.NaN;
                            ave = Double.NaN;
                            stddev = Double.NaN;
                            pop = Double.NaN;
                        }

                        List<KeyValuePair<String, String>> subitems = new();
                        if (MeasPresenter[i].MeasureType == MeasureType.Single)
                        {
                            if (Width <= _MinWidth)
                            {
                                subitems.Add(new("v", MeasPresenter.CalcResultStringNow(value, pfx, unit).ToFormat(candidates[MeasPresenter[i].Name].Format)));
                                subitems.Add(new("M", MeasPresenter.CalcResultStringNow(max, pfx, unit).ToFormat(candidates[MeasPresenter[i].Name].Format)));
                                subitems.Add(new("m", MeasPresenter.CalcResultStringNow(min, pfx, unit).ToFormat(candidates[MeasPresenter[i].Name].Format)));
                                subitems.Add(new("μ", MeasPresenter.CalcResultStringNow(ave, pfx, unit).ToFormat(candidates[MeasPresenter[i].Name].Format)));
                                subitems.Add(new("σ", MeasPresenter.CalcResultStringNow(stddev, pfx, unit).ToFormat(candidates[MeasPresenter[i].Name].Format)));
                                subitems.Add(new("#", MeasPresenter.CalcResultStringNow(pop, Prefix.Empty, QuantityUnit.Count.ToUnitString()).ToFormat(candidates[MeasPresenter[i].Name].Format)));
                            }
                            else
                            {
                                //subitems.Add(new(Properties.Resources.Statistic_VAL, MeasPresenter.CalcResultStringNow(value, pfx, unit)));
                                //subitems.Add(new(Properties.Resources.Statistic_MAX, MeasPresenter.CalcResultStringNow(max, pfx, unit)));
                                //subitems.Add(new(Properties.Resources.Statistic_MIN, MeasPresenter.CalcResultStringNow(min, pfx, unit)));
                                //subitems.Add(new(Properties.Resources.Statistic_AVG, MeasPresenter.CalcResultStringNow(ave, pfx, unit)));
                                //subitems.Add(new(Properties.Resources.Statistic_DEV, MeasPresenter.CalcResultStringNow(stddev, pfx, unit)));
                                //subitems.Add(new(Properties.Resources.Statistic_POP, MeasPresenter.CalcResultStringNow(pop, Prefix.Empty, QuantityUnit.Count.ToUnitString())));

                                subitems.Add(new(ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Statistic_VAL"), MeasPresenter.CalcResultStringNow(value, pfx, unit).ToFormat(candidates[MeasPresenter[i].Name].Format)));
                                subitems.Add(new(ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Statistic_MAX"), MeasPresenter.CalcResultStringNow(max, pfx, unit).ToFormat(candidates[MeasPresenter[i].Name].Format)));
                                subitems.Add(new(ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Statistic_MIN"), MeasPresenter.CalcResultStringNow(min, pfx, unit).ToFormat(candidates[MeasPresenter[i].Name].Format)));
                                subitems.Add(new(ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Statistic_AVG"), MeasPresenter.CalcResultStringNow(ave, pfx, unit).ToFormat(candidates[MeasPresenter[i].Name].Format)));
                                subitems.Add(new(ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Statistic_DEV"), MeasPresenter.CalcResultStringNow(stddev, pfx, unit).ToFormat(candidates[MeasPresenter[i].Name].Format)));
                                subitems.Add(new(ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Statistic_POP"), MeasPresenter.CalcResultStringNow(pop, Prefix.Empty, QuantityUnit.Count.ToUnitString()).ToFormat(candidates[MeasPresenter[i].Name].Format)));
                            }
                        }
                        else if (MeasPresenter[i].MeasureType == MeasureType.Composite)
                        {
                            if (Width <= _MinWidth)
                            {
                                subitems.Add(new("v", MeasPresenter.CalcResultStringNow(value, pfx, unit)));
                                subitems.Add(new("M", MeasPresenter.CalcResultStringNow(max, pfx, unit)));
                                subitems.Add(new("m", MeasPresenter.CalcResultStringNow(min, pfx, unit)));
                                subitems.Add(new("μ", MeasPresenter.CalcResultStringNow(ave, pfx, unit)));
                                subitems.Add(new("σ", MeasPresenter.CalcResultStringNow(stddev, pfx, unit)));
                                subitems.Add(new("#", MeasPresenter.CalcResultStringNow(pop, Prefix.Empty, QuantityUnit.Count.ToUnitString())));
                            }
                            else
                            {
                                subitems.Add(new(ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Statistic_VAL"), MeasPresenter.CalcResultStringNow(value, pfx, unit)));
                                subitems.Add(new(ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Statistic_MAX"), MeasPresenter.CalcResultStringNow(max, pfx, unit)));
                                subitems.Add(new(ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Statistic_MIN"), MeasPresenter.CalcResultStringNow(min, pfx, unit)));
                                subitems.Add(new(ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Statistic_AVG"), MeasPresenter.CalcResultStringNow(ave, pfx, unit)));
                                subitems.Add(new(ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Statistic_DEV"), MeasPresenter.CalcResultStringNow(stddev, pfx, unit)));
                                subitems.Add(new(ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Statistic_POP"), MeasPresenter.CalcResultStringNow(pop, Prefix.Empty, QuantityUnit.Count.ToUnitString())));
                            }
                        }
                        _MeasureInfo[i].DataSource = subitems;

                        #endregion
                        if (this.IsHandleCreated && _MeasureInfo[i].DroppedDown != MeasPresenter.IsStatActive)
                        {
                            _MeasureInfo[i].DroppedDown = MeasPresenter.IsStatActive;
                        }

                        if (this.IsHandleCreated)
                        {
                            if (_MeasureInfo[i].DroppedDown != MeasPresenter.IsStatActive && (ParentForm as DsoForm).WindowState == FormWindowState.Normal)
                            {
                                _MeasureInfo[i].DroppedDown = MeasPresenter.IsStatActive;
                            }
                            if (_MeasureInfo[i].DroppedDown == true && (ParentForm as DsoForm).WindowState == FormWindowState.Minimized)
                            {
                                _MeasureInfo[i].DroppedDown = false;
                            }
                        }

                        _MeasureInfo[i].Visible = true;
                        flag = true;
                    }
                    else
                    {
                        _MeasureInfo[i].Visible = false;
                    }
                }

                if (CymometerPresenter.Active) // 触发频率计绑定触发源，部分高级屏蔽频率计功能，触发源关闭显示一样需要显示。
                {
                    LpCymometerItem.HeaderFont = LanguageFactory.Current == Language.简体中文 ? AppStyleConfig.DefaultMeasureFont : AppStyleConfig.DefaultBoldFont;
                    try
                    {
                        //Boolean sourceactive = Program.Oscilloscope.TryGetRange(c => c.Id.IsAnalog() && c.Active).Count > 0;
                        //List<String> frequencystring = FrequencyToString();
                        //LpCymometerItem.BackColor = Color.FromArgb(32, 32, 32);
                        var value = CymometerPresenter.FrequencyByHz;
                        if (value == 0) value = Double.NaN;
                        var max = CymometerPresenter.StaBuffer.Max;
                        var min = CymometerPresenter.StaBuffer.Min;
                        var ave = CymometerPresenter.StaBuffer.Average;
                        var stddev = CymometerPresenter.StaBuffer.Stddev;
                        var pop = CymometerPresenter.StaBuffer.Count;
                        List<KeyValuePair<String, String>> subitems = new();
                        if (Width <= _MinWidth)
                        {
                            subitems.Add(new("v", CymometerPresenter.FrequencyToString(value)));
                            subitems.Add(new("M", CymometerPresenter.FrequencyToString(max ?? Double.NaN)));
                            subitems.Add(new("m", CymometerPresenter.FrequencyToString(min ?? Double.NaN)));
                            subitems.Add(new("μ", CymometerPresenter.FrequencyToString(ave ?? Double.NaN)));
                            subitems.Add(new("σ", CymometerPresenter.FrequencyToString(stddev)));
                            subitems.Add(new("#", new Quantity(pop, Prefix.Empty, QuantityUnit.Count.ToUnitString()).ToString("##0", true, 6)));
                        }
                        else
                        {
                            subitems.Add(new(ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Statistic_VAL"), CymometerPresenter.FrequencyToString(value)));
                            subitems.Add(new(ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Statistic_MAX"), CymometerPresenter.FrequencyToString(max ?? Double.NaN)));
                            subitems.Add(new(ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Statistic_MIN"), CymometerPresenter.FrequencyToString(min ?? Double.NaN)));
                            subitems.Add(new(ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Statistic_AVG"), CymometerPresenter.FrequencyToString(ave ?? Double.NaN)));
                            subitems.Add(new(ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Statistic_DEV"), CymometerPresenter.FrequencyToString(stddev)));
                            subitems.Add(new(ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Statistic_POP"), new Quantity(pop, Prefix.Empty, QuantityUnit.Count.ToUnitString()).ValueChangeToSI(3)));
                        }
                        LpCymometerItem.HeaderInfos = new string[] { "", String.Empty };//CymometerPresenter.Source.ToString();
                        if (!LpCymometerItem.DroppedDown)
                        {
                            LpCymometerItem.MeasItemName = subitems[0].Value;
                            LpCymometerItem.DataSource = new List<KeyValuePair<String, String>>() { new(" ", CymometerPresenter.Unit) };
                        }
                        else
                        {
                            LpCymometerItem.MeasItemName = CymometerPresenter.Unit;
                            LpCymometerItem.DataSource = subitems;
                        }
                        //LpCymometerItem.DataSource = subitems;
                        LpCymometerItem.HeaderInfoForeColors[0] = CymometerPresenter.DrawColor;
                        if (!LpCymometerItem.Visible)
                        {
                            //LpCymometerItem.Visible = true;
                            LpCymometerItem.Visible = false;
                        }
                        if (this.IsHandleCreated)
                        {
                            if(LpCymometerItem.DroppedDown != CymometerPresenter.IsStatActive && (ParentForm as DsoForm).WindowState == FormWindowState.Normal)
                            {
                                LpCymometerItem.DroppedDown = CymometerPresenter.IsStatActive;
                            }
                            if (LpCymometerItem.DroppedDown == true && (ParentForm as DsoForm).WindowState == FormWindowState.Minimized)
                            {
                                LpCymometerItem.DroppedDown = false;
                            }
                        }
                        
                        flag = true;
                    }
                    catch (Exception)
                    {

                    }
                }
                else
                {
                    LpCymometerItem.Visible = false;
                }

                if (VoltmeterPresenter.Active)
                {
                    Boolean sourceactive = true;
                    if (Program.Oscilloscope.TryGetChannel(VoltmeterPresenter.Source, out IChnlPrsnt channel))
                    {
                        sourceactive = channel.Active;
                    }
                    var value = VoltmeterPresenter.Current;
                    var max = VoltmeterPresenter.StaBuffer.Max;
                    var min = VoltmeterPresenter.StaBuffer.Min;
                    var ave = VoltmeterPresenter.StaBuffer.Average;
                    var stddev = VoltmeterPresenter.StaBuffer.Stddev;
                    var pop = VoltmeterPresenter.StaBuffer.Count;
                    List<KeyValuePair<String, String>> subitems = new();
                    if (Width <= _MinWidth)
                    {
                        subitems.Add(new("v", VoltmeterPresenter.VoltageToString(value)));
                        subitems.Add(new("M", VoltmeterPresenter.VoltageToString(max ?? Double.NaN)));
                        subitems.Add(new("m", VoltmeterPresenter.VoltageToString(min ?? Double.NaN)));
                        subitems.Add(new("μ", VoltmeterPresenter.VoltageToString(ave ?? Double.NaN)));
                        subitems.Add(new("σ", VoltmeterPresenter.VoltageToString(stddev)));
                        subitems.Add(new("#", new Quantity(pop, Prefix.Empty, QuantityUnit.Count.ToUnitString()).ToString("##0", true, 6)));
                    }
                    else
                    {
                        subitems.Add(new(ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Statistic_VAL"), VoltmeterPresenter.VoltageToString(value)));
                        subitems.Add(new(ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Statistic_MAX"), VoltmeterPresenter.VoltageToString(max ?? Double.NaN)));
                        subitems.Add(new(ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Statistic_MIN"), VoltmeterPresenter.VoltageToString(min ?? Double.NaN)));
                        subitems.Add(new(ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Statistic_AVG"), VoltmeterPresenter.VoltageToString(ave ?? Double.NaN)));
                        subitems.Add(new(ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Statistic_DEV"), VoltmeterPresenter.VoltageToString(stddev)));
                        subitems.Add(new(ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Statistic_POP"), new Quantity(pop, Prefix.Empty, QuantityUnit.Count.ToUnitString()).ValueChangeToSI(3)));
                    }
                    //LpVoltmeterItem.BackColor = Color.FromArgb(32, 32, 32);
                    LpVoltmeterItem.HeaderInfos[0] = VoltmeterPresenter.Source.ToString();
                    LpVoltmeterItem.MeasItemName = CouplingToString();
                    //LpVoltmeterItem.DataSource = new List<KeyValuePair<String, String>>() { new(" ", CouplingToString()) };
                    LpVoltmeterItem.DataSource = subitems;
                    //LpVoltmeterItem.HeaderInfoForeColor = CymometerPresenter.DrawColor;
                    LpVoltmeterItem.HeaderInfoForeColors[0] = channel.DrawColor;
                    if (!LpVoltmeterItem.Visible)
                    {
                        LpVoltmeterItem.Visible = true;
                    }
                    if (this.IsHandleCreated)
                    {
                        if (LpVoltmeterItem.DroppedDown != VoltmeterPresenter.IsStatActive && (ParentForm as DsoForm).WindowState == FormWindowState.Normal)
                        {
                            LpVoltmeterItem.DroppedDown = VoltmeterPresenter.IsStatActive;
                        }
                        if (LpVoltmeterItem.DroppedDown == true && (ParentForm as DsoForm).WindowState == FormWindowState.Minimized)
                        {
                            LpVoltmeterItem.DroppedDown = false;
                        }
                    }
                    flag = true;
                }
                else
                {
                    LpVoltmeterItem.Visible = false;
                }

                //Visible = flag;
                TmUpdate.Enabled = flag;

                if (BtnAddMeasure.Visible)
                {
                    TlpBody.ColumnStyles[11].SizeType = SizeType.Percent;
                    TlpBody.ColumnStyles[11].Width = 100;
                    BtnAddMeasure.Width = 24;
                }
                else
                {
                    BtnAddMeasure.Width = 0;
                }
                LpCymometerItem.Visible = false;
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
            ScopeX.UserControls.Style.DefaultStyleManager.Instance.RegisterControlRecursion(this);
            this.BackColor = Color.FromArgb(33, 33, 40);
            //!!!Disable UpdateView because MeasureApp is not initialized.
            //UpdateView();
            if (!DesignMode)
            {
                SizeChanged += DsoResultStrip_SizeChanged;
            }
            InitMeasItemPrsnt();
        }

        private void DsoResultStrip_SizeChanged(Object sender, EventArgs args)
        {
            if (Width <= _MinWidth)
            {
                LpCymometerItem.Size = LpCymometerItem.MinimumSize;
                LpCymometerItem.ShowHeader = false;
                LpCymometerItem.BorderThickness = 0;

                LpVoltmeterItem.Size = LpVoltmeterItem.MinimumSize;
                LpVoltmeterItem.ShowHeader = false;
                LpVoltmeterItem.BorderThickness = 0;

                for (Int32 i = 0; i < _MeasureInfo.Count; i++)
                {
                    _MeasureInfo[i].Size = _MeasureInfo[i].MinimumSize;
                    _MeasureInfo[i].ShowHeader = false;
                    _MeasureInfo[i].BorderThickness = 0;
                    _MeasureInfo[i].Font = _MeasureInfo[i].DropedMeasNameItemFont = _MeasureInfo[i].DropedValueFont = new("Arial", 8);
                    _MeasureInfo[i].Percentage = 0.2f;
                }
            }
            else
            {
                LpCymometerItem.Size = _CymometerNormalSize;

                LpCymometerItem.ShowHeader = true;
                LpCymometerItem.BorderThickness = 1;

                LpVoltmeterItem.Size = _VoltmeterNormalSize;
                LpVoltmeterItem.ShowHeader = true;
                LpVoltmeterItem.BorderThickness = 1;

                for (Int32 i = 0; i < _MeasureInfo.Count; i++)
                {
                    _MeasureInfo[i].Size = _MeasureNormalSize;
                    _MeasureInfo[i].ShowHeader = true;
                    _MeasureInfo[i].BorderThickness = 1;
                    _MeasureInfo[i].Font = _MeasureInfo[i].DropedMeasNameItemFont = _MeasureInfo[i].DropedValueFont = new("Arial", 10.5F);
                    _MeasureInfo[i].Percentage = 0.45f;
                }
            }
        }

        private void TmUpdate_Tick(Object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(TickEvent));
            }
            else
            {
                TickEvent();
            }

            void TickEvent()
            {
                UpdateView();
                if (_FirstSetParentflag)
                {
                    SetParent();
                    _FirstSetParentflag = false;
                    _IsSetParent = false;
                }
            }
        }

        private void SetParent()
        {
            for (int i = 0; i < _MeasureInfo.Count; i++)
            {
                if (MeasPresenter[i].Active && _MeasureInfo[i].DroppedDown)
                {
                    _MeasureInfo[i].SetParent();
                }
            }
        }
        MeasItemCfgForm measItemCfgForm = null;

        private void LpMeasureItem_MouseClick(Object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                return;
            }

            if (sender is ScopeX.UserControls.ScopeXListPage measitempage)
            {
                Int32 pos = Int32.Parse(measitempage.Header[1..]) - 1;
                var point = new Point(0, 0);
                if (measitempage.DroppedDown)
                {
                    point = new Point(measitempage.Location.X + measitempage.Width / 2, (ParentForm as DsoForm).ActiveControl.Location.Y - measitempage.DropDownPageHeight + (ParentForm as DsoForm).ActiveControl.Height);
                }
                else
                {
                    point = new Point(measitempage.Location.X + measitempage.Width / 2, (ParentForm as DsoForm).ActiveControl.Location.Y);
                }
                
                point = (ParentForm as DsoForm).PointToScreen(point);
                (ParentForm as DsoForm)?.MakeOperateForm("MeasItemCfgForm", point, PopOrientation.Above, () =>
                {
                    var mif = new MeasItemCfgForm(pos)
                    {
                        Presenter = MeasPresenter,
                        Anchor = AnchorStyles.Bottom,
                    };
                    mif.Text = mif.Text + " " + measitempage.Header;
                    mif.Presenter.TryAddView(mif);
                    measItemCfgForm = mif;
                    return mif;
                });
            }
        }

        private void LpCymometerItem_MouseClick(Object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                return;
            }

            (ParentForm as DsoForm)?.MakeOperateForm("Cymometer", GetPreferredLocation(sender as Control), PopOrientation.Above, () =>
            {
                var cf = new CymometerForm()
                {
                    Presenter = CymometerPresenter,
                    Anchor = AnchorStyles.Bottom,
                };
                cf.Presenter.TryAddView(cf);

                return cf;
            });
        }

        private void LpVoltmeterItem_MouseClick(Object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                return;
            }

            var point = new Point(0, 0);
            if (LpVoltmeterItem.DroppedDown)
            {
                point = new Point(LpVoltmeterItem.Location.X + LpVoltmeterItem.Width / 2, (ParentForm as DsoForm).ActiveControl.Location.Y - LpVoltmeterItem.DropDownPageHeight + (ParentForm as DsoForm).ActiveControl.Height);
            }
            else
            {
                point = new Point(LpVoltmeterItem.Location.X + LpVoltmeterItem.Width / 2, (ParentForm as DsoForm).ActiveControl.Location.Y);
            }

            (ParentForm as DsoForm)?.MakeOperateForm("Voltmeter", point, PopOrientation.Above, () =>
            {
                var vf = new VoltmeterForm()
                {
                    Presenter = VoltmeterPresenter,
                    Anchor = AnchorStyles.Bottom,
                };
                vf.Presenter.TryAddView(vf);

                return vf;
            });
        }



        //private Point GetPreferredLocation(Int32 index)
        //{
        //    if (index < 0)
        //    {
        //        index = TlpBody.ColumnCount + index;
        //    }
        //    if (index < TlpBody.ColumnCount)
        //    {
        //        var c = TlpBody.GetControlFromPosition(index + 1, 0);
        //        if (c is not null)
        //        {
        //            return new(c.Left + c.Width / 2, Top);
        //        }
        //    }
        //    return new(Left, Top);
        //}

        private Point GetPreferredLocation(Control c)
        {
            if (c is not null)
            {
                return new(c.Left + c.Width / 2, Top);
            }

            return new(Left, Top);
        }

        private String CouplingToString()
        {
            return VoltmeterPresenter.Mode switch
            {
                VoltmeterMode.DC => "DC",
                VoltmeterMode.ACrms => "ACrms",
                VoltmeterMode.DCACrms => "DC+AC",
                _ => "",
            };
        }

        #region ToolTip
        //public void SetToolTip(ToolTip toolTip)
        //{
        //    toolTip.SetToolTip(SiCymometer, Properties.ToolTips.AboutCymometer);
        //    toolTip.SetToolTip(SiVoltmeter, Properties.ToolTips.AboutVoltmeter);
        //    foreach (var mi in _MeasureInfo)
        //    {
        //        toolTip.SetToolTip(mi, Properties.ToolTips.AboutMeasurement + " " + mi.Text);
        //    }
        //}

        #endregion

        #region DragDrop
        //!!!Sample code for DragDrop, here the DsoResultStrip's AllowDrop must be true.
        private ScopeX.UserControls.ScopeXListPage _DeletedItem;

        private Rectangle _DragBox;

        private void LpItemDragSource_MouseDown(Object sender, MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Left)
            {

                if (sender is ScopeX.UserControls.ScopeXListPage)
                {
                    // Remember the point where the mouse down occurred. The DragSize indicates
                    // the size that the mouse can move before a drag event should be started.
                    Size ds = SystemInformation.DragSize;

                    // Create a rectangle using the DragSize, with the mouse position being
                    // at the center of the rectangle.
                    _DragBox = new Rectangle(new Point(e.X - (ds.Width / 2), e.Y - (ds.Height / 2)), ds);
                    _IsSetParent = false;

                }
            }
        }

        private void LpItemDragSource_MouseUp(Object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _DragBox = Rectangle.Empty;
            }
        }

        private void LpItemDragSource_MouseMove(Object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                // If the mouse moves outside the rectangle, start the drag.
                if (_DragBox != Rectangle.Empty && !_DragBox.Contains(e.X, e.Y))
                {
                    // Proceed with the drag-and-drop, passing in the list item.
                    _DeletedItem = sender as ScopeX.UserControls.ScopeXListPage;
                    if (_DeletedItem is not null)
                    {
                        DoDragDrop(_DeletedItem, DragDropEffects.Move);
                        _IsSetParent = false;
                    }
                }
            }
        }

        private void LpItemDragSource_GiveFeedback(Object sender, GiveFeedbackEventArgs e)
        {
            // Sets the custom cursor based upon the effect.
            e.UseDefaultCursors = false;
            if ((e.Effect & DragDropEffects.Move) == DragDropEffects.Move)
            {
                Cursor.Current = Cursors.Hand;
            }
            else
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void LpItemDragSource_QueryContinueDrag(Object sender, QueryContinueDragEventArgs e)
        {
            // Cancel the drag if the mouse moves off the form.
            if (_DeletedItem is not null)
            {
                if (e.EscapePressed)
                {
                    e.Action = DragAction.Cancel;
                    _DeletedItem = null;
                }
            }
        }

        //private void DsoResultStrip_DragLeave(object sender, EventArgs e)
        //{
        //    if (_DeletedItem is not null)
        //    {
        //        if (_DeletedItem.Name.Contains("LpMeasure"))
        //        {
        //            Int32 idx = Int32.Parse(_DeletedItem.Header[1..]) - 1;
        //            MeasPresenter[idx].Active = false;
        //        }
        //        else if (_DeletedItem.Name.Contains("LpCymometer"))
        //        {
        //            CymometerPresenter.Active = false;
        //        }
        //        else if (_DeletedItem.Name.Contains("LpVoltmeter"))
        //        {
        //            VoltmeterPresenter.Active = false;
        //        }

        //        _DeletedItem = null;
        //    }
        //}
        #endregion

        //!!!Change focus to DsoResultStrip
        private void TlpBody_MouseDown(Object sender, MouseEventArgs e)
        {
            if (ActiveControl is null)
            {
                SelectNextControl(ActiveControl, true, true, true, true);
            }
        }

        private void BtnAddMeasure_Click(Object sender, EventArgs e)
        {
            var appenditem = MeasPresenter.LastChangedItem != null ? MeasPresenter.LastChangedItem :
                                        (MeasPresenter.SelectedItems.Any(x => x.Active) ? MeasPresenter.SelectedItems.Last(x => x.Active) :
                                        MeasPresenter.SelectedItems.First(x => !x.Active));
            MeasSelectionForm msf = new(appenditem.Name, DsoPrsnt.FocusId, (n, s) =>
            {
                if (MeasPresenter.IsAllActive())
                {
                    WeakTip.Default.Write("Measure", MsgTipId.NoMoreMeasuerLabel);
                    return false;
                }
                var mi = MeasPresenter.SelectedItems.FirstOrDefault(o => o.Active && o.Name == n && o.Source == s);
                if (mi != null)
                {
                    WeakTip.Default.Write("Measure", MsgTipId.MeasuerLabelExisted);
                    return false;
                }

                var ai = MeasPresenter.SelectedItems.First(o => !o.Active);
                ai.MeasureType = MeasureType.Single;
                ai.Name = n;
                ai.Source = s;
                ai.Active = true;

                return true;
            }, false)
            {
                StartPosition = FormStartPosition.CenterScreen,
            };
            _IsSetParent = false;

            msf.ShowDialogByEvent();
            //if (msf.DialogResult == DialogResult.Yes)
            //{
            //    appenditem.Name = msf.SelectedItemName;
            //    appenditem.Source = msf.Source;
            //    appenditem.Active = true;
            //}
        }
    }
}

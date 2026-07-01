using EventBus;
using ScopeX.ComModel;
using ScopeX.Controls.Common.Default;
using ScopeX.Controls.Common.Helper;
using ScopeX.Controls.Language;
using ScopeX.Core;
using ScopeX.Core.Tools;
using ScopeX.Core.Tools.DataExport;
using ScopeX.UserControls.Style;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScopeX.U2
{
    public partial class DataExportPage : UserControl, IFileView, IStylize
    {
        private Boolean _ArgToCtrl;

        public DataExportPage()
        {
            InitializeComponent();
            InitLangControl();
            TbxFileName.Text = "Measure";
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            base.OnHandleDestroyed(e);
            DsoPrsnt.DefaultDsoPrsnt.Measure.PublisherChanged -= Measure_PublisherChanged;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            DefaultStyleManager.Instance.RegisterControlRecursion(ChkMeasure, StyleFlag.FontSize);
            DsoPrsnt.DefaultDsoPrsnt.Measure.PublisherChanged += Measure_PublisherChanged;
            ChkMeasure.Enabled = DsoPrsnt.DefaultDsoPrsnt.Measure.Active;
            ChkMeasure.Checked = DsoPrsnt.DefaultDsoPrsnt.Measure.Active;
            BtnSave.Enabled = ChkMeasure.Enabled && ChkMeasure.Checked;
        }

        private void Measure_PublisherChanged(object sender, CustomEventArg e)
        {
            ChkMeasure.Enabled = DsoPrsnt.DefaultDsoPrsnt.Measure.Active;
            ChkMeasure.Checked = DsoPrsnt.DefaultDsoPrsnt.Measure.Active;
            BtnSave.Enabled = ChkMeasure.Enabled && ChkMeasure.Checked;
        }

        private void InitLangControl()
        {
            ChkMeasure.Text = LanguageManger.Instance.GetIDMessage("CanShuCeLiang");
            BtnSave.Text = LanguageManger.Instance.GetIDMessage("BtnSaveText");
            LabelFileName.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("MingCheng");
            LblTime.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("RiQiHouZhui");
            ChkSuffix.CheckedText = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Kai");
            ChkSuffix.Text= ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Guan");
        }

        [Description("是否风格化"), Browsable(true), DefaultValue(typeof(Boolean)), Category(Const.Category)]
        public bool StylizeFlag { get; set; } = true;

        [Browsable(false)]
        public StyleFlag StyleFlags { get; set; } = StyleFlag.FontSize;

        public FilePrsnt Presenter
        {
            get => (FilePrsnt)(ParentForm as IFileView).Presenter;
            set => (ParentForm as IFileView).Presenter = value;
        }

        IFilePrsnt IView<IFilePrsnt>.Presenter
        {
            get => Presenter;
            set => Presenter = (FilePrsnt)value;
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

        public void UpdateView(object prsnt, string propertyName)
        {
            if (String.IsNullOrEmpty(propertyName))
            {
                UpdateView();
                return;
            }

            _ArgToCtrl = true;

            // TODO:

            _ArgToCtrl = false;
        }

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                _ArgToCtrl = true;
                //RdoRegion.ChoosedButtonIndex = Presenter.PicRegion == PicArea.Application ? 0 : 1;
                _ArgToCtrl = false;
            }
        }

        private readonly String[] _DualSrcMeasItems =
        {
            "Delay@lv",
            "Phase@lv",
            "Setup",
            "Hold",
            "Crossing"
        };

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                string defaultfilename = TbxFileName.Text;
                if (ChkSuffix.Checked)
                {
                    defaultfilename += $"_{DateTime.Now.ToString("yyyyMMddHHmmssfff")}";
                }
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.FileName = defaultfilename;
                dialog.Filter = "Text(*.txt)|*.txt|Excel(*.xls)|*.xls";
                dialog.SupportMultiDottedExtensions = false;
                dialog.OverwritePrompt = true;
                dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                dialog.SetWindowInCenter();
                if (dialog.ShowDialog() != DialogResult.OK || String.IsNullOrWhiteSpace(dialog.FileName))
                    return;

                var ext = System.IO.Path.GetExtension(dialog.FileName).ToUpper();
                FileType fileType;
                switch (ext)
                {
                    case ".TXT":
                        fileType = FileType.Text;
                        break;
                    case ".XLS":
                        fileType = FileType.Excel;
                        break;
                    default:
                        StrongTip.Default.Show(MsgTipId.Warning, MsgTipId.UnSupportedFormat, MessageType.Warning);
                        return;
                }

                List<DataTable> tables = new List<DataTable>();
                if (ChkMeasure.Checked)
                {
                    // 下面代码参考 DsoResultStrip.cs代码中的UpdateView()函数逻辑
                    var candidates = MeasureApp.Default.MeasCandidates;
                    var prsnts = DsoPrsnt.DefaultDsoPrsnt.Measure.SelectedItems.Where(c => c.Active);
                    if (prsnts == null || !prsnts.Any())
                        return;

                    MeasItemPrsnt item = null;
                    DataTable dt = new DataTable() { TableName = LanguageManger.Instance.GetIDMessage("CanShuCeLiang") };
                    dt.Columns.Add(" ");
                    dt.Columns.Add("  ");
                    dt.Columns.Add("   ");
                    dt.Columns.Add("    ");

                    DataRow dr;

                    for (Int32 i = 0; i < DsoPrsnt.DefaultDsoPrsnt.Measure.Length; i++)
                    {
                        item = DsoPrsnt.DefaultDsoPrsnt.Measure[i];
                        if (!item.Active)
                            continue;

                        var measureName = candidates[item.Name].Text;

                        Boolean dualsrc = false;
                        foreach (var dual_item in _DualSrcMeasItems)
                        {
                            if (item.Name.Contains(dual_item))
                            {
                                dualsrc = true;
                                break;
                            }
                        }

                        var headerinfo = item.Source.ToString() + (dualsrc ? String.Concat(" ", item.Source2nd.ToString()) : String.Empty);

                        #region Value

                        var (pfx, unit) = DsoPrsnt.DefaultDsoPrsnt.Measure.GetPfxUnitString(i);

                        var value = DsoPrsnt.DefaultDsoPrsnt.Measure.GetResult(i) ?? Double.NaN;
                        var max = DsoPrsnt.DefaultDsoPrsnt.Measure.GetStatMax(i) ?? Double.NaN;
                        var min = DsoPrsnt.DefaultDsoPrsnt.Measure.GetStatMin(i) ?? Double.NaN;
                        var ave = DsoPrsnt.DefaultDsoPrsnt.Measure.GetStatAverage(i) ?? Double.NaN;
                        var stddev = DsoPrsnt.DefaultDsoPrsnt.Measure.GetStatStddev(i) ?? Double.NaN;
                        var pop = (Double)DsoPrsnt.DefaultDsoPrsnt.Measure.GetStatCount(i);

                        if (Program.Oscilloscope.TryGetChannel(item.Source, out var ch))
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

                        List<KeyValuePair<String, String>> subitems = new()
                        {
                            new(LanguageManger.Instance.GetIDMessage("Statistic_VAL"), DsoPrsnt.DefaultDsoPrsnt.Measure.CalcResultStringNow(value, pfx, unit).ToFormat(candidates[item.Name].Format)),
                            new(LanguageManger.Instance.GetIDMessage("Statistic_MAX"), DsoPrsnt.DefaultDsoPrsnt.Measure.CalcResultStringNow(max, pfx, unit).ToFormat(candidates[item.Name].Format)),
                            new(LanguageManger.Instance.GetIDMessage("Statistic_MIN"), DsoPrsnt.DefaultDsoPrsnt.Measure.CalcResultStringNow(min, pfx, unit).ToFormat(candidates[item.Name].Format)),
                            new(LanguageManger.Instance.GetIDMessage("Statistic_AVG"), DsoPrsnt.DefaultDsoPrsnt.Measure.CalcResultStringNow(ave, pfx, unit).ToFormat(candidates[item.Name].Format)),
                            new(LanguageManger.Instance.GetIDMessage("Statistic_DEV"), DsoPrsnt.DefaultDsoPrsnt.Measure.CalcResultStringNow(stddev, pfx, unit).ToFormat(candidates[item.Name].Format)),
                            new(LanguageManger.Instance.GetIDMessage("Statistic_POP"), DsoPrsnt.DefaultDsoPrsnt.Measure.CalcResultStringNow(pop, Prefix.Empty, QuantityUnit.Count.ToUnitString()).ToFormat(candidates[item.Name].Format))
                        };

                        var datasources = subitems;

                        #endregion

                        /*==================增加测量项描述=====================*/
                        dr = dt.NewRow();
                        dr[dt.Columns[0]] = $"{LanguageManger.Instance.GetIDMessage("CeLiangXiang")}：{measureName}";
                        dr[dt.Columns[1]] = $"{LanguageManger.Instance.GetIDMessage("Yuan")}：{headerinfo}";
                        dt.Rows.Add(dr);

                        if (datasources != null)
                        {
                            foreach (var datasource in datasources)
                            {
                                dr = dt.NewRow();
                                dr[dt.Columns[0]] = datasource.Key;
                                dr[dt.Columns[1]] = datasource.Value;
                                dt.Rows.Add(dr);
                            }
                        }

                        // 将多个测量项，放到一个表格中导出，使得用户方便对比
                        dt.Rows.Add(dt.NewRow()); // 增加空行间隔
                        dt.Rows.Add(dt.NewRow()); // 增加空行间隔
                        dt.Rows.Add(dt.NewRow()); // 增加空行间隔
                    }

                    tables.Add(dt);
                }

                if (!tables.Any())
                    return;

                try
                {
                    var bytes = DataExportHelper.ConvertTables2FileBytes(fileType, tables.ToArray());
                    System.IO.File.WriteAllBytes(dialog.FileName, bytes);
                    WeakTip.Default.Write("Export", MsgTipId.SavingSuccess, false, System.IO.Path.GetDirectoryName(dialog.FileName));
                }
                catch (Exception ex)
                {
                    EventBroker.Instance.GetEvent<LogEventArgs>().Publish(null, new LogEventArgs(ex, LogLevel.Error));
                    WeakTip.Default.Write("Export", MsgTipId.SavingFailed);
                }
            }
        }

        private void ChkMeasure_CheckStateChanged(object sender, EventArgs e)
        {
            BtnSave.Enabled = ChkMeasure.Enabled && ChkMeasure.Checked;
        }
    }
}

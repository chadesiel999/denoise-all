using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScopeX.UserControls;
using ScottPlot;
using ScottPlot.Plottable;
using System.Threading;
using ScopeX.UserControls.Style;
using ScopeX.U2.LanguageSupoort;

namespace ScopeX.U2
{
    public partial class TempChcekForm : FloatForm
    {
        private Double[] _XData = new Double[100];
        private Double[] _YPiceBoard = new Double[100];
        private Double[] _YProBoard = new Double[100];
        private Double[] _YAcqBoard = new Double[100];
        private Double[] _YChBoard = new Double[100];
        private Double[] _YPiceCore = new Double[100];
        private Double[] _YProCore = new Double[100];
        private Double[] _YAcqCore = new Double[100];
        private Thread _TempPlotThread = null;
        public delegate void UpdateDataToTextBoxDelegate(String piceBoardStr, String proBoardStr, String acqBoardStr,
            String chBoardStr, String piceCoreStr, String proCoreStr, String acqCoreStr, String currentTempStr);
        public event UpdateDataToTextBoxDelegate UpdateTextBox;
        public Double Ret = 0;
        #region 私有字段
        private PlotType _PlotType;
        private String _PiceBoardStr;
        private String _ProBoardStr;
        private String _AcqBoardStr;
        private String _ChBoardStr;
        private String _PiceCoreStr;
        private String _ProCoreStr;
        private String _AcqCoreStr;
        private String _CurrentTempStr;
        private Double _PiceBoardData;
        private Double _ProBoardData;
        private Double _AcqBoardData;
        private Double _ChBoardData;
        private Double _PiceCoreData;
        private Double _ProCoreData;
        private Double _AcqCoreData;
        private Boolean _OpenRet = true;
        #endregion
        public TempChcekForm()
        {
            InitializeComponent();
            PlotInit();
            UpdatePlot();
            UpdateTextBox += new UpdateDataToTextBoxDelegate(UpdateData);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Stylize();
            // LanguageFactory.CacheFormLanguageControls(this);
        }

        private void Stylize()
        {
            IsShowHelp = false;
            DefaultStyleManager.Instance.RegisterControlRecursion(this);
            
        }
        private void PlotInit()
        {
            var plt = Plot.Plot;
            plt.SetAxisLimits(0, 100, 0, 100);
            plt.SetViewLimits(xMin: -32768, xMax: 32768, yMin: -10000, yMax: 10000);
            plt.Style(figureBackground: Color.Transparent, dataBackground: Color.Transparent, grid: Color.Gray, tick: Color.Gray, axisLabel: Color.Gray, titleLabel: Color.Gray);
            plt.GridCrosslineVisible(false);
            Plot.BackColor = Color.Black;
            plt.YAxis.MinorLogScale(false);
            plt.ResetChannelParameter(0, 10, "", "", 10, false);
            plt.XAxis.MinorLogScale(false);
            plt.ResetTimebaseParameter(0, 10, "", "", 10, false);
            //X轴数据
            for (int i = 0; i < _XData.Length; i++)
            {
                _XData[i] = i + 1;
                _YPiceBoard[i] = 0;
                _YProBoard[i] = 0;
                _YAcqBoard[i] = 0;
                _YChBoard[i] = 0;
                _YPiceCore[i] = 0;
                _YProCore[i] = 0;
                _YAcqCore[i] = 0;
            }
            Plot.Render();
            //formsPlot1.Refresh();
        }

        protected void UpdatePlot()
        {
            _TempPlotThread = new Thread(() =>
            {

                while (true)
                {
                    try
                    {
                        Random r = new Random();
                        _PiceBoardData = r.Next(0, 100);
                        _ProBoardData = r.Next(0, 100);
                        _AcqBoardData = r.Next(0, 100);
                        _ChBoardData = r.Next(0, 100);
                        _PiceCoreData = r.Next(0, 100);
                        _ProCoreData = r.Next(0, 100);
                        _AcqCoreData = r.Next(0, 100);
                        _PiceBoardStr = _PiceBoardData + " °C";
                        _ProBoardStr = _ProBoardData + " °C";
                        _AcqBoardStr = _AcqBoardData + " °C";
                        _ChBoardStr = _ChBoardData + " °C";
                        _PiceCoreStr = _PiceCoreData + " °C";
                        _ProCoreStr = _ProCoreData + " °C";
                        _AcqCoreStr = _AcqCoreData + " °C";
                        _CurrentTempStr = _PiceBoardStr;
                        switch (_PlotType)
                        {
                            case PlotType.PiceBoard:
                                Plot.Plot.Clear();
                                for (int i = 0; i < _YPiceBoard.Length - 1; i++)
                                    _YPiceBoard[i] = _YPiceBoard[i + 1];
                                _YPiceBoard[_YPiceBoard.Length - 1] = _PiceBoardData;
                                _CurrentTempStr = _PiceBoardStr;
                                Plot.Plot.AddScatter(_XData, _YPiceBoard);
                                break;
                            case PlotType.ProBoard:
                                Plot.Plot.Clear();
                                for (int i = 0; i < _YProBoard.Length - 1; i++)
                                    _YProBoard[i] = _YProBoard[i + 1];
                                _YProBoard[_YProBoard.Length - 1] = _ProBoardData;
                                _CurrentTempStr = _ProBoardStr;
                                Plot.Plot.AddScatter(_XData, _YProBoard);
                                break;
                            case PlotType.AcqBoard:
                                Plot.Plot.Clear();
                                for (int i = 0; i < _YAcqBoard.Length - 1; i++)
                                    _YAcqBoard[i] = _YAcqBoard[i + 1];
                                _YAcqBoard[_YAcqBoard.Length - 1] = _AcqBoardData;
                                _CurrentTempStr = _AcqBoardStr;
                                Plot.Plot.AddScatter(_XData, _YAcqBoard);
                                break;
                            case PlotType.ChBoard:
                                Plot.Plot.Clear();
                                for (int i = 0; i < _YChBoard.Length - 1; i++)
                                    _YChBoard[i] = _YChBoard[i + 1];
                                _YChBoard[_YChBoard.Length - 1] = _ChBoardData;
                                _CurrentTempStr = _ChBoardStr;
                                Plot.Plot.AddScatter(_XData, _YChBoard);
                                break;
                            case PlotType.PiceCore:
                                Plot.Plot.Clear();
                                for (int i = 0; i < _YPiceCore.Length - 1; i++)
                                    _YPiceCore[i] = _YPiceCore[i + 1];
                                _YPiceCore[_YPiceCore.Length - 1] = _PiceCoreData;
                                _CurrentTempStr = _PiceCoreStr;
                                Plot.Plot.AddScatter(_XData, _YPiceCore);
                                break;
                            case PlotType.ProCore:
                                Plot.Plot.Clear();
                                for (int i = 0; i < _YProCore.Length - 1; i++)
                                    _YProCore[i] = _YProCore[i + 1];
                                _YProCore[_YProCore.Length - 1] = _ProCoreData;
                                _CurrentTempStr = _ProCoreStr;
                                Plot.Plot.AddScatter(_XData, _YProCore);
                                break;
                            case PlotType.AcqCore:
                                Plot.Plot.Clear();
                                for (int i = 0; i < _YAcqCore.Length - 1; i++)
                                    _YAcqCore[i] = _YAcqCore[i + 1];
                                _YAcqCore[_YAcqCore.Length - 1] = _AcqCoreData;
                                _CurrentTempStr = _AcqCoreStr;
                                Plot.Plot.AddScatter(_XData, _YAcqCore);
                                break;
                        }
                        Plot.Render();
                        Thread.Sleep(50);
                        if (UpdateTextBox != null)
                        {
                            UpdateTextBox.Invoke(_PiceBoardStr, _ProBoardStr, _AcqBoardStr, _ChBoardStr, _PiceCoreStr, _ProCoreStr, _AcqCoreStr, _CurrentTempStr);
                        }

                    }
                    catch (ThreadAbortException) { return; }
                    catch (Exception)
                    {
                        return;
                    }
                    Thread.Sleep(1000);
                }
            })
            { Name = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("WenDuQuXianHuiZhi"), IsBackground = true };
            _TempPlotThread.Start();
        }

        public void UpdateData(string PiceBoardStr, string ProBoardStr, string AcqBoardStr, string ChBoardStr, string PiceCoreStr, string ProCoreStr, string AcqCoreStr, string CurrentTempStr)
        {
            if (!this.CurrentTemp.InvokeRequired)
            {
                CurrentTemp.Text = CurrentTempStr;
            }
            else
            {

                this.CurrentTemp.Invoke(new UpdateDataToTextBoxDelegate(UpdateData), PiceBoardStr, ProBoardStr, AcqBoardStr, ChBoardStr, PiceCoreStr, ProCoreStr, AcqCoreStr, CurrentTempStr);
            }

            if (!this.PiceBoard.InvokeRequired)
            {
                PiceBoard.Text = PiceBoardStr;
            }
            else
            {

                this.PiceBoard.Invoke(new UpdateDataToTextBoxDelegate(UpdateData), PiceBoardStr, ProBoardStr, AcqBoardStr, ChBoardStr, PiceCoreStr, ProCoreStr, AcqCoreStr, CurrentTempStr);
            }

            if (!this.ProBoard.InvokeRequired)
            {
                ProBoard.Text = ProBoardStr;
            }
            else
            {

                this.ProBoard.Invoke(new UpdateDataToTextBoxDelegate(UpdateData), PiceBoardStr, ProBoardStr, AcqBoardStr, ChBoardStr, PiceCoreStr, ProCoreStr, AcqCoreStr, CurrentTempStr);
            }

            if (!this.AcqBoard.InvokeRequired)
            {
                AcqBoard.Text = AcqBoardStr;
            }
            else
            {

                this.AcqBoard.Invoke(new UpdateDataToTextBoxDelegate(UpdateData), PiceBoardStr, ProBoardStr, AcqBoardStr, ChBoardStr, PiceCoreStr, ProCoreStr, AcqCoreStr, CurrentTempStr);
            }

            if (!this.ChBoard.InvokeRequired)
            {
                ChBoard.Text = ChBoardStr;
            }
            else
            {

                this.ChBoard.Invoke(new UpdateDataToTextBoxDelegate(UpdateData), PiceBoardStr, ProBoardStr, AcqBoardStr, ChBoardStr, PiceCoreStr, ProCoreStr, AcqCoreStr, CurrentTempStr);
            }

            if (!this.PiceCore.InvokeRequired)
            {
                PiceCore.Text = PiceCoreStr;
            }
            else
            {

                this.PiceCore.Invoke(new UpdateDataToTextBoxDelegate(UpdateData), PiceBoardStr, ProBoardStr, AcqBoardStr, ChBoardStr, PiceCoreStr, ProCoreStr, AcqCoreStr, CurrentTempStr);
            }

            if (!this.ProCore.InvokeRequired)
            {
                ProCore.Text = ProCoreStr;
            }
            else
            {

                this.ProCore.Invoke(new UpdateDataToTextBoxDelegate(UpdateData), PiceBoardStr, ProBoardStr, AcqBoardStr, ChBoardStr, PiceCoreStr, ProCoreStr, AcqCoreStr, CurrentTempStr);
            }

            if (!this.AcqCore.InvokeRequired)
            {
                AcqCore.Text = AcqCoreStr;
            }
            else
            {

                this.AcqCore.Invoke(new UpdateDataToTextBoxDelegate(UpdateData), PiceBoardStr, ProBoardStr, AcqBoardStr, ChBoardStr, PiceCoreStr, ProCoreStr, AcqCoreStr, CurrentTempStr);
            }
        }

        private void TempChcekForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_TempPlotThread != null && _TempPlotThread.IsAlive)
            {
                _TempPlotThread.Join(50);
                _TempPlotThread = null;
            }
        }

        private void TempSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            _PlotType = (PlotType)TempSource.SelectedIndex;
        }
        private enum PlotType
        {
            PiceBoard = 0,
            ProBoard = 1,
            AcqBoard = 2,
            ChBoard = 3,
            PiceCore = 4,
            ProCore = 5,
            AcqCore = 6
        }

        private void FanControl_CheckedChangedEvent(object sender, EventArgs e)
        {
            _OpenRet = !_OpenRet;
            if (_OpenRet)
            {

            }
            else
            {

            }
        }
    }
}

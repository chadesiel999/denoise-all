using PdfSharpCore.Internal;
using ScopeX.ComModel;
using ScopeX.Core;
using ScopeX.U2;
using ScopeX.UserControls;
using ScopeX.UserControls.Style;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using ScopeX.Core.Tools;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static ScopeX.Controls.Common.APIs.APIsStructs;
using static System.Windows.Forms.AxHost;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Diagnostics.Eventing.Reader;

namespace ScopeX.U2
{
    public enum ProbeUserCaliStep
    {
        ProbeCaliStep_Finished,         //空闲(位开始)
        ProbeCaliStep_MeanEnable,       //启动测量

        ProbeCaliStep_GainCaliInit,     //增益校准初始化
        ProbeCaliStep_GainCaliCheck,    //连接检查
        ProbeCaliStep_GainCali,         //增益校准

        ProbeCaliStep_OffsetCaliInit,   //偏置校准初始化
        ProbeCaliStep_OffsetCaliCheck,  //连接检查1
        ProbeCaliStep_OffsetCali,       //偏置校准
        ProbeCaliStep_Saving,           //保存校准数据

        ProbeCaliStep_Botton,         //空闲(位开始)
    }

    public enum ProbeCaliItemState
    {
        CaliItemState_Todo,     //待做
        CaliItemState_Doing,    //进行中
        CaliItemState_Cancel,   //待做

        CaliItemState_Succ,     //成功
        CaliItemState_Fail,     //失败
        CaliItemState_CkSucc,   //检查通过
        CaliItemState_CkFail,   //检查不通过
    }


    public partial class ProbeUserCaliPage : UserControl
    {

        //校准线程
        private CancellationTokenSource? _CancellTokenSrc;
        private Task? _WorkTask;
        private Boolean _WorkIsOn;

        //校准结果值
        private Double _ProbeGainRatio;
        private Double _ProbeOffsetVal;

        //校准状态机
        private ProbeUserCaliStep _OldSetp = ProbeUserCaliStep.ProbeCaliStep_Botton;
        private ProbeUserCaliStep _CurrentSetp = ProbeUserCaliStep.ProbeCaliStep_Botton;
        private List<ProbeUserCaliItem> _AllCaliItem = new List<ProbeUserCaliItem>();

        //校准依赖
        private ChannelId _ProbeChlNo;
        private AnalogPrsnt _ChlPresenter;
        private MeasPrsnt _MeaPresenter;
        private TimebasePrsnt _TimebasePrsnt;

        //输入源检查次数
        private readonly String Amp = "Amplitude";
        private readonly String Avg = "Average";
        private Double _RefSigTheoryAmp = 3000; //参考信息的幅度
        private Double _RefSigAmpDealta = 0.25; //参考信息误差
        private Double _RefSigNowMeaAmp = 0.0;

        private int _InputSrorceChkTotal = 3;
        private int _InputSrorceChkCount = 0;
        private int _CaliOffsetCount = 0;

        private Int32 _HasCaliCount = 0;
        private String _ShowResultInfo = "";
        private Boolean _IsDifferProbe = false;
        public ProbeUserCaliPage(ChannelId channelId)
        {
            InitializeComponent();
            _ProbeChlNo = channelId;
            IChnlPrsnt cprsnt;
            if (DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(_ProbeChlNo, out cprsnt))
            {
                _ChlPresenter = (AnalogPrsnt)cprsnt;
            }
            _MeaPresenter = DsoPrsnt.DefaultDsoPrsnt.Measure;
            _TimebasePrsnt = DsoPrsnt.DefaultDsoPrsnt.Timebase;

            InitControlLang();
            Stylize();
            InitInfo();
            InitLvCalib();
            UpdateView();

            //隐藏列表，显示进度条
            LvCalibItems.Visible = false;
            //打开列表，隐藏进度条
            //LvCalibItems.Visible = true;
            //PgBar.Visible = false;
            //LblInfo.Visible = false;
        }

        #region 界面函数

        private void InitControlLang()
        {
            LblSpaceFrst.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ProbeUserCaliForm.Note");
            BtnCalib.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ProbeUserCaliForm.Ok");
            BtnCancel.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ProbeUserCaliForm.Cancel");

            BtnReadCali.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ProbeUserCaliForm.Read");
            BtnClrCali.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ProbeUserCaliForm.Clear");
            if (DsoPrsnt.DefaultDsoPrsnt.SquareWaveSwitch)
            {
                BtnCloseRef.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ProbeUserCaliForm.CloseRef");
            }
            else
            {
                BtnCloseRef.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ProbeUserCaliForm.OpenRef");
            }
            BtnReadCali.Visible = false;
            BtnClrCali.Visible = false;
            BtnCloseRef.Visible = false;
            BtnOther.Visible = false;
            if (_ChlPresenter!.SerailNumber.Contains("UT-PD"))
            {
                _IsDifferProbe = true;
            }
            else
            {
                _IsDifferProbe = false;
            }
            LblSpaceFrst.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ProbeUserCaliForm.Note");
            //PbWiring.Image = _ChlPresenter.GetProbeType(_ProbeChlNo) == ProbeType.Singled ? Properties.Resources.ProbeSingleCalib1 : Properties.Resources.ProbeDiffCalib1;
            PbWiring.Image = !_IsDifferProbe ? Properties.Resources.ProbeSingleCalib : Properties.Resources.ProbeDiffCalib;
        }

        private void Stylize()
        {
            DefaultStyleManager.Instance.RegisterControlRecursion(this, StyleFlag.FontSize);
        }

        /// <summary>
        /// 初始化各标签的内容信息
        /// </summary>
        private void InitInfo()
        {
        }

        /// <summary>
        /// 初始化校准步骤列表
        /// </summary>
        private void InitLvCalib()
        {
            LvCalibItems.HeaderForeColor = AppStyleConfig.DefaultTitleForeColor;
            LvCalibItems.HeaderBackColor = AppStyleConfig.DefaultTitleBackColor.GetBrightnessColor(0.05);
            LvCalibItems.ForeColor = AppStyleConfig.DefaultContextForeColor;
            LvCalibItems.BackColor = AppStyleConfig.DefaultContextBackColor.GetBrightnessColor(0.05);
            LvCalibItems.SelectedRowColor = AppStyleConfig.DefaultCheckedBackColor;
            LvCalibItems.Font = AppStyleConfig.DefaultLabelFont;

            LvCalibItems.Columns.Add(ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ProbeUserCaliForm.Item.Status"), 90);
            LvCalibItems.Columns.Add(ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ProbeUserCaliForm.Item.Step"), -2);
            InitAllOptionInfo();
        }

        private void InitAllOptionInfo()
        {
            _AllCaliItem.Add(new ProbeUserCaliItem()
            {
                Setp = ProbeUserCaliStep.ProbeCaliStep_MeanEnable,
                State = ProbeCaliItemState.CaliItemState_Todo,
                SetpDescription = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ProbeUserCaliForm.Item.Step.MeanEnable")
            });

            ///
            _AllCaliItem.Add(new ProbeUserCaliItem()
            {
                Setp = ProbeUserCaliStep.ProbeCaliStep_GainCaliInit,
                State = ProbeCaliItemState.CaliItemState_Todo,
                SetpDescription = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ProbeUserCaliForm.Item.Step.GainCaliInit")
            });
            _AllCaliItem.Add(new ProbeUserCaliItem()
            {
                Setp = ProbeUserCaliStep.ProbeCaliStep_GainCaliCheck,
                State = ProbeCaliItemState.CaliItemState_Todo,
                SetpDescription = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ProbeUserCaliForm.Item.Step.GainCaliCheck")
            });
            _AllCaliItem.Add(new ProbeUserCaliItem()
            {
                Setp = ProbeUserCaliStep.ProbeCaliStep_GainCali,
                State = ProbeCaliItemState.CaliItemState_Todo,
                SetpDescription = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ProbeUserCaliForm.Item.Step.GainCalib")
            });


            _AllCaliItem.Add(new ProbeUserCaliItem()
            {
                Setp = ProbeUserCaliStep.ProbeCaliStep_OffsetCaliInit,
                State = ProbeCaliItemState.CaliItemState_Todo,
                SetpDescription = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ProbeUserCaliForm.Item.Step.OffsetCaliInit")
            });
            _AllCaliItem.Add(new ProbeUserCaliItem()
            {
                Setp = ProbeUserCaliStep.ProbeCaliStep_OffsetCaliCheck,
                State = ProbeCaliItemState.CaliItemState_Todo,
                SetpDescription = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ProbeUserCaliForm.Item.Step.OffsetCaliCheck")
            });
            _AllCaliItem.Add(new ProbeUserCaliItem()
            {
                Setp = ProbeUserCaliStep.ProbeCaliStep_OffsetCali,
                State = ProbeCaliItemState.CaliItemState_Todo,
                SetpDescription = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ProbeUserCaliForm.Item.Step.OffsetCalib")
            });

            ///
            _AllCaliItem.Add(new ProbeUserCaliItem()
            {
                Setp = ProbeUserCaliStep.ProbeCaliStep_Saving,
                State = ProbeCaliItemState.CaliItemState_Todo,
                SetpDescription = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ProbeUserCaliForm.Item.Step.SaveCaliData")
            });
        }

        protected void Update(ProbeUserCaliStep step, ProbeCaliItemState state)
        {
            if (step == ProbeUserCaliStep.ProbeCaliStep_Botton)
            {
                _AllCaliItem.ForEach((item) =>
                {
                    item.State = state;
                });
            }
            else
            {
                _AllCaliItem.ForEach((item) =>
                {
                    if (item.Setp == step)
                    {
                        item.State = state;
                    }
                });
            }
        }

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                LvCalibItems.BeginUpdate();
                Int32 row = 0;
                foreach (var item in _AllCaliItem)
                {
                    if (row == LvCalibItems.Items.Count)
                    {
                        LvCalibItems.Items.Add(new ListViewItem(new String[LvCalibItems.Columns.Count]));
                    }
                    LvCalibItems.Items[row].SubItems[0].Text = Cvt(item.State);
                    LvCalibItems.Items[row].SubItems[1].Text = item.SetpDescription;
                    row++;
                }
                LvCalibItems.EndUpdate();

                BtnsStateUpdate();
            }
        }

        #endregion

        #region 操作响应

        private bool MsgStrongTip(MsgTipId MsgTitleId, MsgTipId MsgContentId, MessageType MsgType, params object[] args)
        {
            if (InvokeRequired)
            {
                var res = StrongTip.Default.Show(MsgTitleId, MsgContentId, MsgType, args);
            }
            else
            {

            }

            return true;
        }

        private void BtnCalib_Click(object sender, EventArgs e)
        {
            if (_WorkTask != null && (_WorkTask.Status == TaskStatus.Created
                || _WorkTask.Status == TaskStatus.WaitingForActivation
                || _WorkTask.Status == TaskStatus.WaitingToRun
                || _WorkTask.Status == TaskStatus.Running
                || _WorkTask.Status == TaskStatus.WaitingForChildrenToComplete
                ))
            {
                return;
            }
            LblInfo.Invoke((MethodInvoker)delegate
            {
                LblInfo.Text = "";
            });
            PgBar.BeginInvoke((MethodInvoker)delegate
            {
                PgBar.Value = 0;
            });
            _HasCaliCount = 0;
            _WorkIsOn = true;
            _CaliOffsetCount = 0;
            _InputSrorceChkCount = 0;
            _CurrentSetp = ProbeUserCaliStep.ProbeCaliStep_MeanEnable;
            if (!_ChlPresenter!.Active)
            {
                _ChlPresenter.Active = true;
            }
            if (_ChlPresenter!.SerailNumber.Contains("UT-PD"))
            {
                _IsDifferProbe = true;
            }
            else
            {
                _IsDifferProbe = false;
            }
            TodoProbeCalib();
            Update(ProbeUserCaliStep.ProbeCaliStep_Botton, ProbeCaliItemState.CaliItemState_Todo);
            UpdateView();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            if (_CancellTokenSrc != null)
            {
                _CancellTokenSrc.Cancel();
            }
            _WorkIsOn = false;
            {//恢复示波器合适档位
                DsoPrsnt.DefaultDsoPrsnt.SquareWaveSwitch = true;
                _ChlPresenter.Bias = 0;                                  //Bias=0
                _ChlPresenter.PosIndexBymDiv = 0;                        //0Div
                _ChlPresenter.ScaleIndex = (int)ScopeX.Core.AnaChnlScaleIndex.Lv100m; //x10为500mV 初设大档位避免信号超出屏幕外无法测量 
                _TimebasePrsnt.Mode = AnaChnlAcqMode.Normal;
            }
            _CurrentSetp = ProbeUserCaliStep.ProbeCaliStep_Botton;
            LblInfo.Invoke((MethodInvoker)delegate
            {
                LblInfo.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ProbeUserCaliForm.Title") + ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ProbeUserCaliForm.Item.Status.Cancel");
            });
            Update(ProbeUserCaliStep.ProbeCaliStep_Botton, ProbeCaliItemState.CaliItemState_Cancel);
            UpdateView();
        }

        private void TodoProbeCalib()
        {
            _CancellTokenSrc = new();
            _WorkTask = new Task(() =>
            {
                try
                {
                    do
                    {
                        Update(_CurrentSetp, ProbeCaliItemState.CaliItemState_Doing);
                        UpdateByWorker();

                        Thread.Sleep(500);
                        if (_CancellTokenSrc.IsCancellationRequested)
                        {
                            PgBar.Invoke((MethodInvoker)delegate
                            {
                                PgBar.Value = 0;
                            });
                            break;
                        }

                        Int32 index = _AllCaliItem.FindIndex(o=>o.Setp == _CurrentSetp);
                        if (ProbeCalibHand() == ProbeUserCaliStep.ProbeCaliStep_Finished)
                        {
                            if (_AllCaliItem[_AllCaliItem.Count() - 1].State == ProbeCaliItemState.CaliItemState_Succ)
                            {
                                LblInfo.Invoke((MethodInvoker)delegate
                                {
                                    LblInfo.Text += _AllCaliItem[index].SetpDescription + ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ProbeUserCaliForm.Item.Status.Succ") + "!\n";
                                    LblInfo.Text += ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ProbeUserCaliForm.Title") + ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ProbeUserCaliForm.Item.Status.AllComplete") + "!";
                                    _HasCaliCount = 0;
                                });
                                PgBar.Invoke((MethodInvoker)delegate
                                {
                                    PgBar.Value = 100;
                                });
                            } 
                            else
                            {
                                LblInfo.Invoke((MethodInvoker)delegate
                                {
                                    LblInfo.Text = _AllCaliItem[index].SetpDescription + ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ProbeUserCaliForm.Item.Status.Fail") + "!";
                                });
                            }
                            break;
                        }
                        if (_OldSetp != _CurrentSetp && PgBar.InvokeRequired) {
                            PgBar.Invoke((MethodInvoker)delegate
                            {
                                if (++_HasCaliCount < _AllCaliItem.Count())
                                {
                                    PgBar.Value = _HasCaliCount * 100 / _AllCaliItem.Count(); ;
                                }
                            });
                            if (index == 3 || index == 6)
                            {
                                LblInfo.Invoke((MethodInvoker)delegate
                                {
                                    LblInfo.Text += _AllCaliItem[index].SetpDescription + ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ProbeUserCaliForm.Item.Status.Succ") + "!\n";
                                });
                            }
                            _OldSetp = _CurrentSetp;
                        }

                        UpdateByWorker();

                    } while (true);
                }
                catch (Exception)
                {
                }
                
                DsoPrsnt.DefaultDsoPrsnt.SquareWaveSwitch = true;
                _ChlPresenter.Bias = 0;                                  //Bias=0
                _ChlPresenter.PosIndexBymDiv = 0;                        //0Div
                _ChlPresenter.ScaleIndex = (int)ScopeX.Core.AnaChnlScaleIndex.Lv100m; //x10为500mV 初设大档位避免信号超出屏幕外无法测量 
                _TimebasePrsnt.Mode = AnaChnlAcqMode.Normal;
                _WorkIsOn = false;
                UpdateByWorker();
            }, _CancellTokenSrc.Token, TaskCreationOptions.LongRunning | TaskCreationOptions.PreferFairness);
            _WorkTask.Start();
        }

        private ProbeUserCaliStep ProbeCalibHand()
        {
            switch (_CurrentSetp)
            {
                case ProbeUserCaliStep.ProbeCaliStep_MeanEnable:
                    return ProbeCalibToMeanEnable();
                case ProbeUserCaliStep.ProbeCaliStep_GainCaliInit:
                    return ProbeCalibToGainCaliInit();
                case ProbeUserCaliStep.ProbeCaliStep_GainCaliCheck:
                    return ProbeCalibToGainCaliCheck();
                case ProbeUserCaliStep.ProbeCaliStep_GainCali:
                    return ProbeCalibToGainCali();
                case ProbeUserCaliStep.ProbeCaliStep_OffsetCaliInit:
                    return ProbeCalibToOffsetCaliInit();
                case ProbeUserCaliStep.ProbeCaliStep_OffsetCaliCheck:
                    return ProbeCalibToOffsetCaliCheck();
                case ProbeUserCaliStep.ProbeCaliStep_OffsetCali:
                    return ProbeCalibToOffsetCali();
                case ProbeUserCaliStep.ProbeCaliStep_Saving:
                    return ProbeCalibToSaving();
                default:
                    return ProbeUserCaliStep.ProbeCaliStep_Finished;
            }
        }

        /// <summary>
        /// 打开测量项目
        /// </summary>
        private ProbeUserCaliStep ProbeCalibToMeanEnable()
        {
            //将校准数据清除
            _ChlPresenter.ProbeOffsetCaliBias = 0;
            _ChlPresenter.ProbeGainCaliRatio = 1;

            Update(_CurrentSetp, ProbeCaliItemState.CaliItemState_CkSucc);
            return _CurrentSetp = ProbeUserCaliStep.ProbeCaliStep_GainCaliInit;
        }

        /// <summary>
        /// ProbeCaliStep_GainCaliInit
        /// 增益校准初始化
        /// </summary>
        /// <returns></returns>
        private ProbeUserCaliStep ProbeCalibToGainCaliInit()
        {
            //增益校准初始化

            DsoPrsnt.DefaultDsoPrsnt.SquareWaveSwitch = true;       //打开方波
            _ChlPresenter.Bias = 0;                                 //Bias=0
            _ChlPresenter.PosIndexBymDiv = -3000;                   //-3Div
            _ChlPresenter.ScaleIndex = (int)ScopeX.Core.AnaChnlScaleIndex.Lv50m;//x10为500mV

            _TimebasePrsnt.ClockSrc = AnaChnlClkSrc.Inner;          //??
            _TimebasePrsnt.EnhancedBitsActive = false;              //??
            _TimebasePrsnt.StorageMode = AnaChnlStorageMode.Long;   //??
            _TimebasePrsnt.ScaleIndex = ScopeX.Core.AnaChnlTimebaseIndex.Lv200u;
            _TimebasePrsnt.StorageDepthOpt = (int)AnaChnlLengthOpt.Of25KDots;
            _TimebasePrsnt.Mode = AnaChnlAcqMode.HighRes;

            Update(_CurrentSetp, ProbeCaliItemState.CaliItemState_Succ);
            return _CurrentSetp = ProbeUserCaliStep.ProbeCaliStep_GainCaliCheck;
        }

        /// <summary>
        /// ProbeCaliStep_GainCaliCheck
        /// 增益校准检查
        /// </summary>
        /// <returns></returns>
        private ProbeUserCaliStep ProbeCalibToGainCaliCheck()
        {
            //增益校准输入方波检测
            if (_InputSrorceChkCount < _InputSrorceChkTotal)
            {
                _InputSrorceChkCount++;
                var amp = _MeaPresenter.CalcResultNow(Amp, _ChlPresenter.Id);
                if (amp == null)
                {
                    return _CurrentSetp;
                }

                var diffVal = _RefSigTheoryAmp - (double)amp;
                if (Math.Abs(diffVal) > _RefSigTheoryAmp * _RefSigAmpDealta)
                {
                    //再次执行
                    return _CurrentSetp;
                }
                else
                {
                    //进入下一步
                    _RefSigNowMeaAmp = (double)amp;
                    Update(_CurrentSetp, ProbeCaliItemState.CaliItemState_CkSucc);
                    return _CurrentSetp = ProbeUserCaliStep.ProbeCaliStep_GainCali;
                }
            }
            else
            {
                Boolean res = false;
                this.Invoke(new Action(() =>
                {   //提示未检测到方波
                    res = StrongTip.Default.Show(MsgTipId.Information, MsgTipId.ProbeCaliRefSigUnInput, MessageType.Information);
                }));

                if (res)
                {
                    //再次检测
                    _InputSrorceChkCount = 0;
                    return _CurrentSetp;
                }
                else
                {
                    //退出校准
                    Update(_CurrentSetp, ProbeCaliItemState.CaliItemState_CkFail);
                    PgBar.Invoke((MethodInvoker)delegate
                    {
                        PgBar.Value = 0;
                        _HasCaliCount = 0;
                    });
                    return _CurrentSetp = ProbeUserCaliStep.ProbeCaliStep_Finished;
                }
            }
        }

        /// <summary>
        /// ProbeCaliStep_GainCali
        /// 增益校准-记录校正系数
        /// </summary>
        /// <returns></returns>
        private ProbeUserCaliStep ProbeCalibToGainCali()
        {
            //计算增益修正系数
            _ProbeGainRatio = _RefSigTheoryAmp / _RefSigNowMeaAmp;

            //下发增益修正系数
            _ChlPresenter.ProbeGainCaliRatio = _ProbeGainRatio;

            //进入下一步
            Update(_CurrentSetp, ProbeCaliItemState.CaliItemState_Succ);
            return _CurrentSetp = ProbeUserCaliStep.ProbeCaliStep_OffsetCaliInit;
        }


        /// <summary>
        /// ProbeCaliStep_OffsetCaliInit
        /// 偏置校准初始化
        /// </summary>
        /// <returns></returns>
        private ProbeUserCaliStep ProbeCalibToOffsetCaliInit()
        {
            //提示移除参考信号
            Boolean res = false;
            this.Invoke(new Action(() =>
            {
                res = StrongTip.Default.Show(MsgTipId.Information, MsgTipId.ProbeCaliRefSigRemove, MessageType.Information);
            }));
            //偏置校准初始化
            DsoPrsnt.DefaultDsoPrsnt.SquareWaveSwitch = false;       //关闭方波
            _ChlPresenter.Bias = 0;                                  //Bias=0
            _ChlPresenter.PosIndexBymDiv = 0;                    //-3Div
            _ChlPresenter.ScaleIndex = (int)ScopeX.Core.AnaChnlScaleIndex.Lv1m; //x10为500mV 初设大档位避免信号超出屏幕外无法测量 

            _TimebasePrsnt.ClockSrc = AnaChnlClkSrc.Inner;          //??
            _TimebasePrsnt.EnhancedBitsActive = false;              //??
            _TimebasePrsnt.StorageMode = AnaChnlStorageMode.Long;   //??
            //_TimebasePrsnt.ScaleIndex = ScopeX.Core.AnaChnlTimebaseIndex.Lv200u;
            _TimebasePrsnt.ScaleIndex = ScopeX.Core.AnaChnlTimebaseIndex.Lv2m;
            _TimebasePrsnt.StorageDepthOpt = (int)AnaChnlLengthOpt.Of25KDots;
            //_TimebasePrsnt.Mode = AnaChnlAcqMode.HighRes;
            _TimebasePrsnt.Mode = AnaChnlAcqMode.Average;
            _TimebasePrsnt.AverageCnt = 8;
            _TimebasePrsnt.EnvelopeCnt = 8;

            Update(_CurrentSetp, ProbeCaliItemState.CaliItemState_Succ);
            return _CurrentSetp = ProbeUserCaliStep.ProbeCaliStep_OffsetCaliCheck;
        }

        /// <summary>
        /// ProbeCaliStep_OffsetCaliCheck
        /// 偏置校准检查
        /// </summary>
        /// <returns></returns>
        private ProbeUserCaliStep ProbeCalibToOffsetCaliCheck()
        {
            if (_InputSrorceChkCount >= _InputSrorceChkTotal)
            {
                //检测超限
                Boolean res = false;
                this.Invoke(new Action(() =>
                {   //提示不应检测到信号输入
                    res = StrongTip.Default.Show(MsgTipId.Information, MsgTipId.ProbeCaliNoiseChecked, MessageType.Information);
                }));

                if (res)
                {
                    //再次检测
                    _InputSrorceChkCount = 0;
                    return _CurrentSetp;
                }
                else
                {
                    //退出校准
                    Update(_CurrentSetp, ProbeCaliItemState.CaliItemState_CkFail);
                    return _CurrentSetp = ProbeUserCaliStep.ProbeCaliStep_Finished;
                }
            }
            else
            {
                //粗调测量值读取
                _InputSrorceChkCount++;
                var amp = _MeaPresenter.CalcResultNowWithUnit(Amp, _ChlPresenter.Id);
                var avg = _MeaPresenter.CalcResultNowWithUnit(Avg, _ChlPresenter.Id);
                if (amp == null || avg == null)
                {
                    return _CurrentSetp;
                }

                var ampVal = Quantity.ConvertByPrefix(amp.Value.Result, amp.Value.Prefix, Prefix.Milli);
                var avgVal = Quantity.ConvertByPrefix(avg.Value.Result, avg.Value.Prefix, Prefix.Micro);
                if (ampVal < _ChlPresenter.ScaleBymV * 1)
                {
                    //幅度再1DIV内，说明探头无信号输入，可以进入Offset的细调阶段；此时要将粗调值应用下去
                    _ChlPresenter.ProbeOffsetCaliBias = _ProbeOffsetVal = avgVal;

                    //设置细调参数
                    _ChlPresenter.PosIndexBymDiv = 0;                      //-3Div
                    _ChlPresenter.ScaleIndex = (int)ScopeX.Core.AnaChnlScaleIndex.Lv1m; //x10为10mV 

                    Update(_CurrentSetp, ProbeCaliItemState.CaliItemState_CkSucc);
                    return _CurrentSetp = ProbeUserCaliStep.ProbeCaliStep_OffsetCali;
                }

                return _CurrentSetp;
            }
        }

        /// <summary>
        /// ProbeCaliStep_OffsetCali
        /// 偏置校准
        /// </summary>
        /// <returns></returns>
        private ProbeUserCaliStep ProbeCalibToOffsetCali()
        {
            _CaliOffsetCount++;
            if (_CaliOffsetCount > _InputSrorceChkTotal)
            {
                //超过次数失败
                Update(_CurrentSetp, ProbeCaliItemState.CaliItemState_CkFail);
                return _CurrentSetp = ProbeUserCaliStep.ProbeCaliStep_Finished;
            }

            var amp = _MeaPresenter.CalcResultNowWithUnit(Amp, _ChlPresenter.Id);
            var avg = _MeaPresenter.CalcResultNowWithUnit(Avg, _ChlPresenter.Id);
            if (amp == null || avg == null)
            {
                //读取测量值失败
                return _CurrentSetp;
            }

            var ampVal = Quantity.ConvertByPrefix(amp.Value.Result, amp.Value.Prefix, Prefix.Milli);
            var avgVal = Quantity.ConvertByPrefix(avg.Value.Result, avg.Value.Prefix, Prefix.Micro);
            if (ampVal > _ChlPresenter.ScaleBymV * 4)
            {
                //幅度值超过半屏，说信号异常本次失败
                return _CurrentSetp;
            }

            //校准成功，粗调制 + 细调值
            _ChlPresenter.ProbeOffsetCaliBias = (_ProbeOffsetVal + avgVal);
            Update(_CurrentSetp, ProbeCaliItemState.CaliItemState_Succ);
            return _CurrentSetp = ProbeUserCaliStep.ProbeCaliStep_Saving;
        }

        /// <summary>
        /// CaliItemState_Succ
        /// 探头校准-保存数据
        /// </summary>
        /// <returns></returns>
        private ProbeUserCaliStep ProbeCalibToSaving()
        {
            _ChlPresenter.SaveProbeUserCalibDataToLocal();
            Update(_CurrentSetp, ProbeCaliItemState.CaliItemState_Succ);
            return _CurrentSetp = ProbeUserCaliStep.ProbeCaliStep_Finished;
        }

        #endregion

        #region 按钮状态

        private void BtnsStateUpdate()
        {
            if (_WorkTask != null && (_WorkTask.Status == TaskStatus.Created
                || _WorkTask.Status == TaskStatus.WaitingForActivation
                || _WorkTask.Status == TaskStatus.WaitingToRun
                || _WorkTask.Status == TaskStatus.Running
                || _WorkTask.Status == TaskStatus.WaitingForChildrenToComplete
                ))
            {
                BtnsStateWork(true);
            }
            else
            {
                BtnsStateWork(false);
            }
        }

        private void BtnsStateWork(Boolean isOn)
        {
            if (isOn)
            {
                BtnCancel.Enabled = true;
                BtnCalib.Enabled = false;
            }
            else
            {
                BtnCancel.Enabled = false;
                BtnCalib.Enabled = true;
            }
        }

        public void UpdateByWorker()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(UpdateView));
            }
            else
            {
                UpdateView();
            }
        }
        #endregion

        #region 工具函数
        protected String Cvt(ProbeCaliItemState state)
        {
            return state switch
            {
                ProbeCaliItemState.CaliItemState_Todo => ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ProbeUserCaliForm.Item.Status.Todo"),
                ProbeCaliItemState.CaliItemState_Doing => ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ProbeUserCaliForm.Item.Status.Doing"),
                ProbeCaliItemState.CaliItemState_Cancel => ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ProbeUserCaliForm.Item.Status.Cancel"),
                ProbeCaliItemState.CaliItemState_Succ => ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ProbeUserCaliForm.Item.Status.Succ"),
                ProbeCaliItemState.CaliItemState_Fail => ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ProbeUserCaliForm.Item.Status.Fail"),
                ProbeCaliItemState.CaliItemState_CkFail => ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ProbeUserCaliForm.Item.Status.ChkFail"),
                ProbeCaliItemState.CaliItemState_CkSucc => ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ProbeUserCaliForm.Item.Status.ChkSucc"),
            };
        }


        private void BtnCloseRef_Click(object sender, EventArgs e)
        {
            DsoPrsnt.DefaultDsoPrsnt.SquareWaveSwitch = !DsoPrsnt.DefaultDsoPrsnt.SquareWaveSwitch;
            if (DsoPrsnt.DefaultDsoPrsnt.SquareWaveSwitch)
            {
                BtnCloseRef.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ProbeUserCaliForm.CloseRef");
            }
            else
            {
                BtnCloseRef.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ProbeUserCaliForm.OpenRef");
            }
        }
        #endregion

        private void BtnClrCali_Click(object sender, EventArgs e)
        {
            _ChlPresenter.ProbeGainCaliRatio = 1;
            _ChlPresenter.ProbeOffsetCaliBias = 0;
        }

        private void BtnReadCali_Click(object sender, EventArgs e)
        {
            var msg = _ChlPresenter.ProbeGainCaliRatio.ToString() + "," + _ChlPresenter.ProbeOffsetCaliBias.ToString();

            WeakTip.Default.Write("Probe Cali Data:", msg, false, "", 2);
        }

        private void BtnOther_Click(object sender, EventArgs e)
        {
            if(false)
            {//临时读取测量值
                var amp = _MeaPresenter.CalcResultNowWithUnit(Amp, _ChlPresenter.Id);
                var avg = _MeaPresenter.CalcResultNowWithUnit(Avg, _ChlPresenter.Id);
                if (amp == null || avg == null)
                {
                    return;
                }

                var ampVal = Quantity.ConvertByPrefix(amp.Value.Result, amp.Value.Prefix, Prefix.Milli);
                var avgVal = Quantity.ConvertByPrefix(avg.Value.Result, avg.Value.Prefix, Prefix.Micro);


                var msg = ampVal.ToString() + "," + avgVal.ToString();

                WeakTip.Default.Write("Probe Cali Data:", msg, false, "", 2);
            }

            if(true)
            {//临时验证探头校正数据存区
                _ChlPresenter.SaveProbeUserCalibDataToLocal();
            }
        }
    }

    /// <summary>
    /// 校准项
    /// </summary>
    internal class ProbeUserCaliItem
    {
        public ProbeUserCaliStep Setp { get; set; }
        public ProbeCaliItemState State { get; set; }
        public String SetpDescription { get; set; }
    }
}

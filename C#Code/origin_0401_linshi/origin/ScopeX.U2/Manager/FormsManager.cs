using EventBus;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ScopeX.Core.Tools;
using ScopeX.UserControls;
using ScopeX.Controls.Common.Structs;
using ScopeX.Controls.Common.Helper;
using ScopeX.Core.Tools.Tips;
using ScopeX.U2.TipMsg;

namespace ScopeX.U2
{
    public class FormsManager
    {
        private readonly Form _MainForm = null;
        /// <summary>
        ///  提示消息相关链表，主要是消息弹窗和弱提示
        /// </summary>
        private readonly LinkedList<Form> _MsgFormList = new();
        /// <summary>
        /// 设置窗口
        /// </summary>
        private Form _SettingForm = null;
        /// <summary>
        /// 显示窗口相关链表(如测量窗口)
        /// </summary>
        private readonly LinkedList<Form> _InfoFormList = new();
        /// <summary>
        ///  波形显示窗口相关链表
        /// </summary>
        private readonly LinkedList<Form> _WaveFormList = new();

        private MsgTipForm _Messageform;
        //private LinkedList<Form> _ShowDialogForms= new();
        private Form _OtherShowDialogForm = null;

        public FormsManager(Form mainForm)
        {
            _MainForm = mainForm;
            if (mainForm != null)
            {
                mainForm.Activated += (sender, e) =>
                {
                    if (_SettingForm is FloatForm { CanClose: true, IsFixed: false })
                        _SettingForm?.Close();
                };
            }
            Init();
        }

        private void Init()
        {
            //初始化弱提示窗口事件（初始化+订阅）
            InitTipMsgFormSubscrip();
            // 初始化可控弱提示的消息订阅
            // InitControlableTipMsgFormSubscrip();
            //订阅消息弹出事件
            InitMessageFormSubscrip();
            //订阅其他窗口事件
            InitFormEventSubscrip();
            //订阅窗口关闭事件，目前只支持设置窗口
            InitFormCloseEventSubscrip();
            //订阅模态窗口事件
            InitFormShowDialogEventSubscrip();
            InitKeybordSubscrip();
            //订阅其他模态窗口，主要用于统一管理
            InitFormOtherShowDialogEventSubscrip();
        }

        private void InitKeybordSubscrip()
        {
            EventBroker.Instance.GetEvent<NumberKeyboardArgs, Double>().Subscrip((sender, args) =>
            {
                NumberKeybordForm nkf = new NumberKeybordForm().UniformInitKeyBoard(sender as ContainerControl);
                Double result = args.Data.DefaultValue;
                nkf.SetKeyBoardValue(args.Data.Title, args.Data.Unit, args.Data.DecimalNumber, (value) =>
                {
                    result = value;
                }, args.Data.DefaultValue, args.Data.MaxValue, args.Data.MinValue, args.Data.UseSI);
                nkf.ShowDialogByPosition();
                return result;
            });

            EventBroker.Instance.GetEvent<HexNumberKeyboardArgs, Int64>().Subscrip((sender, args) =>
            {
                HexNumberKeyboardFrom hkf = new HexNumberKeyboardFrom().UniformInitKeyBoard(sender as ContainerControl);
                Int64 result = args.Data.DefaultValue;
                hkf.SetKeyBoardValue(args.Data.Title, (value) =>
                {
                    result = value;
                }, args.Data.DefaultValue, args.Data.MaxValue, args.Data.MinValue, args.Data.HexValueType);
                hkf.ShowDialogByPosition();
                return result;
            });
        }

        private void InitFormShowDialogEventSubscrip()
        {
            EventBroker.Instance.GetEvent<FormShowDialogEventArgs, DialogResult>().Subscrip((sender, args) =>
            {
                if (args.Data.Current != null)
                {
                    if (args.Data.Current is FloatForm ff)
                    {
                        ff.ActiveBorderColor = AppStyleConfig.DefaultFormActiveBorderColor;
                        ff.ActiveBorderVisiable = true;
                        ff.IsShowPin = false;
                    }
                    return args.Data.Current.ShowDialog();
                }
                return DialogResult.Cancel;
            });
        }
        private void InitFormOtherShowDialogEventSubscrip()
        {
            EventBroker.Instance.GetEvent<FormShowDialogEventArgs>().Subscrip((sender, args) =>
            {
                if (args.Data.Current != null)
                {
                    if (_OtherShowDialogForm != null && !_OtherShowDialogForm.IsDisposed)
                    {
                        _OtherShowDialogForm.DialogResult = DialogResult.Cancel;
                        _OtherShowDialogForm.Dispose();
                    }
                    _OtherShowDialogForm = args.Data.Current;
                }
            });
        }

        private void InitFormCloseEventSubscrip()
        {
            EventBroker.Instance.GetEvent<FormCloseEventArgs>().Subscrip((sender, args) =>
            {
                switch (args.Data.Type)
                {
                    case FormType.SettingForm://目前只支持设置窗口关闭
                        {
                            if (_SettingForm != null && _SettingForm.Text == args.Data.Name)
                            {
                                _SettingForm?.Close();
                            }
                            else if (_SettingForm != null && _SettingForm.Tag != null && _SettingForm.Tag.ToString() == args.Data.Name)
                            {
                                _SettingForm?.Close();
                            }
                            else if (_SettingForm != null && string.IsNullOrEmpty(args.Data.Name))
                            {
                                _SettingForm?.Close();
                            }
                        }
                        break;
                    case FormType.InfoForm:
                        break;
                    case FormType.WaveForm:
                        break;
                    case FormType.ShowDialogForm:
                        {
                            if (null != _Messageform)
                            {
                                _Messageform.DialogResult = DialogResult.Cancel;
                                _Messageform?.Dispose();
                                _Messageform = null;
                            }
                            if (_OtherShowDialogForm != null && !_OtherShowDialogForm.IsDisposed)
                            {
                                _OtherShowDialogForm.DialogResult = DialogResult.Cancel;
                                _OtherShowDialogForm?.Dispose();
                                _OtherShowDialogForm = null;
                            }
                            //if (_OtherShowDialogForm != null && !_OtherShowDialogForm.IsDisposed)
                            //{
                            //    _OtherShowDialogForm.DialogResult = DialogResult.Cancel;
                            //    _OtherShowDialogForm.Dispose();
                            //    _OtherShowDialogForm = null;
                            //}
                        }
                        break;
                    default:
                        break;
                }
            });
        }

        private void InitFormEventSubscrip()
        {
            //设置窗口、显示窗口(如测量窗口)、波形显示窗口
            EventBroker.Instance.GetEvent<FormEventArgs>().Subscrip((sender, args) =>
            {
                try
                {
                    switch (args.Data.Type)
                    {
                        case FormType.SettingForm://设置窗口
                            {
                                if (args.Data.Current != null)
                                {
                                    if (_SettingForm != null && _SettingForm.IsDisposed == false)
                                    {
                                        _SettingForm?.Close();
                                    }

                                    args.Data.Current.StartPosition = FormStartPosition.Manual;
                                    args.Data.Current.FormClosed += ShowForm_FormClosed;

                                    Boolean canclose = true;
                                    if (args.Data.Current is FloatForm floatform)
                                    {
                                        canclose = floatform.CanClose;
                                        floatform.CanClose = false;
                                        floatform.StartPositionInOwner = floatform.Location;
                                    }

                                    args.Data.Current.Owner = _MainForm;

                                    //!!!zqc11.05
                                    args.Data.Current.Shown += (o, e) => SetSettingFrontForm(args.Data.Current);

                                    if (args.Data.Current != null && !args.Data.Current.IsDisposed)
                                    {
                                        args.Data.Current?.Show();

                                        if (args.Data.Current is FloatForm form)
                                        {
                                            form.CanClose = canclose;
                                        }
                                        _SettingForm = args.Data.Current;
                                    }
                                }
                            }
                            break;
                        case FormType.InfoForm://显示窗口
                            {
                                if (args.Data.Current != null)
                                {
                                    _InfoFormList.AddFirst(args.Data.Current);
                                    args.Data.Current.FormClosed += (sender, e) =>
                                    {
                                        if (_InfoFormList.Contains(args.Data.Current))
                                        {
                                            _InfoFormList.Remove(args.Data.Current);
                                        }
                                    };

                                    args.Data.Current.StartPosition = FormStartPosition.Manual;

                                    if (args.Data.Current is FloatForm floatform)
                                    {
                                        floatform.StartPositionInOwner = floatform.Location;
                                    }
                                    args.Data.Current.Owner = _MainForm;

                                    args.Data.Current.Shown += (o, e) => SetInfoFrontForm(args.Data.Current);
                                    args.Data.Current.Activated += (o, e) => SetInfoFrontForm(args.Data.Current);
                                    args.Data.Current?.Show();
                                }
                            }
                            break;
                        case FormType.WaveForm://波形显示窗口
                            {
                                if (args.Data.Current != null)
                                {
                                    _WaveFormList.AddFirst(args.Data.Current);
                                    args.Data.Current.FormClosed += (sender, e) =>
                                    {
                                        if (_WaveFormList.Contains(args.Data.Current))
                                        {
                                            _WaveFormList.Remove(args.Data.Current);
                                        }
                                    };
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    EventBroker.Instance.GetEvent<LogEventArgs>().Publish(new Object(), new LogEventArgs($"{ex.Message}\n{ex.StackTrace}", LogLevel.Error));
                }
            });
        }

        private void InitTipMsgFormSubscrip()
        {
            ///弱提示，只有一个，先初始化好
            WeakTipForm wtf = new(_MainForm);
            _MsgFormList.AddFirst(wtf);
            wtf.Activated += (o, e) =>
            {
                _MsgFormList.Remove(wtf);
                _MsgFormList.AddFirst(wtf);
            };

            //利用显示来进行Handle的提前初始化；
            wtf.StartPosition = FormStartPosition.Manual;
            wtf.Location = new Point(-1000, -1000);
            wtf.Show();
            wtf.Hide();

            wtf.Owner = _MainForm;
            wtf.BringToFront();
            wtf.ShowInTaskbar = false;
            //弱提示
            EventBroker.Instance.GetEvent<WeakTipEventArgs>().Subscrip((sender, args) =>
            {
                if (_MainForm.WindowState == FormWindowState.Minimized)
                {
                    return;
                }
                try
                {
                    wtf?.Invoke(new Action(() =>
                    {
                        wtf?.Show(args.Data);
                    }));
                }
                catch (Exception ex)
                {
                    EventBroker.Instance.GetEvent<LogEventArgs>().Publish(new Object(), new LogEventArgs($"{ex.Message}\n{ex.StackTrace}", LogLevel.Error));
                }
            });
        }

        /// <summary>
        /// 可控弱提示框消息订阅处理
        /// </summary>
        private void InitControlableTipMsgFormSubscrip()
        {
            EventBroker.Instance.GetEvent<ControlableWeakTipEventArgs>().Subscrip((sender, args) =>
            {
                if (_MainForm.WindowState == FormWindowState.Minimized)
                    return;

                try
                {
                    ControlableWeakTipForm form = ControlableWeakTipCacher.GetForm(args.Data.TipId);
                    switch (args.Data.type)
                    {
                        case ControlableWeakTipEventControlType.Create:
                        default:
                            if (form == null)
                            {
                                form = new ControlableWeakTipForm(_MainForm);
                                ControlableWeakTipCacher.CacheForm(args.Data.TipId, form);
                            }
                            form!.Show(args.Data, null);
                            break;
                        case ControlableWeakTipEventControlType.Close:
                            form?.Close();
                            break;
                        case ControlableWeakTipEventControlType.UpdateText:
                            form?.UpdateContent(args.Data.ContentName);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    EventBroker.Instance.GetEvent<LogEventArgs>().Publish(new Object(), new LogEventArgs(ex.ToString(), LogLevel.Error));
                }
            });
        }

        private void InitMessageFormSubscrip()
        {
            EventBroker.Instance.GetEvent<StrongTipEventArgs, DialogResult>().Subscrip((sender, args) =>
            {
                if (_MainForm.WindowState == FormWindowState.Minimized)
                {
                    return DialogResult.Cancel;
                }
                try
                {
                    DialogResult dialogresult = DialogResult.Cancel;
                    //<Remark>作者：彭博 创建日期：2024/2/26 17:01:00 创建原因：保证只有一个提示窗口</Remark>
                    _MainForm.Invoke(new MethodInvoker(() =>
                    {
                        if (_Messageform != null)
                        {
                            //_Messageform.DialogResult = DialogResult.Cancel;
                            //_Messageform.Dispose();
                            return;
                        }
                        _Messageform = new(args.Data);
                        _Messageform.Owner = _MainForm;
                        _Messageform.ShowInTaskbar = false;
                        _Messageform.TopMost = true;
                        //_Messageform.SetWindowInCenter(_MainForm);
                        dialogresult = _Messageform.ShowDialog();
                        _Messageform = null;
                    }));
                    return dialogresult;
                }
                catch (Exception ex)
                {
                    EventBroker.Instance.GetEvent<LogEventArgs>().Publish(new Object(), new LogEventArgs(ex.ToString(), LogLevel.Error));
                    return DialogResult.Cancel;
                }

            });
        }

        /// <summary>
        /// 关闭消息提示窗口
        /// </summary>
        /// <Remark>作者：彭博 创建日期：2024/2/26 14:14:00 创建原因：主动关闭提示窗口，保证只有一个提示窗口</Remark>
        internal void CloseMessageFormSubscrip()
        {
            if (null != _Messageform)
            {
                _Messageform.DialogResult = DialogResult.Cancel;
                _Messageform.Dispose();
                _Messageform = null;
            }
        }

        /// <summary>
        /// 设置设置窗口的层级
        /// </summary>
        /// <param name="form">需要设置的窗口</param>
        private void SetSettingFrontForm(Form form)
        {
            if (form == null)
            {
                return;
            }
            if (_MsgFormList.Last != null)
            {
                if (_MsgFormList.Last.Value is Form { Visible: true } lastfrontform)
                {
                    try
                    {
                        NativeMethods.SetWindowPos(form.Handle, lastfrontform.Handle, 0, 0, 0, 0, NativeMethods.SWP_NOSIZE | NativeMethods.SWP_NOMOVE);
                    }
                    catch (Exception ex)
                    {
                        EventBroker.Instance.GetEvent<LogEventArgs>().Publish(new Object(), new LogEventArgs(ex.ToString(), LogLevel.Error));
                    }
                }
            }
        }
        /// <summary>
        /// 设置显示窗口层级
        /// </summary>
        /// <param name="form"></param>
        private void SetInfoFrontForm(Form form)
        {
            if (form == null)
            {
                return;
            }
            if (_SettingForm != null)
            {
                try
                {
                    NativeMethods.SetWindowPos(form.Handle, _SettingForm.Handle, 0, 0, 0, 0, NativeMethods.SWP_NOSIZE | NativeMethods.SWP_NOMOVE);
                }
                catch (Exception ex)
                {
                    EventBroker.Instance.GetEvent<LogEventArgs>().Publish(new Object(), new LogEventArgs(ex.ToString(), LogLevel.Error));
                }
            }
            else
            {
                //if (_MsgFormList.Last != null)
                {
                    //!!!Only call SetWindowPos to change z-order when tip form is visible
                    if (_MsgFormList.Last?.Value is Form { Visible: true } lastfrontform)
                    {
                        try
                        {
                            //!!!SetWindowPos make the MainForm not active                            
                            NativeMethods.SetWindowPos(form.Handle, lastfrontform.Handle, 0, 0, 0, 0, NativeMethods.SWP_NOSIZE | NativeMethods.SWP_NOMOVE);
                        }
                        catch (Exception ex)
                        {
                            EventBroker.Instance.GetEvent<LogEventArgs>().Publish(new Object(), new LogEventArgs(ex.ToString(), LogLevel.Error));
                        }
                        //!!!Maybe need keep MainForm active
                        //NativeMethods.SetActiveWindow(form.Owner.Handle);
                    }
                }
            }
        }

        private void ShowForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            _SettingForm = null;
        }
    }
}

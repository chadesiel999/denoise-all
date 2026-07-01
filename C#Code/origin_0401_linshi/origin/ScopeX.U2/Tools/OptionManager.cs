using ScopeX.ComModel;
using ScopeX.Controls.Language;
using ScopeX.Core;
using ScopeX.Core.Tools;
using System;
using System.Linq;

namespace ScopeX.U2.Tools
{
    internal class OptionManager
    {
        public static readonly OptionManager Default = new();

        public readonly DsoPrsnt _Oscilloscope;

        private OptionManager()
        {
            _Oscilloscope = Program.Oscilloscope;
        }
        public Boolean Checked(OptionType type)
        {
            try
            {
                if (Constants.OPTION_LIMIT == 1011101011)
                {
                    return true;
                }

                var isactive = _Oscilloscope.OptionsManager.GetOptionAvailable(type);
                if (!isactive)
                {
                    if (type == OptionType.BW10T20)
                    {
                        //Constants.LZBANDWIDTHNAMES = Constants.LZ1GBANDWIDTHNAMES;
                    }
                    else
                    {
                        ShowMsg();
                    }
                    return false;
                }
                //else
                //{
                //    if (type == OptionType.BW10T20)
                //    {
                //        Constants.LZBANDWIDTHNAMES = Constants.LZ2GBANDWIDTHNAMES;
                //    }
                //}
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public void SetModel()
        {
            //if (_Oscilloscope.OptionsManager.Is2GHz || _Oscilloscope.OptionsManager.GetRemainingTimeByHour() > 0)
            //{
            //    Constants.LZBANDWIDTHNAMES = Constants.LZ2GBANDWIDTHNAMES;
            //}
            //else
            //{
            //    Constants.LZBANDWIDTHNAMES = Constants.LZ1GBANDWIDTHNAMES;
            //}
        }
        MsgBox _MsgTipForm;
        private void ShowMsg()
        {
            var msg = LanguageManger.Instance.GetIDMessage(EnumOperateMethod.GetEnumFullName(MsgTipId.PurchaseOptions));
            if (Program.Oscilloscope.View is DsoForm form && form!=null)
            {
                form.Invoke(new Action(() =>
                {
                    
                    if (_MsgTipForm == null)
                    {
                        form.Activate();
                        _MsgTipForm = new MsgBox();
                        _MsgTipForm.Message = msg;
                        _MsgTipForm.Title = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("TiShi"); // "提示";
                        //_MsgTipForm.TopMost = true;
                        _MsgTipForm.Owner = form;
                        _MsgTipForm.FormClosed += MsgTipForm_FormClosed;
                        _MsgTipForm.ShowDialog();
                        //form.Activate();
                    }
                }));
            }
        }

        private void MsgTipForm_FormClosed(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            if(_MsgTipForm!=null)
                _MsgTipForm= null;
        }
    }
    //public enum OptionType
    //{
    //    AWG,
    //    LA,
    //    Pwr,
    //    Jitter,
    //    Decode_CanFD,
    //    Decode_FlexRay,
    //    Decode_SENT,
    //    Decode_MIL,
    //    Decode_ARINC429,
    //    Decode_AudioBus,
    //}
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ScopeX.ComModel;
using ScopeX.Core;
using ScopeX.UserControls.Style;
using ScopeX.Controls.Common.Default;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using static ScopeX.UserControls.SelectComboBox;

namespace ScopeX.U2
{
    [Description(nameof(MathType.Binary))]
    public partial class MathBinarySubPage :MathSubPageBase
    {
        MathBinaryArg _BinaryArg;
        public MathBinarySubPage()
        {
            InitializeComponent();
        }

        #region Base Method
        protected override void SubPageUpdateView(object prsnt, string propertyName)
        {
            UpdateView();
        }

        protected override void LoadMethod()
        {
            base.LoadMethod();
            _BinaryArg = (MathBinaryArg)Presenter.GetOrMakeArg(MathType.Binary);

            var srcs = Program.Oscilloscope.FindIdentities(c => c.Id.IsAnalog());
            if (Presenter.PreMathChannels.Count > 0)
            {
                srcs.Add(Presenter.PreMathChannels[0]);
            }

            LoadSourceList(srcs); // 直接使用 srcs

            UpdateView(); // 确保这个方法尽可能高效
        } 
        #endregion

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                if (_BinaryArg is null)
                {
                    return;
                }

                _ArgToCtrl = true;
                //CbxLeftSource.SelectedIndex = CbxLeftSource.FindString(_BinaryArg.Source1st.ToString());
                CbxLeftSource.SelectValue = _BinaryArg.Source1st;
                //CbxRightSource.SelectedIndex = CbxLeftSource.FindString(_BinaryArg.Source2nd.ToString());
                CbxRightSource.SelectValue = _BinaryArg.Source2nd;
                RdoOperator.ChoosedButtonIndex = (Int32)_BinaryArg.BinaryOp;
                _ArgToCtrl = false;
            }
        }

        private void LoadSourceList(IEnumerable<ChannelId> sources)
        {
            if (CbxLeftSource.DataSource!=null)
            {
                if (((CbxLeftSource.DataSource is IEnumerable<KeyValuePair<String, ChannelId>> channels) && channels.Select(x=>x.Value.ToString()).ToArray().SequenceEqual(sources.Select(x => x.ToString()).ToArray())))
                {
                    return;
                }
            }
            
            CbxLeftSource.DataSource= sources.Select(x => new ComboBoxItem(x.ToString(), x, null)).ToList();
            CbxLeftSource.SelectedIndexChanged += (_, _) =>
            {
                if (!_ArgToCtrl)
                {
                    //if (Program.Oscilloscope.TryGetChannel((ChannelId)CbxLeftSource.SelectValue, out var chnlPrsnt))
                    //{
                    //    if (!chnlPrsnt.Active)
                    //    {
                    //        chnlPrsnt.Active = true;
                    //    }
                    //}
                    //_BinaryArg.Source1st = Enum.Parse<ChannelId>((String)CbxLeftSource.SelectedValue);
                    _BinaryArg.Source1st = (ChannelId)CbxLeftSource.SelectValue;
                }
            };


            //CbxLeftSource.DataSource = sources.Select(x => new KeyValuePair<String, ChannelId>(x.ToString(), x)).ToList();
            //CbxLeftSource.DisplayMember = "Key";
            //CbxLeftSource.ValueMember = "Value";

            //CbxLeftSource.SelectedValueChanged += (_, _) =>
            //{
            //    if (!_ArgToCtrl)
            //    {
            //        if (Program.Oscilloscope.TryGetChannel((ChannelId)CbxLeftSource.SelectedValue, out var chnlPrsnt))
            //        {
            //            if (!chnlPrsnt.Active)
            //            {
            //                chnlPrsnt.Active = true;//不再自动打开了
            //            }
            //        }
            //        //_BinaryArg.Source1st = Enum.Parse<ChannelId>((String)CbxLeftSource.SelectedValue);
            //        _BinaryArg.Source1st = (ChannelId)CbxLeftSource.SelectedValue; 
            //    }
            //};

            CbxRightSource.DataSource = sources.Select(x => new ComboBoxItem(x.ToString(), x,null)).ToList();
            CbxRightSource.SelectedIndexChanged += (_, _) =>
            {
                if (!_ArgToCtrl)
                {
                    //if (Program.Oscilloscope.TryGetChannel((ChannelId)CbxRightSource.SelectValue, out var chnlPrsnt))
                    //{
                    //    if (!chnlPrsnt.Active)
                    //    {
                    //        chnlPrsnt.Active = true;//不再自动打开了
                    //    }
                    //}
                    //_BinaryArg.Source1st = Enum.Parse<ChannelId>((String)CbxLeftSource.SelectedValue);
                    _BinaryArg.Source2nd = (ChannelId)CbxRightSource.SelectValue;
                }
            };
            //CbxRightSource.DataSource = sources.Select(x => new KeyValuePair<String, ChannelId>(x.ToString(), x)).ToList();
            //CbxRightSource.DisplayMember = "Key";
            //CbxRightSource.ValueMember = "Value";

            //CbxRightSource.SelectedValueChanged += (_, _) =>
            //{
            //    if (!_ArgToCtrl)
            //    {
            //        if (Program.Oscilloscope.TryGetChannel((ChannelId)CbxRightSource.SelectedValue, out var chnlPrsnt))
            //        {
            //            if (!chnlPrsnt.Active)
            //            {
            //                chnlPrsnt.Active = true;
            //            }
            //        }
            //        //_BinaryArg.Source2nd = Enum.Parse<ChannelId>((String)CbxRightSource.SelectedValue);
            //        _BinaryArg.Source2nd = (ChannelId)CbxRightSource.SelectedValue;
            //    }
            //};
        }

        private void CbxLeftSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
            }
        }

        private void CbxRightSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
            }
        }

        private void RdoOperator_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                _BinaryArg.BinaryOp = (MathBinaryType)RdoOperator.ChoosedButtonIndex;
            }
        }
        
        private void CbxLeftSource_Click(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
            }
        }

        private void CbxRightSource_Click(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
            }
        }
    }
}

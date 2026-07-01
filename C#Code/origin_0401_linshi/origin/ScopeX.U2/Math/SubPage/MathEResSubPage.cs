using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using ScopeX.ComModel;
using ScopeX.Core;
using ScopeX.UserControls.Style;
using ScopeX.Controls.Common.Default;
using ScopeX.Controls.Common.Helper;
using System.Diagnostics;
using System.Drawing;
using static ScopeX.UserControls.SelectComboBox;

namespace ScopeX.U2
{
    [Description(nameof(MathType.ERes))]
    public partial class MathEResSubPage :MathSubPageBase
    {
        MathEResArg Arg;

        public MathEResSubPage()
        {
            InitializeComponent();
            ViewInit();
        }

        #region Base Method

        protected override void ViewInit()
        {
            ControlsHotKnob.Default.InitHotKnob(NebBits);
            NebBits.EditValueOnceClicked += (_, _) => 
            {
                ControlsHotKnob.Default.SetHotKnob(Arg, NebBits,nameof(Arg.EnhancedBits));
            };
            //NebBits
            NebBits.AddClicked = (a, b) => Arg.AdjEnhancedBits(1);
            NebBits.SubClicked = (a, b) => Arg.AdjEnhancedBits(-1);
            NebBits.StringFormatFunc = (value) => BitsToString();
            NebBits.EditValueChicked = (_, _) =>
            {
                NumberKeybordForm nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebBits);
                Action<Double> onokclickeventaction = (data) =>
                    Arg.EnhancedBits = data;

                nkf.SetKeyBoardValue(LblBits.Text, String.Empty, 1, onokclickeventaction,
                    Arg.EnhancedBits,
                    Constants.MaxEnhancedBits,
                    Constants.MinEnhancedBits);

                nkf.ShowDialogByPosition();
            };
        }

        protected override void SubPageUpdateView(Object presenter, String propertyName)
        {
            UpdateView();
        }

        protected override void LoadMethod()
        {
            base.LoadMethod();
            Arg = (MathEResArg)Presenter.GetOrMakeArg(MathType.ERes);

            var srcs = Program.Oscilloscope.FindIdentities(c => c.Id.IsAnalog() || (c.Active && c.Id.IsReference()) || (c.Active && c.Id.IsMath()&&c.Id!=Presenter.Id));
            if (Presenter.PreMathChannels.Count > 0)
            {
                srcs.Add(Presenter.PreMathChannels[0]);
            }
            LoadSourceList(srcs);

            UpdateView();
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
                if(CbxSource.DataSource is List<ComboBoxItem> com && !com.Contains(new ComboBoxItem( CbxSource.SelectValue.ToString(),CbxSource.SelectValue,null)))
                    CbxSource.SelectValue= (ChannelId)0;
                else
                    CbxSource.SelectValue = Arg.Source;
                NebBits.UpdateValueString();
                _ArgToCtrl = false;
            }
        }
        
        private void LoadSourceList(IEnumerable<ChannelId> sources)
        {
            if (CbxSource.DataSource != null)
            {
                if (((CbxSource.DataSource is IEnumerable<KeyValuePair<String, ChannelId>> channels) && channels.Select(x=>x.Value.ToString()).ToArray().SequenceEqual(sources.Select(x => x.ToString()))))
                {
                    return;
                }
            }
            CbxSource.DataSource= sources.Select(x => new ComboBoxItem(x.ToString(), x, null)).ToList();
            CbxSource.SelectedIndexChanged += (_, _) => 
            {
                if (!_ArgToCtrl)
                {
                    if (Program.Oscilloscope.TryGetChannel((ChannelId)CbxSource.SelectValue, out var chnlPrsnt))
                    {
                        //if (!chnlPrsnt.Active)
                        //{
                        //    chnlPrsnt.Active = true;
                        //}
                    }
                    //_EResArg.Source = Enum.Parse<ChannelId>((String)CbxSource.SelectedValue);
                    
                    //if (CbxSource.DataSource is List<ComboBoxItem> com && com.Contains(CbxSource.SelectValue))
                        Arg.Source = (ChannelId)CbxSource.SelectValue;
                }
            };

        }

        private void CbxSource_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private String BitsToString()
        {
            return Arg.EnhancedBits.ToString("0.0");
        }

        private void CbxSource_Click(object sender, EventArgs e)
        {
        }
    }
}

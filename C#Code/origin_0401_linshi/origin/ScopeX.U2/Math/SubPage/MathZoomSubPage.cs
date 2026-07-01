using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using ScopeX.ComModel;
using ScopeX.Core;
using ScopeX.UserControls.Style;
using ScopeX.Controls.Common.Default;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ScopeX.U2
{
    [Description(nameof(MathType.Zoom))]
    public partial class MathZoomSubPage :MathSubPageBase
    {
        MathZoomArg Arg;

        public MathZoomSubPage()
        {
            InitializeComponent();
        }

        #region Base Method
        protected override void SubPageUpdateView(object prsnt, string propertyName)
        {
            if (String.IsNullOrEmpty(propertyName))
            {
                UpdateView();
                return;
            }

            _ArgToCtrl = true;
            switch (propertyName)
            {
                case nameof(Arg.Source):
                    CbxSource.SelectedIndex = (Int32)Arg.Source;
                    //_ZoomArg.Init();
                    break;
            }
            _ArgToCtrl = false;
        }
        protected override void LoadMethod()
        {
            base.LoadMethod();
            Arg = (MathZoomArg)Presenter.GetOrMakeArg(MathType.Zoom);

            var srcs = Program.Oscilloscope.FindIdentities(c => (c.Active && (c.Id.IsAnalog() || c.Id.IsReference())));
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
                CbxSource.SelectedIndex = (Int32)Arg.Source;
                //_ZoomArg.Init();
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
            CbxSource.DataSource = sources.Select(x => new KeyValuePair<String, ChannelId>(x.ToString(), x)).ToList();
            CbxSource.DisplayMember = "Key";
            CbxSource.ValueMember = "Value";

            CbxSource.SelectedValueChanged += (_, _) =>
            {
                if (!_ArgToCtrl)
                {
                    if (Program.Oscilloscope.TryGetChannel((ChannelId)CbxSource.SelectedValue, out var chnlPrsnt))
                    {
                        if (!chnlPrsnt.Active)
                        {
                            chnlPrsnt.Active = true;
                        }
                    }
                    //_ZoomArg.Source = Enum.Parse<ChannelId>((String)CbxSource.SelectedValue);
                    Arg.Source = (ChannelId)CbxSource.SelectedIndex;
                }
            };

        }
        private void CbxSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                //_ZoomArg.Source = (ChannelId)CbxSource.SelectedIndex;
            }
        }
        private void CbxSource_Click(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
               
            }
        }
    }
}

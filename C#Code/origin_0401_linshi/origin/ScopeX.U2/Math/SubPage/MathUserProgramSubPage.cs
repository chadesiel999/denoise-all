using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Windows.Forms;
using ScopeX.ComModel;
using ScopeX.Core;
using ScopeX.UserControls;
using ScopeX.UserControls.Style;
using ScopeX.Controls.Common.Default;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using static ScopeX.UserControls.SelectComboBox;

namespace ScopeX.U2
{
    [Description(nameof(MathType.UserProgram))]
    public partial class MathUserProgramSubPage :MathSubPageBase
    {
        MathUserProgramArg Arg;

        public MathUserProgramSubPage()
        {
            InitializeComponent();
        }
        #region Base Method

        protected override void SubPageUpdateView(Object prsnt, String propertyName = "")
        {
            UpdateView();
        }

        protected override void LoadMethod()
        {
            base.LoadMethod();
            Arg = (MathUserProgramArg)Presenter.GetOrMakeArg(MathType.UserProgram);
            _ArgToCtrl = true;

            var srcs = Program.Oscilloscope.FindIdentities(c => (/*c.Active && (*/c.Id.IsAnalog() /*|| c.Id.IsPowers()*/ || (c.Id.IsReference() && c.Active) ));
            //var srcs = Enum.GetValues<ChannelId>().Where(x=>x.IsAnalog() || x.IsReference()).Select(i=>new ComboBoxItem(i.ToString(),i)).ToList();
            //if (Presenter.PreMathChannels.Count > 0)
            //{
            //    srcs.Add(Presenter.PreMathChannels[0]);
            //}
            LoadSourceList(srcs);
            
            
            UpdateView();
            _ArgToCtrl = false;
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

                CbxSource.SelectValue = Arg.Source;
            }
        }

        private void LoadSourceList(IEnumerable<ChannelId> sources)
        {
            if (CbxSource.DataSource != null)
            {
                if (((CbxSource.DataSource is IEnumerable<KeyValuePair<String, ChannelId>> channels) && channels.Select(x => x.Value.ToString()).ToArray().SequenceEqual(sources.Select(x => x.ToString()))))
                {
                    return;
                }
            }
            CbxSource.DataSource = sources.Select(x => new ComboBoxItem(x.ToString(), x)).ToList();
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
                    Arg.Source = (ChannelId)CbxSource.SelectValue;
                }
            };

            if (Constants.MSO7000)
            {
                //CbxEngineType.DataSource= (new string[1] { UserProgramType.Matlab.ToString() }).ToList();
                //CbxEngineType.SelectedIndex = 0;
                List<ComboBoxItem> data = new List<ComboBoxItem>
                {
                    new ComboBoxItem ( UserProgramType.Matlab.ToString(),0),
                };
                CbxEngineType.DataSource = data;
                CbxEngineType.SelectValue = 0;
            }
            else
            {
                CbxEngineType.DataSource = Enum.GetNames<UserProgramType>();
                CbxEngineType.SelectValue = Arg.UserProgramType;
                CbxEngineType.SelectedIndexChanged += (_, _) =>
                {
                    if (!_ArgToCtrl)
                    {
                        Arg.UserProgramType = (UserProgramType)CbxEngineType.SelectValue;
                        Presenter.Formula = Arg.MakeFormula();
                        BtnEditCode.Enabled = Arg.UserProgramType != UserProgramType.Close;
                    }
                };

            }
        }
        private void BtnEditCode_Click(object sender, EventArgs e)
        {
            var mcf = new UserCodeForm(Arg);

            (ParentForm as FloatForm).CanClose = false;
            if (mcf.ShowDialogByEvent() == DialogResult.OK)
            {
                //BtnFormula.Text = FixAmpersand(_CustomArg.Expression);
            }
            (ParentForm as FloatForm).CanClose = true;
        }

        private void CbxSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                //_MathUserProgramArgs.Source = Enum.Parse<ChannelId>((String)CbxSource.SelectedItem);
            }
        }

        private void CbxEngineType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                
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

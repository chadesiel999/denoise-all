using ScopeX.ComModel;
using ScopeX.Core;
using ScopeX.U2.AWG;
using ScopeX.UserControls;
using ScopeX.UserControls.Style;
using System.Drawing;

namespace ScopeX.U2
{
    public partial class ProbeUserCaliForm : FloatForm
    {
        private ProbeUserCaliPage _CaliPage;
        private ChannelId _ProbeChlNo;
        private AnalogPrsnt _ChlPresenter;

        public ProbeUserCaliForm(ChannelId channelId)
        {
            InitializeComponent();
            _ProbeChlNo = channelId;
            IChnlPrsnt? cprsnt;
            if (DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(_ProbeChlNo, out cprsnt))
            {
                _ChlPresenter = (AnalogPrsnt)cprsnt;
            }
            InitControlLang();
            InitInfo();
            _CaliPage = new ProbeUserCaliPage(channelId)
            {
                BackColor = Color.Transparent,
                Dock = System.Windows.Forms.DockStyle.Fill
            };
            Size = new System.Drawing.Size(_CaliPage.Size.Width, _CaliPage.Size.Height + HeadHeight);
            Controls.Add(_CaliPage);
            Controls.SetChildIndex(_CaliPage, 0);
            this.IsShowHelp = false;
            DefaultStyleManager.Instance.RegisterControlRecursion(this, StyleFlag.FontSize);
        }
        private void InitControlLang()
        {
            Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ProbeUserCaliForm.Text");
            Title = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ProbeUserCaliForm.Title");
        }
        /// <summary>
        /// 初始化各标签的内容信息
        /// </summary>
        private void InitInfo()
        {
        }
    }
}

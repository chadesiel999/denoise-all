using System;
using System.Drawing;
using System.Windows.Forms;
using ScopeX.Core.Decode;
using ScopeX.U2.BaseControl;
using ScopeX.UserControls.Style;

namespace ScopeX.U2
{
    public partial class CustomDecodeForm :FlashBorderForm
    {
        private ComModel.SerialProtocolType _ProtocolType;
        private int _Width;
        private int _Height;
        public CustomDecodeForm(ComModel.SerialProtocolType protocolType = ComModel.SerialProtocolType.Close)
        {
            InitializeComponent();
            _ProtocolType = protocolType;
            DefaultStyleManager.Instance.RegisterControlRecursion(this, StyleFlag.FontSize);
            this.HeadBackColor = AppStyleConfig.DefaultTitleBackColor;
            _Width=Width; 
            _Height=Height;
            if (_ProtocolType != ComModel.SerialProtocolType.Close)
            {
                InitTriggerDecodeView();
            }
            HelpClick += (_, _) =>
            {
                //var res = Int32.TryParse(HelpLabel, out var index);
                //if (!res)
                //{
                //    HelpProcessManager.SendCommand();
                //    EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(this, new EventBus.LogEventArgs($"Failed to obtain help index information({HelpLabel})!", EventBus.LogLevel.Debug));
                //    return;
                //}
                HelpProcessManager.SendCommand(HelpDocumentManager.Default.GetCommand(nameof(CustomDecodeForm)));
            };
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Size = new Size(_Width, _Height);
        }
        //protected override void OnClosing(CancelEventArgs e)
        //{
        //    base.OnClosing(e);
        //    this.Dispose();
        //}
        private void InitTriggerDecodeView()
        {
            if (_ProtocolType != ComModel.SerialProtocolType.Close)
            {
                var result = DecodeApp.Default.TriggerDecodeViews[_ProtocolType];
                if (result != null)
                {
                    result.Presenter = ProtocolPrsnt.GetTrigSerialDecodePrsnt(null, _ProtocolType, result);
                    result.Presenter.IsTrigger = true;
                    result.Presenter.TryAddView(result);
                    this.ContentControl.Fill = DecodeBaseControl.FillEnum.AutoSize;
                    this.ContentControl.Content = result as Control;
                    result.ReLoadSource();
                    result.UpdateView(result.Presenter, String.Empty);
                }
                else
                {

                }
            }
            else
            {

            }
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (e.KeyChar == (Char)Keys.Escape)
            {
                Close();
                return;
            }
            base.OnKeyPress(e);
        }

        private void CustomDecodeForm_Load(object sender, EventArgs e)
        {
            // LanguageFactory.CacheFormLanguageControls(this);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }
    }
}

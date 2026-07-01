using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace ScopeX.U2
{
    public class DecodeBaseControl : Control
    {
        public event EventHandler ContentChanged;
        public DecodeBaseControl()
        {
            SetStyle(ControlStyles.Selectable, false);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
        }

        private FillEnum _Fill = FillEnum.AutoSize;
        public FillEnum Fill
        {
            get { return _Fill; }
            set
            {
                if (_Fill != value)
                {
                    _Fill = value;
                    SetSize();
                }
            }
        }
        public class ControlBrowserEditor : UITypeEditor
        {
            public ControlBrowserEditor()
            {
            }
            public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
            {
                return UITypeEditorEditStyle.DropDown;
            }
            public override Object EditValue(ITypeDescriptorContext context, IServiceProvider provider, Object value)
            {
                if (value is Control)
                    return value;

                IWindowsFormsEditorService edsvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

                if (edsvc != null)
                {
                    ControlBrowserControl controlBrowserControl = new ControlBrowserControl();
                    edsvc.DropDownControl(controlBrowserControl);
                }

                return base.EditValue(context, provider, value);
            }
        }
        internal class ControlBrowserControl : UserControl
        {
            //private Label _Label;
            public ControlBrowserControl()
            {
                // _Label = new Label();
                // _Label.Text = "";
                // Controls.Add(_Label);
            }
        }
        private Control _Content;
        /// <summary>
        /// 需要绑定的控件
        /// 如果设置的控件拥有MinHeight属性且属性类型为<see cref="int"/>时，本控件会将MinHeight属性值设置为控件的Height属性
        /// </summary>

        [Editor(typeof(ControlBrowserEditor), typeof(UITypeEditor))]
        [Browsable(true)]
        public Control Content
        {
            get => _Content;
            set
            {
                //if (value != _Content)
                //{
                _Content = value;
                SetContext(value);
                ContentChanged?.Invoke(this, EventArgs.Empty);
                //}

                if (value == null)
                    Controls.Clear();
            }
        }
        private void SetContext(Control component)
        {
            this.Controls.Clear();

            if (component != null)
            {
                this.Controls.Add(component);//bug here
                SetSize();
                component.SizeChanged -= Component_SizeChanged;
                component.SizeChanged += Component_SizeChanged;
            }
        }

        private void Component_SizeChanged(object sender, EventArgs e) => SetSize();

        private void SetSize()
        {
            if (_Content == null)
                return;

            switch (Fill)
            {
                case FillEnum.AutoSize:
                    Height = _Content.Height;
                    Width = _Content.Width;
                    break;
                case FillEnum.AutoWidth:
                    Width = _Content.Width;
                    break;
                case FillEnum.AutoHeight:
                    Height = _Content.Height;
                    break;
                case FillEnum.None:
                    break;
            }
            _Content.Dock = DockStyle.Fill;
        }
        private new ControlCollection Controls => base.Controls;

        public enum FillEnum
        {
            None,
            AutoHeight,
            AutoWidth,
            AutoSize,
        }
    }
}

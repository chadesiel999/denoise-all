// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/4/20</date>

namespace ScopeX.U2
{
    using ScopeX.ComModel;
    using ScopeX.Controls.Common.Default;
    using ScopeX.Core;
    using ScopeX.UserControls.Style;
    using System;
    using System.ComponentModel;
    using System.Windows.Forms;

    /// <summary>
    /// Defines the <see cref="DisplaySettingPage" />.
    /// </summary>
    public partial class DisplaySettingPage : UserControl, IDisplayView, IStylize
    {
        [Browsable(false)]
        public StyleFlag StyleFlags { get; set; } = StyleFlag.None;

        [Description("是否风格化"), Browsable(true), DefaultValue(typeof(Boolean)), Category(Const.Category)]
        public Boolean StylizeFlag { get; set; } = false;

        private Boolean _ArgToCtrl = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="DisplaySettingPage"/> class.
        /// </summary>
        public DisplaySettingPage()
        {
            InitializeComponent();

            LblBrightness.Visible = PlatformUIManager.Default.Platform.Attribute.SupportGetOrSetBrightness;
            TrbBrigtness.Visible = BtnBrigtnessDefultValue.Visible = LblBrightness.Visible;

            TrbBrigtness.ValueChanged += (s, e) =>
            {
                if (_ArgToCtrl)
                {
                    return;
                }
                //  ScreenUtility.SetBrigtness((Int32)TrbBrigtness.Value);
                Presenter.ScreenBrightness = (Int32)TrbBrigtness.Value;
            };
            //TrbBrigtness.MouseUp += (s, e) => { base.OnMouseUp(e); };
            TrbContrast.ValueChanged += (s, e) =>
            {
                if (_ArgToCtrl)
                {
                    return;
                }
                //  ScreenUtility.SetContrast((Int32)TrbContrast.Value);
                Presenter.ScreenContrast = (Int32)TrbContrast.Value;
            };
            //TrbContrast.MouseUp += (s, e) => { base.OnMouseUp(e); };
            BtnBrigtnessDefultValue.Click += (s, e) =>
            {
                TrbBrigtness.Value = TrbBrigtness.MaxValue * 0.9f;
                Presenter.ScreenBrightness = (Int32)TrbBrigtness.Value;
            };

            BtnContrastDefultValue.Click += (s, e) =>
            {
                TrbContrast.Value = TrbContrast.MaxValue * 0.9f;
                Presenter.ScreenContrast = (Int32)TrbContrast.Value;
            };
        }

        /// <summary>
        /// Gets the DesignMode.
        /// </summary>
        protected new Boolean DesignMode
        {
            get
            {
                Boolean rtnflag = false;
#if DEBUG
                rtnflag = DesignTimeHelper.InDesignMode(this);
#endif
                return rtnflag;
            }
        }

        /// <summary>
        /// Gets or sets the Presenter.
        /// </summary>
        IDisplayPrsnt IView<IDisplayPrsnt>.Presenter
        {
            get => Presenter;
            set => Presenter = (DisplayPrsnt)value;
        }

        public DisplayPrsnt Presenter
        {
            get;
            set;
        }

        public void UpdateView(Object prsnt, String propertyName)
        {
            if (String.IsNullOrEmpty(propertyName))
            {
                UpdateView();
                return;
            }

            _ArgToCtrl = true;
            switch (propertyName)
            {
                case nameof(Presenter.ScreenBrightness):
                    TrbBrigtness.Value = Presenter.ScreenBrightness;
                    break;
                case nameof(Presenter.ScreenContrast):
                    TrbContrast.Value = Presenter.ScreenContrast;
                    break;
                default:
                    break;
            }
            _ArgToCtrl = false;
        }

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                _ArgToCtrl = true;
                TrbBrigtness.Value = Presenter.ScreenBrightness;
                TrbContrast.Value = Presenter.ScreenContrast;
                _ArgToCtrl = false;
            }
        }

        /// <summary>
        /// The OnLoad.
        /// </summary>
        /// <param name="e">The e<see cref="EventArgs"/>.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            UpdateView();
        }
    }
}

using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using ScopeX.ComModel;
using ScopeX.Core;
using ScopeX.UserControls.Style;
using ScopeX.Controls.Common.Default;

namespace ScopeX.U2
{
    public partial class OtherPage : UserControl, IChnlView, IStylize
    {
        #region Field&Property

        [Browsable(false)]
        public StyleFlag StyleFlags { get; set; } = StyleFlag.None;

        [Description("是否风格化"), Browsable(true), DefaultValue(typeof(Boolean)), Category(Const.Category)]
        public Boolean StylizeFlag { get; set; } = false;

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

        public AnalogPrsnt Presenter
        {
            get => (AnalogPrsnt)(ParentForm as IChnlView).Presenter;
            set => (ParentForm as IChnlView).Presenter = value;
        }

        //IChnlPrsnt IView<IChnlPrsnt>.Presenter
        //{
        //    get => Presenter;
        //    set => Presenter = (AnaChnlPrsnt)value;
        //}

        IBadge IView<IBadge>.Presenter
        {
            get => Presenter;
            set => Presenter = (AnalogPrsnt)value;
        }
        #endregion Field&Property

        public OtherPage()
        {
            InitializeComponent();
        }

        public void UpdateView(Object presenter, String propertyName)
        {
            if (String.IsNullOrEmpty(propertyName))
            {
                return;
            }
        }

        private void BtnHistgram_Click(object sender, EventArgs e)
        {
            var activedprsnt = (Program.Oscilloscope.View as DsoForm).Presenter.TryGetRange(c => c.Active && c.Id.IsMath());
            var hp = activedprsnt.FirstOrDefault(ap => (ap as MathPrsnt).Args is MathHistArg hma && hma.Source == Presenter.Id);
            if (hp == null)
            {
                (Program.Oscilloscope.View as DsoForm).TryAddMathWaveform(mp =>
                {
                    var mha = (MathHistArg)mp.GetOrMakeArg(MathType.Histgram);
                    mha.Source = Presenter.Id;
                });
            }
            else
            {
                DsoPrsnt.FocusId = hp.Id;//切换到当前channel
            }
        }
    }
}

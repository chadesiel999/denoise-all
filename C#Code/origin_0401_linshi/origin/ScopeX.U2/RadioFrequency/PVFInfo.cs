using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScopeX.Core;
using ScopeX.Core.Tools;

namespace ScopeX.U2
{
    public partial class PVFInfo : ChnlBadgeInfo
    {
        public PVFInfo(ChannelPrsnt cp) : base(cp, typeof(PVFForm))
        {
            InitializeComponent();
            ColumnStyles = new List<(Single Data, ContentAlignment Align)>()
        {
            (30, ContentAlignment.MiddleLeft),
            (70, ContentAlignment.MiddleRight),
        };

            Size = new(150, 110);
        }

        public PhaseVSFrequencyPrsnt Presenter
        {
            get => (PhaseVSFrequencyPrsnt)InternalPrsnt;
            set => InternalPrsnt = value;
        }

        protected override void Update(Object prsnt, String propertyName)
        {
            switch (propertyName)
            {
                case nameof(Presenter.Source):
                    Invalidate();
                    break;
                case nameof(Presenter.PhaseScale):
                    UpdateView();
                    break;
            }
        }

        protected override void UpdateView()
        {
            DataSource = new List<Object>()
        {
            new List<Object>{"Phase", VirticalScaleToString() },
        };
        }

        protected override void OnDrawHeader(Graphics g)
        {
            base.OnDrawHeader(g);

            TextRenderer.DrawText(g,
               Presenter.Source.ToString(),
               HeaderFont,
               new Rectangle(0, 0, Width, HeaderHeight),
               HeaderForeColor,
               TextFormatFlags.Right | TextFormatFlags.VerticalCenter);
        }

        private String VirticalScaleToString()
        {
            return new Quantity(Presenter.PhaseScale, Presenter.Prefix, Presenter.PhaseVSFrequencyUnitV).ToString();
        }

    }
}

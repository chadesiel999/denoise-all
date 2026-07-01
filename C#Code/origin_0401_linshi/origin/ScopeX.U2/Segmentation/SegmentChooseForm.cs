using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScopeX.ComModel;
using ScopeX.Core;
using ScopeX.U2.LanguageSupoort;
using ScopeX.UserControls;
using ScopeX.UserControls.Style;

namespace ScopeX.U2
{
    public partial class SegmentChooseForm : FloatForm, ITimebaseView
    {
        public SegmentChooseForm(TimebasePrsnt prsnt)
        {
            InitializeComponent();
            _Presenter = prsnt;
            Stylize();

        }

        private void Stylize()
        {
            DefaultStyleManager.Instance.RegisterControlRecursion(this, StyleFlag.FontSize);
            //HeadBackColor = Color.FromArgb(62, 62, 62);
        }

        ITimebasePrsnt IView<ITimebasePrsnt>.Presenter
        {
            get => _Presenter;
            set => _Presenter = (TimebasePrsnt)value;
        }

        private TimebasePrsnt _Presenter;

        public void UpdateView(object presenter, string propertyName)
        {
            if (InvokeRequired)
            {
                BeginInvoke(() => { UpdateView(propertyName); });
            }
            UpdateView(propertyName);
        }

        public void UpdateView(string propertyName = null)
        {
            if (_Presenter == null) return;
            LvFrame.Controls.Clear();
            int width = 60;
            int height = 45;
            Int32 columnNum = LvFrame.Width / width;

            for (Int32 index = 0; index < _Presenter.ChoseFrameIds.Count; index++)
            {
                var infoTmp = new SegmentFrameInfoPage(_Presenter);
                infoTmp.SegmentId = index;
                Int32 x = (index % columnNum) * width;
                Int32 y = (index / columnNum) * height;
                infoTmp.Width = width;
                infoTmp.Height = height;
                infoTmp.Location = new Point(x, y);
                LvFrame.Controls.Add(infoTmp);
            }
        }

        private void FragementChooseForm_Load(object sender, EventArgs e)
        {
            UpdateView();
            // LanguageFactory.CacheFormLanguageControls(this);
        }
    }
}

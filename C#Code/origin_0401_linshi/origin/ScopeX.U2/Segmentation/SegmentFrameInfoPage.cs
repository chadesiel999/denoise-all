using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScopeX.Controls.Common.Helper;
using ScopeX.Core;
using ScopeX.Core.Tools;

namespace ScopeX.U2
{
    public partial class SegmentFrameInfoPage : UserControl, ITimebaseView
    {
        public SegmentFrameInfoPage(TimebasePrsnt prsnt)
        {
            InitializeComponent();
            _Presenter = prsnt;
        }

        private TimebasePrsnt _Presenter;

        ITimebasePrsnt IView<ITimebasePrsnt>.Presenter
        {
            get => _Presenter;
            set => _Presenter = (TimebasePrsnt)value;
        }

        private void UpdateView(string propertyName = null)
        {
            if (_Presenter == null) return;
            if (SegmentId >= 0 && SegmentId < _Presenter.ChoseFrameIds.Count)
                BtnFramesInfo.Text = (_Presenter.ChoseFrameIds[SegmentId] + 1).ToString();
        }

        public void UpdateView(object presenter, string propertyName)
        {
            if (InvokeRequired)
            {
                BeginInvoke(() => { UpdateView(propertyName); });
            }
            UpdateView(propertyName);
        }

        private Int32 _SegmentId = 0;
        public Int32 SegmentId 
        {
            get => _SegmentId;
            set
            {
                _SegmentId = value;
                if (_SegmentId >= _Presenter.ChoseFrameIds.Count)
                    _SegmentId = _Presenter.ChoseFrameIds.Count - 1;
                if (_SegmentId < 0)
                    _SegmentId = 0;
                UpdateView();
            }
        }


        private void BtnFramesInfo_Click(object sender, EventArgs e)
        {
            if (_Presenter.ChoseFrameIds == null || _Presenter.ChoseFrameIds.Count == 0)
            {
                return;
            }

            var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, BtnFramesInfo);
            var onokclickeventaction = new Action<Double>((data) =>
            {
                _Presenter.ChoseFrameIds[SegmentId] = (Int32)data - 1;
                UpdateView();
            });

            nkf.SetKeyBoardValue($"{SegmentId} / {_Presenter.FrameCount}", "", 0, onokclickeventaction,
                _Presenter.ChoseFrameIds[SegmentId] + 1, _Presenter.FrameCount, 1);
            nkf.ShowDialogByPosition();
        }
    }
}

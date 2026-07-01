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
using ScopeX.Controls.Common.Default;
using ScopeX.UserControls.Style;

namespace ScopeX.U2
{
    public partial class LissajousPage : UserControl, IStylize
    {
        private static ChannelId _LissaSrcX = ChannelId.C1;
        private static ChannelId _LissaSrcY = ChannelId.C2;

        public LissajousPage()
        {
            InitializeComponent();
        }


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

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                CbxSourceX.SelectValue = (Int32)_LissaSrcX;
                CbxSourceY.SelectValue = (Int32)_LissaSrcY;

               // CbxSourceX.SelectedIndex = (Int32)_LissaSrcX;
                //CbxSourceY.SelectedIndex = (Int32)_LissaSrcY;
            }
        }

        public override void Refresh()
        {
            base.Refresh();
            UpdateView();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            UpdateView();
        }


       // private void CbxSourceX_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    //Presenter.Lissa.SourceX = (ChannelId)CbxSourceX.SelectedIndex;
         //   _LissaSrcX = (ChannelId)CbxSourceX.SelectedIndex;

        //}

       // private void CbxSourceY_SelectedIndexChanged(object sender, EventArgs e)
       // {
           // //Presenter.Lissa.SourceY = (ChannelId)CbxSourceY.SelectedIndex;
          //  _LissaSrcY = (ChannelId)CbxSourceY.SelectedIndex;

        //}

        private void BtnAddLissa_Click(object sender, EventArgs e)
        {
            (ParentForm.Owner as DsoForm).TryAddLissajousUI(_LissaSrcX, _LissaSrcY);
        }

        private void CbxSourceX_SelectedIndexChanged(object sender, EventArgs e)
        {
            _LissaSrcX = (ChannelId)CbxSourceX.SelectValue;
        }

        private void CbxSourceY_SelectedIndexChanged(object sender, EventArgs e)
        {
            _LissaSrcY = (ChannelId)CbxSourceY.SelectValue;
        }

    }
}

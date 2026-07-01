using ScopeX.ComModel;
using ScopeX.U2.BaseControl;
using ScopeX.UserControls;
using ScopeX.UserControls.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;


namespace ScopeX.U2
{
    public partial class MeasSelectionForm :FlashBorderForm
    {
        private const Int32 _ITEM_WIDTH = 166;
        private const Int32 _ITEM_HEIGHT = 45;

        private ScopeXIconButton _LastSelected;

        private readonly Func<String, ChannelId, Boolean> _GetResults;
        private List<ScopeXIconButton> VerButtons = new List<ScopeXIconButton>();
        private List<ScopeXIconButton> HorButtons = new List<ScopeXIconButton>();
        private List<ScopeXIconButton> OtherButtons = new List<ScopeXIconButton>();

        private Boolean _IsDoubleClickColose = true;
        public MeasSelectionForm(String pn, ChannelId src, Func<String, ChannelId, Boolean> fnGetResult,Boolean isDoubleClickColose=true)
        {
            InitializeComponent();
            _IsDoubleClickColose = isDoubleClickColose;
            InitControlsText();
            this.HelpClick += (_, _) =>
            {
                //var res = Int32.TryParse(HelpLabel, out var index);
                //if (!res)
                //{
                //    HelpProcessManager.SendCommand();
                //    EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(this, new EventBus.LogEventArgs($"Failed to obtain help index information({HelpLabel})!", EventBus.LogLevel.Debug));
                //    return;
                //}
                HelpProcessManager.SendCommand(HelpDocumentManager.Default.GetCommand(nameof(MeasSelectionForm)));
            };
            var candidates = MeasureApp.Default.MeasCandidates;
            var srcs = Program.Oscilloscope.FindIdentities(c => c.Id.IsAnalog() || (c.Active && (c.Id.IsReference() /*|| c.Id.IsMath()*/))).OrderBy(x => x);
            //if (Program.Oscilloscope.TryGetChannel(ChannelId.D0, out var cp))//暂时屏蔽参数测量的LA源选择
            //{
            //    var dp = (DigitalPrsnt)cp;
            //    for (Int32 i = 0; i < dp.BitLength; i++)
            //    {
            //        if (dp.GetActiveAt(i))
            //        {
            //            srcs.Add(ChannelId.D0 + i);
            //        }
            //    }
            //}
            InitSourceList(srcs, src);
            VerButtons.Clear();
            HorButtons.Clear();
            OtherButtons.Clear();
            InitTabPage(TpgVertical, candidates.Values.Where(item => item.Catalog.Trim() == "Vertical"), _ITEM_WIDTH, _ITEM_HEIGHT);
            InitTabPage(TpgHorizontal, candidates.Values.Where(item => item.Catalog.Trim() == "Horizontal"), _ITEM_WIDTH, _ITEM_HEIGHT);
            InitTabPage(TpgOther, candidates.Values.Where(item => item.Catalog.Trim() == "Other"), _ITEM_WIDTH, _ITEM_HEIGHT);

            if (!candidates.ContainsKey(pn))
            {
                pn = "Max";
            }
            var _selected = candidates[pn];
            TcmParameters.SelectedIndex = _selected.Catalog.Trim() switch
            {
                "Vertical" => 0,
                "Horizontal" => 1,
                "Other" or _ => 2,
            };

            var uib = TcmParameters.TabPages[TcmParameters.SelectedIndex].Controls[$"Btn{_selected.Name.Replace("@", "")}"] as ScopeXIconButton;
            BtnItem_Clicked(uib, null);

            LblSelectedName.Text = _LastSelected.Text;
            LblSelectedDescription.Text = _LastSelected.AccessibleDescription;
            //PbxSelectedIcon.Image = ImageList.Images[_selected.IconFileName];
            PbxSelectedIcon.Image = _LastSelected.Icon;

            _GetResults = fnGetResult;
        }

        private void InitControlsText()
        {
            TpgVertical.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ChuiZhi");
            TpgHorizontal.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ShuiPing");
            TpgOther.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("QiTa");
            BtnSelect.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("QueDing");
            BtnClose.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("GuanBi");
            LblSource.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("CeLiangYuan");
            Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("XuanZeCeLiangCanShu");
            Title = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("XuanZeCeLiangCanShu");
        }

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

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }

        private void InitSourceList(IEnumerable<ChannelId> sources, ChannelId current)
        {
            int num = CbxSource.CbbItem.Length + CbxSource.MainItem.Length - 1;
            var source = sources.Select(ch => $"{ch}").ToArray();
            if (source.Length >= num + 1)
            {
                string[] btnsources = source.Take(num).ToArray();
                string[] cbsources = source.Skip(num).ToArray();
                var str = CbxSource.CbbItem.ToList();
                str.Clear();
                str.AddRange(cbsources);
                CbxSource.CbbItem = str.ToArray();
            }
            //var index = CbxSource.FindIndexInAllItem(current.ToString());
            CbxSource.SelectedIndex = CbxSource.FindIndexInAllItem(current.ToString(), 0);
            CbxSource.SelectedValueChanged += (_, _) =>
            {
                Refresh();
            };
            //if (index == -1)
            //{
            //    = source[0];
            //}
            //else
            //{
            //    CbxSource.SelectedItem = source[index];
            //    //CbxSource.SelectedIndex = CbxSource.FindString($"{current}");
            //}

            //CbxSource.DataSource = sources.Select(x => new SelectComboBox.ComboBoxItem(x.ToString(), x)).ToList();
            //if (sources.Contains(current))
            //    CbxSource.SelectValue = current;
            //CbxSource.DataSource = sources.Select(x => new KeyValuePair<String, ChannelId>(x.ToString(), x)).ToList();
            //CbxSource.DisplayMember = "Key";
            //CbxSource.ValueMember = "Value";
            //CbxSource.SelectValue = CbxSource.Items[(int)current];
            //var idx = CbxSource.FindStringExact(current.ToString());
            //if (idx >= 0)
            //{
            //	CbxSource.SelectedIndex = idx;
            //}

            //CbxSource.SelectedIndexChanged += (s, e) =>
            //{
            //    if (Program.Oscilloscope.TryGetChannel((ChannelId)CbxSource.SelectedValue, out var ap))
            //    {
            //        HeadBackColor = ap.DrawColor.GetBrightnessColor(-0.2);
            //    }
            //};
        }
        public override void Refresh()
        {
            var candidates = MeasureApp.Default.MeasCandidates;
            TpgVertical.Controls.Clear();
            TpgHorizontal.Controls.Clear();
            TpgOther.Controls.Clear();
            foreach (var uib in VerButtons)
            {
                uib.Enabled = MeasureApp.Default.Presenter.SelectedItems.FirstOrDefault(o => o.Active && o.Name == uib.Tag.ToString() && o.Source.ToString() == CbxSource.SelectedValue) == null ? true : false;
                if (!uib.Enabled)
                {
                    uib.BackColor = Color.Gray;
                }
                else
                {
                    uib.BackColor = Color.Transparent;
                }
                TpgVertical.Controls.Add(uib);
            }
            foreach (var uib in HorButtons)
            {
                uib.Enabled = MeasureApp.Default.Presenter.SelectedItems.FirstOrDefault(o => o.Active && o.Name == uib.Tag.ToString() && o.Source == (ChannelId)CbxSource.SelectedIndex) == null ? true : false;
                if (!uib.Enabled)
                {
                    uib.BackColor = Color.Gray;
                }
                else
                {
                    uib.BackColor = Color.Transparent;
                }
                TpgHorizontal.Controls.Add(uib);
            }
            foreach (var uib in OtherButtons)
            {
                uib.Enabled = MeasureApp.Default.Presenter.SelectedItems.FirstOrDefault(o => o.Active && o.Name == uib.Tag.ToString() && o.Source == (ChannelId)CbxSource.SelectedIndex) == null ? true : false;
                if (!uib.Enabled)
                {
                    uib.BackColor = Color.Gray;
                }
                else
                {
                    uib.BackColor = Color.Transparent;
                }
                TpgOther.Controls.Add(uib);
            }
            base.Refresh();

        }
        private void InitTabPage(TabControlMenuPage tcmp, IEnumerable<MeasureItemProperties> items, Int32 width, Int32 height)
        {
            Int32 count = 0;

            foreach (var item in items)
            {
                Int32 x = (count % 7) * width;
                Int32 y = (count / 7) * height;

                Image img = (Image)Properties.Resources.ResourceManager.GetObject(item.IconFileName);
                if (img is null)
                {
                    img = Properties.Resources.ImageUtility;
                }

                ScopeXIconButton uib = new()
                {
                    Size = new Size(width, height),
                    Location = new Point(x, y),
                    Name = $"Btn{item.Name.Replace("@", "")}",
                    Tag = item.Name,
                    Text = item.Text,
                    AccessibleDescription = item.Description,
                    ForeColor = Color.White,
                    BorderColor = Color.FromArgb(72, 72, 72),
                    BorderThickness = 1,
                    IconSize = new Size(40, 40),
                    Icon = img,
                    CornerRadius = 0,
                    MouseinBorderColor = Color.FromArgb(72, 72, 72),
                    PressedForeColor = Color.White,
                    MouseinForeColor = Color.White,
                    PressedBorderColor = Color.FromArgb(72, 72, 72),
                    DaskArray = new int[] { 4, 4 },
                    TextAlign = ContentAlignment.MiddleLeft,
                    TabIndex = count,
                };
                uib.Enabled = MeasureApp.Default.Presenter.SelectedItems.FirstOrDefault(o => o.Active && o.Name == uib.Tag.ToString() && o.Source == (ChannelId)CbxSource.SelectedIndex) == null ? true : false;
                if (!uib.Enabled)
                {
                    uib.BackColor = Color.Gray;
                }
                else
                {
                    uib.BackColor = Color.Transparent;
                }
                if (Program.Oscilloscope.SysLanguage == Language.English)
                {
                    uib.StylizeFlag = false;
                    uib.Font = new System.Drawing.Font("MiSans", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
                }

                if (tcmp == TpgVertical)
                    VerButtons.Add(uib);
                else if (tcmp == TpgHorizontal)
                    HorButtons.Add(uib);
                else
                    OtherButtons.Add(uib);
                uib.Click += BtnItem_Clicked;
                uib.DoubleClick += BtnItem_DoubleClicked;
                tcmp.Controls.Add(uib);
                count++;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Stylize();
#if SaveLanguage
            // LanguageFactory.CacheFormLanguageControls(this);
#endif
        }

        private void Stylize()
        {
            IsShowHelp = false;
            ScopeX.UserControls.Style.DefaultStyleManager.Instance.RegisterControlRecursion(this, StyleFlag.FontSize);
            //ScopeX.UserControls.Style.DefaultStyleManager.Instance.RegisterControlRecursion(LblSelectedDescription, StyleFlag.FontSize);
            LblSelectedDescription.MultyLineFlag = true;
            //this.HeadBackColor = AppStyleConfig.DefaultTitleBackColor;
            //this.ContentBackColor=AppStyleConfig.DefaultContextBackColor;
            LblSelectedDescription.TextAlign = System.Drawing.ContentAlignment.TopLeft;
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (e.KeyChar == (Char)Keys.Escape)
            {
                BtnClose_Click(this, new());
                return;
            }
            base.OnKeyPress(e);
        }

        private void BtnItem_Clicked(Object obj, EventArgs args)
        {
            if (obj is ScopeXIconButton uib && _LastSelected != uib)
            {
                if (_LastSelected != null)
                {
                    _LastSelected.BorderColor = Color.FromArgb(72, 72, 72);
                }

                if (uib.Parent != null && uib.Parent is TabPage tpage)
                {
                    foreach (var item in tpage.Controls)
                    {
                        if (item is ScopeXIconButton ctl)
                            ctl.BorderColor = Color.FromArgb(72, 72, 72);
                    }
                }

                _LastSelected = uib;

                LblSelectedName.Text = _LastSelected.Text;
                LblSelectedDescription.Text = _LastSelected.AccessibleDescription;
                PbxSelectedIcon.Image = _LastSelected.Icon;

                _LastSelected.BorderColor = AppStyleConfig.DefaultCheckedBackColor;
                _LastSelected.MouseinBorderColor = _LastSelected.BorderColor;
            }
        }

        private void BtnItem_Selected(Object sender, EventArgs e)
        {
            if (!Enum.TryParse(typeof(ChannelId), CbxSource.SelectedValue, out object? channelId) || channelId == null)
            {
                return;
            }

            if (_GetResults((String)_LastSelected.Tag, (ChannelId)channelId))
            {
                DialogResult = DialogResult.Cancel;
                Close();
            }
        }

        private void BtnItem_DoubleClicked(Object sender, EventArgs e)
        {
            if (!Enum.TryParse(typeof(ChannelId), CbxSource.SelectedValue, out object? channelId) || channelId == null)
            {
                return;
            }

            if (_GetResults((String)_LastSelected.Tag, (ChannelId)channelId))
            {
                if(_IsDoubleClickColose||MeasureApp.Default.Presenter.SelectedItems.Where(x=>!x.Active).Count()== 0)//界面加号小时，通过加号进入的界面需要关闭
                {
                    DialogResult = DialogResult.Cancel;
                    Close();
                }
                else
                {
                    this.Refresh();
                }
            }
        }


        private void BtnClose_Click(Object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}

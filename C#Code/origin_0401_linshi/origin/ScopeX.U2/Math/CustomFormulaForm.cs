using Newtonsoft.Json.Linq;
using ScopeX.ComModel;
using ScopeX.Controls.Language;
using ScopeX.Core;
using ScopeX.Core.Tools;
using ScopeX.MathExt;
using ScopeX.U2.BaseControl;
using ScopeX.UserControls;
using ScopeX.UserControls.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace ScopeX.U2
{
    public partial class CustomFormulaForm : FlashBorderForm, IChnlView
    {
        private readonly List<String> _TokenBkup = new();

        private List<RadioButtonItem> _ChsItems = new List<RadioButtonItem>();

        private String _Formula;

        private MathCustomArg _CustomArg;

        private List<String> _Token = new List<String>();

        private DsoPrsnt _Oscilloscope;
        public CustomFormulaForm(DsoPrsnt Oscilloscope)
        {
            InitializeComponent();
            InitChannelSelectItems();
            InitRdoSource();
            InitToolTip();
            _Oscilloscope = Oscilloscope;
            LanguageManger.Instance.LanguageChanged += Instance_LanguageChanged;
            HelpClick += (_, _) =>
            {
                //var res = Int32.TryParse(HelpLabel, out var index);
                //if (!res)
                //{
                //    HelpProcessManager.SendCommand();
                //    EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(this, new EventBus.LogEventArgs($"Failed to obtain help index information({HelpLabel})!", EventBus.LogLevel.Debug));
                //    return;
                //}
                HelpProcessManager.SendCommand(HelpDocumentManager.Default.GetCommand(nameof(CustomFormulaForm)));
            };
        }

        /// <summary>
        /// 产品适配，如果布局调整后，请确认每个产品是否需要适配更改
        /// </summary>
        private void InitRdoSource()
        {
            var temp = RdoSource.ButtonItems[..].ToList();
            if (!PlatformUIManager.Default.Platform.Attribute.SupportDigital)
            {
                temp.RemoveAt(2);
            }
            RdoSource.ButtonItems = temp.ToArray();
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

        public MathPrsnt Presenter
        {
            get;
            set;
        }

        IBadge IView<IBadge>.Presenter
        {
            get => Presenter;
            set => Presenter = (MathPrsnt)value;
        }

        public void UpdateView(Object prsnt, String propertyName)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<Object, String>(Update), new[] { prsnt, propertyName });
            }
            else
            {
                Update(prsnt, propertyName);
            }
        }

        protected void Update(Object prsnt, String propertyName)
        {
            if (propertyName is nameof(_CustomArg.Formula))
            {
                BackupToken();
                UpdateFormulaText();
                _CustomArg.Formula = _Formula;
            }
        }

        private void BackupToken()
        {
            _Token = new List<String>();
            foreach (var item in _CustomArg.Token)
            {
                _Token.Add(item);
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            Presenter.TryRemoveView(this);
            LanguageManger.Instance.LanguageChanged += Instance_LanguageChanged;
            base.OnFormClosed(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            //风格调整
            Stylize();
            _CustomArg = (MathCustomArg)Presenter.GetOrMakeArg(MathType.Custom);
            BackupToken();
            InitMathFormulaInfo();
            UpdateFormulaText();
            RtbxEditor.SelectionStart = RtbxEditor.Text.Length;
#if SaveLanguage
            LanguageFactory.CacheFormLanguageControls(this);
#endif
        }

        private void Instance_LanguageChanged(object sender, Controls.Language.ILanguage e) => InitMathFormulaInfo();

        private void InitMathFormulaInfo()
        {
            if (_Oscilloscope.MathFormulaCollections != null && _Oscilloscope.MathFormulaCollections.Count > 0)
            {
                InitChannelImages();
                InitRtbxEditor();
                LoadSources(name => name[0] != 'D');
                LoadFunctions();
                LoadNumberics();
            }
        }

        private void Stylize()
        {
            DefaultStyleManager.Instance.RegisterControlRecursion(this, StyleFlag.FontSize);
            //HeadBackColor = Color.FromArgb(62, 62, 62);
            ActiveBorderColor = Color.DeepSkyBlue;
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

        private void LoadFunctions()
        {
            LvFunctions.Controls.Clear();
            Size itemsize = new(116, 45);
            Int32 count = 0;

            foreach (var func in _Oscilloscope.MathFormulaCollections.Where(kvp => kvp.Value.Type == MathDefineFormulaType.Func))
            {
                ScopeXIconButton uib = CreateItemBtn(func, LvFunctions.Width / itemsize.Width, count++, itemsize);
                uib.IconSize = new(28, 28);
                uib.Font = LanguageFactory.Current == Language.简体中文 ? new System.Drawing.Font("MiSans", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point) : new System.Drawing.Font("MiSans", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
                //uib.IconOffset = (uib.Width - uib.IconSize.Width) / 2;
                ToolTip.SetToolTip(uib, func.Value.WeakTipWrapper.Replace("\\n", Environment.NewLine));

                LvFunctions.Controls.Add(uib);
                uib.Click += FunctionsItemBtn_Click;
            }
            ScFunctions.FreshScroller();
        }

        private void LoadNumberics()
        {
            LvNumberics.Controls.Clear();
            Size itemsize = new(59, 45);
            Int32 count = 0;

            foreach (var func in _Oscilloscope.MathFormulaCollections.Where(kvp => kvp.Value.Type == MathDefineFormulaType.Numberic))
            {
                ScopeXIconButton uib = CreateItemBtn(func, LvNumberics.Width / itemsize.Width, count++, itemsize, 1);
                //uib.IconSize = new Size(22, 22);
                //uib.IconOffset = (uib.Width - uib.IconSize.Width) / 2;
                //uib.Text = "";
                uib.Font = new("Arial", 14F);
                ToolTip.SetToolTip(uib, func.Value.WeakTipWrapper.Replace("\\n", Environment.NewLine));

                LvNumberics.Controls.Add(uib);
                uib.Click += FunctionsItemBtn_Click;
            }
        }

        private void LoadSources(Func<String, Boolean> predicate)
        {
            LvSources.Controls.Clear();
            Size itemsize = new(89, 45);
            Int32 count = 0;

            foreach (var src in _Oscilloscope.MathFormulaCollections.Where(kvp => kvp.Value.Type == MathDefineFormulaType.Source && predicate(kvp.Value.Name)))
            {
                ScopeXIconButton uib = CreateItemBtn(src, LvSources.Width / itemsize.Width, count++, itemsize, 2);
                uib.IconSize = new(30, 30);
                uib.IconOffset = (uib.Width - uib.IconSize.Width) / 2;
                ToolTip.SetToolTip(uib, src.Value.WeakTipWrapper.Replace("\\n", Environment.NewLine));

                LvSources.Controls.Add(uib);
                uib.Click += FunctionsItemBtn_Click;
            }
            ScSource.FreshScroller();
        }

        private ScopeXIconButton CreateItemBtn(KeyValuePair<String, MathFormulaInfo> kvpInfo, Int32 columnNum, Int32 index, Size itemSize, Int32 iconText = 3)
        {
            Int32 x = (index % columnNum) * itemSize.Width;
            Int32 y = (index / columnNum) * itemSize.Height;

            ScopeXIconButton uib = new()
            {
                Size = new Size(itemSize.Width, itemSize.Height),
                Location = new Point(x, y),
                Tag = kvpInfo.Key,

                ForeColor = Color.White,
                BorderColor = Color.FromArgb(72, 72, 72),
                BorderThickness = 1,
                //IconSize = new Size(40, 40),

                CornerRadius = 0,
                MouseinBorderColor = Color.FromArgb(40, 71, 193),
                PressedForeColor = Color.White,
                MouseinForeColor = Color.White,
                PressedBorderColor = Color.FromArgb(72, 72, 72),
                DaskArray = new int[] { 4, 4 },
                TextAlign = ContentAlignment.MiddleLeft,
            };
            uib.TabIndex = index;

            if ((iconText & 0x02) != 0)
            {
                uib.Icon = (Image)Properties.Resources.ResourceManager.GetObject(kvpInfo.Value.ImageKey);
                var ttt = Properties.Resources.ResourceManager;
                if (uib.Icon is null)
                {
                    uib.Icon = ImgSelection.Images[kvpInfo.Value.ImageKey];
                    if (uib.Icon is null)
                    {
                        uib.TextAlign = ContentAlignment.MiddleCenter;
                    }
                }
            }
            else
            {
                uib.TextAlign = ContentAlignment.MiddleCenter;
            }

            if ((iconText & 0x01) != 0)
            {
                uib.Text = kvpInfo.Value.Name;
            }

            return uib;
        }


        private void UpdateFormulaText()
        {
            StringBuilder vsb = new();
            StringBuilder expsb = new();
            foreach (var id in _Token)
            {
                if (_Oscilloscope.MathFormulaCollections.Keys.Contains(id))
                {
                    vsb.Append(_Oscilloscope.MathFormulaCollections[id].Symbol);
                    expsb.Append(_Oscilloscope.MathFormulaCollections[id].Expression);
                }
            }

            RtbxEditor.Text = vsb.ToString();
            _Formula = expsb.ToString();
        }

        /// <summary>
        /// 恢复原始的Token对应的内容
        /// </summary>
        private void RevertToken()
        {
            BackupToken();
            UpdateFormulaText();
            _CustomArg.Expression = RtbxEditor.Text;
            _CustomArg.Formula = _Formula;
        }

        private void InitRtbxEditor()
        {
            RtbxEditor.ReadOnly = true;
            RtbxEditor.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Back)
                {
                    if (_Token.Count > 0)
                    {

                    }
                }
            };
        }

        /// <summary>
        /// 根据TokenIndex指定的Token，找到其对应的SelecttionStart;(在Token尾部)
        /// </summary>
        /// <param name="tokenIndex"></param>
        /// <returns></returns>
        private Int32 GetSuitableSelectionStart(Int32 tokenIndex)
        {
            if (tokenIndex == -1)
                return 0;

            Int32 selectionstart = 0;
            for (int i = 0; i <= tokenIndex && i < _Token.Count; i++)
                selectionstart += _Oscilloscope.MathFormulaCollections[_Token[i]].Expression.Length;
            return selectionstart;
        }

        /// <summary>
        /// 获取当前RtbxEditor光标前面的Token的Index;
        /// </summary>
        /// <param name="selecttionstart"></param>
        /// <returns>-1:表示光标在最左边;>=0:表示光标前面的Token的Index</returns>
        private Int32 GetTokenIndex(Int32 selecttionstart)
        {
            Int32 cummulatestart = 0;
            for (int i = 0; i < _Token.Count; i++)
            {
                cummulatestart += _Oscilloscope.MathFormulaCollections[_Token[i]].Expression.Length;
                if (selecttionstart < cummulatestart)
                    return i - 1;
            }
            return _Token.Count - 1;
        }

        private void InitChannelImages()
        {
            foreach (var src in _Oscilloscope.MathFormulaCollections.Where(kvp => kvp.Value.Type == MathDefineFormulaType.Source))
            {
                //if (!ImgSelection.Images.ContainsKey(src.Value.ImageKey))
                //{
                Image img = new Bitmap(32, 32);
                Graphics grp = Graphics.FromImage(img);
                //添加图像
                grp.SetSmoothMode(true);
                grp.DrawEllipse(new Pen(AppStyleConfig.DefaultCheckedBackColor, 2), new Rectangle(1, 1, img.Width - 2, img.Height - 2));
                grp.SetSmoothMode(false);

                //添加文字
                Size textsize = TextRenderer.MeasureText(src.Value.Symbol, AppStyleConfig.DefaultLabelFont);
                Point textpos = new Point((img.Width - textsize.Width) / 2, (img.Height - textsize.Height) / 2);
                TextRenderer.DrawText(grp, src.Value.Symbol, AppStyleConfig.DefaultLabelFont, textpos, AppStyleConfig.DefaultTitleForeColor);

                ImgSelection.Images.Add(src.Value.ImageKey, img);
                //}
            }
        }


        private void BtnClear_Click(object sender, EventArgs e)
        {
            if (_Token.Count != 0)
            {
                _Token.Clear();
                UpdateFormulaText();
            }
        }

        private void BtnBackspace_Click(object sender, EventArgs e)
        {
            if (_Token.Count != 0)
            {
                _Token.RemoveAt(_Token.Count - 1);
                UpdateFormulaText();
            }
        }

        private void FunctionsItemBtn_Click(object sender, EventArgs e)
        {
            if (sender is ScopeXIconButton itembtn)
            {
                //if it is function, add description.
                if (_Oscilloscope.MathFormulaCollections[(String)itembtn.Tag].Type == MathDefineFormulaType.Func)
                {
                    LblDescription.Text = _Oscilloscope.MathFormulaCollections[(String)itembtn.Tag].WeakTipWrapper.Replace("\\n", Environment.NewLine);
                }

                _Token.Add((String)itembtn.Tag);
                UpdateFormulaText();
            }

        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            RevertToken();
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void BtnAccept_Click(object sender, EventArgs e)
        {
            var res = DynamicExecute.Evaluate("Custom", _Formula, out String errmsg);
            if (res || String.IsNullOrEmpty(_Formula)) //如果清空了公式，还是可以点击确定的
            {
                this.DialogResult = DialogResult.OK;
                //Presenter.SetFormula(Presenter.CalcType, _Formula);
                //_CustomArg.UpdateFormula();
                _CustomArg.Token.Clear();
                _CustomArg.Token.AddRange(_Token);
                _CustomArg.Expression = RtbxEditor.Text;
                _CustomArg.Formula = _Formula;

                this.Close();
            }
            else
            {
                EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(this, new EventBus.LogEventArgs(errmsg, EventBus.LogLevel.Warn));
                WeakTip.Default.Write("Formula Editor", MsgTipId.InputFormatError);
            }
        }

        private void TbxDescription_MouseDown(object sender, MouseEventArgs e)
        {
            NativeMethods.HideCaret((sender as TextBox).Handle);
            //LvFunctions.Focus();
        }

        private void BtnLoadFormula_Click(Object sender, EventArgs e)
        {
            //FileBrowserForm fbf = FileBrowserForm.Instance;
            //fbf.SetFileFilter(Enum.GetValues<WfmFormat>().Where(x => x == WfmFormat.Text));
            //fbf.SetPath(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Text(*.txt)|*.txt";
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            // if (fbf.ShowDialogByEvent() == DialogResult.Yes)
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                if (_CustomArg.LoadFormula(dialog.FileName/*fbf.FullFileName*/, out String formula))
                {
                    _Token = _CustomArg.Token;
                    UpdateFormulaText();
                }
                else
                {
                    WeakTip.Default.Write("File", MsgTipId.ReadingFailed);
                }
            }
        }

        private void BtnSaveFormula_Click(Object sender, EventArgs e)
        {
            //FileBrowserForm form = FileBrowserForm.Instance;

            //form.CanEditFileName = true;
            //form.SetFileFilter(Enum.GetValues<WfmFormat>().Where(x => x == WfmFormat.Text));
            //form.SetPath(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));

            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Text(*.txt)|*.txt";
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            //if (form.ShowDialogByEvent() == DialogResult.Yes)
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                FileInfo fileInfo = new FileInfo(dialog.FileName);
                var temp = _CustomArg.Token.ToArray();
                _CustomArg.Token.Clear();
                _CustomArg.Token.AddRange(_Token);
                if (_CustomArg.SaveFormula(/*form.ChoosedFolderPath*/fileInfo.DirectoryName, dialog.FileName /*form.ChoosedFolderName*/, $"{MathType.Custom}:{RtbxEditor.Text}"))
                {
                    WeakTip.Default.Write("File", MsgTipId.SavingSuccess);
                }
                else
                {
                    WeakTip.Default.Write("File", MsgTipId.SavingFailed);
                }
                _CustomArg.Token.Clear();
                _CustomArg.Token.AddRange(temp.ToList());
            }
            // form.CanEditFileName = false;
        }

        private void RdoSource_IndexChanged(Object sender, EventArgs e)
        {
            if (_ChsItems.Count <= RdoSource.ChoosedButtonIndex)
                return;

            var selectitem = _ChsItems[RdoSource.ChoosedButtonIndex];

            switch (selectitem.Tag.ToString())
            {
                case "All":
                    LoadSources(name => name[0] != 'D');
                    break;
                case "Analog":
                    LoadSources(name => name[0] == 'C');
                    break;
                case "Digital":
                    LoadSources(name => name[0] == 'D');
                    break;
                case "Ref":
                    LoadSources(name => name[0] == 'R');
                    break;
            }
        }

        private void InitToolTip()
        {
            ToolTip.Draw += DrawToolTip;
            //ToolTip.Popup += PopupToolTip;
        }

        private void InitChannelSelectItems()
        {
            _ChsItems = new List<RadioButtonItem>();

            _ChsItems.Add(new RadioButtonItem()
            {
                Icon = null,
                Padding = new Padding(0),
                Tag = "All",
                Text = LanguageManger.Instance.GetIDMessage("CustomFormulaForm.TlpEditor.RdoSource.panel1.Btn_0")// "所有通道"
            });

            _ChsItems.Add(new RadioButtonItem()
            {
                Icon = null,
                Padding = new Padding(0),
                Tag = "Analog",
                Text = LanguageManger.Instance.GetIDMessage("CustomFormulaForm.TlpEditor.RdoSource.panel1.Btn_1")// "模拟通道"
            });

            //items.Add(new RadioButtonItem()
            //{
            //    Icon = null,
            //    Padding = new Padding(0),
            //    Tag = "Digital",
            //    Text = LanguageManger.Instance.GetIDMessage("CustomFormulaForm.TlpEditor.RdoSource.panel1.Btn_2")// "所有通道"
            //});

            _ChsItems.Add(new RadioButtonItem()
            {
                Icon = null,
                Padding = new Padding(0),
                Tag = "Ref",
                Text = LanguageManger.Instance.GetIDMessage("CustomFormulaForm.TlpEditor.RdoSource.panel1.Btn_3")// "参考通道"
            });

            //UserControls.RadioButtonItem radioButtonItem1 = new UserControls.RadioButtonItem();
            //UserControls.RadioButtonItem radioButtonItem2 = new UserControls.RadioButtonItem();
            //UserControls.RadioButtonItem radioButtonItem3 = new UserControls.RadioButtonItem();
            //UserControls.RadioButtonItem radioButtonItem4 = new UserControls.RadioButtonItem();
            //radioButtonItem1.Icon = null;
            //radioButtonItem1.Padding = new System.Windows.Forms.Padding(0);
            //radioButtonItem1.Tag = null;
            //radioButtonItem1.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("CustomFormulaForm.TlpEditor.RdoSource.panel1.Btn_0"); // "所有通道";
            //radioButtonItem2.Icon = null;
            //radioButtonItem2.Padding = new System.Windows.Forms.Padding(0);
            //radioButtonItem2.Tag = null;
            //radioButtonItem2.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("CustomFormulaForm.TlpEditor.RdoSource.panel1.Btn_1"); // "模拟通道";
            //radioButtonItem3.Icon = null;
            //radioButtonItem3.Padding = new System.Windows.Forms.Padding(0);
            //radioButtonItem3.Tag = null;
            //radioButtonItem3.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("CustomFormulaForm.TlpEditor.RdoSource.panel1.Btn_2"); // "数字通道";
            //radioButtonItem4.Icon = null;
            //radioButtonItem4.Padding = new System.Windows.Forms.Padding(0);
            //radioButtonItem4.Tag = null;
            //radioButtonItem4.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("CustomFormulaForm.TlpEditor.RdoSource.panel1.Btn_3"); // "参考通道";
            RdoSource.ButtonItems = _ChsItems.ToArray();
        }

        private void DrawToolTip(object sender, DrawToolTipEventArgs e)
        {
            e.DrawBackground();

            TextFormatFlags sf = TextFormatFlags.VerticalCenter |
                TextFormatFlags.Left | TextFormatFlags.LeftAndRightPadding;
            e.DrawText(sf);
        }

        //private void PopupToolTip(object sender, PopupEventArgs e)
        //{
        //    using Font f = new("Arial", 9f);
        //    e.ToolTipSize = TextRenderer.MeasureText(ToolTip.GetToolTip(e.AssociatedControl), f, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.LeftAndRightPadding) + new Size(8, 20);
        //}
    }

}

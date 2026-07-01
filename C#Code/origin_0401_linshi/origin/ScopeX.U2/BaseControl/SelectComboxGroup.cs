using Newtonsoft.Json.Linq;
using ScopeX.Measure;
using ScopeX.UserControls;
using ScopeX.UserControls.Style;
using Svg;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;


namespace ScopeX.U2.BaseControl
{
    public partial class SelectComboxGroup :UserControl
    {
        private FlowLayoutPanel panel;

        private SelectComboBox comboBox = new SelectComboBox();

        private List<ScopeXIconButton> btnList = new List<ScopeXIconButton>();
        private List<KeyValuePair<String, int>> datasource = new List<KeyValuePair<String, int>>();

        private Boolean _IsSelectedChanged = false;

        private int selectedIndex;
        public event EventHandler SelectedValueChanged;


        private String[] _CbbItem = null;
        /// <summary>
        /// ComboBox中展示的项
        /// </summary>
        public String[] CbbItem
        {
            get { return _CbbItem; }
            set
            {
                if (value != _CbbItem)
                {
                    _CbbItem = value;
                    _Items = GetItems();
                    if (value != null && value.Length > 0 && !String.IsNullOrEmpty(value[0]))
                    {
                        comboBox.Items = _CbbItem;
                        comboBox.Enabled = true;
                    }
                    _IsSelectedChanged = true;
                }
            }
        }
        private String _SelectedItem = String.Empty;

        public String SelectedValue
        {
            get => _SelectedItem;
            set
            {
                if (!_SelectedItem.Equals(value))
                {
                    _SelectedItem = value;
                    SelectedIndex = FindIndexInAllItem(value);
                    OnSelectedValueChanged();
                    UpdateView();
                }
            }
        }


        public Int32 SelectedIndex
        {
            get => selectedIndex;
            set
            {
                if (selectedIndex != value|| _IsSelectedChanged)
                {
                    _IsSelectedChanged = false;
                    selectedIndex = value;
                    SelectedValue = Items[value];
                    OnSelectedValueChanged();
                    UpdateView();
                }
            }
        }
        public List<ScopeXIconButton> BtnList
        {
            set => btnList = value;
            get => btnList;
        }

        private String[] _MainItem = null;

        /// <summary>
        /// 直接展示的项
        /// </summary>
        public String[] MainItem
        {
            get
            {
                return _MainItem;
            }
            set
            {
                if (value != _MainItem)
                {
                    _MainItem = value;
                    _Items = GetItems();
                    if (value != null && value.Length > 0)
                    {
                        for (var i = 0; i < value.Length; i++)
                        {
                            ScopeXIconButton btn = new ScopeXIconButton();
                            btnList.Add(btn);
                            btn.BackColor = Color.FromArgb(53, 54, 58);
                            btn.BorderColor = Color.FromArgb(53, 54, 58);
                            btn.ForeColor = Color.White;
                            btn.Cursor = Cursors.Hand;
                            btn.Size = new Size(panel.Width / _MainItem.Length, panel.Height);

                            btn.MouseClick += Btn_Click;
                            btn.Margin = new Padding(0);
                            btn.BorderColor = Color.FromArgb(64, 65, 72);
                            btn.Paint += btn_OnPaint;
                            btn.Text = value[i];
                            DefaultStyleManager.Instance.RegisterControlRecursion(btn, StyleFlag.FontSize);

                            datasource.Add(new KeyValuePair<String, int>(value[i], i));
                            panel.Controls.Add(btn);
                        }
                        _IsSelectedChanged = true;
                    }
                }
            }
        }


        private String[] _Items;

        public String[] Items
        {
            get => _Items;
        }

        private String[] GetItems()
        {
            if (MainItem != null && CbbItem != null)
            {
                return MainItem.Concat(CbbItem).ToArray();
            }
            else if (MainItem != null)
            {
                return CbbItem;
            }
            else if (CbbItem != null)
            {
                return CbbItem;
            }
            else
            {
                return null;
            }
        }


        public SelectComboxGroup()
        {
            InitializeComponent();
            AutoSize = false;
            panel = new FlowLayoutPanel();
            panel.FlowDirection = FlowDirection.LeftToRight;
            panel.Size = new Size((Width / 4) * 3 + 15, Height);
            panel.Margin = new Padding(0);
            panel.Padding = new Padding(0);
            comboBox.Margin = new Padding(0);
            comboBox.ClientSize = new Size((Width / 4) - 15, Height);
            comboBox.AutoSize = false;
            // comboBox.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            comboBox.SelectedIndexChanged += ComboBox_SelectedIndexChanged;
            flowLayoutPanel1.Controls.Add(panel);
            flowLayoutPanel1.Controls.Add(comboBox);
        }
        public int FindIndexInAllItem(String s, int @default = -1)
        {
            var items = Items.ToList();
            var index = items == null ? -1 : items.IndexOf(s);
            return index == -1 ? @default : index;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            UpdateView();
        }

        private void btn_OnPaint(object sender, PaintEventArgs e)
        {
            if (SelectedIndex < btnList.Count && SelectedIndex >= 0)
            {
                ScopeXIconButton btn = btnList[SelectedIndex];
                if (sender is ScopeXIconButton)
                {
                    if ((ScopeXIconButton)sender == btn)
                    {
                        SolidBrush brush = new SolidBrush(Color.FromArgb(0, 162, 198));
                        e.Graphics.FillRectangle(brush, btn.ClientRectangle);
                        TextRenderer.DrawText(e.Graphics, btn.Text, btn.Font, btn.ClientRectangle, Color.Black, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);
                    }
                }
            }

            // 绘制焦点边框

            ControlPaint.DrawBorder(
                e.Graphics,
                ClientRectangle,
                Color.FromArgb(64, 65, 72),      // 左侧线颜色
                1,               // 左侧线宽度
                ButtonBorderStyle.Solid,
                Color.FromArgb(64, 65, 72),      // 右侧线颜色
                1,               // 右侧线宽度
                ButtonBorderStyle.Solid,
                Color.FromArgb(64, 65, 72),      // 上侧线颜色
                1,               // 上侧线宽度
                ButtonBorderStyle.Solid,
                Color.FromArgb(64, 65, 72),      // 下侧线颜色
                1,               // 下侧线宽度
                ButtonBorderStyle.Solid);
        }
        private void Btn_Click(object sender, EventArgs e)
        {
            ScopeXIconButton btn = (ScopeXIconButton)sender;
            int index = btnList.IndexOf(btn);
            SelectedIndex = index;
        }
        //private void Panel_Paint(object sender, PaintEventArgs e)
        //{
        //    Control con = (Control)sender;

        //    int borderWidth = 2;
        //    e.Graphics.FillRectangle(new SolidBrush(Color.Gray), con.Width - borderWidth, 2, borderWidth, con.Height-5);
        //}
        protected virtual void OnSelectedValueChanged()
        {
            SelectedValueChanged?.Invoke(this, EventArgs.Empty);
        }
        public void UpdateComboBoxItems()
        {
            if (CbbItem != null && CbbItem[0] != "")
            {
                comboBox.DataSource = CbbItem;
                for (int i = _MainItem.Length, j = 0; j < CbbItem.Length; i++, j++)
                {
                    datasource.Add(new KeyValuePair<String, int>(CbbItem[j], i));
                }
            }
        }

        private void UpdateView()
        {
            if (comboBox.Items != null && comboBox.Items.Length <= 0)
            {
                comboBox.Enabled = false;
            }
            else
            {
                comboBox.Enabled = true;
            }
            var selectedCom = SelectedIndex - (_MainItem==null?0:_MainItem.Length);
            if (selectedCom >= 0)
            {
                comboBox.ExtText = String.Empty;
                comboBox.SelectIndex = selectedCom;
                comboBox.BackColor = Color.FromArgb(0, 162, 198);
                comboBox.ForeColor = Color.Black;

                foreach (var btn in btnList)
                {
                    btn.BackColor = Color.FromArgb(53, 54, 58);
                    btn.ForeColor = Color.White;
                }
            }
            else
            {
                for (int i = 0; i < _MainItem.Length; i++)
                {
                    if (selectedIndex == i)
                    {
                        btnList[i].BackColor = Color.FromArgb(0, 162, 198);
                        btnList[i].ForeColor = Color.Black;
                        comboBox.SelectIndex = -1;
                    }
                    else
                    {
                        btnList[i].BackColor = Color.FromArgb(53, 54, 58);
                        btnList[i].ForeColor = Color.White;
                    }
                }
                if (comboBox.Enabled)
                {
                    comboBox.ExtText = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("QiTa");
                    comboBox.SelectIndex = -1;
                }
                else
                {
                    comboBox.ExtText = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Wu");
                }
                comboBox.BackColor = Color.FromArgb(53, 54, 58);
                comboBox.ForeColor = Color.White;
            }
        }
        private void ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var length = _MainItem.Length;
            SelectedValue = comboBox.SelectKey.ToString();
        }

    }
}

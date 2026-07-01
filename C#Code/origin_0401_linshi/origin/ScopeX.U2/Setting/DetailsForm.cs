using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using ScopeX.Core.Hardware;
using ScopeX.U2.LanguageSupoort;
using ScopeX.UserControls;
using ScopeX.UserControls.Style;

namespace ScopeX.U2
{
    public partial class DetailsForm : FloatForm
    {
        private List<String> _DllsName = new List<String>();

        public DetailsForm()
        {
            InitializeComponent();
            ReadXmlInfo();
            InitLvModel();
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Stylize();
            RTBFpgaInfo.Text = ExportHdFuncs.GetAllFPGAVersionInfo();
#if SaveLanguage
            // LanguageFactory.CacheFormLanguageControls(this);
#endif
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
        private void Stylize()
        {
            DefaultStyleManager.Instance.RegisterControlRecursion(this, StyleFlag.FontSize);
            //this.HeadBackColor = AppStyleConfig.DefaultTitleBackColor;
            LblModel.Font = new Font(LblModel.Font.FontFamily, LblModel.Font.Size, FontStyle.Bold);
        }
        private void ReadXmlInfo()
        {
            Type type = typeof(AboutForm);
            _DllsName.Clear();
            try
            {
                Stream sm = System.IO.File.OpenRead("./Resources/AboutInfo.xml");
                XmlReader reader = XmlReader.Create(sm);
                reader.ReadToFollowing("AboutInfo");

                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        String name = reader.Name;
                        reader.Read();
                        if (reader.NodeType == XmlNodeType.Whitespace)
                        {
                            switch (name)
                            {
                                case "Dlls":
                                    while (reader.Read())
                                    {
                                        name = reader.Name;
                                        reader.Read();
                                        if (reader.NodeType == XmlNodeType.Text)
                                        {
                                            switch (name)
                                            {
                                                case "Dll":
                                                    _DllsName.Add(reader.Value);
                                                    break;
                                            }

                                        }
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
            catch
            {
                //@todo：提示并记录日志
            }
        }
        private void InitLvModel()
        {
            //设置Listview的外观
            LvOption.HeaderForeColor = AppStyleConfig.DefaultTitleForeColor;
            LvOption.HeaderBackColor = AppStyleConfig.DefaultTitleBackColor.GetBrightnessColor(0.05);
            LvOption.ForeColor = AppStyleConfig.DefaultContextForeColor;
            LvOption.BackColor = AppStyleConfig.DefaultContextBackColor.GetBrightnessColor(0.05);
            LvOption.SelectedRowColor = AppStyleConfig.DefaultCheckedBackColor;

            LvOption.Columns.Add(ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("MoKuaiMing"), this.Width / 3 * 2);
            LvOption.Columns.Add(ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("BanBenHao"), -2);

            foreach (var item in _DllsName)
            {
                try
                {
                    System.Diagnostics.FileVersionInfo name = System.Diagnostics.FileVersionInfo.GetVersionInfo(Path.Combine(System.Environment.CurrentDirectory, item));
                    LvOption.Items.Add(new ListViewItem(new String[] { item, name.ProductVersion.ToString() }));
                }
                catch (Exception)
                { }

            }
        }
    }
}

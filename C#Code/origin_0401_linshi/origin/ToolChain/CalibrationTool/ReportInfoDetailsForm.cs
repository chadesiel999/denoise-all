using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScopeX.Hardware.Calibration.Tool.Base;
using static ScopeX.Hardware.Calibration.Tool.TabPageBatchTask;
using ScopeX.Hardware.Calibration.Tool.Utilities;

namespace ScopeX.Hardware.Calibration.Tool
{
    public partial class ReportInfoDetailsForm : Form
    {
        Dictionary<int, ReportInfo> _InfoDir = new Dictionary<int, ReportInfo>();
        public ReportInfoDetailsForm(List<ReportInfo> infos)
        {
            InitializeComponent();
            ImageList il = new ImageList();
            il.Images.Add("Success", Properties.Resources.ok);
            il.Images.Add("Fail", Properties.Resources.error);
            listView1.SmallImageList = il;
            Init(infos);
        }

        private void Init(List<ReportInfo> infos)
        {
            //更新数据
            int numId = 1;
            foreach (var info in infos)
                _InfoDir.Add(numId++, info);

            UpdateUI();

            //事件处理
            listView1.SelectedIndexChanged += (s, e) =>
            {
                if (listView1.SelectedIndices.Count > 0)
                {
                    int itemIndex = listView1.SelectedIndices[0];
                    int infoIndex = Int32.Parse(listView1.Items[itemIndex].Text);
                    richTextBox1.Text = _InfoDir[infoIndex].WholeInfo;

                    string pattern = @"ExecuteId=(\d+);";
                    Match match = Regex.Match(richTextBox1.Text, pattern);
                    if(long.TryParse(match.Groups[1].Value, out long executeId))
                    {
                        var caliLogs = CaliLogInfo.SelectInfosFromDb(executeId);
                        richTextBox1.Text += ("\r校准过程信息:");
                        foreach(var caliInfo in caliLogs)
                        {
                            richTextBox1.Text += ("\r" + caliInfo.Content);
                        }
                    }
                }
            };

            CbOnlyFailed.CheckedChanged += (s, e) =>
            {
                if (CbOnlyFailed.Checked)
                    FillContent(info => info.Result != BatchTaskPartResult.Succeed);
                else
                    FillContent(_ => true);
            };
        }

        private void UpdateUI()
        {
            if (_InfoDir.Count > 0)
                this.Text = "详细信息 :  " + _InfoDir[1].CurrXmlFileName;

            FillContent(_ => true);

            //统计信息
            int fildCount = _InfoDir.Count(info => info.Value.Result != BatchTaskPartResult.Succeed);
            toolStripStatusLabel1.Text = $"总计[ {_InfoDir.Count} ]  ;  失败[ {fildCount} ]";
        }

        /// <summary>
        /// 填充列表
        /// </summary>
        /// <param name="infos"></param>
        private void FillContent(Func<ReportInfo, bool> predicate)
        {
            listView1.Items.Clear();
            foreach (var infoPair in _InfoDir)
            {
                if(predicate.Invoke(infoPair.Value))
                {
                    bool isSuccess = (infoPair.Value.Result == BatchTaskPartResult.Succeed);
                    ListViewItem item = new ListViewItem(new String[] {
                        infoPair.Key.ToString(),
                        infoPair.Value.FuncName,
                        infoPair.Value.StepInfo,
                        isSuccess ? "Success" : "Fail"});

                    item.ImageKey = item.SubItems[3].Text;
                    listView1.Items.Add(item);
                }
            }
        }
    }
}

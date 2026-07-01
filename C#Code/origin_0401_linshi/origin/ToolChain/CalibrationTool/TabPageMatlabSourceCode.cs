using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.IO;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.Json;
using System.Threading;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using ScopeX.Hardware.Calibration.Tool.Base;
using ScopeX.ComModel;

namespace ScopeX.Hardware.Calibration.Tool
{
    public partial class TabPageMatlabSourceCode : UserControl, IMainFormTabPage
    {
        public TabPageMatlabSourceCode()
        {
            InitializeComponent();
            InitPrePosProcessor();
            Running = false;
        }
        private List<ProductType> _Used4ProductTypes = new List<ProductType>()
        {
            ProductType.Base,
            ProductType.B21_JinHui_PXI,
            ProductType.B21_HB8G,
            ProductType.B21_HD4G,
            ProductType.B21_DBI20G,
            ProductType.B21_DBI16G,
            ProductType.B21_MD8G,
            ProductType.B21_HR1G,
            ProductType.B21_MS2G,
            ProductType.ForTest,
            ProductType.JiHe_MSO7000X,
            ProductType.JiHe_MSO7000A,

        };
        #region IMainFormTabPage
        public List<ProductType> Used4ProductTypes => _Used4ProductTypes;
        public IInstrumentSession? currInstrumentSession = null;

        public void SetInstrumentInteract(IInstrumentSession? instrumentInteract)
        {
            currInstrumentSession = instrumentInteract;
        }

        public void RefreshData()
        {
        }
        #endregion IMainFormTabPage


        public void SetInstrument(IInstrumentSession? instrumentSession)
        {
            currInstrumentSession = instrumentSession;
        }
        private void InitPrePosProcessor()
        {
            comboBoxMatlabSourceCode_PrePosProcessType.Items.Clear();
            comboBoxMatlabSourceCode_PrePosProcessType.SelectedIndex = -1;
            string jsonFileName = Path.GetFileNameWithoutExtension(Application.ExecutablePath) + ".MatlabSourceCodePrePosProcessor.config";
            if (!File.Exists(jsonFileName))
                return;
            List<TitleClassNamePair>? allDefinedProcessors = JsonSerializer.Deserialize<List<TitleClassNamePair>>(File.ReadAllText(jsonFileName, Encoding.UTF8));
            //allDefinedProcessors.Clear();
            //allDefinedProcessors.Add(new TitleClassNamePair() { Title = "这是一个测试", ClassName = "MatlabSourceCodePrePosProcessor_Test" });
            //JsonSerializerOptions options = new JsonSerializerOptions{Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)};
            //File.WriteAllText(jsonFileName, JsonSerializer.Serialize<List<TitleClassNamePair>>(allDefinedProcessors, options),Encoding.UTF8);
            if (allDefinedProcessors?.Count > 0)
            {
                foreach (TitleClassNamePair pair in allDefinedProcessors)
                {
                    if (ExistsPrePosProcessos(pair.ClassName))
                        comboBoxMatlabSourceCode_PrePosProcessType.Items.Add(new ComboxItemObject { Text = pair.Title, Value = pair.ClassName });
                }
                if (allDefinedProcessors.Count > 0)
                    comboBoxMatlabSourceCode_PrePosProcessType.SelectedIndex = 0;
            }
            buttonMatlabSourceCode_LoadFromFile.Enabled = BaseHelper.bMatlabInstalled;
            buttonMatlabSourceCode_RunStopStatus.Enabled = BaseHelper.bMatlabInstalled;
        }
        IMatlabSourceCodePrePosProcessor? currMatlabSourceCodePrePosProcessor = null;
        IMatlabSourceCodePrePosProcessor? CreatePrePosProcessos(string className)
        {
            var q = from t in BaseHelper.AllMatlabSourceCodePrePosProcessorClass!.ToArray()
                    where t.Name == className
                    select t;
            if (Enumerable.Count(q) > 0)
                return (IMatlabSourceCodePrePosProcessor)Activator.CreateInstance((Type)q.ToList()[0])!;
            else
                return null;
        }
        Boolean ExistsPrePosProcessos(string className)
        {
            var q = from t in BaseHelper.AllMatlabSourceCodePrePosProcessorClass!.ToArray()
                    where t.Name == className
                    select t;
            return (Enumerable.Count(q) > 0);
        }
        private Boolean bFirstRun = true;
        IntPtr figure1;//图像句柄
        private String DisposeText(String matlabcode, out Boolean hasclose, out String closecode)
        {
            String result = "";
            hasclose = false;
            String[] sArray = Regex.Split(matlabcode, "\n");
            closecode = "";
            foreach (String code in sArray)
            {
                if (code.Contains("close"))
                {
                    hasclose = true;
                    String[] mixcode = code.Split(';');
                    foreach (String currtcode in mixcode)
                    {
                        if (currtcode.Contains("close"))
                        {
                            closecode = currtcode + ";\n";
                        }
                        else if (currtcode.Equals(""))
                            continue;
                        else
                            result += currtcode + ";\n";
                    }
                }
                else
                    result += code + "\n";
            }
            return result;
        }
        private string matLabSourceCode = "";
        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();

            string sourceCode = richTextBoxMatlabSourceCode_SourceCode.Text;
            if (sourceCode.Trim() == "")
                return;
            if (matlabApp == null)
                return;
            if (figure1 == IntPtr.Zero)
                return;
            if (currMatlabSourceCodePrePosProcessor != null)
                currMatlabSourceCodePrePosProcessor.PutInputData(matlabApp, currInstrumentSession);
            try
            {
                String response = matlabApp.Execute(matLabSourceCode);

                if (!String.IsNullOrEmpty(response))
                {
                    try
                    {
                        richTextBoxMatlabSourceCode_Result.Text = response;
                    }
                    catch (Exception ee)
                    {
                        System.Diagnostics.Trace.WriteLine(ee);
                    }
                }
                if (currMatlabSourceCodePrePosProcessor != null)
                    currMatlabSourceCodePrePosProcessor.GetOutputData(matlabApp, currInstrumentSession);
            }
            catch
            {
            }
            timer1.Interval = (int)numericUpDown1.Value;
            if (!checkBoxAutoStop.Checked)
                timer1.Start();
            else
                Running = false;
        }
        private bool Running
        {
            get => buttonMatlabSourceCode_RunStopStatus.Tag.ToString() != "stop";
            set
            {
                buttonMatlabSourceCode_RunStopStatus.Tag = value ? "run" : "stop";
                buttonMatlabSourceCode_RunStopStatus.Text = value ? "Stop" : "Run";
                richTextBoxMatlabSourceCode_SourceCode.ReadOnly = value;
                buttonMatlabSourceCode_LoadFromFile.Enabled = !value;
            }
        }
        private MLApp.MLApp? matlabApp = null;
        bool bHasClose = false;
        string closeCode = "";
        private void AttachMatlabFigure()
        {
            matlabApp = Activator.CreateInstance(Type.GetTypeFromProgID("Matlab.Application")!) as MLApp.MLApp;
            matlabApp!.Visible = 0;
            matlabApp!.Execute("close all");
            matlabApp!.Execute("figure('toolbar','none','menubar','none')");//创建基础图层 后面的图层皆在这一图层上绘制
            Thread.Sleep(1000);
            figure1 = ImportOutFuncs.FindWindow("SunAwtFrame", "Figure 1");
            if (figure1 != IntPtr.Zero)
                panelFigure_SizeChanged(null, null);
        }
        private void buttonMatlabSourceCode_RunStopStatus_Click(object sender, EventArgs e)
        {
            if (Running)
            {
                Running = false;
                timer1.Stop();
            }
            else
            {
                if (currInstrumentSession == null)
                {
                    MessageBox.Show("请先连接我们的示波器！");
                    return;
                }
                string sourceCode = richTextBoxMatlabSourceCode_SourceCode.Text;
                if (string.Empty == sourceCode.Trim())
                {
                    MessageBox.Show("没有可执行的代码！");
                    return;
                }
                //string formula = "result=zeros(1,length(source1));" + DisposeText(sourceCode, out bHasClose, out closeCode);
                string formula = DisposeText(sourceCode, out bHasClose, out closeCode);
                if (bFirstRun)
                {
                    AttachMatlabFigure();
                    bFirstRun = false;
                }
                if (matlabApp == null || figure1 == IntPtr.Zero)
                {
                    timer1.Stop();
                    MessageBox.Show("出现问题，不能处理！");
                    return;
                }

                matLabSourceCode = formula;
                timer1.Interval=(int)numericUpDown1.Value;
                timer1.Start();
                Running = true;
            }
        }
        private string loadedFileName = "";
        private void buttonMatlabSourceCode_LoadFromFile_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Running = false;
                richTextBoxMatlabSourceCode_SourceCode.LoadFile(openFileDialog1.FileName, RichTextBoxStreamType.PlainText);
                loadedFileName = openFileDialog1.FileName;
            }
        }

        private void buttonMatlabSourceCode_SaveToFile_Click(object sender, EventArgs e)
        {
            if (loadedFileName == "")
                return;
            if (saveFileDialog1.ShowDialog() != DialogResult.OK)
                return;
            richTextBoxMatlabSourceCode_SourceCode.SaveFile(saveFileDialog1.FileName, RichTextBoxStreamType.PlainText);
        }

        private void comboBoxMatlabSourceCode_PrePosProcessType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxMatlabSourceCode_PrePosProcessType.SelectedIndex >= 0)
                currMatlabSourceCodePrePosProcessor = CreatePrePosProcessos((comboBoxMatlabSourceCode_PrePosProcessType.SelectedItem as ComboxItemObject)!.Value?.ToString()??"");
            else
                currMatlabSourceCodePrePosProcessor = null;
        }

        private delegate void UpdateUI();//委托用于更新UI
        private void panelFigure_SizeChanged(object? sender, EventArgs? e)
        {
            if (figure1 == IntPtr.Zero)
                return;
            //跨线程，用委托方式执行
            UpdateUI update = delegate
            {
                //设置matlab图像窗体的父窗体为panel
                ImportOutFuncs.SetParent(figure1, panelFigure.Handle);
                //获取窗体原来的风格
                var style = ImportOutFuncs.GetWindowLong(figure1, ImportOutFuncs.GWL_STYLE);
                //设置新风格，去掉标题,取消所有边框，不能通过边框改变尺寸 
                ImportOutFuncs.SetWindowLong(figure1, ImportOutFuncs.GWL_STYLE, style & ~ImportOutFuncs.WS_CAPTION & ~ImportOutFuncs.WS_THICKFRAME);
                //移动到panel里合适的位置并重绘
                ImportOutFuncs.MoveWindow(figure1, 0, 0, panelFigure.Width + 0, panelFigure.Height + 0, true);
                Application.DoEvents();
                //MoveWindow(figure1, 0, 0, 400 + 0, 300 + 0, true);
                //调用显示窗体函数，隐藏再显示相当于刷新一下窗体
            };
            panelFigure.Invoke(update);
            //再移动一次，防止显示错误
            //Thread.Sleep(100);
            //ImportOutFuncs.MoveWindow(figure1, 0, 0, panelFigure.Width + 0, panelFigure.Height + 0, true);
            Application.DoEvents();
        }
    }
}


using System.Reflection;
using System.Linq;
using ScopeX.ComModel;
using ScopeX.Hardware.Calibration.Tool.Base;
using System.Windows.Forms;
using System;
using System.Collections.Generic;
using ScopeX.Hardware.Calibration.Data.Base;
using CalibrationData = ScopeX.Hardware.Calibration.Data.Base;
using System.Threading;

namespace ScopeX.Hardware.Calibration.Tool
{
    public partial class MainForm
    {

        #region 与产品相关的UI内容配置

        private List<IMainFormTabPage> _TabPages = new List<IMainFormTabPage>();

        private void InitUIInfoByProductType()
        {
            _TabPages.Clear();
            //tabcontrol的控件信息
            foreach (var pg in tabControl1.Controls)
            {
                if (pg is TabPage tpg &&
                   tpg.Controls.Count > 0 &&
                   tpg.Controls[0] is IMainFormTabPage mftpg)
                {
                    _TabPages.Add(mftpg);
                }
            }
        }

        private void ConstructUIByProductType()
        {
            tabControl1.SuspendLayout();
            tabControl1.Controls.Clear();
            (int show, int close) count = new(0, 0);
            foreach (var mftpg in _TabPages)
            {
                Boolean showFlag = mftpg.Used4ProductTypes.Contains(ServerDomainConstants.ProductType);
                showFlag = true;
                if (((UserControl)mftpg).Parent is TabPage tpg && showFlag)
                    tabControl1.Controls.Add(tpg);

                if (showFlag)
                    count.show++;
                else
                    count.close++;
            }

            tabControl1.ResumeLayout();
        }

        #endregion 与产品相关的UI内容配置

        #region 供xml调用的 UIFunc 实现
        private void RefreshAllCaliDataOfUI()
        {
            foreach (TabPage tabPage in this.tabControl1.TabPages)
            {
                if (tabPage.Controls.Count > 0 && tabPage.Controls[0] is IMainFormTabPage MainFormTabPage)
                {
                    if (MainFormTabPage.CaliDataType != CaliDataType.None)
                        MainFormTabPage.RefreshData();
                }
            }
        }
        private BatchTaskPartResult UIFunction_RefreshCaliDataAtUI(XmlScpiCmd xmlScpiCmd, IInstrumentSession? instrumentSession, out string message, Action<int, string, string>? updateAction, CancellationToken? cancellationToken)
        {
            lock (TimerAutoActionList_Locker)
            {
                TimerAutoActionList.Add(RefreshAllCaliDataOfUI);
            }
            message = "OK!";
            return BatchTaskPartResult.Succeed;
        }

        private BatchTaskPartResult UIFunction_LoadCaliDataDefualtValues(XmlScpiCmd xmlScpiCmd, IInstrumentSession? instrumentSession, out string message, Action<int, string, string>? updateAction, CancellationToken? cancellationToken)
        {
            if (instrumentSession != this.currInstrument)
            {
                message = "仪器制定不正确错误";
                return BatchTaskPartResult.ErrorParameter;
            }
            string[] allParams = xmlScpiCmd.ProgramFuncName.Split(' ');
            if (allParams.Length < 2)
            {
                message = "参数错误";
                return BatchTaskPartResult.ErrorParameter;
            }
            //约定：
            //第一个为函数名称LoadCaliDataDefualtValues
            //第二个参数为数据类型，使用CaliDataType枚举的名称
            string caliDataTypeName = allParams[1].Trim();
            bool bOK = false;
            if (caliDataTypeName.ToUpper() == "ALL")
            {
                foreach (CaliDataType dataType in Enum.GetValues(typeof(CaliDataType)))
                {
                    ICaliData? caliData = CalibrationData.Helper.GetICaliData(dataType);
                    if (caliData != null)
                    {
                        bool bLoadOK = true;
                        if (dataType== CaliDataType.PhyChannel)
                        {
                            switch(ServerDomainConstants.ProductType)
                            {
                                case ProductType.JiHe_MSO7000X:
                                    ChannelParams.Default.LoadDefaultValue(AnalogChannelType.JiHe2d5G);
                                    break;
                                default:
                                    bLoadOK = false;
                                    break;
                            }
                        }
                        else
                            caliData.LoadDefaultValue();
                        if (bLoadOK)
                        {
                            InstrumentInteract.CaliData_Send(instrumentSession, dataType);
                            InstrumentInteract.CaliData_Get(instrumentSession!, dataType);
                        }
                    }
                }
                bOK = true;
            }
            else
            {
                foreach (CaliDataType dataType in Enum.GetValues(typeof(CaliDataType)))
                {
                    if (dataType.ToString() == caliDataTypeName)
                    {
                        ICaliData? caliData = CalibrationData.Helper.GetICaliData(dataType);
                        if (caliData != null)
                        {
                            caliData.LoadDefaultValue();

                            InstrumentInteract.CaliData_Send(instrumentSession, dataType);
                            InstrumentInteract.CaliData_Get(instrumentSession!, dataType);
                            bOK = true;
                            break;
                        }
                    }
                }
            }
            if (!bOK)
            {
                message = "数据类型制定错误！";
                return BatchTaskPartResult.ErrorParameter;
            }

            message = "OK!";
            return BatchTaskPartResult.Succeed;

        }
        private void InstallProcessXMLAndPartTask_UIFunctions()
        {
            BaseHelper.ProcessXMLAndPartTask_UIFuncList.Add(UIFunction_LoadCaliDataDefualtValues);
            BaseHelper.ProcessXMLAndPartTask_UIFuncList.Add(UIFunction_RefreshCaliDataAtUI);
        }
        #endregion
    }
}

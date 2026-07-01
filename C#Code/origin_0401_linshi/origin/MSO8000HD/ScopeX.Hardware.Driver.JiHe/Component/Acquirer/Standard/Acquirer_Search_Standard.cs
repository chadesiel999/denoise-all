using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Hardware.Driver
{
    internal class Acquirer_Search_Standard : AbstractAcquirer_Search
    {
        internal override void CreateAcquireAttribute() { }
        internal override bool bDataVaild { get; set; }
       
        internal override void Init() 
        {
        }
        /// <summary>
        /// 开启下一次新的采集
        /// </summary>
        internal override void InitAcq()
        {
            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.search_PC_search_point_num, 0x0200);//512
            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.search_PC_search_data_numh16, 0x0000);
            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.search_PC_search_data_numl16, 0x2800);//10K数据
            var enable = Hd.UIMessage!.Search!.Active;
            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.search_pc_search_pro_en, (UInt16)(enable? 0x0001 : 0x0000));
            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.search_PC_read_en, 0x0000);

        }
        /// <summary>
        /// 读取采集到的数据
        /// </summary>
        /// <returns></returns>
        internal override bool ReadAcqData(List<ReadInfo> readInfoList, out double samplingRateByus, CancellationToken? softResetToken)
        {
            samplingRateByus = 1.0;
            var searchs = Acquisition.AcqedDataMsg!.Search!.Searchs;
            var enable = Acquisition.AcqedDataMsg!.Search!.Active;
            if (!enable)
            {
                return false;
            }
            if (searchs == null)
                return false;
            AcqedDataPool.SearchData.Data.Clear();
            Dictionary<long, (Double[,] Result, Int32 ResultCount)> results = new Dictionary<long, (Double[,] Result, Int32 ResultCount)>();
            foreach (var item in searchs)
            {
                ConfigAction[item.Value.Type](item.Value.Option);
                //HdIO.WriteReg(ProcBdReg.W.search_pc_search_pro_en, 0x0001);
                Thread.Sleep(2);
                if (ReadFromBus(out var address,out var resultCount))
                {
                    results.Add(item.Key, (address, resultCount));
                }
                else
                {
                    results.Add(item.Key, (new Double[1,0], 0));
                }
            }

            Monitor.Enter(SearchData.UpdateDataLock);

            foreach (var item in results)
                AcqedDataPool.SearchData.Data.Add(item.Key,item.Value);

            Monitor.Exit(SearchData.UpdateDataLock);


            return true; 
        }
        /// <summary>
        /// 采集数据的后处理
        /// </summary>
        internal override void PostProcess(List<ReadInfo> readInfoList, CancellationToken? softResetToken) 
        {
        }

     

        private Boolean ReadFromBus(out Double[,] address, out Int32 resultCount)
        {

            address = new Double[1,1000];

            resultCount=0;
            //comment for JiHe_MSO7000X 
            //Thread.Sleep(2);
            //Boolean finishFlag = HdIO.CheckRegisterValue((UInt32)ProcBdReg.R.search_search_finish_flag, 0x1,1,1000);
            //if (!finishFlag)
            //{
            //   // HdIO.WriteReg(ProcBdReg.W.search_pc_search_pro_en, 0);
            //    return false;
            //}
            //else
            //{
            //    Int32 falsetime = 0;
            //    while (HdIO.ReadReg(ProcBdReg.R.search_search_fifo_rd_finish) ==0)
            //    {
            //        HdIO.WriteReg(ProcBdReg.W.search_PC_read_en, 1);
            //        Int16 add = (Int16)HdIO.ReadReg(ProcBdReg.R.search_search_stamp);
            //        if ((add & 0x4000) != 0)
            //        {
            //            add = (Int16)(add & 0x3FFF);
            //            HdIO.WriteReg(ProcBdReg.W.search_PC_read_en, 0);
            //            address[0, resultCount] = add;
            //            resultCount++;
            //        }
            //        falsetime++;
            //        Thread.Sleep(2);
            //        if (falsetime > 1000)
            //        {
            //            break;
            //        }
            //    }
            //    HdIO.WriteReg(ProcBdReg.W.search_pc_search_pro_en, 0);
            //    return true;
            //}

            return false;
        }

        private static Dictionary<SearchType, Action<ISearchTypeOptions>> ConfigAction = new Dictionary<SearchType, Action<ISearchTypeOptions>>() 
        {
            {SearchType.Edge ,CtrlSearch_Standard.Config_Edge},
            {SearchType.Pulse ,CtrlSearch_Standard.Config_Pulse},
            {SearchType.Timeout ,CtrlSearch_Standard.Config_Timeout},
            {SearchType.Runt ,CtrlSearch_Standard.Config_Runt},
            {SearchType.Window ,CtrlSearch_Standard.Config_Window},
            {SearchType.Transition ,CtrlSearch_Standard.Config_Transition},
            //{SearchType.Pattern ,CtrlSearch_Standard.Config_Pattern},
            {SearchType.SetupHold ,CtrlSearch_Standard.Config_SetupHold},
            {SearchType.Auto ,CtrlSearch_Standard.Config_Auto},
        };
    }
}

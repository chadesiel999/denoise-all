using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Hardware.Driver
{
//#if Scan
    internal static class HdCtrl_BoardSync
    {
        private static Boolean _EnableSaveToFile = true;

        #region Proc2Acq

        #region Sync Flag

        private static Dictionary<BoardSyncEnumP2A, AcqBdReg.W> _SyncFlagWriteRegP2A = new()//????
        {
            //{ BoardSyncEnumP2A.Write,       AcqBdReg.W.BoardSync_Ctrl_p2a_sync_wr_en        },
            //{ BoardSyncEnumP2A.Read,        AcqBdReg.W.BoardSync_Ctrl_p2a_sync_rd_en        },
            //{ BoardSyncEnumP2A.Reset,       AcqBdReg.W.BoardSync_Ctrl_p2a_sync_RST          },
            //{ BoardSyncEnumP2A.TrigLocat,   AcqBdReg.W.BoardSync_Ctrl_p2a_sync_trig_locat   },
        };

        private static Dictionary<BoardSyncEnumP2A, AcqBdReg.R> _SyncFlagReadRegP2A = new()//????
        {
            //{ BoardSyncEnumP2A.Write,       AcqBdReg.R.BoardSync_Ctrl_p2a_sync_flag_wr_en        },
            //{ BoardSyncEnumP2A.Read,        AcqBdReg.R.BoardSync_Ctrl_p2a_sync_flag_rd_en        },
            //{ BoardSyncEnumP2A.Reset,       AcqBdReg.R.BoardSync_Ctrl_p2a_sync_flag_RST          },
            //{ BoardSyncEnumP2A.TrigLocat,   AcqBdReg.R.BoardSync_Ctrl_p2a_sync_flag_trig_locat   },
        };
        internal static void ResetSyncFlagP2A()//????
        {
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.BoardSync_Ctrl_p2a_sync_wr_en, 0x0);//rst
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.BoardSync_Ctrl_p2a_sync_wr_en, 0x1);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.BoardSync_Ctrl_p2a_sync_wr_en, 0x0);

            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.BoardSync_Ctrl_p2a_sync_rd_en, 0x0);//rst
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.BoardSync_Ctrl_p2a_sync_rd_en, 0x1);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.BoardSync_Ctrl_p2a_sync_rd_en, 0x0);

            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.BoardSync_Ctrl_p2a_sync_RST, 0x0);//rst
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.BoardSync_Ctrl_p2a_sync_RST, 0x1);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.BoardSync_Ctrl_p2a_sync_RST, 0x0);

            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.BoardSync_Ctrl_p2a_sync_trig_locat, 0x0);//rst
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.BoardSync_Ctrl_p2a_sync_trig_locat, 0x1);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.BoardSync_Ctrl_p2a_sync_trig_locat, 0x0);
        }

        internal static void StartSearchP2A()//????
        {
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.BoardSync_Ctrl_p2a_sync_wr_en, 0);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.BoardSync_Ctrl_p2a_sync_rd_en, 0);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.BoardSync_Ctrl_p2a_sync_RST, 0);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.BoardSync_Ctrl_p2a_sync_trig_locat, 0);

            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.BoardSync_Ctrl_p2a_sync_wr_en, 0x8);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.BoardSync_Ctrl_p2a_sync_rd_en, 0x8);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.BoardSync_Ctrl_p2a_sync_RST, 0x8);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.BoardSync_Ctrl_p2a_sync_trig_locat, 0x8);
        }

        internal static void SwitchNormalPathP2A()//????
        {
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.BoardSync_Ctrl_p2a_sync_wr_en, 0);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.BoardSync_Ctrl_p2a_sync_rd_en, 0);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.BoardSync_Ctrl_p2a_sync_RST, 0);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.BoardSync_Ctrl_p2a_sync_trig_locat, 0);

            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.BoardSync_Ctrl_p2a_sync_wr_en, 0x4);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.BoardSync_Ctrl_p2a_sync_rd_en, 0x4);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.BoardSync_Ctrl_p2a_sync_RST, 0x4);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.BoardSync_Ctrl_p2a_sync_trig_locat, 0x4);

            //HdIO.WriteReg(ProcBdReg.W.BoardSync_Ctrl_a2p_trig_location_scan_switch_pro, 0x1);
        }
        internal static void SwitchTestFlashPathP2A()//????
        {
            ConfigSyncFlagP2A(new UInt32[] { 0x0 });

            //HdIO.WriteReg(ProcBdReg.W.BoardSync_Ctrl_a2p_trig_location_scan_switch_pro, 0x0);
            //HdIO.WriteReg(ProcBdReg.W.BoardSync_Ctrl_RxSwitchPro, 0x0);
        }

        /// <summary>
        /// 依次将数组中的元素下发给每块采集板的_SyncFlagRegP2A中的寄存器
        /// </summary>
        /// <param name="data"></param>
        private static void ConfigSyncFlagP2A(UInt32[] data)
        {
            foreach (BoardSyncEnumP2A syncType in _SyncFlagWriteRegP2A.Keys)
            {
                for (Int32 i = 0; i < data.Length; i++)
                {
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(_SyncFlagWriteRegP2A[syncType], data[i]);
                }
            }
        }
        internal static Dictionary<BoardSyncEnumP2A, Dictionary<AcqBdNo, UInt32>> ReadSyncFlagP2A()
        {
            Dictionary<BoardSyncEnumP2A, Dictionary<AcqBdNo, UInt32>> result = new();

            foreach (BoardSyncEnumP2A syncType in _SyncFlagReadRegP2A.Keys)
            {
                result[syncType] = Hd.CurrProduct?.AcqBd?.ReadFromAllFpga(_SyncFlagReadRegP2A[syncType], 0x1) ?? new();
            }
            return result;
        }
        //internal static Dictionary<AcqBdNo, BoardSyncP2A> ReadSyncFlagP2A()//????
        //{
        //    //var wr = Hd.CurrProduct?.AcqBd?.ReadFromAllFpga(AcqBdReg.R.BoardSync_Ctrl_p2a_sync_flag_wr_en, 0x1) ?? new();
        //    //var rd = Hd.CurrProduct?.AcqBd?.ReadFromAllFpga(AcqBdReg.R.BoardSync_Ctrl_p2a_sync_flag_rd_en, 0x1) ?? new();
        //    //var rst = Hd.CurrProduct?.AcqBd?.ReadFromAllFpga(AcqBdReg.R.BoardSync_Ctrl_p2a_sync_flag_RST, 0x1) ?? new();
        //    //var trig = Hd.CurrProduct?.AcqBd?.ReadFromAllFpga(AcqBdReg.R.BoardSync_Ctrl_p2a_sync_flag_trig_locat, 0x1) ?? new();

        //    //Dictionary<AcqBdNo, BoardSyncP2A> syncFlag = new();
        //    //foreach (AcqBdNo acqBdNo in Enum.GetValues<AcqBdNo>())
        //    //{
        //    //    if (wr.ContainsKey(acqBdNo) && rd.ContainsKey(acqBdNo) && rst.ContainsKey(acqBdNo) && trig.ContainsKey(acqBdNo))
        //    //        syncFlag.Add(acqBdNo, new BoardSyncP2A(wr[acqBdNo], rd[acqBdNo], rst[acqBdNo], trig[acqBdNo]));
        //    //}

        //    //return syncFlag;

        //    return new();
        //}
#endregion

#region Tap Value
        internal static void InitSyncTapNumP2A()//????
        {
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.BoardSync_Ctrl_p2a_tap_load_set_rd_en, 0x2000);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.BoardSync_Ctrl_p2a_tap_load_set_rst, 0x2000);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.BoardSync_Ctrl_p2a_tap_load_set_trig_locat, 0x2000);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.BoardSync_Ctrl_p2a_tap_load_set_wr_en, 0x2000);
        }
        internal static void ConfigTapNumP2A(UInt32 tapNum, UInt32 mask = 0)
        {
            foreach (BoardSyncEnumP2A syncType in _SetTapRegTableP2A.Keys)
            {
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(_SetTapRegTableP2A[syncType], tapNum | mask);
            }
        }

        internal static void ConfigTapNumP2A(Dictionary<BoardSyncEnumP2A, UInt32> tapNumTable, UInt32 mask = 0)
        {
            foreach (BoardSyncEnumP2A syncType in _SetTapRegTableP2A.Keys)
            {
                if (tapNumTable.ContainsKey(syncType))
                {
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(_SetTapRegTableP2A[syncType], tapNumTable[syncType] | mask);
                }
            }
        }
        internal static Dictionary<BoardSyncEnumP2A, Dictionary<AcqBdNo, UInt32>> ReadTestDataP2A()
        {
            Dictionary<BoardSyncEnumP2A, Dictionary<AcqBdNo, UInt32>> result = new();

            foreach (BoardSyncEnumP2A syncType in _SyncTestDataRegP2A.Keys)
            {
                result[syncType] = Hd.CurrProduct?.AcqBd?.ReadFromAllFpga(_SyncTestDataRegP2A[syncType], 0x1ff) ?? new();
            }
            return result;
        }
        internal static void ConfigTapNumP2A(Dictionary<BoardSyncEnumP2A, Dictionary<AcqBdNo, UInt32>> tapNumTable, UInt32 mask = 0)
        {
            foreach (BoardSyncEnumP2A syncType in _SetTapRegTableP2A.Keys)
            {
                if (tapNumTable.ContainsKey(syncType))
                {
                    foreach (AcqBdNo acqBdNo in tapNumTable[syncType].Keys)
                    {
                        Hd.CurrProduct?.AcqBd?.WriteReg(_SetTapRegTableP2A[syncType], acqBdNo, mask | tapNumTable[syncType][acqBdNo]);
                    }
                }
            }
        }

        //internal static void ConfigTapNumP2A(Dictionary<AcqBdNo, BoardSyncP2A> tapNumTable)//????
        //{
        //    //HdCtrl_BoardSync.ResetSyncFlagP2A();

        //    //foreach (var tapnum in tapNumTable)
        //    //{
        //    //    Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.BoardSync_Ctrl_p2a_tap_load_set_wr_en, tapnum.Key, 0x3 << 12 | tapnum.Value.wr);
        //    //    Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.BoardSync_Ctrl_p2a_tap_load_set_rd_en, tapnum.Key, 0x3 << 12 | tapnum.Value.rd);
        //    //    Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.BoardSync_Ctrl_p2a_tap_load_set_rst, tapnum.Key, 0x3 << 12 | tapnum.Value.rst);
        //    //    Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.BoardSync_Ctrl_p2a_tap_load_set_trig_locat, tapnum.Key, 0x3 << 12 | tapnum.Value.trigLocat);
        //    //}

        //    //HdCtrl_BoardSync.StartSearchP2A();

        //    //foreach (var tapnum in tapNumTable)
        //    //{
        //    //    Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.BoardSync_Ctrl_p2a_tap_load_set_wr_en, tapnum.Key, 0x1 << 12 | tapnum.Value.wr);
        //    //    Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.BoardSync_Ctrl_p2a_tap_load_set_rd_en, tapnum.Key, 0x1 << 12 | tapnum.Value.rd);
        //    //    Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.BoardSync_Ctrl_p2a_tap_load_set_rst, tapnum.Key, 0x1 << 12 | tapnum.Value.rst);
        //    //    Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.BoardSync_Ctrl_p2a_tap_load_set_trig_locat, tapnum.Key, 0x1 << 12 | tapnum.Value.trigLocat);
        //    //}

        //}

        internal static void ConfigSyncTapRangeP2A(UInt32 startTap, UInt32 endTap)//????
        {
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.BoardSync_Ctrl_p2a_TAP_start_wr_en, startTap);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.BoardSync_Ctrl_p2a_TAP_stop_wr_en, endTap);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.BoardSync_Ctrl_p2a_TAP_start_rd_en, startTap);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.BoardSync_Ctrl_p2a_TAP_stop_rd_en, endTap);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.BoardSync_Ctrl_p2a_TAP_start_RST, startTap);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.BoardSync_Ctrl_p2a_TAP_stop_RST, endTap);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.BoardSync_Ctrl_p2a_TAP_start_trig_locat, startTap);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.BoardSync_Ctrl_p2a_TAP_stop_trig_locat, endTap);
        }

        //internal static Dictionary<AcqBdNo, BoardSyncP2A> ReadTapValueP2A()//????
        //{
        //    //Dictionary<AcqBdNo, UInt32> wr = Hd.CurrProduct?.AcqBd?.ReadFromAllFpga(AcqBdReg.R.BoardSync_Ctrl_p2a_tap_read_wr_en, 0x1FF) ?? new();
        //    //Dictionary<AcqBdNo, UInt32> rd = Hd.CurrProduct?.AcqBd?.ReadFromAllFpga(AcqBdReg.R.BoardSync_Ctrl_p2a_tap_read_rd_en, 0x1FF) ?? new();
        //    //Dictionary<AcqBdNo, UInt32> rst = Hd.CurrProduct?.AcqBd?.ReadFromAllFpga(AcqBdReg.R.BoardSync_Ctrl_p2a_tap_read_rst, 0x1FF) ?? new();
        //    //Dictionary<AcqBdNo, UInt32> trig = Hd.CurrProduct?.AcqBd?.ReadFromAllFpga(AcqBdReg.R.BoardSync_Ctrl_p2a_tap_read_trig_locat, 0x1FF) ?? new();

        //    //Dictionary<AcqBdNo, BoardSyncP2A> ans = new();

        //    //foreach (AcqBdNo acqBdNo in Enum.GetValues<AcqBdNo>())
        //    //{
        //    //    if (wr.ContainsKey(acqBdNo) && rd.ContainsKey(acqBdNo) &&
        //    //        rst.ContainsKey(acqBdNo) && trig.ContainsKey(acqBdNo))
        //    //        ans.Add(acqBdNo, new BoardSyncP2A(wr[acqBdNo], rd[acqBdNo], rst[acqBdNo], trig[acqBdNo]));
        //    //}

        //    //return ans;

        //    return new();
        //}
#endregion

#region File
        internal static void SaveBoardSyncP2AToFile(String fileName, Dictionary<AcqBdNo, BoardSyncP2A> tapTable)
        {
            StreamWriter sw = new StreamWriter(fileName);
            foreach (var tap in tapTable)
            {
                sw.WriteLine($"{tap.Key}:{tap.Value.wr},{tap.Value.rd},{tap.Value.rst},{tap.Value.trigLocat}");
            }
            sw.Flush();
            sw.Close();
        }

        //internal static Dictionary<AcqBdNo, BoardSyncP2A> ReadBoardSyncP2AFromFile(String fileName)
        //{
        //    Dictionary<AcqBdNo, BoardSyncP2A> ans = new();

        //    if (!File.Exists(fileName))
        //        return ans;
        //    StreamReader sr = new StreamReader(fileName);
        //    while (!sr.EndOfStream)
        //    {
        //        String? info = sr.ReadLine();
        //        if (info != null)
        //        {
        //            String[] keyValue = info.Split(":");
        //            if (keyValue.Length == 0)
        //                continue;

        //            String[] tapArray = keyValue[1].Split(",");
        //            if (tapArray.Length < 4)
        //                continue;

        //            AcqBdNo acqBd = (AcqBdNo)Enum.Parse(typeof(AcqBdNo), keyValue[0]);
        //            BoardSyncP2A tap = new(UInt32.Parse(tapArray[0]), UInt32.Parse(tapArray[1]), UInt32.Parse(tapArray[2]), UInt32.Parse(tapArray[3]));

        //            ans.Add(acqBd, tap);
        //        }
        //    }

        //    return ans;
        //}
#endregion

#endregion

#region Acq2Proc

#region Sync Flag
        internal static Dictionary<AcqBdNo, WriteRegA2P> _SyncFlagWriteReg = new()//????
        {
            //{ AcqBdNo.B1, new(ProcBdReg.W.BoardSync_Ctrl_a2p_setting_trig_locat_acq1, ProcBdReg.W.BoardSync_Ctrl_a2p_setting_trig_acq1)},
            //{ AcqBdNo.B2, new(ProcBdReg.W.BoardSync_Ctrl_a2p_setting_trig_locat_acq2, ProcBdReg.W.BoardSync_Ctrl_a2p_setting_trig_acq2) },
            //{ AcqBdNo.B3, new(ProcBdReg.W.BoardSync_Ctrl_a2p_setting_trig_locat_acq3, ProcBdReg.W.BoardSync_Ctrl_a2p_setting_trig_acq3) },
            //{ AcqBdNo.B4, new(ProcBdReg.W.BoardSync_Ctrl_a2p_setting_trig_locat_acq4, ProcBdReg.W.BoardSync_Ctrl_a2p_setting_trig_acq4) },
            //{ AcqBdNo.B5, new(ProcBdReg.W.BoardSync_Ctrl_a2p_setting_trig_locat_acq5, ProcBdReg.W.BoardSync_Ctrl_a2p_setting_trig_acq5) },
            //{ AcqBdNo.B6, new(ProcBdReg.W.BoardSync_Ctrl_a2p_setting_trig_locat_acq6, ProcBdReg.W.BoardSync_Ctrl_a2p_setting_trig_acq6) },
            //{ AcqBdNo.B7, new(ProcBdReg.W.BoardSync_Ctrl_a2p_setting_trig_locat_acq7, ProcBdReg.W.BoardSync_Ctrl_a2p_setting_trig_acq7) },
            //{ AcqBdNo.B8, new(ProcBdReg.W.BoardSync_Ctrl_a2p_setting_trig_locat_acq8, ProcBdReg.W.BoardSync_Ctrl_a2p_setting_trig_acq8) },
            //{ AcqBdNo.B9, new(ProcBdReg.W.BoardSync_Ctrl_a2p_setting_trig_locat_acq9, ProcBdReg.W.BoardSync_Ctrl_a2p_setting_trig_acq9) },
            //{ AcqBdNo.B10, new(ProcBdReg.W.BoardSync_Ctrl_a2p_setting_trig_locat_acq10, ProcBdReg.W.BoardSync_Ctrl_a2p_setting_trig_acq10) },
            //{ AcqBdNo.B11, new(ProcBdReg.W.BoardSync_Ctrl_a2p_setting_trig_locat_acq11, ProcBdReg.W.BoardSync_Ctrl_a2p_setting_trig_acq11) },
            //{ AcqBdNo.B12, new(ProcBdReg.W.BoardSync_Ctrl_a2p_setting_trig_locat_acq12, ProcBdReg.W.BoardSync_Ctrl_a2p_setting_trig_acq12) },
        };

        internal static Dictionary<AcqBdNo, ReadRegA2P> _SyncFlagRedReg = new()//????
        {
            //{ AcqBdNo.B1, new(ProcBdReg.R.BoardSync_Ctrl_a2p_sync_trig_locat_flag_acq1, ProcBdReg.R.BoardSync_Ctrl_a2p_sync_trig_flag_acq1)},
            //{ AcqBdNo.B2, new(ProcBdReg.R.BoardSync_Ctrl_a2p_sync_trig_locat_flag_acq2, ProcBdReg.R.BoardSync_Ctrl_a2p_sync_trig_flag_acq2)},
            //{ AcqBdNo.B3, new(ProcBdReg.R.BoardSync_Ctrl_a2p_sync_trig_locat_flag_acq3, ProcBdReg.R.BoardSync_Ctrl_a2p_sync_trig_flag_acq3)},
            //{ AcqBdNo.B4, new(ProcBdReg.R.BoardSync_Ctrl_a2p_sync_trig_locat_flag_acq4, ProcBdReg.R.BoardSync_Ctrl_a2p_sync_trig_flag_acq4)},
            //{ AcqBdNo.B5, new(ProcBdReg.R.BoardSync_Ctrl_a2p_sync_trig_locat_flag_acq5, ProcBdReg.R.BoardSync_Ctrl_a2p_sync_trig_flag_acq5)},
            //{ AcqBdNo.B6, new(ProcBdReg.R.BoardSync_Ctrl_a2p_sync_trig_locat_flag_acq6, ProcBdReg.R.BoardSync_Ctrl_a2p_sync_trig_flag_acq6)},
            //{ AcqBdNo.B7, new(ProcBdReg.R.BoardSync_Ctrl_a2p_sync_trig_locat_flag_acq7, ProcBdReg.R.BoardSync_Ctrl_a2p_sync_trig_flag_acq7)},
            //{ AcqBdNo.B8, new(ProcBdReg.R.BoardSync_Ctrl_a2p_sync_trig_locat_flag_acq8, ProcBdReg.R.BoardSync_Ctrl_a2p_sync_trig_flag_acq8)},
            //{ AcqBdNo.B9, new(ProcBdReg.R.BoardSync_Ctrl_a2p_sync_trig_locat_flag_acq9, ProcBdReg.R.BoardSync_Ctrl_a2p_sync_trig_flag_acq9)},
            //{ AcqBdNo.B10, new(ProcBdReg.R.BoardSync_Ctrl_a2p_sync_trig_locat_flag_acq10, ProcBdReg.R.BoardSync_Ctrl_a2p_sync_trig_flag_acq10)},
            //{ AcqBdNo.B11, new(ProcBdReg.R.BoardSync_Ctrl_a2p_sync_trig_locat_flag_acq11, ProcBdReg.R.BoardSync_Ctrl_a2p_sync_trig_flag_acq11)},
            //{ AcqBdNo.B12, new(ProcBdReg.R.BoardSync_Ctrl_a2p_sync_trig_locat_flag_acq12, ProcBdReg.R.BoardSync_Ctrl_a2p_sync_trig_flag_acq12)},
        };

        internal static void ResetSyncFlagA2P()
        {
            foreach (var syncflag in _SyncFlagWriteReg)
            {
                HdIO.WriteReg(syncflag.Value.TrigLocat, 0x0);
                HdIO.WriteReg(syncflag.Value.TrigLocat, 0x1);
                HdIO.WriteReg(syncflag.Value.TrigLocat, 0x0);

                HdIO.WriteReg(syncflag.Value.Trig, 0x0);
                HdIO.WriteReg(syncflag.Value.Trig, 0x1);
                HdIO.WriteReg(syncflag.Value.Trig, 0x0);
            }
        }

        internal static void StartSearchA2P()
        {
            foreach (var syncflag in _SyncFlagWriteReg)
            {
                HdIO.WriteReg(syncflag.Value.TrigLocat, 0x0);
                HdIO.WriteReg(syncflag.Value.TrigLocat, 0x8);

                HdIO.WriteReg(syncflag.Value.Trig, 0x0);
                HdIO.WriteReg(syncflag.Value.Trig, 0x8);
            }
        }

        internal static void SwitchNormalPathA2P()//????
        {
            foreach (var syncflag in _SyncFlagWriteReg)
            {
                HdIO.WriteReg(syncflag.Value.TrigLocat, 0x0);
                HdIO.WriteReg(syncflag.Value.TrigLocat, 0x4);

                HdIO.WriteReg(syncflag.Value.Trig, 0x0);
                HdIO.WriteReg(syncflag.Value.Trig, 0x4);
            }


            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.BoardSync_Ctrl_a2p_trig_location_scan_switch_acq, 1);
        }

        //internal static Dictionary<AcqBdNo, BoardSyncA2P> ReadSyncFlagA2P()
        //{
        //    Dictionary<AcqBdNo, BoardSyncA2P> ans = new();
        //    foreach (var syncflag in _SyncFlagRedReg)
        //    {
        //        UInt32 triglocat = HdIO.ReadReg(syncflag.Value.TrigLocat);
        //        UInt32 trig = HdIO.ReadReg(syncflag.Value.Trig);
        //        ans.Add(syncflag.Key, new(triglocat & 0x1, trig & 0x1));
        //    }

        //    return ans;
        //}
#endregion

#region Tap Value
        internal static Dictionary<AcqBdNo, WriteRegA2P> _ScanTapRegStartDefine = new()//????
        {
            //{ AcqBdNo.B1, new(ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_locat_TAP_start_acq1, ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_TAP_start_acq1)},
            //{ AcqBdNo.B2, new(ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_locat_TAP_start_acq2, ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_TAP_start_acq2) },
            //{ AcqBdNo.B3, new(ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_locat_TAP_start_acq3, ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_TAP_start_acq3) },
            //{ AcqBdNo.B4, new(ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_locat_TAP_start_acq4, ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_TAP_start_acq4) },
            //{ AcqBdNo.B5, new(ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_locat_TAP_start_acq5, ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_TAP_start_acq5) },
            //{ AcqBdNo.B6, new(ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_locat_TAP_start_acq6, ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_TAP_start_acq6) },
            //{ AcqBdNo.B7, new(ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_locat_TAP_start_acq7, ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_TAP_start_acq7) },
            //{ AcqBdNo.B8, new(ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_locat_TAP_start_acq8, ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_TAP_start_acq8) },
            //{ AcqBdNo.B9, new(ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_locat_TAP_start_acq9, ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_TAP_start_acq9) },
            //{ AcqBdNo.B10, new(ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_locat_TAP_start_acq10, ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_TAP_start_acq10) },
            //{ AcqBdNo.B11, new(ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_locat_TAP_start_acq11, ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_TAP_start_acq11) },
            //{ AcqBdNo.B12, new(ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_locat_TAP_start_acq12, ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_TAP_start_acq12) },
        };

        internal static Dictionary<AcqBdNo, WriteRegA2P> _ScanTapRegStopDefine = new()//????
        {
            //{ AcqBdNo.B1, new(ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_locat_TAP_stop_acq1, ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_TAP_stop_acq1)},
            //{ AcqBdNo.B2, new(ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_locat_TAP_stop_acq2, ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_TAP_stop_acq2) },
            //{ AcqBdNo.B3, new(ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_locat_TAP_stop_acq3, ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_TAP_stop_acq3) },
            //{ AcqBdNo.B4, new(ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_locat_TAP_stop_acq4, ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_TAP_stop_acq4) },
            //{ AcqBdNo.B5, new(ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_locat_TAP_stop_acq5, ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_TAP_stop_acq5) },
            //{ AcqBdNo.B6, new(ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_locat_TAP_stop_acq6, ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_TAP_stop_acq6) },
            //{ AcqBdNo.B7, new(ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_locat_TAP_stop_acq7, ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_TAP_stop_acq7) },
            //{ AcqBdNo.B8, new(ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_locat_TAP_stop_acq8, ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_TAP_stop_acq8) },
            //{ AcqBdNo.B9, new(ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_locat_TAP_stop_acq9, ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_TAP_stop_acq9) },
            //{ AcqBdNo.B10, new(ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_locat_TAP_stop_acq10, ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_TAP_stop_acq10) },
            //{ AcqBdNo.B11, new(ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_locat_TAP_stop_acq11, ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_TAP_stop_acq11) },
            //{ AcqBdNo.B12, new(ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_locat_TAP_stop_acq12, ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_TAP_stop_acq12) },
        };

        internal static Dictionary<AcqBdNo, WriteRegA2P> _ScanTapRegSetDefine = new()//????
        {
            //{ AcqBdNo.B1, new(ProcBdReg.W.BoardSync_Ctrl_a2p_tap_load_set_trig_locat_acq1, ProcBdReg.W.BoardSync_Ctrl_a2p_tap_load_set_trig_acq1)},
            //{ AcqBdNo.B2, new(ProcBdReg.W.BoardSync_Ctrl_a2p_tap_load_set_trig_locat_acq2, ProcBdReg.W.BoardSync_Ctrl_a2p_tap_load_set_trig_acq2) },
            //{ AcqBdNo.B3, new(ProcBdReg.W.BoardSync_Ctrl_a2p_tap_load_set_trig_locat_acq3, ProcBdReg.W.BoardSync_Ctrl_a2p_tap_load_set_trig_acq3) },
            //{ AcqBdNo.B4, new(ProcBdReg.W.BoardSync_Ctrl_a2p_tap_load_set_trig_locat_acq4, ProcBdReg.W.BoardSync_Ctrl_a2p_tap_load_set_trig_acq4) },
            //{ AcqBdNo.B5, new(ProcBdReg.W.BoardSync_Ctrl_a2p_tap_load_set_trig_locat_acq5, ProcBdReg.W.BoardSync_Ctrl_a2p_tap_load_set_trig_acq5) },
            //{ AcqBdNo.B6, new(ProcBdReg.W.BoardSync_Ctrl_a2p_tap_load_set_trig_locat_acq6, ProcBdReg.W.BoardSync_Ctrl_a2p_tap_load_set_trig_acq6) },
            //{ AcqBdNo.B7, new(ProcBdReg.W.BoardSync_Ctrl_a2p_tap_load_set_trig_locat_acq7, ProcBdReg.W.BoardSync_Ctrl_a2p_tap_load_set_trig_acq7) },
            //{ AcqBdNo.B8, new(ProcBdReg.W.BoardSync_Ctrl_a2p_tap_load_set_trig_locat_acq8, ProcBdReg.W.BoardSync_Ctrl_a2p_tap_load_set_trig_acq8) },
            //{ AcqBdNo.B9, new(ProcBdReg.W.BoardSync_Ctrl_a2p_tap_load_set_trig_locat_acq9, ProcBdReg.W.BoardSync_Ctrl_a2p_tap_load_set_trig_acq9) },
            //{ AcqBdNo.B10, new(ProcBdReg.W.BoardSync_Ctrl_a2p_tap_load_set_trig_locat_acq10, ProcBdReg.W.BoardSync_Ctrl_a2p_tap_load_set_trig_acq10) },
            //{ AcqBdNo.B11, new(ProcBdReg.W.BoardSync_Ctrl_a2p_tap_load_set_trig_locat_acq11, ProcBdReg.W.BoardSync_Ctrl_a2p_tap_load_set_trig_acq11) },
            //{ AcqBdNo.B12, new(ProcBdReg.W.BoardSync_Ctrl_a2p_tap_load_set_trig_locat_acq12, ProcBdReg.W.BoardSync_Ctrl_a2p_tap_load_set_trig_acq12) },
        };

        internal static Dictionary<AcqBdNo, ReadRegA2P> _ScanTapRedReg = new()//????
        {
            //{ AcqBdNo.B1, new(ProcBdReg.R.BoardSync_Ctrl_a2p_tap_read_trig_locat_acq1, ProcBdReg.R.BoardSync_Ctrl_a2p_tap_read_trig_acq1)},
            //{ AcqBdNo.B2, new(ProcBdReg.R.BoardSync_Ctrl_a2p_tap_read_trig_locat_acq2, ProcBdReg.R.BoardSync_Ctrl_a2p_tap_read_trig_acq2)},
            //{ AcqBdNo.B3, new(ProcBdReg.R.BoardSync_Ctrl_a2p_tap_read_trig_locat_acq3, ProcBdReg.R.BoardSync_Ctrl_a2p_tap_read_trig_acq3)},
            //{ AcqBdNo.B4, new(ProcBdReg.R.BoardSync_Ctrl_a2p_tap_read_trig_locat_acq4, ProcBdReg.R.BoardSync_Ctrl_a2p_tap_read_trig_acq4)},
            //{ AcqBdNo.B5, new(ProcBdReg.R.BoardSync_Ctrl_a2p_tap_read_trig_locat_acq5, ProcBdReg.R.BoardSync_Ctrl_a2p_tap_read_trig_acq5)},
            //{ AcqBdNo.B6, new(ProcBdReg.R.BoardSync_Ctrl_a2p_tap_read_trig_locat_acq6, ProcBdReg.R.BoardSync_Ctrl_a2p_tap_read_trig_acq6)},
            //{ AcqBdNo.B7, new(ProcBdReg.R.BoardSync_Ctrl_a2p_tap_read_trig_locat_acq7, ProcBdReg.R.BoardSync_Ctrl_a2p_tap_read_trig_acq7)},
            //{ AcqBdNo.B8, new(ProcBdReg.R.BoardSync_Ctrl_a2p_tap_read_trig_locat_acq8, ProcBdReg.R.BoardSync_Ctrl_a2p_tap_read_trig_acq8)},
            //{ AcqBdNo.B9, new(ProcBdReg.R.BoardSync_Ctrl_a2p_tap_read_trig_locat_acq9, ProcBdReg.R.BoardSync_Ctrl_a2p_tap_read_trig_acq9)},
            //{ AcqBdNo.B10, new(ProcBdReg.R.BoardSync_Ctrl_a2p_tap_read_trig_locat_acq10, ProcBdReg.R.BoardSync_Ctrl_a2p_tap_read_trig_acq10)},
            //{ AcqBdNo.B11, new(ProcBdReg.R.BoardSync_Ctrl_a2p_tap_read_trig_locat_acq11, ProcBdReg.R.BoardSync_Ctrl_a2p_tap_read_trig_acq11)},
            //{ AcqBdNo.B12, new(ProcBdReg.R.BoardSync_Ctrl_a2p_tap_read_trig_locat_acq12, ProcBdReg.R.BoardSync_Ctrl_a2p_tap_read_trig_acq12)},
        };

        internal static void InitSyncTapNumA2p()
        {
            foreach (var setreg in _ScanTapRegSetDefine)
            {
                HdIO.WriteReg(setreg.Value.TrigLocat, 0x2000);
                HdIO.WriteReg(setreg.Value.Trig, 0x2000);
            }
        }
        internal static Dictionary<AcqBdNo, UInt32> ReadSyncFlagData()
        {
            Dictionary<AcqBdNo, UInt32> ans = new();
            foreach (var syncflag in _DataSyncStatusRegTable)
            {
                ans.Add(syncflag.Key, HdIO.ReadReg(syncflag.Value));
            }

            return ans;
        }

        internal static void ConfigDataTapRange(Dictionary<AcqBdNo, BoradSyncRangeValue> tapRangeTable)
        {
            foreach (var acqbdno in Enum.GetValues<AcqBdNo>())
            {
                if (_DataStartRegTable.ContainsKey(acqbdno) && tapRangeTable.ContainsKey(acqbdno))
                {
                    HdIO.WriteReg(_DataStartRegTable[acqbdno].TapStart, tapRangeTable[acqbdno].ValueStart);
                    HdIO.WriteReg(_DataStartRegTable[acqbdno].TapStop, tapRangeTable[acqbdno].ValueStop);
                }
            }
        }
        internal static Dictionary<AcqBdNo, UInt32> ReadSyncTapData()
        {
            Dictionary<AcqBdNo, UInt32> ans = new();
            foreach (var tap in _DataSyncTapRegTable)
            {
                ans.Add(tap.Key, (HdIO.ReadReg(tap.Value) & 0xFFFF) >> 4);
            }

            return ans;
        }

        internal static void ConfigScanTapRangeA2P(UInt32 start, UInt32 stop)
        {
            foreach (var startreg in _ScanTapRegStartDefine)
            {
                HdIO.WriteReg(startreg.Value.TrigLocat, start);
                HdIO.WriteReg(startreg.Value.Trig, start);
            }

            foreach (var stopreg in _ScanTapRegStopDefine)
            {
                HdIO.WriteReg(stopreg.Value.TrigLocat, stop);
                HdIO.WriteReg(stopreg.Value.Trig, stop);
            }
        }

        internal static void ConfigTapNumA2P(Dictionary<AcqBdNo, BoardSyncA2P> tapNumTable)
        {
            HdCtrl_BoardSync.ResetSyncFlagA2P();
            foreach (var acqbdno in Enum.GetValues<AcqBdNo>())
            {
                if (tapNumTable.ContainsKey(acqbdno) && _ScanTapRegSetDefine.ContainsKey(acqbdno))
                {
                    HdIO.WriteReg(_ScanTapRegSetDefine[acqbdno].Trig, (0x3 << 12) | tapNumTable[acqbdno].Trig);
                    HdIO.WriteReg(_ScanTapRegSetDefine[acqbdno].TrigLocat, (0x3 << 12) | tapNumTable[acqbdno].TrigLocat);
                }
            }
            HdCtrl_BoardSync.StartSearchA2P();

            foreach (var acqbdno in Enum.GetValues<AcqBdNo>())
            {
                if (tapNumTable.ContainsKey(acqbdno) && _ScanTapRegSetDefine.ContainsKey(acqbdno))
                {
                    HdIO.WriteReg(_ScanTapRegSetDefine[acqbdno].Trig, (0x1 << 12) | tapNumTable[acqbdno].Trig);
                    HdIO.WriteReg(_ScanTapRegSetDefine[acqbdno].TrigLocat, (0x1 << 12) | tapNumTable[acqbdno].TrigLocat);
                }
            }
        }
        internal static void ConfigTapNumA2P(Dictionary<BoardSyncEnumA2P, Dictionary<AcqBdNo, UInt32>> tapNumTable, UInt32 mask = 0)
        {
            foreach (BoardSyncEnumA2P syncType in _SetTapRegTableA2P.Keys)
            {
                if (tapNumTable.ContainsKey(syncType))
                {
                    foreach (AcqBdNo acqBdNo in _SetTapRegTableA2P[syncType].Keys)
                    {
                        if (tapNumTable[syncType].ContainsKey(acqBdNo))
                            HdIO.WriteReg(_SetTapRegTableA2P[syncType][acqBdNo], mask | tapNumTable[syncType][acqBdNo]);
                    }
                }
            }
        }
        //internal static Dictionary<AcqBdNo, BoardSyncA2P> ReadTapValueA2P()
        //{
        //    Dictionary<AcqBdNo, BoardSyncA2P> ans = new();
        //    foreach (var tap in _ScanTapRedReg)
        //    {
        //        UInt32 triglocat = HdIO.ReadReg(tap.Value.TrigLocat);
        //        UInt32 trig = HdIO.ReadReg(tap.Value.Trig);
        //        ans.Add(tap.Key, new(triglocat & 0x1ff, trig & 0x1ff));
        //    }

        //    return ans;
        //}
        #endregion

        #region File
        internal static void SaveBoardSyncA2PToFile(String fileName, Dictionary<AcqBdNo, BoardSyncA2P> tapTable)
        {
            StreamWriter sw = new StreamWriter(fileName);//创建文件
            foreach (var tap in tapTable)
            {
                sw.WriteLine($"{tap.Key}:{tap.Value.TrigLocat},{tap.Value.Trig}");//
            }
            sw.Flush();//强制同步到硬盘中
            sw.Close();//关闭句柄
        }
        internal static void SaveBoardSyncDataToFile(String filePath, String fileName, Dictionary<AcqBdNo, UInt32> tapTable)
        {
            if (!_EnableSaveToFile)
                return;

            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            StreamWriter sw = new StreamWriter(filePath + fileName);
            foreach (var tap in tapTable)
            {
                sw.WriteLine($"{tap.Key}:{tap.Value},{tap.Value}");
            }
            sw.Flush();
            sw.Close();
        }


        //internal static Dictionary<AcqBdNo, BoardSyncA2P> ReadBoardSyncA2PFromFile(String fileName)
        //{
        //    Dictionary<AcqBdNo, BoardSyncA2P> ans = new();

        //    if (!File.Exists(fileName))
        //        return ans;
        //    StreamReader sr = new StreamReader(fileName);
        //    while (!sr.EndOfStream)
        //    {
        //        String? info = sr.ReadLine();
        //        if (info != null)
        //        {
        //            String[] keyValue = info.Split(":");
        //            if (keyValue.Length == 0)
        //                continue;

        //            String[] tapArray = keyValue[1].Split(",");
        //            if (tapArray.Length < 2)
        //                continue;

        //            AcqBdNo acqBd = (AcqBdNo)Enum.Parse(typeof(AcqBdNo), keyValue[0]);
        //            BoardSyncA2P tap = new(UInt32.Parse(tapArray[0]), UInt32.Parse(tapArray[1]));

        //            ans.Add(acqBd, tap);
        //        }
        //    }

        //    return ans;
        //}

        internal static Dictionary<AcqBdNo, BoradSyncRangeValue> ReadBoardSyncDataFromFile(String fileName)
        {
            Dictionary<AcqBdNo, BoradSyncRangeValue> ans = new();

            if (!File.Exists(fileName))
                return ans;
            StreamReader sr = new StreamReader(fileName);
            while (!sr.EndOfStream)
            {
                String? info = sr.ReadLine();
                if (info != null)
                {
                    String[] keyValue = info.Split(":");
                    if (keyValue.Length == 0)
                        continue;

                    String[] tapArray = keyValue[1].Split(",");
                    if (tapArray.Length < 2)
                        continue;

                    if (!Enum.GetNames<AcqBdNo>().Contains(keyValue[0]))
                    {
                        continue;
                    }
                    AcqBdNo acqBd = (AcqBdNo)Enum.Parse(typeof(AcqBdNo), keyValue[0]);
                    BoradSyncRangeValue tap = new(UInt32.Parse(tapArray[0]), UInt32.Parse(tapArray[1]));

                    ans.Add(acqBd, tap);
                }
            }
            sr.Close();
            return ans;
        }
        #endregion

        #endregion

        #region Serdes
        internal static Dictionary<AcqBdNo, BoardSyncRangeReg> _DataStartRegTable = new()//????
        {
            //{ AcqBdNo.B1, new(ProcBdReg.W.BoardSync_Data_pro_iserdes_TAP_start,  ProcBdReg.W.BoardSync_Data_pro_iserdes_TAP_stop)},
            //{ AcqBdNo.B2, new(ProcBdReg.W.BoardSync_Data_pro_iserdes_TAP_start2, ProcBdReg.W.BoardSync_Data_pro_iserdes_TAP_stop2) },
            //{ AcqBdNo.B3, new(ProcBdReg.W.BoardSync_Data_pro_iserdes_TAP_start3, ProcBdReg.W.BoardSync_Data_pro_iserdes_TAP_stop3) },
            //{ AcqBdNo.B4, new(ProcBdReg.W.BoardSync_Data_pro_iserdes_TAP_start4, ProcBdReg.W.BoardSync_Data_pro_iserdes_TAP_stop4) },
            //{ AcqBdNo.B5, new(ProcBdReg.W.BoardSync_Data_pro_iserdes_TAP_start5, ProcBdReg.W.BoardSync_Data_pro_iserdes_TAP_stop5) },
            //{ AcqBdNo.B6, new(ProcBdReg.W.BoardSync_Data_pro_iserdes_TAP_start6, ProcBdReg.W.BoardSync_Data_pro_iserdes_TAP_stop6) },
            //{ AcqBdNo.B7, new(ProcBdReg.W.BoardSync_Data_pro_iserdes_TAP_start7, ProcBdReg.W.BoardSync_Data_pro_iserdes_TAP_stop7) },
            //{ AcqBdNo.B8, new(ProcBdReg.W.BoardSync_Data_pro_iserdes_TAP_start8, ProcBdReg.W.BoardSync_Data_pro_iserdes_TAP_stop8) },
            //{ AcqBdNo.B9, new(ProcBdReg.W.BoardSync_Data_pro_iserdes_TAP_start9, ProcBdReg.W.BoardSync_Data_pro_iserdes_TAP_stop9) },
            //{AcqBdNo.B10, new(ProcBdReg.W.BoardSync_Data_pro_iserdes_TAP_start10, ProcBdReg.W.BoardSync_Data_pro_iserdes_TAP_stop10) },
            //{AcqBdNo.B11, new(ProcBdReg.W.BoardSync_Data_pro_iserdes_TAP_start11, ProcBdReg.W.BoardSync_Data_pro_iserdes_TAP_stop11) },
            //{AcqBdNo.B12, new(ProcBdReg.W.BoardSync_Data_pro_iserdes_TAP_start12, ProcBdReg.W.BoardSync_Data_pro_iserdes_TAP_stop12) },
        };

        internal static Dictionary<AcqBdNo, ProcBdReg.R> _DataSyncStatusRegTable = new()//????
        {
            //{ AcqBdNo.B1, ProcBdReg.R.BoardSync_Data_IserdesSyncStatusAcq1 },
            //{ AcqBdNo.B2, ProcBdReg.R.BoardSync_Data_IserdesSyncStatusAcq2 },
            //{ AcqBdNo.B3, ProcBdReg.R.BoardSync_Data_IserdesSyncStatusAcq3 },
            //{ AcqBdNo.B4, ProcBdReg.R.BoardSync_Data_IserdesSyncStatusAcq4 },
            //{ AcqBdNo.B5, ProcBdReg.R.BoardSync_Data_IserdesSyncStatusAcq5 },
            //{ AcqBdNo.B6, ProcBdReg.R.BoardSync_Data_IserdesSyncStatusAcq6 },
            //{ AcqBdNo.B7, ProcBdReg.R.BoardSync_Data_IserdesSyncStatusAcq7 },
            //{ AcqBdNo.B8, ProcBdReg.R.BoardSync_Data_IserdesSyncStatusAcq8 },
            //{ AcqBdNo.B9, ProcBdReg.R.BoardSync_Data_IserdesSyncStatusAcq9 },
            //{AcqBdNo.B10, ProcBdReg.R.BoardSync_Data_IserdesSyncStatusAcq10 },
            //{AcqBdNo.B11, ProcBdReg.R.BoardSync_Data_IserdesSyncStatusAcq11 },
            //{AcqBdNo.B12, ProcBdReg.R.BoardSync_Data_IserdesSyncStatusAcq12 },
        };

        internal static Dictionary<AcqBdNo, ProcBdReg.R> _DataSyncTapRegTable = new()//????
        {
            //{ AcqBdNo.B1, ProcBdReg.R.BoardSync_Data_IserdesTapReadAcq1 },
            //{ AcqBdNo.B2, ProcBdReg.R.BoardSync_Data_IserdesTapReadAcq2 },
            //{ AcqBdNo.B3, ProcBdReg.R.BoardSync_Data_IserdesTapReadAcq3 },
            //{ AcqBdNo.B4, ProcBdReg.R.BoardSync_Data_IserdesTapReadAcq4 },
            //{ AcqBdNo.B5, ProcBdReg.R.BoardSync_Data_IserdesTapReadAcq5 },
            //{ AcqBdNo.B6, ProcBdReg.R.BoardSync_Data_IserdesTapReadAcq6 },
            //{ AcqBdNo.B7, ProcBdReg.R.BoardSync_Data_IserdesTapReadAcq7 },
            //{ AcqBdNo.B8, ProcBdReg.R.BoardSync_Data_IserdesTapReadAcq8 },
            //{ AcqBdNo.B9, ProcBdReg.R.BoardSync_Data_IserdesTapReadAcq9 },
            //{AcqBdNo.B10, ProcBdReg.R.BoardSync_Data_IserdesTapReadAcq10 },
            //{AcqBdNo.B11, ProcBdReg.R.BoardSync_Data_IserdesTapReadAcq11 },
            //{AcqBdNo.B12, ProcBdReg.R.BoardSync_Data_IserdesTapReadAcq12 },
        };


        #endregion
        internal static Dictionary<BoardSyncEnumP2A, AcqBdReg.W> _SetTapRegTableP2A = new()
        {
            //{ BoardSyncEnumP2A.Write,     AcqBdReg.W.BoardSync_Ctrl_p2a_tap_load_set_wr_en      },
            //{ BoardSyncEnumP2A.Read,      AcqBdReg.W.BoardSync_Ctrl_p2a_tap_load_set_rd_en      },
            //{ BoardSyncEnumP2A.Reset,     AcqBdReg.W.BoardSync_Ctrl_p2a_tap_load_set_rst        },
            //{ BoardSyncEnumP2A.TrigLocat, AcqBdReg.W.BoardSync_Ctrl_p2a_tap_load_set_trig_locat },
        };//????

        private static Dictionary<BoardSyncEnumP2A, Dictionary<BoardSyncRangeEnum, AcqBdReg.W>> _TapRangeRegP2A = new()
        {
            [BoardSyncEnumP2A.Write] = new()
            {
                //{ BoardSyncRangeEnum.Start, AcqBdReg.W .BoardSync_Ctrl_p2a_TAP_start_wr_en },
                //{ BoardSyncRangeEnum.Stop, AcqBdReg.W .BoardSync_Ctrl_p2a_TAP_stop_wr_en },
            },

            [BoardSyncEnumP2A.Read] = new()
            {
                //{ BoardSyncRangeEnum.Start, AcqBdReg.W .BoardSync_Ctrl_p2a_TAP_start_rd_en },
                //{ BoardSyncRangeEnum.Stop, AcqBdReg.W .BoardSync_Ctrl_p2a_TAP_stop_rd_en },
            },

            [BoardSyncEnumP2A.Reset] = new()
            {
                //{ BoardSyncRangeEnum.Start, AcqBdReg.W .BoardSync_Ctrl_p2a_TAP_start_RST },
                //{ BoardSyncRangeEnum.Stop, AcqBdReg.W .BoardSync_Ctrl_p2a_TAP_stop_RST },
            },

            [BoardSyncEnumP2A.TrigLocat] = new()
            {
                //{ BoardSyncRangeEnum.Start, AcqBdReg.W .BoardSync_Ctrl_p2a_TAP_start_trig_locat },
                //{ BoardSyncRangeEnum.Stop, AcqBdReg.W .BoardSync_Ctrl_p2a_TAP_stop_trig_locat },
            },
        };//????

        //internal static void ConfigTapNumP2A(UInt32 tapNum, UInt32 mask = 0)
        //{
        //    foreach (BoardSyncEnumP2A syncType in _SetTapRegTableP2A.Keys)
        //    {
        //        Hd.CurrProduct?.AcqBd?.WriteToAllFpga(_SetTapRegTableP2A[syncType], tapNum | mask);
        //    }
        //}
        internal static void ConfigSyncTapRangeP2A(Dictionary<BoardSyncRangeEnum, UInt32> rangeTable)
        {
            foreach (BoardSyncEnumP2A syncType in _TapRangeRegP2A.Keys)
            {
                foreach (BoardSyncRangeEnum rangeType in _TapRangeRegP2A[syncType].Keys)
                {
                    if (rangeTable.ContainsKey(rangeType))
                    {
                        Hd.CurrProduct?.AcqBd?.WriteToAllFpga(_TapRangeRegP2A[syncType][rangeType], rangeTable[rangeType]);
                    }
                }
            }
        }
        internal static Dictionary<BoardSyncEnumP2A, Dictionary<AcqBdNo, UInt32>> ReadTapValueP2A()
        {
            Dictionary<BoardSyncEnumP2A, Dictionary<AcqBdNo, UInt32>> result = new();

            foreach (BoardSyncEnumP2A syncType in _SyncTapReadRegP2A.Keys)
            {
                result[syncType] = Hd.CurrProduct?.AcqBd?.ReadFromAllFpga(_SyncTapReadRegP2A[syncType], 0x1ff) ?? new();
            }
            return result;
        }
        internal static void SaveBoardSyncP2AToFile(String filePath, String fileName, Dictionary<BoardSyncEnumP2A, Dictionary<AcqBdNo, UInt32>> syncTable)
        {
            if (!_EnableSaveToFile)
                return;
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            StreamWriter sw = new StreamWriter(filePath + fileName);
            foreach (BoardSyncEnumP2A syncType in Enum.GetValues<BoardSyncEnumP2A>())
            {
                if (syncTable.ContainsKey(syncType))
                {
                    sw.WriteLine($"{syncType}");
                    foreach (AcqBdNo acqBd in Enum.GetValues<AcqBdNo>())
                    {
                        if (syncTable[syncType].ContainsKey(acqBd))
                        {
                            sw.WriteLine($"{acqBd}:{syncTable[syncType][acqBd]}");
                        }
                    }
                }
            }
            sw.Flush();
            sw.Close();
        }
        private static Dictionary<BoardSyncEnumP2A, AcqBdReg.R> _SyncTapReadRegP2A = new()//????
        {
            //{ BoardSyncEnumP2A.Write,       AcqBdReg.R.BoardSync_Ctrl_p2a_tap_read_wr_en        },
            //{ BoardSyncEnumP2A.Read,        AcqBdReg.R.BoardSync_Ctrl_p2a_tap_read_rd_en        },
            //{ BoardSyncEnumP2A.Reset,       AcqBdReg.R.BoardSync_Ctrl_p2a_tap_read_rst          },
            //{ BoardSyncEnumP2A.TrigLocat,   AcqBdReg.R.BoardSync_Ctrl_p2a_tap_read_trig_locat   },
        };

        private static Dictionary<BoardSyncEnumP2A, AcqBdReg.R> _SyncTestDataRegP2A = new()//????
        {
            //{ BoardSyncEnumP2A.Write,       AcqBdReg.R.BoardSync_Ctrl_p2a_read_test_data_wr_en        },
            //{ BoardSyncEnumP2A.Read,        AcqBdReg.R.BoardSync_Ctrl_p2a_read_test_data_rd_en        },
            //{ BoardSyncEnumP2A.Reset,       AcqBdReg.R.BoardSync_Ctrl_p2a_read_test_data_rst          },
            //{ BoardSyncEnumP2A.TrigLocat,   AcqBdReg.R.BoardSync_Ctrl_p2a_read_test_data_trig_locat   },
        };
        internal static Dictionary<BoardSyncEnumP2A, Dictionary<AcqBdNo, UInt32>> ReadBoardSyncP2AFromFile(String fileName)
        {
            BoardSyncEnumP2A syncType = BoardSyncEnumP2A.Write;
            Dictionary<BoardSyncEnumP2A, Dictionary<AcqBdNo, UInt32>> ans = new();

            if (!File.Exists(fileName))
                return ans;

            StreamReader sr = new StreamReader(fileName);
            while (!sr.EndOfStream)
            {
                String? info = sr.ReadLine();
                if (info != null)
                {
                    String[] keyValue = info.Split(":");
                    if (keyValue.Length == 1 && Enum.GetNames<BoardSyncEnumP2A>().Contains(keyValue[0]))
                    {
                        syncType = (BoardSyncEnumP2A)Enum.Parse(typeof(BoardSyncEnumP2A), keyValue[0]);
                        ans[syncType] = new Dictionary<AcqBdNo, UInt32>();
                        continue;
                    }

                    if (ans.ContainsKey(syncType) && Enum.GetNames<AcqBdNo>().Contains(keyValue[0]))
                    {
                        AcqBdNo acqBd = (AcqBdNo)Enum.Parse(typeof(AcqBdNo), keyValue[0]);
                        UInt32 tap = UInt32.Parse(keyValue[1]);

                        ans[syncType][acqBd] = tap;
                    }
                }
            }

            sr.Close();

            return ans;
        }
        internal static void SwitchTestFlashPathA2P()
        {
            ConfigSyncFlagA2P(new UInt32[] { 0x0 });

            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.BoardSync_Ctrl_a2p_trig_location_scan_switch_acq, 0);
            //HdIO.WriteReg(ProcBdReg.W.BoardSync_Ctrl_RxSwitchPro, 0x0);
        }//????
        internal static void SaveBoardSyncA2PToFile(String filePath, String fileName, Dictionary<BoardSyncEnumA2P, Dictionary<AcqBdNo, UInt32>> syncTable)
        {
            if (!_EnableSaveToFile)
                return;
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            StreamWriter sw = new StreamWriter(filePath + fileName);
            foreach (BoardSyncEnumA2P syncType in syncTable.Keys)
            {
                sw.WriteLine($"{syncType}");
                foreach (AcqBdNo acqBd in syncTable[syncType].Keys)
                {
                    sw.WriteLine($"{acqBd}:{syncTable[syncType][acqBd]}");
                }
            }
            sw.Flush();
            sw.Close();
        }
        internal static void ConfigSyncTapRangeA2P(Dictionary<BoardSyncRangeEnum, UInt32> rangeTable)
        {
            foreach (BoardSyncEnumA2P syncType in _ScanRangeRegA2P.Keys)
            {
                foreach (BoardSyncRangeEnum rangeType in _ScanRangeRegA2P[syncType].Keys)
                {
                    if (rangeTable.ContainsKey(rangeType))
                    {
                        foreach (AcqBdNo acqBd in _ScanRangeRegA2P[syncType][rangeType].Keys)
                        {
                            HdIO.WriteReg(_ScanRangeRegA2P[syncType][rangeType][acqBd], rangeTable[rangeType]);
                        }
                    }
                }
            }
        }
        private static Dictionary<BoardSyncEnumA2P, Dictionary<BoardSyncRangeEnum, Dictionary<AcqBdNo, ProcBdReg.W>>> _ScanRangeRegA2P = new()
        {
            [BoardSyncEnumA2P.TrigLocat] = new()
            {
                [BoardSyncRangeEnum.Start] = new()
                {
                    //{ AcqBdNo.B1,  ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_locat_TAP_start_acq1  },
                    //{ AcqBdNo.B2,  ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_locat_TAP_start_acq2  },
                    //{ AcqBdNo.B3,  ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_locat_TAP_start_acq3  },
                    //{ AcqBdNo.B4,  ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_locat_TAP_start_acq4  },
                    //{ AcqBdNo.B5,  ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_locat_TAP_start_acq5  },
                    //{ AcqBdNo.B6,  ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_locat_TAP_start_acq6  },
                    //{ AcqBdNo.B7,  ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_locat_TAP_start_acq7  },
                    //{ AcqBdNo.B8,  ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_locat_TAP_start_acq8  },
                    //{ AcqBdNo.B9,  ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_locat_TAP_start_acq9  },
                    //{ AcqBdNo.B10, ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_locat_TAP_start_acq10 },
                    //{ AcqBdNo.B11, ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_locat_TAP_start_acq11 },
                    //{ AcqBdNo.B12, ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_locat_TAP_start_acq12 },
                },
                [BoardSyncRangeEnum.Stop] = new()
                {
                    //{ AcqBdNo.B1,  ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_locat_TAP_stop_acq1  },
                    //{ AcqBdNo.B2,  ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_locat_TAP_stop_acq2  },
                    //{ AcqBdNo.B3,  ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_locat_TAP_stop_acq3  },
                    //{ AcqBdNo.B4,  ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_locat_TAP_stop_acq4  },
                    //{ AcqBdNo.B5,  ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_locat_TAP_stop_acq5  },
                    //{ AcqBdNo.B6,  ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_locat_TAP_stop_acq6  },
                    //{ AcqBdNo.B7,  ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_locat_TAP_stop_acq7  },
                    //{ AcqBdNo.B8,  ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_locat_TAP_stop_acq8  },
                    //{ AcqBdNo.B9,  ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_locat_TAP_stop_acq9  },
                    //{ AcqBdNo.B10, ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_locat_TAP_stop_acq10 },
                    //{ AcqBdNo.B11, ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_locat_TAP_stop_acq11 },
                    //{ AcqBdNo.B12, ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_locat_TAP_stop_acq12 },
                },
            },

            [BoardSyncEnumA2P.Trig] = new()
            {
                [BoardSyncRangeEnum.Start] = new()
                {
                    //{ AcqBdNo.B1,  ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_TAP_start_acq1  },
                    //{ AcqBdNo.B2,  ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_TAP_start_acq2  },
                    //{ AcqBdNo.B3,  ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_TAP_start_acq3  },
                    //{ AcqBdNo.B4,  ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_TAP_start_acq4  },
                    //{ AcqBdNo.B5,  ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_TAP_start_acq5  },
                    //{ AcqBdNo.B6,  ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_TAP_start_acq6  },
                    //{ AcqBdNo.B7,  ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_TAP_start_acq7  },
                    //{ AcqBdNo.B8,  ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_TAP_start_acq8  },
                    //{ AcqBdNo.B9,  ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_TAP_start_acq9  },
                    //{ AcqBdNo.B10, ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_TAP_start_acq10 },
                    //{ AcqBdNo.B11, ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_TAP_start_acq11 },
                    //{ AcqBdNo.B12, ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_TAP_start_acq12 },
                },
                [BoardSyncRangeEnum.Stop] = new()
                {
                    //{ AcqBdNo.B1,  ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_TAP_stop_acq1  },
                    //{ AcqBdNo.B2,  ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_TAP_stop_acq2  },
                    //{ AcqBdNo.B3,  ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_TAP_stop_acq3  },
                    //{ AcqBdNo.B4,  ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_TAP_stop_acq4  },
                    //{ AcqBdNo.B5,  ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_TAP_stop_acq5  },
                    //{ AcqBdNo.B6,  ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_TAP_stop_acq6  },
                    //{ AcqBdNo.B7,  ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_TAP_stop_acq7  },
                    //{ AcqBdNo.B8,  ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_TAP_stop_acq8  },
                    //{ AcqBdNo.B9,  ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_TAP_stop_acq9  },
                    //{ AcqBdNo.B10, ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_TAP_stop_acq10 },
                    //{ AcqBdNo.B11, ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_TAP_stop_acq11 },
                    //{ AcqBdNo.B12, ProcBdReg.W.BoardSync_Ctrl_a2p_sync_trig_TAP_stop_acq12 },
                },
            }
        };//????
        private static Dictionary<BoardSyncEnumA2P, Dictionary<AcqBdNo, ProcBdReg.R>> _SyncFlagRedRegA2P = new()
        {
            [BoardSyncEnumA2P.TrigLocat] = new Dictionary<AcqBdNo, ProcBdReg.R>()
            {
                //{ AcqBdNo.B1,  ProcBdReg.R.BoardSync_Ctrl_a2p_sync_trig_locat_flag_acq1},
                //{ AcqBdNo.B2,  ProcBdReg.R.BoardSync_Ctrl_a2p_sync_trig_locat_flag_acq2},
                //{ AcqBdNo.B3,  ProcBdReg.R.BoardSync_Ctrl_a2p_sync_trig_locat_flag_acq3},
                //{ AcqBdNo.B4,  ProcBdReg.R.BoardSync_Ctrl_a2p_sync_trig_locat_flag_acq4},
                //{ AcqBdNo.B5,  ProcBdReg.R.BoardSync_Ctrl_a2p_sync_trig_locat_flag_acq5},
                //{ AcqBdNo.B6,  ProcBdReg.R.BoardSync_Ctrl_a2p_sync_trig_locat_flag_acq6},
                //{ AcqBdNo.B7,  ProcBdReg.R.BoardSync_Ctrl_a2p_sync_trig_locat_flag_acq7},
                //{ AcqBdNo.B8,  ProcBdReg.R.BoardSync_Ctrl_a2p_sync_trig_locat_flag_acq8},
                //{ AcqBdNo.B9,  ProcBdReg.R.BoardSync_Ctrl_a2p_sync_trig_locat_flag_acq9},
                //{AcqBdNo.B10,  ProcBdReg.R.BoardSync_Ctrl_a2p_sync_trig_locat_flag_acq10},
                //{AcqBdNo.B11,  ProcBdReg.R.BoardSync_Ctrl_a2p_sync_trig_locat_flag_acq11},
                //{AcqBdNo.B12,  ProcBdReg.R.BoardSync_Ctrl_a2p_sync_trig_locat_flag_acq12},
            },

            [BoardSyncEnumA2P.Trig] = new Dictionary<AcqBdNo, ProcBdReg.R>()
            {
                //{ AcqBdNo.B1,  ProcBdReg.R.BoardSync_Ctrl_a2p_sync_trig_flag_acq1},
                //{ AcqBdNo.B2,  ProcBdReg.R.BoardSync_Ctrl_a2p_sync_trig_flag_acq2},
                //{ AcqBdNo.B3,  ProcBdReg.R.BoardSync_Ctrl_a2p_sync_trig_flag_acq3},
                //{ AcqBdNo.B4,  ProcBdReg.R.BoardSync_Ctrl_a2p_sync_trig_flag_acq4},
                //{ AcqBdNo.B5,  ProcBdReg.R.BoardSync_Ctrl_a2p_sync_trig_flag_acq5},
                //{ AcqBdNo.B6,  ProcBdReg.R.BoardSync_Ctrl_a2p_sync_trig_flag_acq6},
                //{ AcqBdNo.B7,  ProcBdReg.R.BoardSync_Ctrl_a2p_sync_trig_flag_acq7},
                //{ AcqBdNo.B8,  ProcBdReg.R.BoardSync_Ctrl_a2p_sync_trig_flag_acq8},
                //{ AcqBdNo.B9,  ProcBdReg.R.BoardSync_Ctrl_a2p_sync_trig_flag_acq9},
                //{AcqBdNo.B10,  ProcBdReg.R.BoardSync_Ctrl_a2p_sync_trig_flag_acq10},
                //{AcqBdNo.B11,  ProcBdReg.R.BoardSync_Ctrl_a2p_sync_trig_flag_acq11},
                //{AcqBdNo.B12,  ProcBdReg.R.BoardSync_Ctrl_a2p_sync_trig_flag_acq12},
            },
        };//????

        internal static Dictionary<BoardSyncEnumA2P, Dictionary<AcqBdNo, UInt32>> ReadSyncFlagA2P()
        {
            Dictionary<BoardSyncEnumA2P, Dictionary<AcqBdNo, UInt32>> result = new();
            foreach (BoardSyncEnumA2P syncType in _SyncFlagRedRegA2P.Keys)
            {
                result[syncType] = new();
                foreach (AcqBdNo acqBd in _SyncFlagRedRegA2P[syncType].Keys)
                {
                    result[syncType][acqBd] = HdIO.ReadReg(_SyncFlagRedRegA2P[syncType][acqBd]);
                }
            }
            return result;
        }
        internal static void ConfigTapNumA2P(UInt32 tapNum, UInt32 mask = 0)
        {
            foreach (BoardSyncEnumA2P syncType in _SetTapRegTableA2P.Keys)
            {
                foreach (ProcBdReg.W reg in _SetTapRegTableA2P[syncType].Values)
                    HdIO.WriteReg(reg, tapNum | mask);
            }
        }
        private static Dictionary<BoardSyncEnumA2P, Dictionary<AcqBdNo, ProcBdReg.W>> _SetTapRegTableA2P = new()
        {
            [BoardSyncEnumA2P.TrigLocat] = new Dictionary<AcqBdNo, ProcBdReg.W>()
            {
                //{ AcqBdNo.B1,  ProcBdReg.W.BoardSync_Ctrl_a2p_tap_load_set_trig_locat_acq1},
                //{ AcqBdNo.B2,  ProcBdReg.W.BoardSync_Ctrl_a2p_tap_load_set_trig_locat_acq2},
                //{ AcqBdNo.B3,  ProcBdReg.W.BoardSync_Ctrl_a2p_tap_load_set_trig_locat_acq3},
                //{ AcqBdNo.B4,  ProcBdReg.W.BoardSync_Ctrl_a2p_tap_load_set_trig_locat_acq4},
                //{ AcqBdNo.B5,  ProcBdReg.W.BoardSync_Ctrl_a2p_tap_load_set_trig_locat_acq5},
                //{ AcqBdNo.B6,  ProcBdReg.W.BoardSync_Ctrl_a2p_tap_load_set_trig_locat_acq6},
                //{ AcqBdNo.B7,  ProcBdReg.W.BoardSync_Ctrl_a2p_tap_load_set_trig_locat_acq7},
                //{ AcqBdNo.B8,  ProcBdReg.W.BoardSync_Ctrl_a2p_tap_load_set_trig_locat_acq8},
                //{ AcqBdNo.B9,  ProcBdReg.W.BoardSync_Ctrl_a2p_tap_load_set_trig_locat_acq9},
                //{AcqBdNo.B10,  ProcBdReg.W.BoardSync_Ctrl_a2p_tap_load_set_trig_locat_acq10},
                //{AcqBdNo.B11,  ProcBdReg.W.BoardSync_Ctrl_a2p_tap_load_set_trig_locat_acq11},
                //{AcqBdNo.B12,  ProcBdReg.W.BoardSync_Ctrl_a2p_tap_load_set_trig_locat_acq12},
            },

            [BoardSyncEnumA2P.Trig] = new Dictionary<AcqBdNo, ProcBdReg.W>()
            {
                //{ AcqBdNo.B1,  ProcBdReg.W.BoardSync_Ctrl_a2p_tap_load_set_trig_acq1},
                //{ AcqBdNo.B2,  ProcBdReg.W.BoardSync_Ctrl_a2p_tap_load_set_trig_acq2},
                //{ AcqBdNo.B3,  ProcBdReg.W.BoardSync_Ctrl_a2p_tap_load_set_trig_acq3},
                //{ AcqBdNo.B4,  ProcBdReg.W.BoardSync_Ctrl_a2p_tap_load_set_trig_acq4},
                //{ AcqBdNo.B5,  ProcBdReg.W.BoardSync_Ctrl_a2p_tap_load_set_trig_acq5},
                //{ AcqBdNo.B6,  ProcBdReg.W.BoardSync_Ctrl_a2p_tap_load_set_trig_acq6},
                //{ AcqBdNo.B7,  ProcBdReg.W.BoardSync_Ctrl_a2p_tap_load_set_trig_acq7},
                //{ AcqBdNo.B8,  ProcBdReg.W.BoardSync_Ctrl_a2p_tap_load_set_trig_acq8},
                //{ AcqBdNo.B9,  ProcBdReg.W.BoardSync_Ctrl_a2p_tap_load_set_trig_acq9},
                //{AcqBdNo.B10,  ProcBdReg.W.BoardSync_Ctrl_a2p_tap_load_set_trig_acq10},
                //{AcqBdNo.B11,  ProcBdReg.W.BoardSync_Ctrl_a2p_tap_load_set_trig_acq11},
                //{AcqBdNo.B12,  ProcBdReg.W.BoardSync_Ctrl_a2p_tap_load_set_trig_acq12},
            },
        };//????
        internal static Dictionary<BoardSyncEnumA2P, Dictionary<AcqBdNo, UInt32>> ReadTapValueA2P()
        {
            Dictionary<BoardSyncEnumA2P, Dictionary<AcqBdNo, UInt32>> ans = new();
            foreach (BoardSyncEnumA2P syncType in _ReadTapRegTableA2P.Keys)
            {
                ans[syncType] = new();
                foreach (AcqBdNo acqBd in _ReadTapRegTableA2P[syncType].Keys)
                {
                    ans[syncType][acqBd] = HdIO.ReadReg(_ReadTapRegTableA2P[syncType][acqBd]);
                }
            }
            return ans;
        }
        private static Dictionary<BoardSyncEnumA2P, Dictionary<AcqBdNo, ProcBdReg.R>> _ReadTapRegTableA2P = new()
        {
            //[BoardSyncEnumA2P.TrigLocat] = new Dictionary<AcqBdNo, ProcBdReg.R>()
            //{
            //    { AcqBdNo.B1, ProcBdReg.R.BoardSync_Ctrl_a2p_tap_read_trig_locat_acq1 },
            //    { AcqBdNo.B2, ProcBdReg.R.BoardSync_Ctrl_a2p_tap_read_trig_locat_acq2 },
            //    { AcqBdNo.B3, ProcBdReg.R.BoardSync_Ctrl_a2p_tap_read_trig_locat_acq3 },
            //    { AcqBdNo.B4, ProcBdReg.R.BoardSync_Ctrl_a2p_tap_read_trig_locat_acq4 },
            //    { AcqBdNo.B5, ProcBdReg.R.BoardSync_Ctrl_a2p_tap_read_trig_locat_acq5 },
            //    { AcqBdNo.B6, ProcBdReg.R.BoardSync_Ctrl_a2p_tap_read_trig_locat_acq6 },
            //    { AcqBdNo.B7, ProcBdReg.R.BoardSync_Ctrl_a2p_tap_read_trig_locat_acq7 },
            //    { AcqBdNo.B8, ProcBdReg.R.BoardSync_Ctrl_a2p_tap_read_trig_locat_acq8 },
            //    { AcqBdNo.B9, ProcBdReg.R.BoardSync_Ctrl_a2p_tap_read_trig_locat_acq9 },
            //    {AcqBdNo.B10, ProcBdReg.R.BoardSync_Ctrl_a2p_tap_read_trig_locat_acq10},
            //    {AcqBdNo.B11, ProcBdReg.R.BoardSync_Ctrl_a2p_tap_read_trig_locat_acq11},
            //    {AcqBdNo.B12, ProcBdReg.R.BoardSync_Ctrl_a2p_tap_read_trig_locat_acq12},
            //},
            //[BoardSyncEnumA2P.Trig] = new Dictionary<AcqBdNo, ProcBdReg.R>()
            //{
            //    { AcqBdNo.B1, ProcBdReg.R.BoardSync_Ctrl_a2p_tap_read_trig_acq1 },
            //    { AcqBdNo.B2, ProcBdReg.R.BoardSync_Ctrl_a2p_tap_read_trig_acq2 },
            //    { AcqBdNo.B3, ProcBdReg.R.BoardSync_Ctrl_a2p_tap_read_trig_acq3 },
            //    { AcqBdNo.B4, ProcBdReg.R.BoardSync_Ctrl_a2p_tap_read_trig_acq4 },
            //    { AcqBdNo.B5, ProcBdReg.R.BoardSync_Ctrl_a2p_tap_read_trig_acq5 },
            //    { AcqBdNo.B6, ProcBdReg.R.BoardSync_Ctrl_a2p_tap_read_trig_acq6 },
            //    { AcqBdNo.B7, ProcBdReg.R.BoardSync_Ctrl_a2p_tap_read_trig_acq7 },
            //    { AcqBdNo.B8, ProcBdReg.R.BoardSync_Ctrl_a2p_tap_read_trig_acq8 },
            //    { AcqBdNo.B9, ProcBdReg.R.BoardSync_Ctrl_a2p_tap_read_trig_acq9 },
            //    {AcqBdNo.B10, ProcBdReg.R.BoardSync_Ctrl_a2p_tap_read_trig_acq10},
            //    {AcqBdNo.B11, ProcBdReg.R.BoardSync_Ctrl_a2p_tap_read_trig_acq11},
            //    {AcqBdNo.B12, ProcBdReg.R.BoardSync_Ctrl_a2p_tap_read_trig_acq12},
            //},
        };
        internal static Dictionary<BoardSyncEnumA2P, Dictionary<AcqBdNo, UInt32>> ReadBoardSyncA2PFromFile(String fileName)
        {
            BoardSyncEnumA2P syncType = BoardSyncEnumA2P.Trig;
            Dictionary<BoardSyncEnumA2P, Dictionary<AcqBdNo, UInt32>> ans = new();

            if (!File.Exists(fileName))
                return ans;

            StreamReader sr = new StreamReader(fileName);
            while (!sr.EndOfStream)
            {
                String? info = sr.ReadLine();
                if (info != null)
                {
                    String[] keyValue = info.Split(":");
                    if (keyValue.Length == 1 && Enum.GetNames<BoardSyncEnumA2P>().Contains(keyValue[0]))
                    {
                        syncType = (BoardSyncEnumA2P)Enum.Parse(typeof(BoardSyncEnumA2P), keyValue[0]);
                        ans[syncType] = new Dictionary<AcqBdNo, UInt32>();
                        continue;
                    }

                    if (ans.ContainsKey(syncType) && Enum.GetNames<AcqBdNo>().Contains(keyValue[0]))
                    {
                        AcqBdNo acqBd = (AcqBdNo)Enum.Parse(typeof(AcqBdNo), keyValue[0]);
                        UInt32 tap = UInt32.Parse(keyValue[1]);

                        ans[syncType][acqBd] = tap;
                    }
                }
            }
            sr.Close();

            return ans;
        }    
        /// <summary>
        /// 依次将数组中的元素下发给_SyncFlagWriteRegA2P中的寄存器
        /// </summary>
        /// <param name="data"></param>
        internal static void ConfigSyncFlagA2P(UInt32[] data)
        {
            foreach (BoardSyncEnumA2P syncType in _SyncFlagWriteRegA2P.Keys)
            {
                foreach (ProcBdReg.W reg in _SyncFlagWriteRegA2P[syncType].Values)
                {
                    for (Int32 i = 0; i < data.Length; i++)
                    {
                        HdIO.WriteReg(reg, data[i]);
                    }
                }
            }
        }

        private static Dictionary<BoardSyncEnumA2P, Dictionary<AcqBdNo, ProcBdReg.W>> _SyncFlagWriteRegA2P = new()
        {
            [BoardSyncEnumA2P.TrigLocat] = new Dictionary<AcqBdNo, ProcBdReg.W>()
            {
                //{ AcqBdNo.B1,  ProcBdReg.W.BoardSync_Ctrl_a2p_setting_trig_locat_acq1},
                //{ AcqBdNo.B2,  ProcBdReg.W.BoardSync_Ctrl_a2p_setting_trig_locat_acq2},
                //{ AcqBdNo.B3,  ProcBdReg.W.BoardSync_Ctrl_a2p_setting_trig_locat_acq3},
                //{ AcqBdNo.B4,  ProcBdReg.W.BoardSync_Ctrl_a2p_setting_trig_locat_acq4},
                //{ AcqBdNo.B5,  ProcBdReg.W.BoardSync_Ctrl_a2p_setting_trig_locat_acq5},
                //{ AcqBdNo.B6,  ProcBdReg.W.BoardSync_Ctrl_a2p_setting_trig_locat_acq6},
                //{ AcqBdNo.B7,  ProcBdReg.W.BoardSync_Ctrl_a2p_setting_trig_locat_acq7},
                //{ AcqBdNo.B8,  ProcBdReg.W.BoardSync_Ctrl_a2p_setting_trig_locat_acq8},
                //{ AcqBdNo.B9,  ProcBdReg.W.BoardSync_Ctrl_a2p_setting_trig_locat_acq9},
                //{AcqBdNo.B10,  ProcBdReg.W.BoardSync_Ctrl_a2p_setting_trig_locat_acq10},
                //{AcqBdNo.B11,  ProcBdReg.W.BoardSync_Ctrl_a2p_setting_trig_locat_acq11},
                //{AcqBdNo.B12,  ProcBdReg.W.BoardSync_Ctrl_a2p_setting_trig_locat_acq12},
            },

            [BoardSyncEnumA2P.Trig] = new Dictionary<AcqBdNo, ProcBdReg.W>()
            {
                //{ AcqBdNo.B1,  ProcBdReg.W.BoardSync_Ctrl_a2p_setting_trig_acq1},
                //{ AcqBdNo.B2,  ProcBdReg.W.BoardSync_Ctrl_a2p_setting_trig_acq2},
                //{ AcqBdNo.B3,  ProcBdReg.W.BoardSync_Ctrl_a2p_setting_trig_acq3},
                //{ AcqBdNo.B4,  ProcBdReg.W.BoardSync_Ctrl_a2p_setting_trig_acq4},
                //{ AcqBdNo.B5,  ProcBdReg.W.BoardSync_Ctrl_a2p_setting_trig_acq5},
                //{ AcqBdNo.B6,  ProcBdReg.W.BoardSync_Ctrl_a2p_setting_trig_acq6},
                //{ AcqBdNo.B7,  ProcBdReg.W.BoardSync_Ctrl_a2p_setting_trig_acq7},
                //{ AcqBdNo.B8,  ProcBdReg.W.BoardSync_Ctrl_a2p_setting_trig_acq8},
                //{ AcqBdNo.B9,  ProcBdReg.W.BoardSync_Ctrl_a2p_setting_trig_acq9},
                //{AcqBdNo.B10,  ProcBdReg.W.BoardSync_Ctrl_a2p_setting_trig_acq10},
                //{AcqBdNo.B11,  ProcBdReg.W.BoardSync_Ctrl_a2p_setting_trig_acq11},
                //{AcqBdNo.B12,  ProcBdReg.W.BoardSync_Ctrl_a2p_setting_trig_acq12},
            },
        };//????
    }

    internal enum BoardSyncEnumP2A
    {
        Write,
        Read,
        Reset,
        TrigLocat,
    }
    internal enum BoardSyncEnumA2P
    {
        TrigLocat,
        Trig,
    }

    internal enum BoardSyncRangeEnum
    {
        Start,
        Stop,
    }
    //#endif
    internal record BoardSyncP2A(UInt32 wr, UInt32 rd, UInt32 rst, UInt32 trigLocat);

    internal record WriteRegA2P(ProcBdReg.W TrigLocat, ProcBdReg.W Trig);

    internal record ReadRegA2P(ProcBdReg.R TrigLocat, ProcBdReg.R Trig);

    internal record BoardSyncA2P(UInt32 TrigLocat, UInt32 Trig);

    internal record BoardSyncRangeReg(ProcBdReg.W TapStart, ProcBdReg.W TapStop);

    internal record BoradSyncRangeValue(UInt32 ValueStart, UInt32 ValueStop);
}

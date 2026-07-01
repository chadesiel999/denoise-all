using ScopeX.ComModel;
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Channels;
using static ScopeX.Hardware.Calibration.Data.Base.ProductDataTranslate_MSO8000X;

namespace ScopeX.Hardware.Calibration.Data.Base
{
    /// <summary>
    /// 自校正数据读取、保存流程
    /// 1、查看是否有AutoCalibration.bin文件，有直接读取，否则从Tool校准数据中拷贝一份
    /// 2、Tool端校准数据有更新时，由于不知道是哪个参数更新，所以直接把AutoCalibration.bin文件删除，重新从Tool校准数据中拷贝一份
    /// 3.1、Tool工具保存时（表示Tool端校准数据有更新），直接把AutoCalibration.bin文件删除，重新从Tool校准数据中拷贝一份，再保存
    /// 3.2、自校正完成时，直接保存
    /// </summary>
    public class AutoCaliParams : ICaliData
    {
        private string CaliDataPath => AppDomain.CurrentDomain.BaseDirectory + @"CaliData\";

        private String FileName => CaliDataPath + DataType.ToString() + ".bin";

        private const Int32 ReservedInt32Count = 64;

        private enum ReservedInt32Index
        {
            TemperatureAtCaliBaseline_mCelsius = 0,
            TemperatureAtCaliGain_mCelsius = 1,
        }

        private Int32 _NewAppendUInt32Count = 12;

        private UInt32[] _Reserved = new UInt32[ReservedInt32Count];

        private AutoCaliParamsPerChannel[/*channelIndex*/ , /*impedanceIndex,50_Om=1 */ , /*yScaleIndex*/] _PerScaleData;

        public UInt32 Trig_ACZero
        {
            get;
            set;
        }
        public UInt32 Trig_ACZero3Div
        {
            get;
            set;
        }

        /// <summary>
        /// 校准时的温度，以毫摄氏度为单位，也就是摄氏度*1000
        /// </summary>
        public Int32 TemperatureAtCaliBaseline_mCelsius
        {
            get => (Int32)_Reserved[(int)ReservedInt32Index.TemperatureAtCaliBaseline_mCelsius];
            set => _Reserved[(int)ReservedInt32Index.TemperatureAtCaliBaseline_mCelsius] = (UInt32)value;
        }

        public Int32 TemperatureAtCaliGain_mCelsius
        {
            get => (Int32)_Reserved[(int)ReservedInt32Index.TemperatureAtCaliGain_mCelsius];
            set => _Reserved[(int)ReservedInt32Index.TemperatureAtCaliGain_mCelsius] = (UInt32)value;
        }

        public Int32 TotalBytes
        {
            get
            {
                var perScaleDataBytes = Marshal.SizeOf(_PerScaleData[0, 0, 0]);
                var allPerScaleDataBytes = 2 * CaliConstants.Fixed_MaxPhysicsChannelCount * CaliConstants.Fixed_MaxPhyCoarseScaleCount * perScaleDataBytes;//2=高阻、低阻
                var totalBytes = allPerScaleDataBytes + sizeof(UInt32) + sizeof(UInt32); //+ Trig_ACZero + Trig_ACZero3Div

                totalBytes += ReservedInt32Count * sizeof(UInt32);

                return totalBytes;
            }
        }
        public Int32 OriginTotleBytes
        {
            get => TotalBytes;
            set { }
        }

        public static AutoCaliParams? Default = new();

        private AutoCaliParams()
        {
            _PerScaleData = new AutoCaliParamsPerChannel[CaliConstants.Fixed_MaxPhysicsChannelCount, 2, CaliConstants.Fixed_MaxPhyCoarseScaleCount];
            for (int channelIndex = 0; channelIndex < CaliConstants.Fixed_MaxPhysicsChannelCount; channelIndex++)
            {
                for (int impedanceIndex = 0; impedanceIndex < 2; impedanceIndex++)
                {
                    for (int scaleIndex = 0; scaleIndex < CaliConstants.Fixed_MaxPhyCoarseScaleCount; scaleIndex++)
                    {
                        _PerScaleData[channelIndex, impedanceIndex, scaleIndex] = new();
                    }
                }
            }
        }

        public void UpDateAutoCaliParams() => CopyFromChannelParamsToAutoCaliParams();

        /// <summary>
        /// 没有自校正bin文件时，就全部从Tool端校准数据拷贝至内存中
        /// </summary>
        private void CopyFromChannelParamsToAutoCaliParams()
        {
            for (var channelIndex = 0; channelIndex < ChannelIdExt.GetAnalogs().Count(); channelIndex++)
            {
                for (var impedanceIndex = 0; impedanceIndex < 2; impedanceIndex++)
                {
                    var maxScaleIndex = (int)(impedanceIndex == 0 ? AnaChnlScaleIndex.Lv10 : AnaChnlScaleIndex.Lv1);
                    var minScaleIndex = (int)(AnaChnlScaleIndex.Lv1m);
                    for (var scaleIndex = minScaleIndex; scaleIndex <= maxScaleIndex; scaleIndex++)
                    {
                        ChnlParamsKeyMap map = new ChnlParamsKeyMap((ChannelId)channelIndex, impedanceIndex == 0, (UInt32)AnalogChanneScaleDefine.PhyChCoarseLevelTableByuV[scaleIndex] / 1_000U);
                        if (map==null|| ProductDataTranslate_MSO8000X.GetChnlParamsItem(map!)==null)
                        {
                            //foreach (CaliDataType dataType in ServerDomainConstants.CurrProductIncludeCaliDataTypes!)
                            //{
                            //    var caliData = CalibrationData.Helper.GetICaliData(dataType);
                            //    if (caliData != null)
                            //    {
                            //        InstrumentInteract.CaliData_Get(this.currInstrument, dataType);
                            //    }
                            //}
                            continue;
                        }
                        var channelparams = ProductDataTranslate_MSO8000X.GetChnlParamsItem(map!)!.Value;
                        var perparam = _PerScaleData[channelIndex, impedanceIndex, scaleIndex];
                        perparam.OffsetPosterior = channelparams.Offset;
                        perparam.OffsetPosterior_3Div = channelparams.Offset_Pos3Div;
                        perparam.OffsetPosterior_N3Div = channelparams.Offset_Neg3Div;
                        perparam.DCTrigZero = channelparams.DCTrigZero;
                        perparam.DCTrigZero_3Div = channelparams.DCTrigZero_3Div;
                        perparam.Gain_CoarseCtrlWord = channelparams.Gain;
                        perparam.Gain_FineByAdc = channelparams.Gain_FineByTenThousandByAdc1;
                        perparam.Gain_FineByFpgaThousand = channelparams.Gain_FineByFpgaThousand;
                        perparam.OffsetPreceding = channelparams.Bias;
                        perparam.OffsetPreceding_3Div = channelparams.Bias_Pos3Div;
                        perparam.OffsetPreceding_N3Div = channelparams.Bias_Neg3Div;
                        perparam.Reserved1 = channelparams.Reserved1;
                        perparam.Reserved2 = channelparams.Reserved2;
                        _PerScaleData[channelIndex, impedanceIndex, scaleIndex] = perparam;
                    }
                }
            }
            Trig_ACZero = ChannelParams.Default.Trig_ACZero;
            Trig_ACZero3Div = ChannelParams.Default.Trig_ACZero3Div;

            TemperatureAtCaliBaseline_mCelsius = ChannelParams.Default.TemperatureAtCaliBaseline_mCelsius;
            TemperatureAtCaliGain_mCelsius = ChannelParams.Default.TemperatureAtCaliGain_mCelsius;
        }

        public AutoCaliParamsPerChannel this[Int32 channelIndex, Int32 impedanceIndex, Int32 yScaleIndex]
        {
            get => _PerScaleData[channelIndex, impedanceIndex, yScaleIndex];
            set => _PerScaleData[channelIndex, impedanceIndex, yScaleIndex] = value;
        }

        public CaliDataType DataType => CaliDataType.AutoCalibration;

        public void Deserialize(byte[] content)
        {
            var perscaledatayytes = Marshal.SizeOf(_PerScaleData[0, 0, 0]);

            if (content.Length < (TotalBytes - ReservedInt32Count * sizeof(UInt32)))
            {
                return;
            }

            var bufferindex = 0;
            for (int channelIndex = 0; channelIndex < CaliConstants.Fixed_MaxPhysicsChannelCount; channelIndex++)
            {
                for (int impedanceIndex = 0; impedanceIndex < 2; impedanceIndex++)
                {
                    for (int scaleIndex = 0; scaleIndex < CaliConstants.Fixed_MaxPhyCoarseScaleCount; scaleIndex++)
                    {
                        _PerScaleData[channelIndex, impedanceIndex, scaleIndex] = Helper.BytesToStruct<AutoCaliParamsPerChannel>(content, bufferindex, typeof(AutoCaliParamsPerChannel));
                        bufferindex += perscaledatayytes;
                    }
                }
            }
            var allperscaledatayytes = CaliConstants.Fixed_MaxPhysicsChannelCount * 2 * CaliConstants.Fixed_MaxPhyCoarseScaleCount * perscaledatayytes;
            Trig_ACZero = BitConverter.ToUInt32(content, allperscaledatayytes + 0 * sizeof(UInt32));
            Trig_ACZero3Div = BitConverter.ToUInt32(content, allperscaledatayytes + 1 * sizeof(UInt32));

            if (content.Length >= TotalBytes)
            {
                int newAppendStartByteIndex = allperscaledatayytes + 2 * sizeof(UInt32);
                for (int i = 0; i < ReservedInt32Count; i++)
                {
                    _Reserved[i] = BitConverter.ToUInt32(content, newAppendStartByteIndex + sizeof(Int32) * i);
                }
            }
            //UInt32 startindex = 0;
            //Unsafe.CopyBlock(ref Unsafe.As<AutoCaliParamsPerChannel, byte>(ref _PerScaleData[0, 0, 0]), ref content[startindex], (UInt32)(_PerScaleData.Length * Unsafe.SizeOf<AutoCaliParamsPerChannel>()));

            //startindex += (UInt32)(_PerScaleData.Length * Unsafe.SizeOf<AutoCaliParamsPerChannel>());
            //Trig_ACZero = Unsafe.As<byte, UInt32>(ref content[startindex]);

            //startindex += (UInt32)Unsafe.SizeOf<UInt32>();
            //Trig_ACZero3Div = Unsafe.As<byte, UInt32>(ref content[startindex]);

            //startindex += (UInt32)Unsafe.SizeOf<UInt32>();
            //Unsafe.CopyBlock(ref Unsafe.As<UInt32, byte>(ref _Reserved[0]), ref content[startindex], (UInt32)(_Reserved.Length * Unsafe.SizeOf<UInt32>()));

            CheckValid();
        }

        public byte[] Serialize()
        {
            System.IO.MemoryStream memoryStream = new MemoryStream();
            for (int channelIndex = 0; channelIndex < CaliConstants.Fixed_MaxPhysicsChannelCount; channelIndex++)
            {
                for (int impedanceIndex = 0; impedanceIndex < 2; impedanceIndex++)
                {
                    for (int scaleIndex = 0; scaleIndex < CaliConstants.Fixed_MaxPhyCoarseScaleCount; scaleIndex++)
                    {
                        memoryStream.Write(Helper.StructToBytes(_PerScaleData[channelIndex, impedanceIndex, scaleIndex]));
                    }
                }
            }
            memoryStream.Write(BitConverter.GetBytes(Trig_ACZero));
            memoryStream.Write(BitConverter.GetBytes(Trig_ACZero3Div));

            for (int i = 0; i < ReservedInt32Count; i++)
            {
                memoryStream.Write(BitConverter.GetBytes(_Reserved[i]));
            }

            byte[] result = memoryStream.ToArray();
            memoryStream.Close();

            return result;
            //byte[] buffer = new byte[Unsafe.SizeOf<AutoCaliParamsPerChannel>() * _PerScaleData.Length + Unsafe.SizeOf<UInt32>() + Unsafe.SizeOf<UInt32>() + Unsafe.SizeOf<UInt32>() * _Reserved.Length];
            //UInt32 startindex = 0;
            //Unsafe.CopyBlock(ref buffer[startindex], ref Unsafe.As<AutoCaliParamsPerChannel, byte>(ref _PerScaleData[0, 0, 0]), (UInt32)(Unsafe.SizeOf<AutoCaliParamsPerChannel>() * _PerScaleData.Length));

            //startindex += (UInt32)(Unsafe.SizeOf<AutoCaliParamsPerChannel>() * _PerScaleData.Length);
            //Unsafe.CopyBlock(ref buffer[startindex], ref Unsafe.As<UInt32, byte>(ref Unsafe.AsRef(Trig_ACZero)), (UInt32)Marshal.SizeOf(Trig_ACZero));

            //startindex += (UInt32)Unsafe.SizeOf<UInt32>();
            //Unsafe.CopyBlock(ref buffer[startindex], ref Unsafe.As<UInt32, byte>(ref Unsafe.AsRef(Trig_ACZero3Div)), (UInt32)Marshal.SizeOf(Trig_ACZero3Div));

            //startindex += (UInt32)Unsafe.SizeOf<UInt32>();
            //Unsafe.CopyBlock(ref buffer[startindex], ref Unsafe.As<UInt32, byte>(ref Unsafe.AsRef(_Reserved[0])), (UInt32)(Marshal.SizeOf(_Reserved[0]) * _Reserved.Length));

            //return buffer;
        }

        private void CheckValid()
        {
            for (int channelIndex = 0; channelIndex < CaliConstants.Fixed_MaxPhysicsChannelCount; channelIndex++)
            {
                for (int impedanceIndex = 0; impedanceIndex < 2; impedanceIndex++)
                {
                    for (int scaleIndex = 0; scaleIndex < CaliConstants.Fixed_MaxPhyCoarseScaleCount; scaleIndex++)
                    {
                        //Gain_FineByAdc 在 PXI 项目中有其他约定，故不能限制
                        //if (perScaleData[channelIndex, impedanceIndex, scaleIndex].Gain_FineByAdc==0)
                        //    perScaleData[channelIndex, impedanceIndex, scaleIndex].Gain_FineByAdc = 10000;
                        if (_PerScaleData[channelIndex, impedanceIndex, scaleIndex].Gain_FineByFpgaThousand == 0)
                        {
                            _PerScaleData[channelIndex, impedanceIndex, scaleIndex].Gain_FineByFpgaThousand = 1000;
                        }
                    }
                }
            }
            if (TemperatureAtCaliBaseline_mCelsius == 0)
            {
                TemperatureAtCaliBaseline_mCelsius = 40 * 1000;//默认为40摄氏度
            }

            if (TemperatureAtCaliGain_mCelsius == 0)
            {
                TemperatureAtCaliGain_mCelsius = 40 * 1000;//默认为40摄氏度
            }
        }


        public Boolean LoadFromFile()
        {
            //先拷贝一份到内存中
            CopyFromChannelParamsToAutoCaliParams();//全部拷贝

            //再判断是否读取自校正文件
            var bOK = File.Exists(FileName);
            if (bOK)
            {
                Deserialize(File.ReadAllBytes(FileName));
                CopyDataFrom2mVTo1mV();
            }
            else//没有自校正文件 则使用内存中的数据
            {

            }
            return bOK;
        }

        private void CopyDataFrom2mVTo1mV()
        {
            for (int impedance = 0; impedance < 2; impedance++)
            {
                for (int channelIndex = 0; channelIndex < CaliConstants.Fixed_MaxPhysicsChannelCount; channelIndex++)
                {
                    var perchannelsource = AutoCaliParams.Default![channelIndex, impedance, 2];
                    var perchanneltarget = AutoCaliParams.Default![channelIndex, impedance, 1];
                    perchanneltarget.OffsetPosterior = perchannelsource.OffsetPosterior;
                    perchanneltarget.OffsetPosterior_3Div = perchannelsource.OffsetPosterior_3Div / 2;
                    perchanneltarget.OffsetPosterior_N3Div = perchannelsource.OffsetPosterior_N3Div / 2;
                    //perchanneltarget.DCTrigZero = perchannelsource.DCTrigZero;
                    //perchanneltarget.DCTrigZero_3Div = perchannelsource.DCTrigZero_3Div;
                    //perchanneltarget.Gain_CoarseCtrlWord = perchannelsource.Gain_CoarseCtrlWord;
                    //perchanneltarget.Gain_FineByAdc = perchannelsource.Gain_FineByAdc;
                    //perchanneltarget.Gain_FineByFpgaThousand = perchannelsource.Gain_FineByFpgaThousand;
                    //perchanneltarget.OffsetPreceding = perchannelsource.OffsetPreceding;
                    //perchanneltarget.OffsetPreceding_3Div = perchannelsource.OffsetPreceding_3Div;

                    AutoCaliParams.Default![channelIndex, impedance, 1] = perchanneltarget;
                }
            }
        }


        private void DeleteFile()
        {
            var bExists = File.Exists(FileName);
            if (bExists)
            {
                File.Delete(FileName);
            }
        }

        /// <summary>
        /// 与Hardware同进程的函数中使用
        /// </summary>
        public void SaveCaliDataToFile()
        {
            if (!Directory.Exists(CaliDataPath))
            {
                Directory.CreateDirectory(CaliDataPath);
            }

            DeleteFile();

            File.WriteAllBytes(FileName, Serialize());
        }

        /// <summary>
        /// Tool端调用接口
        /// </summary>
        public void SaveToFile()
        {
            CopyFromChannelParamsToAutoCaliParams();
            DeleteFile();
            SaveCaliDataToFile();
        }
    }
}

using ScopeX.ComModel;
using ScopeX.Core.Tools;
using ScopeX.MathExt;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace ScopeX.Core
{
    [Description(nameof(MathType.UserProgram))]
    public class MathUserProgramArg : MathArgPrsnt
    {
        public MathUserProgramArg(MathPrsnt mp, ChannelId id, String formula) : base(mp, id, MathType.UserProgram)
        {
            _Args = ParseFormula(formula);
            _Formula = formula;
            ProgramResult = "";
            // 三方引擎库统一默认为停止运行，由用户自己在界面上启动运行
            RunState = RunStateType.Stop;
        }

        private UserProgramArgs _Args;
        private String _Formula;

        public ChannelId Source
        {
            get => _Args.Source;
            set
            {
                if (_Args.Source != value)
                {
                    _Args = _Args with { Source = value };

                    Model.Formula = MakeFormula();
                    Dispatcher.SoftReset();
                    OnPropertyChanged();
                }
            }
        }

        public String Code
        {
            get => _Formula;
            set
            {
                if (_Formula != value)
                {
                    Model.Formula = MakeFormula();
                    Dispatcher.SoftReset();
                    OnPropertyChanged();
                }
            }
        }

        private UserProgramVector? CreateUserProgramVector()
        {
            UserProgramVector? userprogramvector = null;
            MathVecBuffer.Default.TryGetVector($"{_Args.Source}", out var sourcevector);
            if (sourcevector != null)
            {
                userprogramvector = new UserProgramVector(sourcevector.Elements, sourcevector.XUnit, sourcevector.YUnit, sourcevector.SampInterval, sourcevector.RefSampPos);
                MathVecBuffer.Default.Provide($"{_Args.Dest}", userprogramvector);
            }
            return userprogramvector;
        }

        /// <summary>
        /// 第一次运行引擎库时，MathVecBuffer中没有对应数学通道的Vector或者Vector不是UserProgramVector；
        /// 本函数尝试获取对应数学通道的UserProgramVector，如果获取不到，则进行添加；如果数据源的vector也没有，则返回null
        /// </summary>
        /// <param name="userProgramVector"></param>
        private void GetOrCreateUserProgramVector(out UserProgramVector? userProgramVector)
        {
            userProgramVector = null;
            if (MathVecBuffer.Default.TryGetVector($"{_Args.Dest}", out var destvector))
            {
                if (destvector != null && destvector is UserProgramVector)
                    userProgramVector = (destvector as UserProgramVector)!;
                else
                    userProgramVector = CreateUserProgramVector();
            }
            else
            {
                userProgramVector = CreateUserProgramVector();
            }
        }

        public String ProgramCode
        {
            get
            {
                GetOrCreateUserProgramVector(out var userprogramvector);
                if (userprogramvector != null)
                    return userprogramvector.ProgramCode;
                return "";
            }
            set
            {
                GetOrCreateUserProgramVector(out var userprogramvector);
                if (userprogramvector != null)
                {
                    userprogramvector.ProgramCode = value;
                    UpdateUserProgramVectorWorkFolder(userprogramvector);
                }
            }
        }

        public UserProgramType UserProgramType
        {
            get => _Args.ProgramType;
            set
            {
                if (_Args.ProgramType != value)
                {
                    RunState = RunStateType.Stop;
                    _Args = _Args with { ProgramType = value };

                    Model.Formula = MakeFormula();
                    Dispatcher.SoftReset();
                    WorkFolder = _WorkFolder;
                    OnPropertyChanged();
                }
            }
        }

        public String ProgramResult;

        private void UpdateUserProgramVectorWorkFolder(UserProgramVector userProgramVector)
        {
            switch (UserProgramType)
            {
                case UserProgramType.Matlab:
                    userProgramVector.WorkFolder = _WorkFolder;
                    break;
                case UserProgramType.JavaScript:
                    userProgramVector.WorkFolder = CodeFileFullPath;
                    break;
                case UserProgramType.VbScript:
                    userProgramVector.WorkFolder = CodeFileFullPath;
                    break;
                case UserProgramType.CPlusPlus:
                    userProgramVector.WorkFolder = CodeFileFullPath;
                    break;
                case UserProgramType.Excel:
                    userProgramVector.WorkFolder = CodeFileFullPath;
                    break;
                default:
                    break;
            }
        }

        private String _WorkFolder = Constants.USERCODE_DEF_PATH;
        public String WorkFolder
        {
            get => _WorkFolder;
            set
            {
                _WorkFolder = value;
                GetOrCreateUserProgramVector(out var userProgramVector);
                if (userProgramVector != null)
                {
                    UpdateUserProgramVectorWorkFolder(userProgramVector);
                }
                    OnPropertyChanged();
            }
        }

        #region 文件扩展名
        private enum FileExtensionType_Matlab
        {
            m
        };
        private enum FileExtensionType_Python
        {
            py
        };
        private enum FileExtensionType_VBS
        {
            vbs
        };
        private enum FileExtensionType_JS
        {
            js
        };
        private enum FileExtensionType_CPlusPlus
        {
            cpp
        };

        private enum FileExtensionType_Close
        {
            txt
        };
        #endregion  文件扩展名

        private Enum CurFileExtensionEnum
        {
            get
            {
                switch (UserProgramType)
                {
                    case UserProgramType.Matlab:
                        return FileExtensionType_Matlab.m;
                    case UserProgramType.JavaScript:
                        return FileExtensionType_JS.js;
                    case UserProgramType.VbScript:
                        return FileExtensionType_VBS.vbs;
                    case UserProgramType.CPlusPlus:
                        return FileExtensionType_CPlusPlus.cpp;
                    case UserProgramType.Excel:
                    default:
                        return FileExtensionType_Close.txt;
                }
            }
        }

        public IEnumerable<Enum> CurFileExtension
        {
            get
            {
                yield return CurFileExtensionEnum;
            }
        }

        public String CodeFileFullPath
        {
            get
            {
                return _WorkFolder + "\\" + _Args.ProgramType.ToString() + "Func." + CurFileExtensionEnum.ToString();
            }
        }

        private String MakeDefaultExcelCode()
        {
            String code = "";
            for (Int32 i = 1; i <= 10000; i++)
                code += $"{i}=A{i}\r\n";
            return code;
        }

        public String CurDefaultCode
        {
            get
            {
                switch (UserProgramType)
                {
                    case UserProgramType.Matlab:
                        return $"function resultData = {UserProgramType}Func(sourceData, sampleInterval)\r\nresultData = sourceData;";
                    case UserProgramType.JavaScript:
                        return $"function {UserProgramType}Func(sourceData, sampleInterval)\r\n{{\r\n\tresultData = sourceData\r\n\treturn resultData;\r\n}}";
                    case UserProgramType.VbScript:
                        return $"Function {UserProgramType}Func(sourceData, sampleInterval)\r\nVbScriptFunc = sourceData\r\nEnd Function";
                    case UserProgramType.CPlusPlus:
                        return $"#include <cstring>\r\nextern \"C\"\r\nint {UserProgramType}Func(Double sourceData[], Int32 sourceDataCnt, Int32 sampleInterval, Double resultData[], Int32 resultDataCnt)\r\n{{\r\n\tmemcpy(resultData, sourceData, sourceDataCnt * sizeof(Double));\r\n\treturn sourceDataCnt;\r\n}}";
                    case UserProgramType.Excel:
                        return MakeDefaultExcelCode();
                    default:
                        return "";
                }
            }
        }

        public String curExecuteCode
        {
            get
            {
                switch (UserProgramType)
                {
                    case UserProgramType.Matlab:
                        return $"resultData = {UserProgramType}Func(sourceData, sampleInterval);";
                    case UserProgramType.JavaScript:
                        return $"{UserProgramType}Func";
                    case UserProgramType.VbScript:
                        return $"{UserProgramType}Func";
                    case UserProgramType.CPlusPlus:
                        return $"{UserProgramType}Func";
                    case UserProgramType.Excel:
                    default:
                        return "";
                }
            }
        }

        public override String Description
        {
            get
            {
                switch (_Args.ProgramType)
                {
                    case UserProgramType.Matlab:
                        return $"Mat({Source})";
                    case UserProgramType.JavaScript:
                        return $"JS({Source})";
                    case UserProgramType.VbScript:
                        return $"VBS({Source})";
                    case UserProgramType.CPlusPlus:
                        return $"Cplusplus({Source})";
                    case UserProgramType.Excel:
                        return $"Excel({Source})";
                    default:
                        return "Close";
                }
            }
        }

        public override String MakeFormula()
        {
            return $"{MathType.UserProgram}:Execute.{_Args.ProgramType}({_Args.Source}, {_Args.Dest})";
        }

        #region Validity And Configuration
        internal sealed record UserProgramArgs(ChannelId Source, ChannelId Dest, UserProgramType ProgramType);

        internal static UserProgramArgs ParseFormula(String formula)
        {
            ChannelId source = ChannelId.C1;
            ChannelId dest = ChannelId.M1;
            UserProgramType programtype = UserProgramType.Close;
            String mathtypename = $"{MathType.UserProgram}:";
            var exp = formula;
            if (exp.Length < mathtypename.Length)
                return new(source, dest, programtype);
            if (exp.Substring(0, mathtypename.Length) == mathtypename)
            {
                exp = exp[mathtypename.Length..];
            }

            String functionname = "Execute.";
            if (exp.Length < functionname.Length)
                return new(source, dest, programtype);
            if (exp.Substring(0, functionname.Length) != functionname)
            {
                return new(source, dest, programtype);
            }
            exp = exp[functionname.Length..];
            var substr = exp.Split(new[] { '(', ')', ',' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (substr.Count() >= 3)
            {
                programtype = Enum.Parse<UserProgramType>(substr[0]);
                source = Enum.Parse<ChannelId>(substr[1]);
                dest = Enum.Parse<ChannelId>(substr[2]);
            }

            return new(source, dest, programtype);
        }

        internal static WfmProperties Config(MathModel mch, String exp, Vector? res)
        {
            var pm = ParseFormula(mch.Formula);

            (Int32 Index, Double Value) cisp = (0, 1);


            (Int32 Index, Double Value) tisp = (0, 1);


            var wfmpkg = DsoModel.Default.GetWfmPack(pm.Source);
            if (wfmpkg is not null)
            {
                cisp = wfmpkg.Properties.ChnlScale;

                tisp = wfmpkg.Properties.TmbScale;
            }
            mch.Conditioning.Prefix = Prefix.Milli;

            mch.Conditioning.Unit = mch.IsAutoUnit ? (res?.YUnit ?? "?") : mch.CustomUnit;

            if (res is UserProgramVector && mch.Args is MathUserProgramArg)
            {
                (mch.Args as MathUserProgramArg)!.ProgramResult = (res as UserProgramVector)!.ProgramResult;
            }

            mch.Conditioning.InitialScale = (0, cisp.Value);
            mch.Conditioning.ScaleMaxIndex = 2;
            mch.Conditioning.ScaleMinIndex = -10;

            mch.Sampling.InitialScale = (0, tisp.Value);
            mch.Sampling.ScaleMinIndex = -5;
            mch.Sampling.ScaleMaxIndex = +5;
            mch.Sampling.Prefix = Prefix.Micro;
            mch.Sampling.Unit = res?.XUnit ?? "?";

            (Double Index, Double Value) tmbposition = (Constants.DEF_XPOS_IDX, mch.Sampling.GetPosition(Constants.DEF_XPOS_IDX));
            Double vustartindex = 0;

            if (mch.InitFlag)
            {
                (Int32 VScaleIndex, Int32 HScaleIndex) scale = mch.ReadMathScale(mch.MathType, res);
                mch.Conditioning.ScaleIndex = mch.IsSwitchWindow ? mch.Conditioning.ScaleIndex : 0;
                mch.Sampling.ScaleIndex = 0;
                mch.Sampling.PosDefIndex = Constants.DEF_XPOS_IDX;
                mch.Sampling.PosIndex = Constants.DEF_XPOS_IDX;
                mch.InitFlag = false;
                mch.IsSwitchWindow = false;
            }

            else
            {
                mch.Conditioning.SetInitScaleValue(0, cisp.Value, -20, 20, false);
                mch.Sampling.SetInitScaleValue(0, tisp.Value, -30, 30, mch.WindowId == DsoModel.Default.AnalogChnls.First().WindowId);
                if (wfmpkg != null && mch.WindowId == DsoModel.Default.AnalogChnls.First().WindowId)
                {
                    var scale = DsoPrsnt.DefaultDsoPrsnt.Timebase.GetScale(DsoPrsnt.DefaultDsoPrsnt.Timebase.ScaleIndex);
                    if (mch.Sampling.Scale != scale)
                    {
                        mch.Sampling.Scale = scale;
                    }
                    mch.Sampling.PosIndex = DsoPrsnt.DefaultDsoPrsnt.Timebase.PosIndexBymDiv;
                    tmbposition = wfmpkg.Properties.TmbPosition;
                    vustartindex = wfmpkg.Properties.VuStartIndex;
                }
                else
                {
                    var index = (TriggerPrsnt.State == SysState.Auto || TriggerPrsnt.State == SysState.Triged) ? wfmpkg.Properties.TmbPosition.Index : Constants.DEF_XPOS_IDX;
                    tmbposition = (index, mch.Sampling.GetPosition(index));
                    var tmb = mch.Sampling.GetPosition(Constants.DEF_XPOS_IDX);
                    vustartindex = (-tmb) / mch.Sampling.Scale * Constants.IDX_PER_XDIV;
                }
            }

            var prop = new WfmProperties(mch.Name)
            {
                ChnlPosition = (mch.Conditioning.PosIndex, mch.Conditioning.Position),
                ChnlScale = (mch.Conditioning.ScaleIndex, mch.Conditioning.Scale),
                ChnlUnit = (mch.Conditioning.Prefix, mch.Conditioning.Unit),

                TmbPosition = tmbposition,
                TmbScale = mch.Sampling.InitialScale,
                TmbUnit = (mch.Sampling.Prefix, mch.Sampling.Unit),
                VuStartIndex = vustartindex,
            };
            if (mch.Args is MathUserProgramArg userp && userp != null)
            {
                if (DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(userp.Source, out var p) && p?.Pack?.Properties != null)
                {
                    prop.SampInterval = p.Pack.Properties.SampInterval;
                }
            }
            return prop;
        }
        #endregion


        public Boolean SaveDataToFile(String fileName, String data)
        {
            try
            {
                StreamWriter sw = new(fileName);
                sw.Write(data);
                sw.Flush();
                sw.Close();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public Boolean ReadDataFromFile(String fileName, out String code)
        {
            try
            {
                if (!File.Exists(fileName))
                {
                    code = CurDefaultCode;
                    return false;
                }

                StreamReader sr = new(fileName);
                code = sr.ReadToEnd();
                sr.Close();
            }
            catch (Exception)
            {
                code = CurDefaultCode;
                return false;
            }
            return true;
        }
    }
}

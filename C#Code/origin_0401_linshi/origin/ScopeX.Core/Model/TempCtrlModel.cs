using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;
using ScopeX.Core.Tools;

namespace ScopeX.Core
{
    internal class TempCtrlModel : INotifyPropertyChanged
    {
        internal TempCtrlModel()
        {
            _Sw = Stopwatch.StartNew();
            _Sw.Start();
        }

        private Stopwatch _Sw;

        private Boolean _AutoCtrlFans = true;
        internal Boolean AutoCtrlFans
        { 
            get => _AutoCtrlFans;
            set
            {
                if (_AutoCtrlFans != value)
                {
                    _AutoCtrlFans = value;
                    OnPropertyChanged();
                }
            }
        }

        private Boolean _AutoCaliSystem = true;
        public Boolean AutoCaliSystem
        {
            get => _AutoCaliSystem;
            set
            {
                if (_AutoCaliSystem != value)
                {
                    _AutoCaliSystem = value;
                    OnPropertyChanged();
                }
            }
        }

        private const Int32 _DefaultFanSpeed = 30;
        private String[] _FansName = new String[0];
        private Dictionary<String, Int32> _FansSpeed = new();

        internal String[] FansName => _FansName;
        internal void InitFansName(IEnumerable<String> fansName)
        { 
            foreach (String name in fansName)
            {
                if (!_FansSpeed.ContainsKey(name))
                {
                    _FansSpeed.Add(name, _DefaultFanSpeed);
                }
            }
            _FansName = _FansSpeed.Keys.ToArray();
        }

        private Int32 _CurFanNameId = 0;
        internal Int32 CurFanNameId
        {
            get => _CurFanNameId;
            set
            {
                if (_CurFanNameId != value && _CurFanNameId < _FansName.Length)
                {
                    _CurFanNameId = value;
                    OnPropertyChanged();
                }
            }
        }

        internal Int32 SpeedMax = 100;
        internal Int32 SpeedMin = 25;

        internal Int32 CurFanSpeed
        {
            get
            {
                if (_CurFanNameId < FansName.Length)
                    return GetFanSpeed(FansName[_CurFanNameId]);
                return _DefaultFanSpeed;
            }
            set
            {
                if (value > SpeedMax)
                    value = SpeedMax;
                if (value < SpeedMin)
                    value = SpeedMin;
                if (_CurFanNameId < FansName.Length)
                {
                    String fanname = FansName[_CurFanNameId];
                    if (_FansSpeed.ContainsKey(fanname) && _FansSpeed[fanname] == value)
                        return;

                    _FansSpeed[fanname] = value;
                    OnPropertyChanged();
                }
            }
        }

        internal Int32 GetFanSpeed(String fansName) => _FansSpeed.ContainsKey(fansName) ? _FansSpeed[fansName] : 0;

        private ConcurrentDictionary<String, Double> _Temp = new();
        internal Dictionary<String, Double> Temp => _Temp.ToDictionary(o => o.Key, o => o.Value);
        internal void UpdateTemp(Dictionary<String, Double> tempInfo)
        {
            foreach (String tempname in tempInfo.Keys)
            {
                _Temp[tempname] = tempInfo[tempname];
            }

            if (_AutoCtrlFans)
            {
                //AutoCtrlFansSpeed();
            }

            //if (_Sw.IsRunning && _Sw.ElapsedMilliseconds > 30 * 60_000)
            //{
            //    AutoCaliSystem = false;
            //    _Sw.Stop();
            //    WeakTip.Default.Write("AutoCaliAtInit", $"开机已超过半小时，自动校准功能关闭", emergent: false, "", 15);
            //}
        }

        private void AutoCtrlFansSpeed()
        {
            Double tempmax = _Temp.Values.Max();
            Double basespeed = -115;
            Double ratio = 3.0;
            Double theoryspeed = basespeed + ratio * tempmax;

            Int32 speed = theoryspeed < SpeedMin ? SpeedMin : (theoryspeed > SpeedMax ? SpeedMax : (Int32)theoryspeed);
            

            foreach (String fansname in _FansSpeed.Keys)
            {
                if (Math.Abs(_FansSpeed[fansname] - speed) > 5)
                {
                    _FansSpeed[fansname] = speed;
                    Hardware.HdCmdFactory.Push(HdCmd.SystemCtrl);//hyppp
                    //Trace.WriteLine($"[AutoCtrlFansSpeed]tempmax = {tempmax},speed = {speed}");
                }
            }
        }

        private Double _CaliTempThreshold = 4.0;
        private Dictionary<String, Double> _CaliTemp = new();
        internal void UpdateCaliTemp()
        {
            _CaliTemp = Temp;
        }

        internal Boolean GetNeedCali()
        {
            foreach (String tempname in _CaliTemp.Keys)
            { 
                if (_Temp.ContainsKey(tempname) && Math.Abs(_Temp[tempname] - _CaliTemp[tempname]) > _CaliTempThreshold)
                    return true;
            }
            return false;
        }

        #region 接口实现
        protected PropertyChangedEventHandler? _PropertyChanged;

        public event PropertyChangedEventHandler? PropertyChanged
        {
            add
            {
                _PropertyChanged = (PropertyChangedEventHandler?)Delegate.Combine(Delegate.Remove(_PropertyChanged, value), value);
            }
            remove
            {
                _PropertyChanged = (PropertyChangedEventHandler?)Delegate.Remove(_PropertyChanged, value);
            }
        }

        protected void OnPropertyChanged([CallerMemberName] String propertyName = "")
        {
            _PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}

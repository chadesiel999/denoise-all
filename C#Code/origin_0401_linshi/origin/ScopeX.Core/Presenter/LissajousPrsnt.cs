using ScopeX.ComModel;
using ScopeX.Core.Tools;
using ScopeX.MathExt;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Channels;

namespace ScopeX.Core
{
    public class LissajousPrsnt : MulticastPrsnt<IXYView>, IXYPrsnt
    {
        private protected override LissajousModel Model
        {
            get;
        }

        public readonly CursorBarPrsnt HCursor;

        public readonly CursorBarPrsnt VCursor;

        private LissajousPrsnt(IDsoPrsnt idp, IXYView? view) : base(idp)
        {
            Model = new LissajousModel();

            Model.PropertyChanged += OnPropertyChanged;

            HCursor = new CursorBarPrsnt(Model.HCursor, null);
            VCursor = new CursorBarPrsnt(Model.VCursor, null);
            if (view is not null)
            {
                view.Presenter = this;

                TryAddView(view);
            }
        }

        public const Int32 MAX_LISSA_CNT = 4;

        private static readonly Object _Locker = new();

        private static Int32 _LissaLength = 0;
        public static Int32 LissaLength { get => _LissaLength; }
        public static IEnumerable<IXYPrsnt> LissajousPrsnts
        {
            get
            {
                foreach (var (_, cm) in _PrsntMap)
                {
                    yield return cm;
                }
            }
        }
        public static Int32 GetLissajousCount()
        {
            return _PrsntMap.Count();
        }
        public static Boolean GetorMakeLissajousPrsnt(Int32 ID, out LissajousPrsnt? prsnt)
        {
            Boolean res = false;
            prsnt = null;
            if (!Constants.ENABLE_Lissajous)
            {
                WeakTip.Default.Write("Lissajous", MsgTipId.FunctionDisabled);
                return res;
            }
            if (_PrsntMap.ContainsKey(ID))
            {
                prsnt = (LissajousPrsnt)_PrsntMap[ID];
                return true;
            }

            if (_LissaLength < MAX_LISSA_CNT)
            {
                prsnt = new LissajousPrsnt(DsoPrsnt.DefaultDsoPrsnt, null);
                prsnt.SourceX = ChannelId.C1;
                prsnt.SourceY = ChannelId.C2;
                prsnt.ID = ID;
                _PrsntMap.TryAdd(prsnt.ID, prsnt);
                _LissaLength++;
                res = true;
            }
            return res;
        }
        public static Boolean TryMake(IDsoPrsnt idp, IXYView? view, ChannelId x, ChannelId y, out LissajousPrsnt? prsnt)
        {
            Boolean res = false;
            prsnt = null;
            if (!Constants.ENABLE_Lissajous)
            {
                WeakTip.Default.Write("Lissajous", MsgTipId.FunctionDisabled);
                return res;
            }
            lock (_Locker)
            {
                if (FunctionLimit.LissajousFunctionLimit(DsoPrsnt.DefaultDsoPrsnt?.MutexFunctionFlag ?? false) == false)
                {
                    return false;
                }

                var p = _PrsntMap.FirstOrDefault(p => ((LissajousPrsnt)p.Value).Active == false);
                if (p.Value != null)
                {
                    prsnt = (LissajousPrsnt)p.Value;
                    prsnt.Model.SourceX = x;
                    prsnt.Model.SourceY = y;
                    return true;
                }

                if (_LissaLength < MAX_LISSA_CNT)
                {
                    prsnt = new LissajousPrsnt(idp, view);
                    prsnt.Model.SourceX = x;
                    prsnt.Model.SourceY = y;
                    prsnt.ID = GenerateID();
                    if (!_PrsntMap.ContainsKey(prsnt.ID))
                    {
                        _PrsntMap.TryAdd(prsnt.ID, prsnt);
                    }
                    _LissaLength++;

                    res = true;
                }
            }
            if (!res)
            {
                WeakTip.Default.Write("XY", MsgTipId.NoMoreChannels);
            }
            return res;
        }

        private static Int32 GenerateID()
        {
            Int32 id = 1;
            if (_PrsntMap.Count == 0)
            {
                return 1;
            }
            for (var index = id; index <= MAX_LISSA_CNT; index++)
            {
                if (!_PrsntMap.ContainsKey(index))
                {
                    id = index;
                    break;
                }
            }
            return id;
        }

        public static Boolean TryGetXYPrsnt(Int32 ID, out IXYPrsnt? prsnt)
        {
            return _PrsntMap.TryGetValue(ID, out prsnt);
        }

        private static ConcurrentDictionary<Int32, IXYPrsnt> _PrsntMap = new ConcurrentDictionary<Int32, IXYPrsnt>();
        public static void TryTryRemoveAll()
        {
            foreach (var item in _PrsntMap)
            {
                if (item.Value is LissajousPrsnt lissajous)
                {
                    lissajous.Active = false;
                    //lissajous.TryRemove();
                }
            }
        }
        public Boolean TryRemove()
        {
            Boolean res = false;
            lock (_Locker)
            {
                Active = false;
                _PrsntMap.TryRemove(this.ID, out var prsnt);
                if (_LissaLength <= MAX_LISSA_CNT)
                {
                    //_LissaLength--;

                    foreach (var v in GetViewList())
                    {
                        TryRemoveView(v);
                    }
                    Dispose();

                    res = true;
                }
            }

            return res;
        }

        public ChannelId SourceX
        {
            get => Model.SourceX;
            set => Model.SourceX = value;
        }

        public ChannelId SourceY
        {
            get => Model.SourceY;
            set => Model.SourceY = value;
        }

        public Boolean Active
        {
            get => Model.Active;
            set
            {
                if (value && !Constants.ENABLE_Lissajous)
                {
                    WeakTip.Default.Write("Lissajous", MsgTipId.FunctionDisabled);
                    Model.Active = false;
                    return;
                }
                Model.Active = value;
                if (!Model.Active && _LissaLength <= MAX_LISSA_CNT)
                    _LissaLength--;
            }
        }

        public Boolean CursorActive
        {
            get => Model.CursorActive;
            set
            {
                Model.CursorActive = value;
            }
        }
        public LissajousCursorType CursorType
        {
            get => Model.CursorType;
            set
            {
                Model.CursorType = value;
                if (Model.CursorType == LissajousCursorType.Wave && CursorActive)
                {
                    KeyLed.Default.SetLed(LedEnum.LedCursor, true);
                    KeyLed.Default.SetLed(LedEnum.LedMultipupose, true);
                }
            }
        }

        public LissajousDataType DataType
        {
            get => Model.DataType;
            set => Model.DataType = value;
        }

        public Int32 ID
        {
            get => Model.ID;
            private set => Model.ID = value;
        }

        public Int64? WindowId
        {
            get => Model.WindowId;
            set => Model.WindowId = value;
        }

        /// <summary>
        /// 单位 (V,°)
        /// </summary>
        public (Double Radius, Double Angle) PosA
        {
            get => Model.PosA;
            set => Model.PosA = value;
        }

        /// <summary>
        /// 单位 (V,°)
        /// </summary>
        public (Double Radius, Double Angle) PosB
        {
            get => Model.PosB;
            set => Model.PosB = value;
        }

        public (Double? value, Prefix pfx, String Unit) GetVValueAxisInfo(Double pos, ChannelId source)
        {
            if (DsoModel.Default.TryGetChannel(source, out var chn))
            {
                var scale = chn.Conditioning.Scale / chn.Conditioning.PosIdxPerDiv;
                var offset = chn.Conditioning.PosIndex;
                var value = (pos - chn.Conditioning.PosIdxPerDiv * 5 - offset) * scale;
                return (value, chn.Conditioning.Prefix, chn.Conditioning.Unit);
            }
            return (0, Prefix.Empty, "?");
        }

        public (Double? value, Prefix pfx, String Unit) GetHValueAxisInfo(Double pos, ChannelId source)
        {
            if (DsoModel.Default.TryGetChannel(source, out var chn))
            {
                var scale = chn.Conditioning.Scale / chn.Conditioning.PosIdxPerDiv;
                var offset = chn.Conditioning.PosIndex;
                var value = (pos - offset) * scale;
                return (value, chn.Conditioning.Prefix, chn.Conditioning.Unit);
            }
            return (0, Prefix.Empty, "?");
        }

    }
}

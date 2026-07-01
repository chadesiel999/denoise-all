using EventBus;
using ScopeX.ComModel;
using ScopeX.Core;
using ScopeX.Touch;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace ScopeX.U2
{
    internal class KeyboardLed
    {
        private List<IKeyData> _SendBuffer = new List<IKeyData>();
        private Int32 _LastRunState = -1;
        public Boolean EnbleWrite
        {
            get => _EnbleWrite;
            set
            {
                if (_EnbleWrite != value)
                {
                    _EnbleWrite = value;
#if !UPO7000L
                    if (value && _SendBuffer.Count > 0)
                    {
                        //缓存中大概率回出现多个设置运行状态的数据包，只保留最后一个，减小数据包大小
                        var states = _SendBuffer.OfType<ScopeStateData>().Where(x => x.ScopeState == ScopeStateType.RunStop).ToList();
                        if (states.Count >= 2)
                        {
                            for (Int32 index = 0; index < states.Count - 1; index++)
                            {
                                _SendBuffer.Remove(states[index]);
                            }
                        }
                        SendData(_SendBuffer);
                        _SendBuffer.Clear();
                    }
                    else
                    {
                        _SendBuffer.Clear();
                    }
#endif
                }
            }
        }
        private ComPort _ComPort;
        private Dictionary<ComModel.ChannelId, KeyboardChannel> _KeyboardChannels = new Dictionary<ComModel.ChannelId, KeyboardChannel>()
        {
            {ComModel.ChannelIdExt.MinAChId,  new KeyboardChannel(0,8) },
            {ComModel.ChannelIdExt.MinMChId,new KeyboardChannel(10,8) },
            {ComModel.ChannelIdExt.MinRChId,new KeyboardChannel(20,8) },
            {ComModel.ChannelIdExt.MinBChId,new KeyboardChannel(30,2) },
            {ComModel.ChannelIdExt.MinDChId,new KeyboardChannel(40,64) },
            {ComModel.ChannelIdExt.MinPChId,new KeyboardChannel(110,8) },
            {ComModel.ChannelIdExt.MinAwgId,new KeyboardChannel(120,4) },
            {ComModel.ChannelId.DVM,new KeyboardChannel(124,1) },
            {ComModel.ChannelId.Ext,new KeyboardChannel(125,1) },
            {ComModel.ChannelId.Ext5,new KeyboardChannel(126,1) },
            {ComModel.ChannelId.AC,new KeyboardChannel(127,1) },
            {ComModel.ChannelId.USER,new KeyboardChannel(130,1) },
        };
        private Boolean _EnbleWrite = true;

        private KeyboardLed()
        {
            EventBus.EventBroker.Instance.GetEvent(ComModel.KeyboardConstants.EventName).Subscrip((sender, args) =>
            {
                if (args.Data.Length == 0)
                {
                    return;
                }
                if (args.Data[0] is ComModel.LedControlCommand command)
                {
                    switch (command)
                    {
                        case ComModel.LedControlCommand.LedStatesControl:
                            LedStatesControl(args.Data[1] as Boolean[]);
                            break;
                        case ComModel.LedControlCommand.LedColorControl:
                            LedColorControl((LedEnum)args.Data[1], (Color)args.Data[2]);
                            break;
                        case ComModel.LedControlCommand.LedStateControl:
                            LedStateControl((LedEnum)args.Data[1], (Boolean)args.Data[2]);
                            break;
                        case ComModel.LedControlCommand.ScopeStateControl:
                            ScopeStateControl((ScopeStateType)args.Data[1], (Int32)args.Data[2]);
                            break;
                        case ComModel.LedControlCommand.ScopeOtherControl:
                            ScopeOtherControl((ControlChannelType)args.Data[1], (Boolean[])args.Data[2]);
                            break;
                        case ComModel.LedControlCommand.ScopeAnalogControl:
                            ScopeAnalogControl((ChannelId)args.Data[1], (Boolean)args.Data[2], (AnaChnlCoupling)args.Data[3], (Int32)args.Data[4], (Boolean)args.Data[5]);
                            break;
                    }
                }
            });
        }
        public async void Init(ComPort comPort)
        {
            _ComPort = comPort;
            ScopeStateControl(ScopeStateType.Close, Convert.ToInt32(true));
            LedColorControl(LedEnum.LedMultipupose, Color.LightGray);
            await System.Threading.Tasks.Task.Delay(1000);
            if (Program.Oscilloscope != null)
            {
                List<IKeyData> datas = new List<IKeyData>();
                datas.Add(new LedStateData(LedEnum.LedAWG, Program.Oscilloscope.ArbWfmGens.Any(x => x.Active)));
                datas.Add(new LedStateData(LedEnum.LedDVM, Program.Oscilloscope.Voltmeter.Active));

                datas.Add(new OtherChannelStateData(ControlChannelType.Bus, Program.Oscilloscope.TryGetRange(c => c.Id.IsDecode())
                    .OrderBy(x => x.Id)
                    .Select(x => x.Active)));


                datas.Add(new OtherChannelStateData(ControlChannelType.Digital, Program.Oscilloscope.TryGetRange(c => c.Id.IsDigital())
                    .OrderBy(x => x.Id)
                    .Select(x => x.Active)));

                datas.Add(new OtherChannelStateData(ControlChannelType.Math, Program.Oscilloscope.TryGetRange(c => c.Id.IsMath())
                    .OrderBy(x => x.Id)
                    .Select(x => x.Active)));


                datas.Add(new OtherChannelStateData(ControlChannelType.Ref, Program.Oscilloscope.TryGetRange(c => c.Id.IsReference())
                    .OrderBy(x => x.Id)
                    .Select(x => x.Active)));

                datas.AddRange(Program.Oscilloscope.TryGetRange(c => c.Id.IsAnalog())
                    .OrderBy(x => x.Id)
                    .Select(x => new AnalogChannelConfigData(ChangeToKeyboardChannel(x.Id), x.Active, ((AnalogPrsnt)x).Coupling, ((AnalogPrsnt)x).Bandwidth)));

                datas.Add(new ScopeStateData()
                {
                    ScopeState = ScopeStateType.RunStop,
                    //State = (Byte)_DsoPrsnt.State,
                    State = (Byte)TriggerPrsnt.State,
                });

                datas.Add(new ScopeStateData()
                {
                    ScopeState = ScopeStateType.SelectedChannel,
                    State = (Byte)Core.DsoPrsnt.FocusId
                });
                datas.Add(new ScopeStateData()
                {
                    ScopeState = ScopeStateType.TriggerModel,
                    State = (Byte)Core.TriggerPrsnt.Mode,
                });
                datas.Add(new ScopeStateData()
                {
                    ScopeState = ScopeStateType.Print,
                    State = 1,
                });
                datas.Add(new ScopeStateData()
                {
                    ScopeState = ScopeStateType.TouchSceen,
                    State = 1,
                });
                datas.Add(new ScopeStateData()
                {
                    ScopeState = ScopeStateType.UltraAcq,
                    State = (Byte)Program.Oscilloscope.Timebase.StorageMode,
                });

                PlatformUIManager.Default.Platform.KeyboardInit(datas);

                datas.Add(new LedStateData()
                {
                    Led = LedEnum.LedCursor,
                    State = Program.Oscilloscope.Cursor.Active
                });

                datas.Add(new LedStateData()
                {
                    Led = LedEnum.LedMultipupose,
                    State = (Program.Oscilloscope.Cursor.HCursor.SelectedIndex != -1 ||
                            Program.Oscilloscope.Cursor.VCursor.SelectedIndex != -1) &&
                            Program.Oscilloscope.Cursor.Active
                });

                datas.Add(new LedStateData()
                {
                    Led = LedEnum.LedMenu,
                    State = Program.Oscilloscope.Cursor.MoveMode == CursorMoveMode.Fine
                });

                datas.Add(new LedStateData()
                {
                    Led = LedEnum.LedMeasure,
                    State = Program.Oscilloscope.Measure.Active
                });

                datas.Add(new LedStateData()
                {
                    Led = LedEnum.LedQuickMeasure,
                    State = false
                });


                void InitTriggerBtnByChannelID(ChannelId channelId)
                {
                    if (Program.Oscilloscope.TryGetChannel(channelId, out IChnlPrsnt? chnlPrsnt))
                    {
                        if (chnlPrsnt != null && chnlPrsnt.Active)
                        {
                            datas.Add(new ScopeStateData(ScopeStateType.TriggerChannel, ChangeToKeyboardChannel(channelId)));
                        }
                    }
                }
                /*if (Program.Oscilloscope.CurrentTrigger is Core.TrigEdgePrsnt edge)
                {
                    datas.Add(new ScopeStateData(ScopeStateType.TriggerChannel, ChangeToKeyboardChannel(edge.Source)));
                }
                else*/
                if (Program.Oscilloscope.CurrentTrigger is Core.TrigSingleSrcPrsnt ssp)
                {
                    InitTriggerBtnByChannelID(ssp.Source!.Value);
                }
                else if (Program.Oscilloscope.CurrentTrigger is Core.TrigRuntPrsnt trunt)
                {
                    InitTriggerBtnByChannelID(trunt.Source);
                }
                else if (Program.Oscilloscope.CurrentTrigger is Core.TrigTransPrsnt trans)
                {
                    InitTriggerBtnByChannelID(trans.Source);
                }

                try
                {
                    var touchable = TouchController.IsTouchable();
                    datas.Add(new ScopeStateData(ScopeStateType.TouchSceen, Convert.ToByte(!touchable)));
                }
                catch (Exception ex)
                {
                    EventBroker.Instance.GetEvent<LogEventArgs>().Publish(null, new LogEventArgs(ex, LogLevel.Error));
                }

                SendData(datas);
            }
            else
            {

            }
            if (DsoPrsnt.FocusId.IsAnalog())
            {
                ScopeStateControl(ScopeStateType.SelectedChannel, (Int32)DsoPrsnt.FocusId);
            }
        }
        private AsyncTaskResult InitProcess(String mark)
        {
            String keyboardserialport = Keyboard.FindKeyBoardSerialPort(out String msg);
            VersionManager.KeyboardVersion = msg;
            if (!String.IsNullOrEmpty(keyboardserialport))
            {
                EventBroker.Instance.GetEvent<LogEventArgs>().Publish(null, new LogEventArgs($"键盘板串口号为:{keyboardserialport}", LogLevel.Info));

                KeyboardLed.Default.Init(new ComPort(keyboardserialport, Keyboard.Default.Receive));

                return new AsyncTaskResult() { Success = true, Mark = mark, ErrorMsg = "" };
            }
            else
            {
                EventBroker.Instance.GetEvent<LogEventArgs>().Publish(null, new LogEventArgs("键盘板未找到", LogLevel.Error));
                ComModel.ErrorCode.ErrorType = ErrorType.S_Keyboard_NotFound_0001;
                return new AsyncTaskResult() { Success = false, Mark = mark, ErrorMsg = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("JianPanBanWeiZhaoDao") };
            }
        }
        public async Task<AsyncTaskResult> InitAsync(String mark)
        {
            return await Task.Run(() => InitProcess(mark));
        }
        private void SendData(List<IKeyData> datas)
        {
            if (!EnbleWrite)
            {
                _SendBuffer.AddRange(datas);
            }
            else
            {
                KeyBoardData keyBoard = new KeyBoardData();
                keyBoard.Buffer.AddRange(datas);
                _ComPort?.WriteBytes(keyBoard.GetBytes());
            }
        }
        private void SendData(IKeyData data)
        {
            if (!EnbleWrite)
            {
                _SendBuffer.Add(data);
            }
            else
            {
                KeyBoardData keyBoard = new KeyBoardData();
                keyBoard.Buffer.Add(data);
                _ComPort?.WriteBytes(keyBoard.GetBytes());

            }
        }
        public void ScopeAnalogControl(ComModel.ChannelId channel, Boolean state, ComModel.AnaChnlCoupling coupling, Int32 bw)
        {
            SendData(new AnalogChannelConfigData(ChangeToKeyboardChannel(channel), state, coupling, bw));
        }

        public void ScopeAnalogControl(ComModel.ChannelId channel, Boolean state, ComModel.AnaChnlCoupling coupling, Int32 bw, Boolean ScaleAdd)
        {
            SendData(new AnalogChannelConfigData(ChangeToKeyboardChannel(channel), state, coupling, bw, ScaleAdd));
        }

        public void SetTouchSceen(Boolean state) => ScopeStateControl(ScopeStateType.TouchSceen, Convert.ToInt32(state));
        public void ScopeOtherControl(ControlChannelType type, Boolean[] states)
        {
            SendData(new OtherChannelStateData(type, states));
        }
        private Byte ChangeToKeyboardChannel(ComModel.ChannelId channel)
        {
            Byte keyboardchannel = byte.MaxValue;
            if (channel >= ComModel.ChannelIdExt.MinAChId && channel - ComModel.ChannelIdExt.MinAChId < ComModel.ChannelIdExt.AnaChnlNum)
            {
                keyboardchannel = (Byte)((channel - ComModel.ChannelIdExt.MinAChId) % _KeyboardChannels[ComModel.ChannelIdExt.MinAChId].ChannelCount + _KeyboardChannels[ComModel.ChannelIdExt.MinAChId].FirstValue);
            }
            else if (channel >= ComModel.ChannelIdExt.MinMChId && channel - ComModel.ChannelIdExt.MinMChId < ComModel.ChannelIdExt.MathChnlNum)
            {
                keyboardchannel = (Byte)((channel - ComModel.ChannelIdExt.MinMChId) % _KeyboardChannels[ComModel.ChannelIdExt.MinMChId].ChannelCount + _KeyboardChannels[ComModel.ChannelIdExt.MinMChId].FirstValue);

            }
            else if (channel >= ComModel.ChannelIdExt.MinRChId && channel - ComModel.ChannelIdExt.MinRChId < ComModel.ChannelIdExt.RefChnlNum)
            {
                keyboardchannel = (Byte)((channel - ComModel.ChannelIdExt.MinRChId) % _KeyboardChannels[ComModel.ChannelIdExt.MinRChId].ChannelCount + _KeyboardChannels[ComModel.ChannelIdExt.MinRChId].FirstValue);

            }
            else if (channel >= ComModel.ChannelIdExt.MinBChId && channel - ComModel.ChannelIdExt.MinBChId < ComModel.ChannelIdExt.BusChnlNum)
            {

                keyboardchannel = (Byte)((channel - ComModel.ChannelIdExt.MinBChId) % _KeyboardChannels[ComModel.ChannelIdExt.MinBChId].ChannelCount + _KeyboardChannels[ComModel.ChannelIdExt.MinBChId].FirstValue);
            }
            else if (channel >= ComModel.ChannelIdExt.MinDChId && channel - ComModel.ChannelIdExt.MinDChId < ComModel.ChannelIdExt.DigiChnlNum)
            {

                keyboardchannel = (Byte)((channel - ComModel.ChannelIdExt.MinDChId) % _KeyboardChannels[ComModel.ChannelIdExt.MinDChId].ChannelCount + _KeyboardChannels[ComModel.ChannelIdExt.MinDChId].FirstValue);
            }
            else if (channel >= ComModel.ChannelIdExt.MinPChId && channel - ComModel.ChannelIdExt.MinPChId < ComModel.ChannelIdExt.MeasChnlNum)
            {
                keyboardchannel = (Byte)((channel - ComModel.ChannelIdExt.MinPChId) % _KeyboardChannels[ComModel.ChannelIdExt.MinPChId].ChannelCount + _KeyboardChannels[ComModel.ChannelIdExt.MinPChId].FirstValue);
            }
            else if (channel >= ComModel.ChannelIdExt.MinAwgId && channel - ComModel.ChannelIdExt.MinAwgId < ComModel.ChannelIdExt.AwgNum)
            {
                keyboardchannel = (Byte)((channel - ComModel.ChannelIdExt.MinAwgId) % _KeyboardChannels[ComModel.ChannelIdExt.MinAwgId].ChannelCount + _KeyboardChannels[ComModel.ChannelIdExt.MinAwgId].FirstValue);

            }
            else if (channel >= ComModel.ChannelId.DVM && channel - ComModel.ChannelId.DVM < ComModel.ChannelIdExt.DvmNum)
            {
                keyboardchannel = (Byte)((channel - ComModel.ChannelId.DVM) % _KeyboardChannels[ComModel.ChannelId.DVM].ChannelCount + _KeyboardChannels[ComModel.ChannelId.DVM].FirstValue);

            }
            else
            {
                if (_KeyboardChannels.TryGetValue(channel, out KeyboardChannel keyboard))
                {
                    keyboardchannel = keyboard.FirstValue;
                }
                else
                {

                }
            }

            return keyboardchannel;
        }
        public void LedStatesControl(Boolean[] states)
        {
            SendData(new LedStatesData(states));
        }
        public void LedStateControl(LedEnum led, Boolean state)
        {
            SendData(new LedStateData(led, state));
        }
        public void LedColorControl(LedEnum led, Color color)
        {
            SendData(new LedColorData(led, color));
        }
        public void ScopeStateControl(ScopeStateType type, Int32 state)
        {
            //// 键盘板那边大于104都是无效通道，不需要去真实设置
            //if (state > 104)
            //    return;

            // 以前7000X 按键板的RunStop只关心 SysState.Stop 和 非 SysState.Stop
            // 所以在此做了状态过滤
            //
            //if (type == ScopeStateType.RunStop)
            //{
            //    state = state >= 1 ? 1 : 0;
            //    if (state == _LastRunState) return;
            //    _LastRunState = state;
            //}

            Byte tempstate = 0;
            if (type == ScopeStateType.SelectedChannel || type == ScopeStateType.TriggerChannel)
            {
                tempstate = ChangeToKeyboardChannel((ChannelId)state);
                if (type == ScopeStateType.SelectedChannel)
                {
                    var ch = Program.Oscilloscope.GetAllChnls().FirstOrDefault(x => x.Id == (ChannelId)state);
                    if (ch != null && !ch.Active) return;

                    if (Core.DsoPrsnt.FocusId.IsAnalog())
                        tempstate = (Byte)Core.DsoPrsnt.FocusId;
                }
            }
            else
            {
                tempstate = (Byte)state;
            }
            SendData(new ScopeStateData(type, tempstate));
        }
        public static KeyboardLed Default { get; } = new KeyboardLed();



        internal interface IKeyData
        {
            public Byte[] GetBytes();

            public LedControlCommand Command { get; }

            public Byte Length { get; }
        }

        internal class KeyBoardData
        {
            public List<IKeyData> Buffer { get; } = new List<IKeyData>();

            public Byte[] GetBytes()
            {
                if (Buffer == null || Buffer.Count == 0)
                {
                    return Array.Empty<Byte>();
                }

                System.IO.MemoryStream stream = new();
                stream.Write(BitConverter.GetBytes(Keyboard.PACKET_HEADER).Reverse().ToArray());//注意高低字节
                stream.WriteByte(0);//占位数据长度，在未完成数据转换前不能知道数据具体长度

                //根据协议，将相同类型的控制进行合并，压缩数据量大小
                Enum.GetValues<LedControlCommand>().OrderBy(x => x).ToList().ForEach(x =>
                  {
                      List<IKeyData> keydatas = Buffer.Where(y => y.Command == x).ToList();

                      if (keydatas.Count > 0)
                      {
                          stream.WriteByte((Byte)x);
                          stream.WriteByte((Byte)(keydatas.Count * keydatas.First().Length));
                          keydatas.ForEach(y => stream.Write(y.GetBytes()));
                      }
                      else
                      {

                      }
                  });


                stream.Write(BitConverter.GetBytes(Keyboard.PACKET_ENDER).Reverse().ToArray());//注意高低字节
                stream.Seek(sizeof(UInt16), System.IO.SeekOrigin.Begin);//重新定位数据包长度位置
                stream.WriteByte((Byte)(stream.Length - sizeof(UInt16) * 2 - sizeof(Byte)));//写入数据包长度,数据包长度应为：数据长度-包头长度-包尾长度-自身长度

                Byte[] tempbytes = stream.ToArray();
                stream.Dispose();
                return tempbytes;

            }
        }

        internal class LedColorData : IKeyData
        {
            public LedColorData(LedEnum led, System.Drawing.Color color)
            {
                Led = led;
                Red = color.R;
                Green = color.G;
                Blue = color.B;
            }

            public LedEnum Led { get; set; }

            public Byte Red { get; set; }

            public Byte Green { get; set; }

            public Byte Blue { get; set; }

            public LedControlCommand Command => LedControlCommand.LedColorControl;

            public Byte Length => 4;

            public Byte[] GetBytes() => new Byte[4] { (Byte)Led, Red, Green, Blue };
        }

        internal class LedStatesData : IKeyData
        {
            public LedStatesData(Boolean[] states)
            {
                States = states;
            }
            public Boolean[] States { get; set; }

            public LedControlCommand Command => LedControlCommand.LedStatesControl;

            public Byte Length => 8;

            public Byte[] GetBytes()
            {
                Byte[] tempbytes = new Byte[Length];

                if (States != null && States.Length > 0)
                {
                    for (Int32 index = 0; index < States.Length; index++)
                    {

                        if (States[index])
                        {
                            tempbytes[7 - (Int32)Math.Floor(index / 8f)] |= (Byte)(0x01 << (index % 8));//高字节在前
                        }
                    }
                }

                return tempbytes;
            }
        }

        internal class LedStateData : IKeyData
        {
            public LedStateData()
            { }

            public LedStateData(LedEnum led, Boolean state) : this()
            {
                Led = led;
                State = state;
            }

            public LedControlCommand Command => LedControlCommand.LedStateControl;

            public Byte Length => 2;

            public LedEnum Led { get; set; }

            public Boolean State { get; set; }

            public Byte[] GetBytes() => new Byte[2] { (Byte)Led, Convert.ToByte(State) };
        }

        internal class ScopeStateData : IKeyData
        {
            public ScopeStateData(ScopeStateType scopeState, Byte state)
            {
                ScopeState = scopeState;
                State = state;
            }
            public ScopeStateData()
            {

            }

            public ScopeStateType ScopeState { get; set; }
            public Byte State { get; set; }
            public LedControlCommand Command => LedControlCommand.ScopeStateControl;

            public Byte Length => 2;

            public Byte[] GetBytes() => new Byte[2] { (Byte)ScopeState, State };
        }

        internal class AnalogChannelConfigData : IKeyData
        {
            public AnalogChannelConfigData(Byte channel, Boolean state, ComModel.AnaChnlCoupling coupling, Int32 bw)
            {
                Channel = channel;
                switch (coupling)
                {
                    case ComModel.AnaChnlCoupling.AC1M:
                        StateAC = true;
                        break;
                    case ComModel.AnaChnlCoupling.DC1M:
                        StateAC = false;
                        break;
                    case ComModel.AnaChnlCoupling.DC50:
                        State50 = true;
                        break;
                    case ComModel.AnaChnlCoupling.Gnd:
                        StateAC = false;
                        State50 = false;
                        break;
                }
                StateBW = bw != 0;
                State = state;
                ScaleAdd = false;
            }
            public AnalogChannelConfigData(Byte channel, Boolean state, ComModel.AnaChnlCoupling coupling, Int32 bw, Boolean scaleAdd)
            {
                Channel = channel;
                switch (coupling)
                {
                    case ComModel.AnaChnlCoupling.AC1M:
                        StateAC = true;
                        break;
                    case ComModel.AnaChnlCoupling.DC1M:
                        StateAC = false;
                        break;
                    case ComModel.AnaChnlCoupling.DC50:
                        State50 = true;
                        break;
                    case ComModel.AnaChnlCoupling.Gnd:
                        StateAC = false;
                        State50 = false;
                        break;
                }
                StateBW = bw != 0;
                State = state;
                ScaleAdd = scaleAdd;
            }

            public Byte Channel { get; private set; }
            public Boolean State { get; private set; }
            public Boolean StateAC { get; private set; }
            public Boolean State50 { get; private set; }
            public Boolean StateBW { get; private set; }

            /// <summary>
            /// 幅度细调
            /// </summary>
            public Boolean ScaleAdd { get; set; }

            public LedControlCommand Command => LedControlCommand.ScopeAnalogControl;

            public Byte Length => 2;

            public Byte[] GetBytes()
            {
                Byte tempstates = 0;
                tempstates |= (Byte)((ScaleAdd ? 0b01 : 0) << 4);//幅度细调
                tempstates |= (Byte)((State ? 0b01 : 0) << 3);
                tempstates |= (Byte)((StateAC ? 0b01 : 0) << 2);
                tempstates |= (Byte)((State50 ? 0b01 : 0) << 1);
                tempstates |= (Byte)((StateBW ? 0b01 : 0));

                return new Byte[2] { Channel, tempstates };
            }
        }

        internal class OtherChannelStateData : IKeyData
        {
            public OtherChannelStateData(ControlChannelType channelType, IEnumerable<Boolean> state)
            {
                ChannelType = channelType;
                Int32 index = 0;
                ChannelStates = new Byte[8];
                state.ToList().ForEach(x =>
                {
                    ChannelStates[index / 8] |= (Byte)((x ? 0b01 : 0b00) << (index % 8));
                    index++;
                });
            }
            public ControlChannelType ChannelType { get; private set; }
            public Byte[] ChannelStates { get; private set; }

            public LedControlCommand Command => LedControlCommand.ScopeOtherControl;

            public Byte Length => 9;

            public Byte[] GetBytes()
            {
                List<Byte> temp = new List<Byte>
                {
                    (Byte)ChannelType
                };
                temp.AddRange(ChannelStates);
                return temp.ToArray();
            }
        }

        private struct KeyboardChannel
        {
            public Byte FirstValue { get; }
            public Byte ChannelCount { get; }
            public KeyboardChannel(Byte firstValue, Byte channelCount)
            {
                FirstValue = firstValue;
                ChannelCount = channelCount;
            }
        }
    }
}

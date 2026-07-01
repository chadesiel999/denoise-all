using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Hardware.Calibration.Data.Base
{
    public class DbiLocalOscillators : ICaliData
    {
        public static DbiLocalOscillators Default = new DbiLocalOscillators();
        private DbiLocalOscillatorsItem[] defaultValue = new DbiLocalOscillatorsItem[]
        {
            new DbiLocalOscillatorsItem(){ CmdIndex='2',CtrlWord=new char[]{'0','1','0','5'} },
            new DbiLocalOscillatorsItem(){ CmdIndex='3',CtrlWord=new char[]{'0','0','9','5'} },
            new DbiLocalOscillatorsItem(){ CmdIndex='4',CtrlWord=new char[]{'0','1','4','5'} },
            new DbiLocalOscillatorsItem(){ CmdIndex='5',CtrlWord=new char[]{'0','3','1','5'} },
            new DbiLocalOscillatorsItem(){ CmdIndex='6',CtrlWord=new char[]{'0','0','8','0'} },
            new DbiLocalOscillatorsItem(){ CmdIndex='7',CtrlWord=new char[]{'0','0','0','0'} },
            new DbiLocalOscillatorsItem(){ CmdIndex='8',CtrlWord=new char[]{'0','0','0','0'} },
            new DbiLocalOscillatorsItem(){ CmdIndex='9',CtrlWord=new char[]{'0','0','0','0'} },
        };
        private DbiLocalOscillators()
        {
            for (int channelIndex = 0; channelIndex < 4; channelIndex++)
            {
                for (int cmdIndex = 0; cmdIndex < 8; cmdIndex++)
                {
                    data[channelIndex, cmdIndex] = defaultValue[cmdIndex];
                }
            }
        }
        private DbiLocalOscillatorsItem[,] data = new DbiLocalOscillatorsItem[4, 8];
        public CaliDataType DataType => CaliDataType.DbiLocalOscillators;
        public DbiLocalOscillatorsItem this[int channelIndex,int cmdIndex]
        {
            get => data[channelIndex, cmdIndex];
        }
        public int TotalBytes => 4 * 8 * 5;//4=4channel,8=8个cmdIndex,5=每个cmdIndex占5个字节
        public Int32 OriginTotleBytes
        {
            get => TotalBytes;
            set { }
        }
        public void LoadDefaultValue()
        {
            for (int channelIndex = 0; channelIndex < 4; channelIndex++)
            {
                for (int cmdIndex = 0; cmdIndex < 8; cmdIndex++)
                {
                    data[channelIndex, cmdIndex] = defaultValue[cmdIndex];
                }
            }
        }
        public void Deserialize(byte[] content)
        {
            if (content.Length < TotalBytes)
                return;
            int perChannelBytes = 8 * 5;//8=8个cmdIndex,5=每个cmdIndex占5个字节
            int perCmdIndexBytes = 5;
            for (int channelID = 0; channelID < 4; channelID++)
            {
                for (int cmdIndex = 0; cmdIndex < 8; cmdIndex++)
                {
                    data[channelID, cmdIndex] = new DbiLocalOscillatorsItem() { CmdIndex = (char)content[perChannelBytes * channelID + cmdIndex * perCmdIndexBytes], CtrlWord = Encoding.ASCII.GetChars(content, perChannelBytes * channelID + cmdIndex * perCmdIndexBytes + 1, 4) };
                }
            }
        }

        public byte[] Serialize()
        {
            System.IO.MemoryStream memoryStream = new MemoryStream();
            for (int channelIndex = 0; channelIndex < 4; channelIndex++)
            {
                for (int cmdIndex = 0; cmdIndex < 8; cmdIndex++)
                {
                    memoryStream.Write(data[channelIndex,cmdIndex].Serialize());
                }
            }
            byte[] result = memoryStream.ToArray();
            memoryStream.Close();
            return result;
        }
    }
    public class DbiLocalOscillatorsItem
    {
        public char CmdIndex
        {
            get;
            set;
        }
        public char[] CtrlWord = new char[4];
        internal byte[] Serialize()
        {
            byte[] data = new byte[5];
            data[0] = (byte)CmdIndex;
            data[1] = (byte)CtrlWord[0];
            data[2] = (byte)CtrlWord[1];
            data[3] = (byte)CtrlWord[2];
            data[4] = (byte)CtrlWord[3];
            return data;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using ScopeX.ComModel;

namespace ScopeX.U2
{
    /// <summary>
    /// 用户使用keycode定义
    /// 本结构兼容了按键和旋钮
    /// </summary>
    public class KeyCode
    {
        //Reserved
        public const Int32 MENU = 0x0d;
        
        public const Int32 LEFTMENU_F1 = 0x01;
        public const Int32 LEFTMENU_F2 = 0x02;
        public const Int32 LEFTMENU_F3 = 0x03;
        public const Int32 LEFTMENU_F4 = 0x04;
        public const Int32 LEFTMENU_F5 = 0x05;
        public const Int32 LEFTMENU_F6 = 0x06;

        public const Int32 KEYUP = 0xff;

        //Basic
        public const Int32 KNOB_UPMULTI_LEFT = 0x63;      
        public const Int32 KNOB_UPMULTI_RIGHT = 0x62;     
        public const Int32 KNOB_UPMULTI_SELECT = 0x3f;    

        public const Int32 KNOB_DNMULTI_LEFT = 0x65;      
        public const Int32 KNOB_DNMULTI_RIGHT = 0x64;     
        public const Int32 KNOB_DNMULTI_SELECT = 0x31;    

        public const Int32 KNOB_MULTI_LEFT = 0x6a;
        public const Int32 KNOB_MULTI_RIGHT = 0x6b;
        public const Int32 KNOB_MULTI_SELECT = 0x6c;

        public const Int32 CH1 = 0x11;       
        public const Int32 CH2 = 0x12;       
        public const Int32 CH3 = 0x13;       
        public const Int32 CH4 = 0x14;
        public const Int32 CH5 = 0x15;
        public const Int32 CH6 = 0x16;
        public const Int32 CH7 = 0x17;
        public const Int32 CH8 = 0x18;
        public const Int32 REF = 0x19;       
        public const Int32 MATH = 0x1a;      
        public const Int32 LOGIC = 0x1b;
        public const Int32 DECODE = 0x1c;

        public const Int32 TIMEBASE = 0x20;
        public const Int32 TRIGGER = 0x21;
        public const Int32 TRIG_FORCE = 0x22;

        public const Int32 MEASURE = 0x23;
        public const Int32 ACQUIRE = 0x24;
        public const Int32 STORAGE = 0x25;
        public const Int32 CURSOR = 0x26;
        public const Int32 DISPLAY = 0x27;
        public const Int32 SETTING = 0x28;
        public const Int32 USERCUSTOM = 0x28A;
        public const Int32 HELP = 0x29;

        public const Int32 AUTOSET = 0x2a;
        public const Int32 RUNSTOP = 0x2b;
        public const Int32 SINGLE = 0x2c;
        public const Int32 CLEAR = 0x2d;
        public const Int32 PRINT = 0x2e;
        public const Int32 DEFAULT = 0x2f;
        public const Int32 LAYEROFF = 0x30;
        public const Int32 AUTOCALIBRATION = 0x21a;
        public const Int32 SAVEDATASOURCE = 0x22a;


        public const Int32 KNOB_CH1YPOS_LEFT = 0x66;    
        public const Int32 KNOB_CH1YPOS_RIGHT = 0x67;   
        public const Int32 KNOB_CH1YPOS_SELECT = 0x34;  

        public const Int32 KNOB_CH1YLEVEL_LEFT = 0x68;  
        public const Int32 KNOB_CH1YLEVEL_RIGHT = 0x69; 
        public const Int32 KNOB_CH1YLEVEL_SELECT = 0x39;

        public const Int32 KNOB_CH2YPOS_LEFT = 0x74;    
        public const Int32 KNOB_CH2YPOS_RIGHT = 0x75;   
        public const Int32 KNOB_CH2YPOS_SELECT = 0x35;  

        public const Int32 KNOB_CH2YLEVEL_LEFT = 0x56;  
        public const Int32 KNOB_CH2YLEVEL_RIGHT = 0x57; 
        public const Int32 KNOB_CH2YLEVEL_SELECT = 0x3a;

        public const Int32 KNOB_CH3YPOS_LEFT = 0x72;    
        public const Int32 KNOB_CH3YPOS_RIGHT = 0x73;   
        public const Int32 KNOB_CH3YPOS_SELECT = 0x36;  

        public const Int32 KNOB_CH3YLEVEL_LEFT = 0x52;  
        public const Int32 KNOB_CH3YLEVEL_RIGHT = 0x53; 
        public const Int32 KNOB_CH3YLEVEL_SELECT = 0x3b;

        public const Int32 KNOB_CH4YPOS_LEFT = 0x76;    
        public const Int32 KNOB_CH4YPOS_RIGHT = 0x77;   
        public const Int32 KNOB_CH4YPOS_SELECT = 0x37;  

        public const Int32 KNOB_CH4YLEVEL_LEFT = 0x78;  
        public const Int32 KNOB_CH4YLEVEL_RIGHT = 0x79; 
        public const Int32 KNOB_CH4YLEVEL_SELECT = 0x3c;

        public const Int32 KNOB_TRIG_YPOS_LEFT = 0x70;  
        public const Int32 KNOB_TRIG_YPOS_RIGHT = 0x71; 
        public const Int32 KNOB_TRIG_YPOS_SELECT = 0x38;

        public const Int32 KNOB_YLEVEL_LEFT = 0x7A;
        public const Int32 KNOB_YLEVEL_RIGHT = 0x7B;
        public const Int32 KNOB_YLEVEL_SELECT = 0x3d;


        public const Int32 KNOB_YPOS_LEFT = 0x7C;
        public const Int32 KNOB_YPOS_RIGHT = 0x7D;
        public const Int32 KNOB_YPOS_SELECT = 0x3e;
        //Substituted By KNOB_TRIG_YPOS_SELECT
        //public const Int32 TRIG_PERCENT50 = 0x1d;

        public const Int32 KNOB_XLEVEL_LEFT = 0x60;      
        public const Int32 KNOB_XLEVEL_RIGHT = 0x61;    
        public const Int32 KNOB_XLEVEL_SELECT = 0x32;

        public const Int32 KNOB_XPOS_LEFT = 0x54;
        public const Int32 KNOB_XPOS_RIGHT = 0x55;
        public const Int32 KNOB_XPOS_SELECT = 0x33;

        //Extension
        public const Int32 FASTACQ = 0x40; 
        public const Int32 TOUCH = 0x41;
        public const Int32 NORMAL = 0x42;
        public const Int32 AUTO = 0x43;

        public const Int32 VK_GROUP_F1F6 = 0x80;
        public const Int32 VK_APPS = 0x81;         
        public const Int32 VK_PASSFAIL = 0x82;
        public const Int32 VK_WAVESEARCH = 0x83;
        public const Int32 VK_SDA = 0x84;
        public const Int32 VK_PWRANALYSIS = 0x85;
        public const Int32 VK_DDRA = 0x86;
        public const Int32 VK_COMPLIANCETEST = 0x87;
        public const Int32 VK_SETPRINTER = 0x88;
        public const Int32 VK_AWG1 = 0x89;
        public const Int32 VK_VOLTMETER = 0x8A;
        public const Int32 VK_CYMOMETER = 0x8B;
        public const Int32 VK_SNAPSHOT = 0x8C;
        public const Int32 VK_MINIMIZE = 0x8D;
        public const Int32 VK_CLOSE = 0x8E;
        public const Int32 VK_SHUTDOWN = 0x8F;
        public const Int32 VK_RESTART = 0x90;
        public const Int32 VK_LOGOUT = 0x91;
        public const Int32 VK_ABOUT = 0x92;
        //public const Int32 VK_DEBUG = 0x93;
        public const Int32 VK_SCREENSHOT = 0x94;
        public const Int32 VK_CLEAR = 0x95;
        public const Int32 VK_RESET = 0x96;
        public const Int32 VK_3D = 0x97;
        public const Int32 VK_AWG2 = 0x98;
        //public const Int32 VK_SETTING = 0x99;
        public const Int32 VK_FFT = 0x9A;
        //public const Int32 VK_FASTREQ = 0x9B;
        public const Int32 VK_HISTOGRAM = 0x9C;
        public const Int32 VK_EYEPATTERN = 0x9D;
        public const Int32 VK_RECOVER = 0x9E;
        public const Int32 VK_ZOOM = 0x9F;
        public const Int32 VK_VSA = 0xA1;
        public const Int32 VK_LISSAJOUS = 0xA2;

		public const Int32 VK_AWGALL = 0xA3;

		public const Int32 VK_WAVESEARCH_ITEMSEETING = 0xA4;
		public const Int32 VK_WAVESEARCH_ITEMCLOSE_CURRENT = 0xA5;
		public const Int32 VK_WAVESEARCH_ITEMCLOSEAll = 0xA6;

        public const Int32 VK_SCOPE_CHECK_MASK = 0xA7; //示波器自检 

        public const Int32 VK_TEMPCTRL = 0xA8;
        public const Int32 VK_AI = 0xA9;

        public const Int32 AI_SET = 0xAA;
        public const Int32 MULTI_DOMAIN = 0xAB;

        public const Int32 R1 = 0xB0;
		public const Int32 R2 = 0xB1;
		public const Int32 R3 = 0xB2;
		public const Int32 R4 = 0xB3;
        public (Byte Code, Int16 Step) Value
        {
            get;
        }

        public KeyCode(Byte code, Int16 step)
        {
            Value = new(code, step);
        }
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential,Pack =1)]
    public struct KeyCodeData
    {
        public UInt16 PacketHeader;
        public Byte PacketLenght;
        public Byte Command;
        public KeyEnum Key;
        public SByte Step;
        public UInt16 PacketEnder;

    }
}

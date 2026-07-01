using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ScopeX.U2;

namespace ScopeX.Core.Tools
{
    public class PowerAnalysisTools
    {
        private static Boolean _PwrLoopRun = false;

        private static Boolean _TouchStatus = false;
        private static Boolean _KeyBoardStatus = false;

        private static List<Int32> _KeyBoardForbidLockKeys = new List<Int32>();

        internal static void OpenPwrLoopFlag(Boolean flag)
        {
            if(_PwrLoopRun != flag)
            {
                if (flag)
                {
                    _TouchStatus = DsoPrsnt.DefaultDsoPrsnt.Display.TouchLock;
                    _KeyBoardStatus = DsoPrsnt.KeyBoardLockEnable;
                    _KeyBoardForbidLockKeys = DsoPrsnt.KeyBoardForbidLockKeys;
                    //屏幕禁用和使能
                    DsoPrsnt.DefaultDsoPrsnt.Display.TouchLock = true;
                    //键盘板禁用和使能
                    DsoPrsnt.KeyBoardLockEnable = true;
                    DsoPrsnt.KeyBoardForbidLockKeys = new List<Int32>() { KeyCode.RUNSTOP };
                }
                else
                {
                    //屏幕禁用和使能
                    DsoPrsnt.DefaultDsoPrsnt.Display.TouchLock = _TouchStatus;
                    //键盘板禁用和使能
                    DsoPrsnt.KeyBoardLockEnable = _KeyBoardStatus;

                    DsoPrsnt.KeyBoardForbidLockKeys = _KeyBoardForbidLockKeys;
                    _KeyBoardForbidLockKeys.Clear();
                }
                _PwrLoopRun = flag;
            }


        }

        internal static void PwrFlagOther(Boolean flag)
        {
            if (flag)
            {
                _TouchStatus = DsoPrsnt.DefaultDsoPrsnt.Display.TouchLock;
                _KeyBoardStatus = DsoPrsnt.KeyBoardLockEnable;
                _KeyBoardForbidLockKeys = DsoPrsnt.KeyBoardForbidLockKeys;
                //屏幕禁用和使能
                //DsoPrsnt.DefaultDsoPrsnt.Display.TouchLock = true;
                //键盘板禁用和使能
                DsoPrsnt.KeyBoardLockEnable = true;
                DsoPrsnt.KeyBoardForbidLockKeys = new List<Int32>() { KeyCode.VK_SCREENSHOT };
            }
            else
            {
                //屏幕禁用和使能
                //DsoPrsnt.DefaultDsoPrsnt.Display.TouchLock = _TouchStatus;
                //键盘板禁用和使能
                DsoPrsnt.KeyBoardLockEnable = _KeyBoardStatus;

                DsoPrsnt.KeyBoardForbidLockKeys = _KeyBoardForbidLockKeys;
                _KeyBoardForbidLockKeys.Clear();
            }
        }
    }
}

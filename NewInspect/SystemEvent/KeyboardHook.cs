using NewInspect.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static NewInspect.Constants.Win32API;

namespace NewInspect.SystemEvent
{
    public class KeyboardHook: INotifyPropertyChanged
    {
        static HookProc KeyboardHookProcedure; //声明KeyboardHookProcedure作为HookProc类型
        static int hKeyboardHook = 0; //声明键盘钩子处理的初始值
        public bool keyInvoke { get; set; } = true;
        
        public event PropertyChangedEventHandler PropertyChanged;

        public void SetKeyInvoke()
        {
            keyInvoke = !keyInvoke;
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs("keyInvoke"));
        }

        public void Start()
        {
            
            // 安装键盘钩子
            if (hKeyboardHook == 0)
            {
                KeyboardHookProcedure = new HookProc(KeyboardHookProc);
                IntPtr handle = System.Runtime.InteropServices.Marshal.GetHINSTANCE(System.Reflection.Assembly.GetExecutingAssembly().GetModules()[0]);
                hKeyboardHook = SetWindowsHookEx(13, KeyboardHookProcedure, handle, 0);
                GC.KeepAlive(KeyboardHookProcedure);
                if (hKeyboardHook == 0)
                {
                    Stop();
                    throw new Exception("安装键盘钩子失败");
                }
            }
        }
        public void Stop()
        {
            bool retKeyboard = true;


            if (hKeyboardHook != 0)
            {
                retKeyboard = UnhookWindowsHookEx(hKeyboardHook);
                hKeyboardHook = 0;
            }

            if (!(retKeyboard)) throw new Exception("卸载钩子失败！");
        }
        private int KeyboardHookProc(int nCode, Int32 wParam, IntPtr lParam)
        {
            if ((nCode >= 0))
            {
                Win32API.KeyboardHookStruct hookStruct = (Win32API.KeyboardHookStruct)Marshal.PtrToStructure(lParam, typeof(Win32API.KeyboardHookStruct));
                //if (hookStruct.vkCode == (int)Keys.X && wParam == 256)  //D
                //{
                //    Logger.Info($"click keynbord ctrl + x");
                //}
                if (wParam == 256)  //D
                {
                    SetKeyInvoke();
                    
                    Logger.Info($"click keynbord ctrl");
                }

            }
            return CallNextHookEx(hKeyboardHook, nCode, wParam, lParam);
        }
    }
}

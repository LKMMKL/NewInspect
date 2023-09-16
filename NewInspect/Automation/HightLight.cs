using NewInspect.Constants;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;
using UIAutomationClient;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;

namespace NewInspect.Automation
{
    public class HightLight
    {
        public static Task draw_task;
        public static CancellationTokenSource tokenSource = new CancellationTokenSource();
        public static Elements root;
        public static bool flag = true;
        public static IUIAutomationElement activeEle = null;
        public static void DrawHightLight(tagRECT r)
        {
            if (draw_task != null)
            {
                tokenSource.Cancel();
                ResetToken();
            }
            var ct = tokenSource.Token;
            Rectangle rect = new Rectangle(r.left - 2, r.top - 2, r.right - r.left + 2, r.bottom - r.top + 2);
            draw_task = new Task(() =>
            {
                IntPtr desktop = Win32API.GetDC(IntPtr.Zero);
                while (!ct.IsCancellationRequested)
                {
                    using (Graphics g = Graphics.FromHdc(desktop))
                    {
                        Pen myPen = new Pen(System.Drawing.Color.BlueViolet, 2);
                        g.DrawRectangle(myPen, rect);
                        g.Dispose();
                    }
                }
                var r1 = IntPtr.Zero;
                IntPtr pnt = Marshal.AllocHGlobal(Marshal.SizeOf(rect));
                Marshal.StructureToPtr(r1, pnt, false);
                //InvalidateRect(IntPtr.Zero,r , true);
                Win32API.InvalidateRect(IntPtr.Zero, r1, true);

            });

            draw_task.Start();
        }

        public static void ResetToken()
        {
            tokenSource = new CancellationTokenSource();
        }

        public static void MouseDetect(Elements rootElement)
        {
            root = rootElement;
            System.Timers.Timer timer = new System.Timers.Timer(3000);
            timer.Elapsed += Timer_Elapsed;
            timer.Start();

        }

        private static void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Point point = new Point();
            tagPOINT tp = new tagPOINT();
            Win32API.GetCursorPos(out point);
            tp.y = point.Y;
            tp.x = point.X;
            // 指针在主窗口内
            if (false)
            {
            }
            else
            {
                try
                {
                    if (flag)
                    {
                        CUIAutomation uia = new CUIAutomation();
                        IUIAutomationElement ele = uia.ElementFromPoint(tp);
                        
                        if (ele != null && ele != activeEle)
                        {
                            activeEle = ele;
                            flag = false;
                            Util.MouseSelect(ele, (Elements)root);
                            flag = true;
                        }
                        
                    }
                    
                }
                catch (Exception ex)
                {
                    string v = ex.Message;
                }
            }
        }

        private static void Time_Tick(object rootElement)
        {
            
        }
    }
}

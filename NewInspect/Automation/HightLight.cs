using NewInspect.Constants;
using NewInspect.SystemEvent;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
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
        public static Func<bool> mouseFunc;
        public static void DrawHightLight(tagRECT r)
        {
            if (draw_task != null)
            {
                tokenSource.Cancel();
                ResetToken();
            }
            var ct = tokenSource.Token;
            Rectangle rect = new Rectangle(r.left - 4, r.top - 4, r.right - r.left + 4, r.bottom - r.top + 4);
            draw_task = new Task(() =>
            {
                //IntPtr desktop = Win32API.GetDC(IntPtr.Zero);
                //while (!ct.IsCancellationRequested)
                //{
                //    using (Graphics g = Graphics.FromHdc(desktop))
                //    {
                //        Pen myPen = new Pen(System.Drawing.Color.BlueViolet, 2);
                //        g.DrawRectangle(myPen, rect);
                //        g.Dispose();
                //    }
                //}
                //var r1 = IntPtr.Zero;
                //IntPtr pnt = Marshal.AllocHGlobal(Marshal.SizeOf(rect));
                //Marshal.StructureToPtr(r1, pnt, false);
                ////InvalidateRect(IntPtr.Zero,r , true);
                //Win32API.InvalidateRect(IntPtr.Zero, r1, true);
                IntPtr desktop = Win32API.GetDC(IntPtr.Zero);
                Pen myPen = new Pen(System.Drawing.Color.BlueViolet, 3);
                Graphics g = Graphics.FromHdc(desktop);
                while (!ct.IsCancellationRequested)
                {
                    Thread.Sleep(500);
                    g.DrawRectangle(myPen, rect);
                    //g.Dispose();
                }
                g.Dispose();
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
            System.Timers.Timer timer = new System.Timers.Timer(2000);
            timer.Elapsed += Timer_Elapsed;
            timer.Start();

        }

        private static void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (mouseFunc.Invoke() )
            {
                Logger.Info("mouse curor in main window");
                return;
            }
            Point point = new Point();
            tagPOINT tp = new tagPOINT();
            Win32API.GetCursorPos(out point);
            tp.y = point.Y;
            tp.x = point.X;

            Logger.Info($"time elapsed, flag:{flag}");
            // 指针在主窗口内
            if (false)
            {
            }
            else
            {
                Task.Run(() => {
                    try
                    {
                        if (flag)
                        {
                            CUIAutomation uia = new CUIAutomation();
                            IUIAutomationElement ele = uia.ElementFromPoint(tp);
                            var currName = ele.CurrentName;
                            var classname = ele.CurrentClassName;
                            if (activeEle == null || (ele != null && uia.CompareElements(ele, activeEle) != 1))
                            {
                                Logger.Info($"mouse select start: element name {currName}");
                                activeEle = ele;
                                flag = false;
                                Util.MouseSelect(ele, (Elements)root);

                                Logger.Info($"mouse select end: element name {currName}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"mag:{ex.Message}\n{ex.StackTrace}");
                    }
                    finally
                    {
                        flag = true;
                    }

                });
            }
        }

        private static void Time_Tick(object rootElement)
        {
            
        }
    }
}

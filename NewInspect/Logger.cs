using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace NewInspect
{
    public class Logger
    {
        private static bool open = false;
        private static string logDir = AppContext.BaseDirectory + "\\Log\\";
        private static string logFileName = $"{logDir}{Assembly.GetExecutingAssembly().GetName().Name}-{DateTime.Now.ToString("yyyyMMddHHmmss")}.log";
        static Logger()
        {
            try
            {
                //using (RegistryKey registry = Registry.CurrentUser.OpenSubKey("SOFTWARE\\NEC\\NPSpeed\\PowerSavingMode"))
                //{
                //    Int32 log = (Int32)registry.GetValue("log", 0);
                //    if (log == 1)
                //    {
                //        open = true;
                //        if (!Directory.Exists(logDir))
                //        {
                //            Directory.CreateDirectory(logDir);
                //        }
                //        logFileName = $"{logDir}{Assembly.GetExecutingAssembly().GetName().Name}-{DateTime.Now.ToString("yyyyMMddHHmmss")}.log";
                //        File.CreateText(logFileName).Close();
                //    }
                //}
                open = true;
                if (!Directory.Exists(logDir))
                {
                    Directory.CreateDirectory(logDir);
                }
                logFileName = $"{logDir}{Assembly.GetExecutingAssembly().GetName().Name}-{DateTime.Now.ToString("yyyyMMddHHmmss")}.log";
                File.CreateText(logFileName).Close();
            }
            catch { }
        }

        public static void Info(string message,
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            try
            {
                if (!open) return;
                if (logFileName != string.Empty)
                {
                    using (StreamWriter sw = File.AppendText(logFileName))
                    {
                        sw.WriteLine(
                            "Level: Info\n" +
                            "Date: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss \n") +
                            "File: " + sourceFilePath + "\n" +
                            "Line: " + sourceLineNumber.ToString() + "\n" +
                            "Data:" + message + "\n");
                    }
                }

            }
            catch { }
        }

        public static void Error(string message,
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            try
            {
                if (!open) return;
                if (logFileName != string.Empty)
                {
                    using (StreamWriter sw = File.AppendText(logFileName))
                    {
                        sw.WriteLine(
                            "Level: Error\n" +
                            "Date: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss \n") +
                            "File: " + sourceFilePath + "\n" +
                            "Line: " + sourceLineNumber.ToString() + "\n" +
                            "Data:" + message + "\n");
                    }
                }
            }
            catch { }
        }
    }
}

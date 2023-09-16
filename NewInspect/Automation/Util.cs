using NewInspect.Constants;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using UIAutomationClient;
using static NewInspect.MainWindow;

namespace NewInspect.Automation
{
    public class Util
    {
        static IUIAutomationElement _rootElement;
        static CUIAutomation uia;
        static Util()
        {
            uia = new CUIAutomation();
            _rootElement = uia.GetRootElement();
        }
        public static Elements LoadDesktop()
        {

            List<Elements> eles = new List<Elements>();
            IUIAutomationPropertyCondition find1_condition =
                   uia.CreatePropertyCondition(UIA_PropertyIds.UIA_ControlTypePropertyId,
                    ControlType.UIA_WindowControlTypeId) as IUIAutomationPropertyCondition;

            IUIAutomationElement root = uia.GetRootElement();
            string rootId = GetRuntimeIdStr(root.GetRuntimeId());
            Elements r = new Elements(rootId, root);
            //IUIAutomationElementArray arry = uia.GetRootElement().FindAll(TreeScope.TreeScope_Children, (new CUIAutomation()).CreateTrueCondition());
            //for (int i = 0; i < arry.Length; i++)
            //{
            //    r.children.Add(new Elements(rootId, arry.GetElement(i)));
            //}
            r.isExpanded = true;
            return r;
        }
        public static void LoadChildren(Elements source, bool loadNextChildren)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                source.children.Clear();
            });

            IUIAutomationElementArray arry = source.curr.FindAll(TreeScope.TreeScope_Children, uia.CreateTrueCondition());
            for (int i = 0; i < arry.Length; i++)
            {
                var ele = new Elements(source.rootId, arry.GetElement(i));
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    source.children.Add(ele);
                });
                if (loadNextChildren)
                {
                    LoadChildren(ele, false);
                }

            }
        }

        public static void MouseSelect(IUIAutomationElement obj, Elements rootElement)
        {
            var pathToRoot = new Stack<IUIAutomationElement>();
            IUIAutomationTreeWalker walker = uia.RawViewWalker;
            while (obj != null)
            {
                // Break on circular relationship (should not happen?)
                if (pathToRoot.Contains(obj) || obj.CurrentName.Equals(rootElement.name))
                {
                    break;
                }

                pathToRoot.Push(obj);
                try
                {
                    obj = walker.GetParentElement(obj);
                }
                catch (Exception ex)
                {
                    // TODO: Log
                    Console.WriteLine($"Exception: {ex.Message}");
                }
            }

            Elements elementVm = rootElement;
            //pathToRoot 一定是桌面 sub item
            while (pathToRoot.Count > 0)
            {
                //System.Windows.Application.Current.Dispatcher.Invoke(() =>
                //{
                //    tmp.isExpanded = true;
                //});

                var elementOnPath = pathToRoot.Pop();
                var nextElementVm = elementVm.children.FirstOrDefault(child => child.runtimeId.Equals(Util.GetRuntimeIdStr(elementOnPath.GetRuntimeId())));
                if (nextElementVm == null)
                {
                    LoadChildren(elementVm, true);
                    nextElementVm = elementVm.children.FirstOrDefault(child => child.runtimeId.Equals(Util.GetRuntimeIdStr(elementOnPath.GetRuntimeId())));
                    if (nextElementVm == null) return;
                }
                elementVm = nextElementVm;
                if (!elementVm.isExpanded)
                {
                    elementVm.isExpanded = true;
                }
                elementVm.isSelected = true;
            }
        }

        public static void GetAllSupportPattern(ObservableCollection<EleDetail> dict, IUIAutomationElement source)
        {
            foreach (PatternId p in Enum.GetValues(typeof(PatternId)))
            {
                int id = (int)p;
                object pattern = source.GetCurrentPattern(id);
                if (pattern != null)
                {
                    dict.Add(new EleDetail { key = $"{p}", value = "true", isPattern = true });
                }
            }
        }
        public static string GetRuntimeIdStr(Array runtimeId)
        {
            string id = string.Empty;
            int index = 0;
            while (index <= runtimeId.Length - 1)
            {
                id += runtimeId.GetValue(index).ToString();
                index++;
                if (index <= runtimeId.Length - 1) id += ",";
            }
            return id;
        }
    }
}

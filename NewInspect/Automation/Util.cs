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

            //IUIAutomationElementArray arry = source.curr.FindAll(TreeScope.TreeScope_Children, uia.CreateTrueCondition());
            List<IUIAutomationElement> arry = GetSubElementInfo(source.curr);
            for (int i = 0; i < arry.Count; i++)
            {
                var ele = new Elements(source.rootId, arry[i]);
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
        public static List<IUIAutomationElement> GetSubElementInfo(IUIAutomationElement source)
        {
            var listResult = new List<IUIAutomationElement>();

            IUIAutomation uiAutomation = new CUIAutomation();
            IUIAutomationTreeWalker treeWalker = uiAutomation.CreateTreeWalker(uiAutomation.CreateTrueCondition());
            IUIAutomationElement subControl = treeWalker.GetFirstChildElement(source);
            if (subControl == null)
            {
                return listResult;
            }

            listResult.Add(subControl);

            IUIAutomationElement nextSubControl = treeWalker.GetNextSiblingElement(subControl);
            if (nextSubControl == null) return listResult;
            while (nextSubControl != null)
            {
                listResult.Add(nextSubControl);
                nextSubControl = treeWalker.GetNextSiblingElement(nextSubControl);
            }

            return listResult;
        }

        public static void MouseSelect(IUIAutomationElement obj, Elements rootElement)
        {
            DateTime before = DateTime.Now;
            IUIAutomationElement s = obj;
            CUIAutomation cui = new CUIAutomation();
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
                    Logger.Error($"Exception: {ex.Message}");
                }
            }

            Logger.Info($"mouse select pathToRoot count:{pathToRoot.Count}, target:{s.CurrentName}, {s.CurrentClassName}, {s.CurrentAutomationId} ");
            Elements elementVm = rootElement;
            //pathToRoot 一定是桌面 sub item
            while (pathToRoot.Count > 0)
            {
                //System.Windows.Application.Current.Dispatcher.Invoke(() =>
                //{
                //    tmp.isExpanded = true;
                //});
                var elementOnPath = pathToRoot.Pop();

                Logger.Info($"mouse select pathToRoot pop: target:{elementOnPath.CurrentName}, {elementOnPath.CurrentClassName}, {elementOnPath.CurrentAutomationId}, {GetRuntimeIdStr(elementOnPath.GetRuntimeId())} ");
                var nextElementVm = elementVm.children.FirstOrDefault(child => cui.CompareElements(child.curr, elementOnPath) == 1);
                if (nextElementVm == null)
                {
                    LoadChildren(elementVm, true);
                    nextElementVm = elementVm.children.FirstOrDefault(child => cui.CompareElements(child.curr, elementOnPath) == 1);
                    if (nextElementVm == null)
                    {
                        Logger.Error($"mouse select find element fail");
                        return;
                    }
                }
                elementVm = nextElementVm;
                if (!elementVm.isExpanded)
                {
                    elementVm.isExpanded = true;
                }

            }
            elementVm.isSelected = true;
            TimeSpan timeSpan = DateTime.Now.Subtract(before);
            Logger.Info($"time span {timeSpan.TotalSeconds}");
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

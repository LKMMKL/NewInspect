using NewInspect.Constants;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using UIAutomationClient;

namespace NewInspect.Automation
{
    public class TParams
    {
        [DefaultValue("")]
        public string windowcaption;
        [DefaultValue("")]
        public string classname;
        [DefaultValue("")]
        public string automationid;
        [DefaultValue("")]
        public string ctrlname;
        
    }

    public class ScrollItemParams: TParams
    {
        [DefaultValue("")]
        public string scroll_automationid;
        [DefaultValue("")]
        public string scroll_ctrlname;
        [DefaultValue("")]
        public string find_automationid;
        [DefaultValue("")]
        public string find_ctrlname;
    }
    public class Teamplate
    {

        public static Dictionary<string, Func<string, Elements, string>> templateDict = new Dictionary<string, Func<string, Elements, string>>();
        static Teamplate()
        {
            // Normal
            templateDict.Add("UI_Invoke", NormalTeamplate);
            templateDict.Add("UI_IsOffscreen", NormalTeamplate);
            templateDict.Add("UI_ClickControl", NormalTeamplate);
            templateDict.Add("UI_ClickControlEx", NormalTeamplate);
            templateDict.Add("UI_Toggle", NormalTeamplate);
            templateDict.Add("UI_GetControlToggleState", NormalTeamplate);
            templateDict.Add("UI_ExpandCollapse", NormalTeamplate);
            templateDict.Add("UI_GetExpandCollapseState", NormalTeamplate);
            templateDict.Add("UI_GetControlValue", NormalTeamplate);
            templateDict.Add("UI_GetScrollInfo", NormalTeamplate);


            // window
            templateDict.Add("IsWindowExist", WindowTeamplate);
            templateDict.Add("CloseWindow", WindowTeamplate);
            templateDict.Add("GetWindowStatus", WindowTeamplate);
            templateDict.Add("IsWindowForward", WindowTeamplate);
            templateDict.Add("IsWindowMaximized", WindowTeamplate);
            templateDict.Add("IsWindowMinimized", WindowTeamplate);

            //scrollItem
            templateDict.Add("UI_Scroll", ScrollItemTeamplate);

        }

        public static string NormalTeamplate(string method, Elements ele)
        {
            JsonSerializerSettings setting = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore };

            var param = new TParams()
            {
                windowcaption = ele.firstName,
                classname = ele.firstClassname,
                automationid = ele.automationId,
                ctrlname = ele.nativeName
            };
            string teamplate = JsonConvert.SerializeObject(param, Formatting.Indented, setting);
            return $"funclib.{method}({teamplate})";
        }
        public static string WindowTeamplate(string method, Elements ele)
        {
            JsonSerializerSettings setting = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore };

            var param = new TParams()
            {
                windowcaption = ele.firstName,
                classname = ele.firstClassname
            };
            string teamplate = JsonConvert.SerializeObject(param, Formatting.Indented, setting);
            return $"funclib.{method}({teamplate})";
        }

        public static string ScrollItemTeamplate(string method, Elements ele)
        {
            CUIAutomation cui = new CUIAutomation();
            JsonSerializerSettings setting = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore };
            IUIAutomationElement parent = cui.RawViewWalker.GetParentElement(ele.curr);
            while (parent != null)
            {
                if (parent.GetCurrentPattern(((int)PatternId.ScrollPattern)) != null)
                {
                    break;
                }
                parent = cui.RawViewWalker.GetParentElement(parent);
            }
            if (parent != null)
            {
                var param = new ScrollItemParams()
                {
                    windowcaption = ele.firstName,
                    classname = ele.firstClassname,
                    scroll_automationid = parent.CurrentAutomationId,
                    scroll_ctrlname = parent.CurrentName,
                    find_automationid = ele.automationId,
                    find_ctrlname = ele.nativeName
                };
                string teamplate = JsonConvert.SerializeObject(param, Formatting.Indented, setting);
                return $"funclib.{method}({teamplate})";
            }
            return string.Empty ;
        }
        public static string CovertToTeamplate(string method, Elements ele)
        {
            Func<string, Elements, string> func;
            templateDict.TryGetValue(method, out func);
            if(func != null)
            {
                return func(method, ele);
            }
            return string.Empty;
        }

        public static string DefaultTeamplate(Elements ele)
        {
            string method = "UI_ClickControl";
            Func<string, Elements, string> func;
            templateDict.TryGetValue(method, out func);
            if (func != null)
            {
                return func(method, ele);
            }
            return string.Empty;
        }
    }
}

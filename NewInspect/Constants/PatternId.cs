﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewInspect.Constants
{
    public class Pattern
    {
        public static Dictionary<PatternId, List<string>> pattern = new Dictionary<PatternId, List<string>>();
        static Pattern()
        {
            var normal = new List<string> {"UI_IsOffscreen", "UI_ClickControl"};
            pattern.Add(PatternId.InvokePattern, new List<string> { "UI_Invoke", "UI_IsOffscreen", "UI_ClickControl" });
            pattern.Add(PatternId.TogglePattern, new List<string> { "UI_Toggle", "UI_GetControlToggleState", "UI_IsOffscreen", "UI_ClickControl" });
            pattern.Add(PatternId.ExpandCollapsePattern, new List<string> { "UI_ExpandCollapse", "UI_GetExpandCollapseState", "UI_IsOffscreen", "UI_ClickControl" });
            pattern.Add(PatternId.WindowPattern, new List<string> { "IsWindowExist", "CloseWindow", "GetWindowStatus", "IsWindowForward", "IsWindowMaximized", "IsWindowMinimized" });
        }
    }
    public enum PatternId
    {
         InvokePattern = 10000,// click
         TogglePattern = 10015,// click toggle
         ExpandCollapsePattern = 10005,
         WindowPattern = 10009,
         ItemContainerPattern = 10019,
         DragPattern = 10030,
         DropTargetPattern = 10031,
         TextEditPattern = 10032,
         ScrollItemPattern = 10017,
         ValuePattern = 10002,
         RangeValuePattern = 10003,
         ScrollPattern = 10004,
         TextPattern = 10014
    }
    
}

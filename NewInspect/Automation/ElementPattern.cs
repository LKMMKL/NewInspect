using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIAutomationClient;

namespace NewInspect.Automation
{
    interface IPattern{
        string patternId { get; set; }
        string GetTemplate();
    }

    static class Teamplates
    {
        public static string MouseMovePattern(Elements ele)
        {
            tagRECT rect = ele.curr.CachedBoundingRectangle;
            int x = (rect.left + rect.right) / 2;
            int y = (rect.top + rect.bottom) / 2;
            string expre = string.Format("funclib.MouseMove({\n" +
                "x: {0}\n" +
                "y: {1}\n" +
                "});",x,y);
            return expre;
        }

        public static string ButtonPattern(Elements ele)
        {
            tagRECT rect = ele.curr.CachedBoundingRectangle;
            int x = (rect.left + rect.right) / 2;
            int y = (rect.top + rect.bottom) / 2;
            string expre = string.Format("funclib.MouseMove1({\n" +
                "x: {0}\n" +
                "y: {1}\n" +
                "});", x, y);
            return expre;
        }


    }
    public class ElementPattern 
    {
        public static Dictionary<int, List<Action>> templates = new Dictionary<int, List<Action>>();

        public void LoadTeamplate()
        {
            
        }
    }
}

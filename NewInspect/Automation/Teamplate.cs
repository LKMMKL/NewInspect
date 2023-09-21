using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.ComponentModel;

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
    public class Teamplate
    {
        

        public Elements ele;
        public Teamplate(Elements ele)
        {
            this.ele = ele;
        }
        //public override string ToString() {
        //    if(string.IsNullOrEmpty(ele.nativeName))
        //    //string.Format("windowcaption:{0}")
        //    return $"{{{windowcaption}}}";
        //}

        public static string CovertToTeamplate(string method, Elements ele)
        {
            JsonSerializerSettings setting = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore };

            var param = new TParams()
            {
                windowcaption= ele.firstName, classname=ele.firstClassname, automationid=ele.automationId, ctrlname=ele.nativeName
            };
            string teamplate = JsonConvert.SerializeObject(param, Formatting.Indented, setting);
            return $"funclib.{method}({teamplate})";
        }
    }
}

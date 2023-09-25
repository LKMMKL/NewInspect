using NewInspect.Constants;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UIAutomationClient;

namespace NewInspect.Automation
{
    public class EleDetail
    {
        public string key { get; set; }
        public object value { get; set; }

        public bool isPattern { get; set; } = false;
    }
    public class Elements : INotifyPropertyChanged
    {
        // 桌面下子节点
        public string firstName; 
        public string firstClassname; 
        // 未进行格式化的控件名
        public string nativeName { get; set; }
        // 格式化后的控件名
        public string name { get; set; }

        public string className { get; set; }
        public string automationId { get; set; }
        public bool offScreen { get; set; }
        public string controlType { get; set; }
        public tagRECT rect { get; set; }
        public int level;
        public List<EleDetail> patternList = new List<EleDetail>();
        public bool isSelected
        {
            get { return GetProperty<bool>(); }
            set
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("isSelected"));
                SetProperty(value);
            }
        }
        public bool isExpanded
        {
            get { return GetProperty<bool>(); }
            set
            {
                SetProperty(value);
                if (value&&HightLight.flag)
                {
                    LoadChildren(true);
                }
            }
        }
        public ObservableCollection<Elements> children { get; set; } = new ObservableCollection<Elements>();

        public IUIAutomationElement curr;
        
        public Elements()
        {

        }
        public Elements(IUIAutomationElement curr, int level)
        {
            this.nativeName = curr.CurrentName;
            this.name = NormalizeString(this.nativeName);
            this.className = curr.CurrentClassName;
            this.automationId = curr.CurrentAutomationId;
            this.controlType  = $"{(ControlType)curr.CurrentControlType}";
            this.curr = curr;
            this.rect = curr.CurrentBoundingRectangle;
            this.level = level;
            GetAllSupportPattern();
        }

        private void LoadChildren(bool v)
        {
            Util.LoadChildren(this, v);
        }

        //public Elements TraceToPrevious()
        //{
            
        //}

        public event PropertyChangedEventHandler PropertyChanged;
        private readonly Dictionary<string, object> _backingFieldValues = new Dictionary<string, object>();
        public void GetAllSupportPattern()
        {
            try
            {
                patternList.Add(new EleDetail { key = $"UI_ClickControlEx", value = "true", isPattern = true });
                foreach (PatternId p in Enum.GetValues(typeof(PatternId)))
                {
                    int id = (int)p;
                    object pattern = curr.GetCurrentPattern(id);
                    if (pattern != null)
                    {
                        var list = new List<string>();
                        Pattern.pattern.TryGetValue(p, out list);
                        if (list == null) break;
                        foreach(var method in list)
                        {
                            patternList.Add(new EleDetail { key = $"{method}", value = "true", isPattern = true });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"{ex.Message}, {ex.StackTrace}");
            }
        }
        protected T GetProperty<T>([CallerMemberName] string propertyName = null)
        {
            if (propertyName == null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }
            object value;
            if (_backingFieldValues.TryGetValue(propertyName, out value))
            {
                return (T)value;
            }
            return default(T);
        }
        protected bool SetProperty<T>(T newValue, [CallerMemberName] string propertyName = null)
        {
            if (propertyName == null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }
            _backingFieldValues[propertyName] = newValue;
            OnPropertyChanged(propertyName);
            return true;
        }
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        protected string NormalizeString(string value)
        {
            if (String.IsNullOrEmpty(value))
            {
                return value;
            }
            return value.Replace(Environment.NewLine, " ").Replace('\r', ' ').Replace('\n', ' ');
        }


    }
}

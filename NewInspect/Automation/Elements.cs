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
        public string name { get; set; }
        //public string name { get; set; }
        public string className { get; set; }
        public string automationId { get; set; }
        public string runtimeId { get; set; }
        public bool offScreen { get; set; }
        public string controlType { get; set; }
        public string rect { get; set; }
        public string rootId;
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
        public Elements(string rootId, IUIAutomationElement curr)
        {
            this.rootId = rootId;
            this.name = NormalizeString(curr.CurrentName);
            this.className = curr.CurrentClassName;
            this.automationId = curr.CurrentAutomationId;
            this.controlType  = $"{(ControlType)curr.CurrentControlType}";
            this.curr = curr;
            this.rect = curr.GetHashCode().ToString();
            //this.runtimeId = Util.GetRuntimeIdStr(curr.GetRuntimeId());
            this.runtimeId = Util.GetRuntimeIdStr(curr.GetRuntimeId());
        }

        private void LoadChildren(bool v)
        {
            Util.LoadChildren(this, v);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private readonly Dictionary<string, object> _backingFieldValues = new Dictionary<string, object>();
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
        private string NormalizeString(string value)
        {
            if (String.IsNullOrEmpty(value))
            {
                return value;
            }
            return value.Replace(Environment.NewLine, " ").Replace('\r', ' ').Replace('\n', ' ');
        }
    }
}

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
        public string name
        {
            get
            {
                return NormalizeString(curr.CurrentName);
            }
            set
            {
                name = curr.CurrentName;
            }

        }
        //public string name { get; set; }
        public string className {
            get
            {
                return curr.CurrentClassName;
            }
            set
            {
                name = curr.CurrentClassName;
            }
        }
        public string automationId {
            get
            {
                return curr.CurrentAutomationId;
            }
            set
            {
                name = curr.CurrentAutomationId;
            }
        }
        public string runtimeId { get; set; }
        public bool offScreen { get; set; }
        public string controlType { get; set; }
        public string rootId;
        public bool isSelected { get; set; }
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
            this.controlType  = $"{(ControlType)curr.CurrentControlType}";
            this.curr = curr;
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

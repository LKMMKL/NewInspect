using NewInspect.Automation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NewInspect
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        //public ObservableCollection<Elements> eles { get; set; } = new ObservableCollection<Elements>();
        //TypeTreeModel model = new TypeTreeModel();
        List<Elements> list = new List<Elements>();
        List<string> patterns = new List<string>();
        ObservableCollection<EleDetail> dict { get; set; } = new ObservableCollection<EleDetail>();
        public MainWindow()
        {
            InitializeComponent();
            Height = 550;
            Width = 700;
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var r = Util.LoadDesktop();
            list.Add(r);
            this.treeview.ItemsSource = list;
            this.details.ItemsSource = dict;
            HightLight.MouseDetect(list[0]);
        }

        private void treeview_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            dict.Clear();
            TreeView treeView = sender as TreeView;
            Elements item = (Elements)treeView.SelectedItem;
            dict.Add(new EleDetail { key = nameof(item.controlType), value = item.controlType });
            dict.Add(new EleDetail { key = nameof(item.name), value = item.curr.CurrentName });
            dict.Add(new EleDetail { key = nameof(item.automationId), value = item.automationId });
            dict.Add(new EleDetail { key = nameof(item.className), value = item.className });
            dict.Add(new EleDetail { key = nameof(item.offScreen), value = item.offScreen });
            Util.GetAllSupportPattern(dict, item.curr);
            HightLight.DrawHightLight(item.curr.CurrentBoundingRectangle);
        }

        private void TextBox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            string v = "";
        }
    }
}

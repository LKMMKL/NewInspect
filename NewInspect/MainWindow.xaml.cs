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
using System.Windows.Threading;

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
        public bool mouseDetectEnable = true;
        public MainWindow()
        {
            InitializeComponent();
            Height = 550;
            Width = 700;
            Loaded += MainWindow_Loaded;
            HightLight.mouseFunc = GetMouseDetectState;
            Logger.Info("MainWindow Setup");
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var r = Util.LoadDesktop();
            list.Add(r);
            this.treeview.ItemsSource = list;
            this.details.ItemsSource = dict;
            HightLight.MouseDetect(list[0]);
        }

        private void TreeViewSelectedHandler(object sender, RoutedEventArgs e)
        {
            var treeViewItem = sender as TreeViewItem;
            
            Elements item = (Elements)treeViewItem.DataContext;
            if (treeViewItem!=null && item!=null)
            {
                selectedChange(item);
                treeViewItem.BringIntoView();
                treeViewItem.HorizontalAlignment = HorizontalAlignment.Left;
                e.Handled = true;
            }
        }
        private void TreeViewItem_RequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        {

            var item = (TreeViewItem)sender;
            if (item != null)
            {
                // move horizontal scrollbar only when event reached last parent item
                if (item.Parent == null)
                {
                    var scrollViewer = this.treeview.Template.FindName("_tv_scrollviewer_", this.treeview) as ScrollViewer;
                    if (scrollViewer != null)
                        Dispatcher.BeginInvoke(DispatcherPriority.Loaded, (Action)(() => scrollViewer.ScrollToLeftEnd()));
                }
            }
        }
        public void selectedChange(Elements item)
        {
            dict.Clear();
            dict.Add(new EleDetail { key = nameof(item.controlType), value = item.controlType });
            dict.Add(new EleDetail { key = nameof(item.name), value = item.curr.CurrentName });
            dict.Add(new EleDetail { key = nameof(item.automationId), value = item.automationId });
            dict.Add(new EleDetail { key = nameof(item.className), value = item.className });
            dict.Add(new EleDetail { key = nameof(item.offScreen), value = item.offScreen });
            dict.Add(new EleDetail { key = nameof(item.rect), value = item.rect });
            Util.GetAllSupportPattern(dict, item.curr);
            HightLight.DrawHightLight(item.curr.CurrentBoundingRectangle);
            
        }

        private void TextBox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            string v = "";
        }

        private void Window_MouseLeave(object sender, MouseEventArgs e)
        {
            mouseDetectEnable = true;
        }

        private void Window_MouseEnter(object sender, MouseEventArgs e)
        {
            mouseDetectEnable = false;
        }

        public bool GetMouseDetectState()
        {
            return mouseDetectEnable;
        }
    }
}

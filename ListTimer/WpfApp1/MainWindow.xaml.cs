using System;
using System.Collections.Generic;
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
using System.Collections.ObjectModel;

namespace WpfApp1
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var item = new TimerObject();
            item.Name = $"Timer{ListMain.Items.Count}";
            item.TimerText.Text = $"{item.Name}";

            ListMain.Items.Add(item);
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            ListMain.MinWidth = new TimerObject().ActualWidth;
        }

       
     
    }

    public class TimerList : ObservableCollection<TimerObject> { }
}


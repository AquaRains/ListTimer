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
using System.Windows.Threading;
using System.Threading;

namespace WpfApp1
{
    /// <summary>
    /// TimerObject.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class TimerObject : UserControl
    {
        DispatcherTimer Timer;
        public TimeSpan elapseTime { get; private set; }

        public TimerObject(TimeSpan time)
        {
            InitializeComponent();

            Timer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromMilliseconds(1),
                IsEnabled = false
            };
            Timer.Tick += Timer_Tick;

        }

        

        private void Timer_Tick(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}

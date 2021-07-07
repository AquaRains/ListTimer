using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using ListTimer.CustomControl;
using Timer = System.Timers.Timer;

namespace ListTimer
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow
    {
        private Timer Timer { get; }

        public MainWindow()
        {
            InitializeComponent();
            Timer = new Timer()
            {
                AutoReset = true,
                Interval = 10,
                Enabled = true
            };
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            TimeSpan ts = TimeSpan.Parse($"{TxtHour.Text}:{TxtMin.Text}:{TxtSec.Text}");
            if (ts == new TimeSpan(0, 0, 0))
            {
                ts = TimeSpan.FromMinutes(5);
            }

            CustomTimerControl item = new CustomTimerControl(ListMain.Items, ts, Timer)
            {
                Name = $"Timer{DateTime.Now.Ticks % 1000000000}"
            };
            item.TimerText.Text = string.IsNullOrEmpty(TxtInputClockName.Text) ? item.Name : TxtInputClockName.Text;

            ListMain.Items.Add(item);
            if (CheckRunOption.IsChecked ?? false)
            {
                item.DoStart();
            }

            TxtInputClockName.Text = "";
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            // ListMain.MinWidth = new TimerObject(TimeSpan.MinValue,this.Timer).ActualWidth;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string s = Regex.Replace(((TextBox)sender).Text, @"[^\d]", "");
            ((TextBox)sender).Text = s;
            if (string.IsNullOrEmpty(((TextBox)sender).Text))
            {
                ((TextBox)sender).Text = "0";
            }

            switch ((sender as TextBox)?.Name)
            {
                case "TxtSec":
                    {
                        TxtSec.Text = (decimal.Parse(TxtSec.Text) % 60).ToString(CultureInfo.InvariantCulture);
                        break;
                    }

                case "TxtMin":
                    {
                        TxtMin.Text = (decimal.Parse(TxtMin.Text) % 60).ToString(CultureInfo.InvariantCulture);
                        break;
                    }

                case "TxtHour":
                    {
                        TxtHour.Text = (decimal.Parse(TxtHour.Text) % 24).ToString(CultureInfo.InvariantCulture);
                        break;
                    }
            }
        }
    }

}


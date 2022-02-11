using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Kalantyr.RemoteControl.WPF.Client;
using Kalantyr.RemoteControl.WPF.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace Kalantyr.RemoteControl.WPF
{
    public partial class MainWindow
    {
        private DispatcherTimer _timer;

        private IServiceProvider ServiceProvider => ((App)Application.Current).Services;

        private IRemoteControlClient RemoteControlClient => ServiceProvider.GetService<IRemoteControlClient>();

        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;
            Unloaded += MainWindow_Unloaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(10) };
            _timer.Tick += MainWindow_Tick;
            _timer.Start();
            await ShowPowerOffStatus();
        }

        private void MainWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
        }

        private async void MainWindow_Tick(object sender, EventArgs e)
        {
            await ShowPowerOffStatus();
        }

        private async Task ShowPowerOffStatus()
        {
            Wait(async () =>
            {
                var delay = await RemoteControlClient.GetPowerOffAsync(CancellationToken.None);
                if (delay == null)
                {
                    _tbPowerOff.Text = "--";
                    _tbPowerOff.Foreground = Brushes.Gray;
                }
                else
                {
                    _tbPowerOff.Text = ToString(delay.Value);
                    _tbPowerOff.Foreground = delay.Value > TimeSpan.FromMinutes(1)
                        ? Brushes.Yellow
                        : Brushes.Red;
                }
            });
        }

        // TODO: unit-tests
        private static string ToString(TimeSpan timeSpan)
        {
            if (timeSpan.TotalSeconds < 60)
                return $"{timeSpan.Seconds} сек.";

            if (timeSpan.TotalMinutes < 60)
                return $"{timeSpan.Minutes} мин.";

            var s = $"{timeSpan.Hours:00}:{timeSpan.Minutes:00}";
            return s.TrimStart('0');
        }

        private void OnPowerOff_10min_Click(object sender, RoutedEventArgs e)
        {
            PowerOff(TimeSpan.FromMinutes(10));
        }

        private void OnPowerOff_30min_Click(object sender, RoutedEventArgs e)
        {
            PowerOff(TimeSpan.FromMinutes(30));
        }

        private void OnPowerOff_60min_Click(object sender, RoutedEventArgs e)
        {
            PowerOff(TimeSpan.FromHours(1));
        }

        private void OnPowerOff_180min_Click(object sender, RoutedEventArgs e)
        {
            PowerOff(TimeSpan.FromHours(3));
        }

        private void OnPowerOff_Custom_Click(object sender, RoutedEventArgs e)
        {
            var window = new TimeSpanWindow { Owner = this };
            if (window.ShowDialog() == true)
                PowerOff(window.TimeSpan);
        }

        private void OnCancelPowerOffClick(object sender, RoutedEventArgs e)
        {
            Wait(async () =>
            {
                await RemoteControlClient.CancelPowerOffAsync(CancellationToken.None);
                await ShowPowerOffStatus();
            });
        }

        private void PowerOff(TimeSpan delay)
        {
            Wait(async () =>
            {
                await RemoteControlClient.PowerOffAsync(delay, CancellationToken.None);
                await ShowPowerOffStatus();
            });
        }

        private async void Wait(Func<Task> func)
        {
            try
            {
                Cursor = Cursors.Wait;
                await func();
                _tbError.Visibility = Visibility.Collapsed;
            }
            catch (Exception e)
            {
                _tbError.Text = e.GetBaseException().Message;
                _tbError.Visibility = Visibility.Visible;
            }
            finally
            {
                Cursor = null;
            }
        }
    }
}

using System;
using System.Windows;

namespace Kalantyr.RemoteControl.WPF.Windows
{
    public partial class TimeSpanWindow
    {
        public TimeSpan TimeSpan { get; private set; }

        public TimeSpanWindow()
        {
            InitializeComponent();
        }

        private void OnOkClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var hh = int.Parse(_tbHours.Text);
                var mm = int.Parse(_tbMinutes.Text);
                TimeSpan = TimeSpan.FromMinutes(hh * 60 + mm);
                DialogResult = true;
            }
            catch (Exception error)
            {
                App.ShowError(error);
            }
        }
    }
}

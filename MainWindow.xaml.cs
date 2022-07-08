using System;
using System.Configuration;
using System.Text;
using System.Windows;
using MQTTnet;
using MQTTnet.Client.Receiving;
using MQTTnet.Server;

namespace MqttSubscriberApp
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window, IDisposable
    {
        MainViewModel viewModel;
        public MainWindow()
        {
            InitializeComponent();

            viewModel = new MainViewModel(this.Dispatcher);
            this.DataContext = viewModel;
            this.InfoListView.ItemsSource = viewModel.InfoList;
        }

        public void Dispose()
        {
            GC.Collect();
            GC.SuppressFinalize(this);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}

using System;
using System.Windows;
using System.Windows.Controls;

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

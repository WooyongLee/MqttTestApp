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
using System.Windows.Shapes;

namespace MqttSubscriberApp
{
    /// <summary>
    /// ClientWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ClientWindow : Window
    {
        ClientViewModel viewModel;

        public ClientWindow(int windowNum)
        {
            InitializeComponent();
            viewModel = new ClientViewModel(windowNum);
            this.DataContext = viewModel;

            Dispatcher.BeginInvoke(new Action(() =>
            {
                this.Title = "Client Window " + windowNum;
            }));
        }

        private async void Window_Closed(object sender, EventArgs e)
        {
            await viewModel.CloseClient();
        }
    }
}

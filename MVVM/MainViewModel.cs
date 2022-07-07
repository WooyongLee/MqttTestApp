using MQTTnet;
using MQTTnet.Server;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Windows.Input;

namespace MqttSubscriberApp
{
    public class MainViewModel : ViewModelBase
    {
        IMqttServer broker;

        ICommand StartCommand;
        ICommand StopCommand;

        private int connectionBacklog;
        private int port;

        public int ConnectionBacklog { get { return connectionBacklog; } set { connectionBacklog = value; NotifyPropertyChanged("ConnectionBacklog"); } }
        public int Port { get { return port; } set { port = value; NotifyPropertyChanged("Port"); } }

        private ObservableCollection<ListViewModel> infoList;
        public ObservableCollection<ListViewModel> InfoList
        {
            get { return infoList; }
            set
            {
                infoList = value;
                NotifyPropertyChanged("InfoList");
            }
        }

        public MainViewModel()
        {
            InfoList = new ObservableCollection<ListViewModel>();

            StartCommand = new CommandBase(StartExcuteFunc, CanExcute);
            StopCommand = new CommandBase(StopExcuteFunc, CanExcute);

            ConnectionBacklog = 100;
            Port = int.Parse(ConfigurationManager.AppSettings["MQTT_PORT"]);

            broker = new MqttFactory().CreateMqttServer();
        }

        private bool CanExcute(object arg)
        {
            return true;
        }

        private void StopExcuteFunc(object obj)
        {

        }

        private void StartExcuteFunc(object obj)
        {

        }
    }
}

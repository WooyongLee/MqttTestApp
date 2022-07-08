using Microsoft.Toolkit.Mvvm.Input;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Receiving;
using MQTTnet.Server;
using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace MqttSubscriberApp
{
    public class MainViewModel : ViewModelBase
    {
        bool bStart = false;

        Dispatcher CurrentDispatcher;

        #region MQTT .net
        IMqttServer broker;
        MqttServerOptionsBuilder optionsBuilder;
        #endregion 

        #region Fields and Observable Properties
        private int clientNum;
        private int connectionBacklog;
        private int port;
        private double communicationTimeout;

        public int ConnectionBacklog { get { return connectionBacklog; } set { connectionBacklog = value; NotifyPropertyChanged("ConnectionBacklog"); } }
        public int Port { get { return port; } set { port = value; NotifyPropertyChanged("Port"); } }
        public double CommunicationTimeout { get { return communicationTimeout; } set { communicationTimeout = value; NotifyPropertyChanged("CommunicationTimeout"); } }

        #endregion

        #region Observable Collection
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
        #endregion

        #region Command
        public ICommand StartCommand { get; set; }
        public ICommand StopCommand { get; set; }
        public ICommand CreateCliendCommand { get; set; }
        #endregion

        public MainViewModel(Dispatcher dispatcher)
        {
            CurrentDispatcher = dispatcher;

            InfoList = new ObservableCollection<ListViewModel>();

            StartCommand = new AsyncRelayCommand(StartExcuteAsyncFunc);
            StopCommand = new AsyncRelayCommand(StopExcuteAsyncFunc);
            CreateCliendCommand = new RelayCommand(CreateClientFunc);

            ConnectionBacklog = 100;
            Port = int.Parse(ConfigurationManager.AppSettings["MQTT_PORT"]);

            optionsBuilder = new MqttServerOptionsBuilder()
                                 .WithConnectionBacklog(ConnectionBacklog)
                                 .WithDefaultEndpointPort(Port)
                                 .WithDefaultCommunicationTimeout(TimeSpan.FromSeconds(CommunicationTimeout));

            broker = new MqttFactory().CreateMqttServer();

            // 로그 찍기위한 Event Handler 생성 및 등록
            broker.ApplicationMessageReceivedHandler = new MqttApplicationMessageReceivedHandlerDelegate(e => OnMessageReceived(e));
            broker.ClientConnectedHandler = new MqttServerClientConnectedHandlerDelegate(e => OnClientConnected(e));
            broker.ClientDisconnectedHandler = new MqttServerClientDisconnectedHandlerDelegate(e => OnClientDisconnected(e));
            broker.ClientSubscribedTopicHandler = new MqttServerClientSubscribedHandlerDelegate(e => OnClientSubscribedToTopic(e));
            broker.ClientUnsubscribedTopicHandler = new MqttServerClientUnsubscribedTopicHandlerDelegate(e => OnClientUnsubscribedToTopic(e));
        }

        private void CreateClientFunc()
        {
            ClientWindow clientWindow = new ClientWindow(++clientNum);

            Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
            {
                clientWindow.Show();
            }));
        }

        private async Task StartExcuteAsyncFunc()
        {
            if (!bStart)
            {
                optionsBuilder = new MqttServerOptionsBuilder()
                                    .WithConnectionBacklog(ConnectionBacklog)
                                    .WithDefaultEndpointPort(Port)
                                    .WithDefaultCommunicationTimeout(TimeSpan.FromSeconds(CommunicationTimeout));

                await broker.StartAsync(optionsBuilder.Build());
                bStart = true;
            }
        }

        private async Task StopExcuteAsyncFunc()
        {
            if (bStart)
            { 
                await broker.StopAsync();
                bStart = false;
            }
        }

        private void OnClientUnsubscribedToTopic(MqttServerClientUnsubscribedTopicEventArgs e)
        {
            string strLog = string.Format("{0}", $"OnClientUnsubscribedToTopic: {e.ClientId} - {e.TopicFilter}");

            CurrentDispatcher.BeginInvoke(new Action(() =>
            {
                InfoList.Add(new ListViewModel(strLog));
            }));
        }

        private void OnClientSubscribedToTopic(MqttServerClientSubscribedTopicEventArgs e)
        {
            string strLog = string.Format("{0}", $"OnClientSubscribedToTopic: {e.ClientId} - {e.TopicFilter.Topic}");

            CurrentDispatcher.BeginInvoke(new Action(() =>
            {
                InfoList.Add(new ListViewModel(strLog));
            }));
        }

        private void OnMessageReceived(MqttApplicationMessageReceivedEventArgs e)
        {
            string strLog = string.Format("{0}", $"OnMessageReceived: {e.ApplicationMessage.Topic}, {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}, {e.ProcessingFailed}");

            CurrentDispatcher.BeginInvoke(new Action(() =>
            {
                InfoList.Add(new ListViewModel(strLog));
            }));
        }

        private void OnClientConnected(MqttServerClientConnectedEventArgs e)
        {
            string strLog = string.Format("{0}", $"OnClientConnected: {e.ClientId}");

            CurrentDispatcher.BeginInvoke(new Action(() =>
            {
                InfoList.Add(new ListViewModel(strLog));
            }));
        }

        private void OnClientDisconnected(MqttServerClientDisconnectedEventArgs e)
        {
            string strLog = string.Format("{0}", $"OnClientDisconnected: {e.ClientId} - {e.DisconnectType}");

            CurrentDispatcher.BeginInvoke(new Action(() =>
            {
                InfoList.Add(new ListViewModel(strLog));
            }));
        }
    }
}

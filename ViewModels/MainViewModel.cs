using Microsoft.Toolkit.Mvvm.Input;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Receiving;
using MQTTnet.Server;
using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace MqttSubscriberApp
{
    public class MainViewModel : ViewModelBase
    {
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
        private bool isRun;

        public int ConnectionBacklog { get { return connectionBacklog; } set { connectionBacklog = value; NotifyPropertyChanged("ConnectionBacklog"); } }
        public int Port { get { return port; } set { port = value; NotifyPropertyChanged("Port"); } }
        public double CommunicationTimeout { get { return communicationTimeout; } set { communicationTimeout = value; NotifyPropertyChanged("CommunicationTimeout"); } }
        public bool IsRun { get { return isRun; } set { isRun = value; NotifyPropertyChanged("IsRun"); } }

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
            InfoList.CollectionChanged += InfoList_CollectionChanged;

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
            broker.StartedHandler = new MqttServerStartedHandlerDelegate(e => OnBrokerStarted(e));
            broker.StoppedHandler = new MqttServerStoppedHandlerDelegate(e => OnBrokerStopped(e));
        }

        private void InfoList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            object InfoGridView = Application.Current.MainWindow.TryFindResource("InfoGridView");

            if (InfoGridView != null)
            {
                var gridView = (GridView)InfoGridView;
                if (gridView != null)
                {
                    var vm = (ObservableCollection<ListViewModel>)sender;
                    foreach (var column in gridView.Columns)
                    {
                        string strColumnHeaderToLower = column.Header.ToString().ToLower();
                        if (strColumnHeaderToLower.Contains("information"))
                        {
                            column.Width = vm.ToList().Max(x => x.Information.Length);
                        }
                        else if (strColumnHeaderToLower.Contains("topic"))
                        {
                            column.Width = vm.ToList().Max(x => x.Topic.Length);
                        }
                    }
                }
            }
        }

        private async Task StartExcuteAsyncFunc()
        {
            if (!IsRun)
            {
                optionsBuilder = new MqttServerOptionsBuilder()
                                    .WithConnectionBacklog(ConnectionBacklog)
                                    .WithDefaultEndpointPort(Port)
                                    .WithDefaultCommunicationTimeout(TimeSpan.FromSeconds(CommunicationTimeout));

                await broker.StartAsync(optionsBuilder.Build());
                IsRun = true;
            }
        }

        private void CreateClientFunc()
        {
            ClientWindow clientWindow = new ClientWindow(++clientNum);

            Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
            {
                clientWindow.Show();
            }));
        }

        private async Task StopExcuteAsyncFunc()
        {
            if (IsRun)
            { 
                await broker.StopAsync();
                IsRun = false;
            }
        }

        private void OnClientUnsubscribedToTopic(MqttServerClientUnsubscribedTopicEventArgs e)
        {
            string strLog = string.Format("{0}", $"OnClientUnsubscribedToTopic");
            string clientId = e.ClientId;
            string topic = e.TopicFilter;

            CurrentDispatcher.BeginInvoke(new Action(() =>
            {
                InfoList.Add(new ListViewModel(strLog, clientId, topic));
            }));
        }

        private void OnClientSubscribedToTopic(MqttServerClientSubscribedTopicEventArgs e)
        {
            string strLog = string.Format("{0}", $"OnClientSubscribedToTopic");
            string clientId = e.ClientId;
            string topic =  e.TopicFilter.Topic ;

            CurrentDispatcher.BeginInvoke(new Action(() =>
            {
                InfoList.Add(new ListViewModel(strLog, clientId, topic));
            }));
        }

        private void OnMessageReceived(MqttApplicationMessageReceivedEventArgs e)
        {
            string strLog = string.Format("{0}", $"OnMessageReceived: {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}, {e.ProcessingFailed}");
            string clientId = e.ClientId;
            string topic = e.ApplicationMessage.Topic;

            CurrentDispatcher.BeginInvoke(new Action(() =>
            {
                InfoList.Add(new ListViewModel(strLog, clientId, topic));
            }));
        }

        private void OnClientConnected(MqttServerClientConnectedEventArgs e)
        {
            string strLog = string.Format("{0}", $"OnClientConnected");
            string clientId = e.ClientId;

            CurrentDispatcher.BeginInvoke(new Action(() =>
            {
                InfoList.Add(new ListViewModel(strLog, clientId));
            }));
        }

        private void OnClientDisconnected(MqttServerClientDisconnectedEventArgs e)
        {
            string strLog = string.Format("{0}", $"OnClientDisconnected - {e.DisconnectType}");
            string clientId = e.ClientId;

            CurrentDispatcher.BeginInvoke(new Action(() =>
            {
                InfoList.Add(new ListViewModel(strLog, clientId));
            }));
        }

        private void OnBrokerStopped(EventArgs e)
        {
            string strLog = string.Format("{0}", $"OnServerStopped!");

            CurrentDispatcher.BeginInvoke(new Action(() =>
            {
                InfoList.Add(new ListViewModel(strLog));
            }));
        }

        private void OnBrokerStarted(EventArgs e)
        {
            string strLog = string.Format("{0}", $"OnServerStarted!");

            CurrentDispatcher.BeginInvoke(new Action(() =>
            {
                InfoList.Add(new ListViewModel(strLog));
            }));
        }
    }
}

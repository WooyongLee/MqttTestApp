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

            viewModel = new MainViewModel();
            this.DataContext = viewModel;
            this.InfoListView.ItemsSource = viewModel.InfoList;

            this.Start();
        }
        
        public async void Start()
        {
            int port = int.Parse(ConfigurationManager.AppSettings["MQTT_PORT"]);

            var optionsBuilder = new MqttServerOptionsBuilder()
                                    .WithConnectionBacklog(100)
                                    .WithDefaultEndpointPort(port);

            // MQTT Server 객체 생성
            IMqttServer broker = new MqttFactory().CreateMqttServer();
            
            // 로그 찍기위한 Event Handler 생성 및 등록
            broker.ApplicationMessageReceivedHandler = new MqttApplicationMessageReceivedHandlerDelegate(e => OnMessageReceived(e));
            broker.ClientConnectedHandler = new MqttServerClientConnectedHandlerDelegate(e => OnClientConnected(e));
            broker.ClientDisconnectedHandler = new MqttServerClientDisconnectedHandlerDelegate(e => OnClientDisconnected(e));
            broker.ClientSubscribedTopicHandler = new MqttServerClientSubscribedHandlerDelegate(e => OnClientSubscribedToTopic(e));
            broker.ClientUnsubscribedTopicHandler = new MqttServerClientUnsubscribedTopicHandlerDelegate(e => OnClientUnsubscribedToTopic(e));

            await broker.StartAsync(optionsBuilder.Build());
            
            Console.WriteLine("Press any key to exit...");
            Console.ReadLine();
            this.Dispose();
        }

        private static void OnClientUnsubscribedToTopic(MqttServerClientUnsubscribedTopicEventArgs e)
        {
            string strCurDateTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            string strLog = string.Format("[{0}] {1}", strCurDateTime, $"OnClientUnsubscribedToTopic: {e.ClientId} - {e.TopicFilter}");
            Console.WriteLine(strLog);
        }

        private static void OnClientSubscribedToTopic(MqttServerClientSubscribedTopicEventArgs e)
        {
            string strCurDateTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            string strLog = string.Format("[{0}] {1}", strCurDateTime, $"OnClientSubscribedToTopic: {e.ClientId} - {e.TopicFilter.Topic}");
            Console.WriteLine(strLog);
        }

        private static void OnMessageReceived(MqttApplicationMessageReceivedEventArgs e)
        {
            string strCurDateTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            string strLog = string.Format("[{0}] {1}", strCurDateTime, $"OnMessageReceived: {e.ApplicationMessage.Topic}, {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
            Console.WriteLine(strLog);
        }

        private static void OnClientConnected(MqttServerClientConnectedEventArgs e)
        {
            string strCurDateTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            string strLog = string.Format("[{0}] {1}", strCurDateTime, $"OnClientConnected: {e.ClientId}");
            Console.WriteLine(strLog);
        }

        private static void OnClientDisconnected(MqttServerClientDisconnectedEventArgs e)
        {
            string strCurDateTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            string strLog = string.Format("[{0}] {1}", strCurDateTime, $"OnClientDisconnected: {e.ClientId} - {e.DisconnectType}");
            Console.WriteLine(strLog);
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

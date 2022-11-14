using Microsoft.Toolkit.Mvvm.Input;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using MQTTnet.Client.Receiving;
using MQTTnet.Protocol;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MqttSubscriberApp
{
    public class ClientViewModel : ViewModelBase
    {
        #region MQTT .net
        MqttClass mqttClient;
        #endregion

        #region Fields and Observable Properties
        private bool isConnected;
        private string topic;

        private bool cleanSession;
        private double communicationTimeout;
        private double keepAlivePeriod;
        private double keepAliveSendInterval;
        public uint maximumPacketSize;
        public uint sessionExpiryInterval;
        public ushort topicAliasMaximum;

        private bool retainFlag;
        public ushort qOSLevel;
        public uint subscriptionIdentifier;
        public uint messageExpiryInterval;

        private string sendMessage;
        private string clientLog;

        public bool IsConnected { get { return isConnected; } set { isConnected = value; NotifyPropertyChanged("IsConnected"); } }
        public string Topic { get { return topic; } set { topic = value; NotifyPropertyChanged("Topic"); } }

        public bool CleanSession { get { return cleanSession; } set { cleanSession = value; NotifyPropertyChanged("CleanSession"); } }
        public double CommunicationTimeout { get { return communicationTimeout; } set { communicationTimeout = value; NotifyPropertyChanged("CommunicationTimeout"); } }
        public double KeepAlivePeriod { get { return keepAlivePeriod; } set { keepAlivePeriod = value; NotifyPropertyChanged("KeepAlivePeriod"); } }
        public double KeepAliveSendInterval { get { return keepAliveSendInterval; } set { keepAliveSendInterval = value; NotifyPropertyChanged("KeepAliveSendInterval"); } }
        public uint MaximumPacketSize { get { return maximumPacketSize; } set { maximumPacketSize = value; NotifyPropertyChanged("MaximumPacketSize"); } }
        public uint SessionExpiryInterval { get { return sessionExpiryInterval; } set { sessionExpiryInterval = value; NotifyPropertyChanged("SessionExpiryInterval"); } }
        public ushort TopicAliasMaximum { get { return topicAliasMaximum; } set { topicAliasMaximum = value; NotifyPropertyChanged("TopicAliasMaximum"); } }
        public bool RetainFlag { get { return retainFlag; } set { retainFlag = value; NotifyPropertyChanged("RetainFlag"); } }
        public ushort QOSLevel { get { return qOSLevel; } set { qOSLevel = value; NotifyPropertyChanged("QOSLevel"); } }
        public uint SubscriptionIdentifier { get { return subscriptionIdentifier; } set { subscriptionIdentifier = value; NotifyPropertyChanged("SubscriptionIdentifier"); } }
        public uint MessageExpiryInterval { get { return messageExpiryInterval; } set { messageExpiryInterval = value; NotifyPropertyChanged("MessageExpiryInterval"); } }

        public string SendMessage { get { return sendMessage; } set { sendMessage = value; NotifyPropertyChanged("SendMessage"); } }
        public string ClientLog { get { return clientLog; } set { clientLog = value; NotifyPropertyChanged("ClientLog"); } }
        #endregion

        #region Command
        public ICommand SetCompleteCommand { get; set; }
        public ICommand SendMessageCommand { get; set; }
        public ICommand DisconnectCommand { get; set; }
        #endregion

        public ClientViewModel(int clientNum)
        {
            Topic = "TestTopic/" + clientNum;

            // Set Default
            CommunicationTimeout = 1;
            KeepAlivePeriod = 15;
            KeepAliveSendInterval = 0;
            MaximumPacketSize = 0;
            SessionExpiryInterval = 0;
            TopicAliasMaximum = 0;

            QOSLevel = 2;
            SubscriptionIdentifier = 0;
            MessageExpiryInterval = 0;

            SetCompleteCommand = new AsyncRelayCommand(SetCompleteAsyncFunc);
            SendMessageCommand = new RelayCommand(SendMessageFunc);
            DisconnectCommand = new AsyncRelayCommand(DisconnectAsyncFunc);
        }

        private async Task DisconnectAsyncFunc()
        {
            await CloseClient();
        }

        private void SendMessageFunc()
        {
            mqttClient.SendMessage(Topic, Encoding.ASCII.GetBytes(SendMessage),
                new MqttApplicationMessageBuilder()
                .WithRetainFlag(RetainFlag)
                .WithQualityOfServiceLevel((MqttQualityOfServiceLevel)QOSLevel)
                .WithSubscriptionIdentifier(SubscriptionIdentifier)
                .WithMessageExpiryInterval(MessageExpiryInterval));
        }

        private async Task SetCompleteAsyncFunc()
        {
            if (mqttClient != null)
            {
                if (mqttClient.IsConnected())
                {
                    await mqttClient.Disconnect();
                }
            }

            mqttClient = new MqttClass();
            mqttClient.messageHandler += OnCheckConnectionMessage;
            mqttClient.receivedMessageHandler += OnCheckConnectionMessage;

            var optionBuilder = new MqttClientOptionsBuilder()
                        .WithTcpServer("192.168.42.2", 1883)
                        .WithCleanSession(CleanSession)
                        .WithCommunicationTimeout(TimeSpan.FromSeconds(CommunicationTimeout))
                        .WithKeepAlivePeriod(TimeSpan.FromSeconds(KeepAlivePeriod))
                        .WithKeepAliveSendInterval(TimeSpan.FromSeconds(KeepAliveSendInterval))
                        .WithMaximumPacketSize(MaximumPacketSize)
                        .WithSessionExpiryInterval(SessionExpiryInterval)
                        .WithTopicAliasMaximum(TopicAliasMaximum);

            mqttClient.BuildOptions("192.168.42.2", 1883, optionBuilder);
            await mqttClient.Connect(Topic);
        }

        private void OnCheckConnectionMessage(object sender, EventArgs e)
        {
            this.IsConnected = mqttClient.IsConnected();
            this.ClientLog = "Client Connection State Changed";
        }

        public async Task CloseClient()
        {
            //mqttClient.messageHandler -= OnCheckConnectionMessage;
            //mqttClient.receivedMessageHandler -= OnCheckConnectionMessage;
            if (mqttClient != null)
            {
                if (mqttClient.IsConnected())
                {
                    await mqttClient.Disconnect();
                }
            }
        }
    }
}

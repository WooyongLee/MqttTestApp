using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using MQTTnet.Client.Receiving;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MqttSubscriberApp
{
    // Message Handle을 위한 델리게이트 정의
    public delegate void HandleMessage(object sender, EventArgs e);
    public delegate void HandleMessageReceived(object sender, MqttApplicationMessageReceivedEventArgs e);

    public class MqttClass : IDisposable
    {
        private MqttFactory _factory; // MQTT Client를 생성하기 위한 Factory Class
        private IMqttClientOptions _options; // 비동기 연결에 필요한 옵션 파라미터에 들어감
        private MqttApplicationMessage _message; // Client가 구독시도하는 메시지
        private CancellationToken _cancellationToken; // 비동기 호출을 취소

        private IMqttClient mqttClient; // MQTT Client
        public HandleMessage messageHandler;
        public HandleMessageReceived receivedMessageHandler;

        public MqttClass()
        {
            _factory = new MqttFactory();
            mqttClient = _factory.CreateMqttClient();
            _cancellationToken = new CancellationToken();

            mqttClient.ApplicationMessageReceivedHandler = new MqttApplicationMessageReceivedHandlerDelegate(e => OnMessageReceived(e));
            mqttClient.ConnectedHandler = new MqttClientConnectedHandlerDelegate(e => OnConnected(e));
            mqttClient.DisconnectedHandler = new MqttClientDisconnectedHandlerDelegate(e => OnDisconnected(e));
        }

        public void Dispose()
        {
            if (mqttClient != null)
            {
                mqttClient.DisconnectAsync();
                mqttClient.Dispose();
                mqttClient = null;
            }
        }

        private void OnDisconnected(MqttClientDisconnectedEventArgs e)
        {
            string log = string.Format("{0}", $"OnDisConnected: {e.AuthenticateResult}, {e.Exception}");
            messageHandler(log, new EventArgs());
        }

        private void OnConnected(MqttClientConnectedEventArgs e)
        {
            string log = string.Format("{0}", $"OnConnected: {e.AuthenticateResult}");
            messageHandler(log, new EventArgs());
        }

        private void OnMessageReceived(MqttApplicationMessageReceivedEventArgs e)
        {
            string log = string.Format("{0}", $"OnMessageReceived: {e.ClientId}, {e.ApplicationMessage}");
            receivedMessageHandler(log, e);
        }

        public bool IsConnected()
        {
            return this.mqttClient.IsConnected;
        }

        // 옵션 빌더를 통해 MQTT 연결 옵션을 지정한다.
        public void BuildOptions(string ip, int port, MqttClientOptionsBuilder optionBuilder)
        {
            // To Do :: Add Will Message Options

            _options = optionBuilder.Build();
        }

        // 메시지 빌더를 통해 MQTT 메시지를 구성한다.
        public byte[] BuildMessage(string topic, byte[] payload)
        {
            // To Do :: Add Transport Message Options

            _message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(payload)
                .WithExactlyOnceQoS()
                .WithRetainFlag()
                .Build();

            return _message.Payload;
        }

        public async Task Connect(string topic)
        {
            try
            {
                await mqttClient.ConnectAsync(_options, _cancellationToken);
                // await mqttClient.ConnectAsync(_options, _cancellationToken).ConfigureAwait(false);
            }
            catch
            {
            }
        }

        public async Task Disconnect()
        {
            await mqttClient.DisconnectAsync();
        }

        public async void SendMessage()
        {
            // Send ~~ Publish
            if (mqttClient.IsConnected)
            {
                // Debugging 시 TimeOut Exception 발생함 -> 실제 운용시에 해당 Exception 발생 시 처리 고민해보기
                try
                {
                    await mqttClient.PublishAsync(_message, _cancellationToken);
                }
                catch (Exception ex)
                {
                    LogMaker.WriteLog("MQTT Send Message Exception!! \n" + ex.ToString());
                }
            }
            else
            {
                LogMaker.WriteLog(_message + " 전송 실패, MQTT 연결 확인 필요");
            }

        }


        // 구독자 관점에서, 메시지 핸들러를 통해 수신
        public void ReceiveMessage()
        {
            mqttClient.UseApplicationMessageReceivedHandler(e =>
            {
                if (e.ApplicationMessage.Payload != null)
                {
                    receivedMessageHandler(this, e);
                }
            });
        }
    }
}

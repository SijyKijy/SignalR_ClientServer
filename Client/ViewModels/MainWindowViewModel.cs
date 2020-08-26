using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Threading;
using System.Net.Http;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Client.Models;
using Polly;

namespace Client.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private HubConnection _connection;
        private SynchronizationContext _uiContext;
        private Uri _connectionString;

        public ObservableCollection<Message> Messages { get; set; }

        public Message Message { get; set; } = new Message();

        private string _status;
        public string Status
        {
            get => _status;
            set
            {
                _status = value;
                Message.Name = value;
                OnPropertyChanged("Status");
            }
        }

        public MainWindowViewModel(IConfiguration configuration)
        {
            _uiContext = SynchronizationContext.Current;
            Messages = new ObservableCollection<Message>();
            _connectionString = new Uri(configuration.GetSection("SignalR:ConnectionString").Value);

            _connection = new HubConnectionBuilder()
                .WithUrl(_connectionString)
                .WithAutomaticReconnect()
                .Build();

            Status = _connection.State.ToString();

            _ = HubConnect();
        }

        private async Task HubConnect()
        {
            _connection.On<string>("Connected", (connectionId) => Status = connectionId);

            _connection.On<Message>("Recive", (msg) => _uiContext.Send(d => Messages.Add(msg), null));
            _connection.On<string>("Notify", (msg) => Messages.Add(new Message { Name = "Controller", Content = msg }));

            _connection.Reconnecting += (exception) =>
                {
                    Status = _connection.State.ToString();
                    _uiContext.Send(d => Messages.Add(new Message { Name = "Error", Content = exception.Message }), null);
                    return Task.CompletedTask;
                };

            await RetryConnect(_connectionString);
        }

        private async Task RetryConnect(Uri uri)
        {
            using HttpClient client = new HttpClient();
            TimeSpan pauseBetweenFailure = TimeSpan.FromSeconds(5);

            var retryPolicy = Policy
                .Handle<HttpRequestException>()
                .WaitAndRetryForeverAsync(_ => pauseBetweenFailure);

            await retryPolicy.ExecuteAsync(async () =>
            {
                var response = await client.DeleteAsync(uri);
            });

            await _connection.StartAsync();
        }

        public ICommand SendMessage
        {
            get => new DelegateCommand(async _ =>
                        await _connection.InvokeAsync("Send", Message), _ => _connection.State == HubConnectionState.Connected);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}

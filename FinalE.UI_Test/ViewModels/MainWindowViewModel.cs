using FinalE.Entities.Transport;
using FinalE.UI_Test.Views;
using Microsoft.AspNetCore.SignalR.Client;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalE.UI_Test.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private HubConnection connection { get; set; }
        private MainWindow mainWindow { get; set; }

        private string _username = "";

        public string Username
        {
            get { return _username; }
            set { _username = this.RaiseAndSetIfChanged(ref _username, value); }
        }

        private GameLobby? _selectedLObby;

        public GameLobby? SelectedLobby
        {
            get { return _selectedLObby; }
            set { _selectedLObby = this.RaiseAndSetIfChanged(ref _selectedLObby, value); }
        }

        private ObservableCollection<GameLobby> _lobbies = new();

        public ObservableCollection<GameLobby> Lobbies
        {
            get { return _lobbies; }
            set { _lobbies = this.RaiseAndSetIfChanged(ref _lobbies, value); }
        }


        public MainWindowViewModel(MainWindow window)
        {
            this.mainWindow = window;
            connection = new HubConnectionBuilder()
                 .WithUrl("https://localhost:49153/socket")
                 .Build();
            connection.On<List<GameLobby>>("Lobbies", GotLobbies);
            connection.On<GameLobby>("LobbyCreated", LobbyCreated);
            connection.On<GameLobby>("LobbyPlayerJoined", JoinedPlayer);
            connection.On<long>("LobbyRemoved", LobbyRemoved);
        }

        private async void JoinedPlayer(GameLobby obj)
        {
            connection.Remove("JoinedPlayer");
            await OpenLobbyView(obj);
        }

        private void LobbyRemoved(long obj)
        {
            var lob = this.Lobbies.First(x => x.Id == obj);
            this.Lobbies.Remove(lob);
        }

        private async void LobbyCreated(GameLobby obj)
        {
            this.Lobbies.Add(obj);
            if (this.connection.ConnectionId == obj.Host)
                await OpenLobbyView(obj);
        }

        private void GotLobbies(List<GameLobby> obj)
        {
            this.Lobbies = new ObservableCollection<GameLobby>(obj);
        }

        public async Task Connect()
        {
            await this.connection.StartAsync();
            var messageBoxStandardWindow = MessageBox.Avalonia.MessageBoxManager
  .GetMessageBoxStandardWindow("Debug UI", "Connected to Websocket!");
            await messageBoxStandardWindow.Show();
        }

        public async Task GetLobbies()
        {
            await this.connection.SendAsync("GetLobbies");
        }

        public async Task CreateLobby()
        {
            var wnd = new CreateLobby();
            wnd.DataContext = new CreateLobbyViewModel(connection, wnd, this.Username);
            await wnd.ShowDialog(this.mainWindow);
        }

        public async Task OpenLobbyView(GameLobby game)
        {
            var lob = new LobbyViewWindow();
            lob.DataContext = new LobbyViewWindowViewModel(connection, lob, game);
            await lob.ShowDialog(mainWindow);
        }
    }
}

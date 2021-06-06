using FinalE.Entities.Game;
using FinalE.Entities.Lobby;
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
    public class LobbyViewWindowViewModel : ViewModelBase
    {
        private readonly HubConnection _hubConnection;
        private readonly LobbyViewWindow _lobbyView;

        private long _lobbyId = 0;  

        public long LobbyId
        {
            get { return _lobbyId; }
            set { _lobbyId = this.RaiseAndSetIfChanged(ref _lobbyId, value); }
        }

        private string _lobbyName = "";

        public string LobbyName
        {
            get { return _lobbyName; }
            set { _lobbyName = this.RaiseAndSetIfChanged(ref _lobbyName, value); }
        }

        private string _lobbyPassword = "";

        public string LobbyPassword
        {
            get { return _lobbyPassword; }
            set { _lobbyPassword = this.RaiseAndSetIfChanged(ref _lobbyPassword, value); }
        }

        private string _lobbyHost = "";

        public string LobbyHost
        {
            get { return _lobbyHost; }
            set { _lobbyHost = this.RaiseAndSetIfChanged(ref _lobbyHost, value); }
        }

        private ObservableCollection<Entities.Lobby.Player> _players = new ObservableCollection<Entities.Lobby.Player>();

        public ObservableCollection<Entities.Lobby.Player> Players
        {
            get { return _players; }
            set { _players = this.RaiseAndSetIfChanged(ref _players, value); }
        }



        public LobbyViewWindowViewModel(HubConnection hubConnection, LobbyViewWindow lobbyView, GameLobby gameLobby)
        {
            this._hubConnection = hubConnection;
            this._lobbyView = lobbyView;
            this.LobbyHost = gameLobby.Host;
            this.LobbyId = gameLobby.Id;
            this.LobbyName = gameLobby.Name;
            this.LobbyPassword = gameLobby.Password;
            this.Players = new ObservableCollection<Entities.Lobby.Player>(gameLobby.Players);
            _hubConnection.On<GameLobby>("LobbyPlayerJoined", JoinedPlayer);
        }

        private void JoinedPlayer(GameLobby obj)
        {
            this.Players = new ObservableCollection<Entities.Lobby.Player>(obj.Players);
        }

        public async Task Leave()
        {
            await this._hubConnection.SendAsync("LeaveLobby", this.LobbyId);
            this._lobbyView.Close();
        }

        public async Task InitGame()
        {
            this._hubConnection.On<Entities.Game.Player, Card>("GameInitialized", GameInitialized);
            this._hubConnection.On("TurnNotification", TurnNotif);
            await this._hubConnection.SendAsync("InitializeGame", this.LobbyId);
        }

        private async void TurnNotif()
        {
            var messageBoxStandardWindow = MessageBox.Avalonia.MessageBoxManager
  .GetMessageBoxStandardWindow("Debug UI", "Your turn!");
            await messageBoxStandardWindow.Show();
        }

        private async void GameInitialized(Entities.Game.Player obj, Card card)
        {
            var wnd = new GameViewWindow();
            wnd.DataContext = new GameViewWindowViewModel(_hubConnection, wnd, obj, card, this.LobbyId);
            await wnd.ShowDialog(this._lobbyView);
        }
    }
}

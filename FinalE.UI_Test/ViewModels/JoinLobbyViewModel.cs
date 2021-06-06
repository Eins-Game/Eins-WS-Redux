using FinalE.Entities.Transport;
using FinalE.UI_Test.Views;
using Microsoft.AspNetCore.SignalR.Client;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalE.UI_Test.ViewModels
{
    public class JoinLobbyViewModel : ViewModelBase
    {
        private readonly HubConnection _hubConnection;
        private readonly JoinLobby _joinLobby;
        private readonly long _lobbyId;
        private readonly string _username;
        private string _password = "";

        public string LobbyPassword
        {
            get { return _password; }
            set { _password = this.RaiseAndSetIfChanged(ref _password, value); }
        }

        public JoinLobbyViewModel(HubConnection hubConnection, JoinLobby joinLobby, long lobbyId, string username)
        {
            this._hubConnection = hubConnection;
            this._joinLobby = joinLobby;
            this._lobbyId = lobbyId;
            this._username = username;
        }

        public async Task JoinLobby()
        {
            await _hubConnection.SendAsync("JoinLobby", _username, _lobbyId, _password);
        }
    }
}

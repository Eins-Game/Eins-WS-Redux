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
    public class CreateLobbyViewModel : ViewModelBase
    {
        private HubConnection connection { get; set; }
        private readonly CreateLobby _createWindow;
        public string Username { get; set; }

        private string _newLobbyName = "";

        public string NewLobbyName
        {
            get { return _newLobbyName; }
            set { _newLobbyName = this.RaiseAndSetIfChanged(ref _newLobbyName, value); }
        }

        private string _newLobbyPassword = "";

        public string NewLobbyPassword
        {
            get { return _newLobbyPassword; }
            set { _newLobbyPassword = this.RaiseAndSetIfChanged(ref _newLobbyPassword, value); }
        }

        public CreateLobbyViewModel(HubConnection hubConnection, CreateLobby createWindow, string username)
        {
            this.connection = hubConnection;
            this._createWindow = createWindow;
            this.Username = username;
        }

        public async Task CreateLobby()
        {
            await this.connection.SendAsync("CreateLobby", Username, NewLobbyName, NewLobbyPassword);
            this._createWindow.Close();
        }
    }
}

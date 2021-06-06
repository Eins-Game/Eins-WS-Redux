using FinalE.Entities;
using IdGen;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalE.WS.Hubs
{
    public partial class SocketHub : Hub
    {
        private readonly ILogger<SocketHub> _logger;
        private readonly IdGenerator _generator;
        private readonly ConcurrentDictionary<long, GameLobby> _lobbies;

        public SocketHub(ILogger<SocketHub> logger,
            IdGenerator generator,
            ConcurrentDictionary<long, GameLobby> lobbies)
        {
            this._logger = logger;
            this._generator = generator;
            this._lobbies = lobbies;
        }

        public async Task GetLobbies()
        {
            var lobbies = this._lobbies.Select(x => new Entities.Transport.GameLobby
            {
                GameRunning = x.Value.Game != null,
                Id = x.Value.Id,
                Name = x.Value.Name,
                Host = x.Value.Host,
                Password = x.Value.Password == "" ? "" :null,
                Players = x.Value.Players
            });
            await this.Clients.Caller.SendAsync("Lobbies", lobbies);
        }

        public async Task CreateLobby(string username, string name, string password = default)
        {
            if (this._lobbies.Any(x => x.Value.Players.Any(x => x.ConnectionId == this.Context.ConnectionId)))
            {
                return;
            }
            if (password == default)
                password = "";
            var lobby = new GameLobby(this._generator.CreateId(), this.Context.ConnectionId, name, password);
            await lobby.AddPlayer(this.Context.ConnectionId, username);
            this._lobbies.TryAdd(lobby.Id, lobby);
            await this.Clients.Others.SendAsync("LobbyCreated", new Entities.Transport.GameLobby
            {
                Host = lobby.Host,
                Id = lobby.Id,
                Name = lobby.Name,
                Password = lobby.Password == "" ? "" : null,
                Players = lobby.Players,
                GameRunning = false
            });
            await this.Clients.Caller.SendAsync("LobbyCreated", new Entities.Transport.GameLobby
            {
                Host = lobby.Host,
                Id = lobby.Id,
                Name = lobby.Name,
                Password = lobby.Password,
                Players = lobby.Players,
                GameRunning = false
            });
        }

        public async Task JoinLobby(string username, long lobbyId, string password = default)
        {
            if (this._lobbies.Any(x => x.Value.Players.Any(x => x.ConnectionId == this.Context.ConnectionId)))
                return;

            if (password == default)
                password = "";

            if (this._lobbies[lobbyId].Password != password)
                return;

            var lobby = this._lobbies[lobbyId];
            var pl = await lobby.AddPlayer(this.Context.ConnectionId, username);
            await this.Clients.AllExcept(lobby.Players.Select(x => x.ConnectionId)).SendAsync("LobbyUpdated", new Entities.Transport.GameLobby
            {
                Host = lobby.Host,
                Id = lobby.Id,
                Name = lobby.Name,
                Password = lobby.Password == "" ? "" : null,
                Players = lobby.Players,
                GameRunning = false
            });
            await this.Clients.Clients(lobby.Players.Select(x => x.ConnectionId)).SendAsync("LobbyPlayerJoined", new Entities.Transport.GameLobby
            {
                Host = lobby.Host,
                Id = lobby.Id,
                Name = lobby.Name,
                Password = lobby.Password,
                Players = lobby.Players,
                GameRunning = false
            });
        }

        public async Task LeaveLobby(long lobbyId)
        {
            var lobby = this._lobbies[lobbyId];
            await lobby.RemovePlayer(this.Context.ConnectionId);
            if (lobby.Players.Count == 0)
            {
                this._lobbies.TryRemove(lobbyId, out _);
                await this.Clients.All.SendAsync("LobbyRemoved", lobbyId);
            }
            else
            {
                await this.Clients.AllExcept(lobby.Players.Select(x => x.ConnectionId)).SendAsync("LobbyUpdated", new Entities.Transport.GameLobby
                {
                    Host = lobby.Host,
                    Id = lobby.Id,
                    Name = lobby.Name,
                    Password = lobby.Password == "" ? "" : null,
                    Players = lobby.Players,
                    GameRunning = false
                });
                await this.Clients.Clients(lobby.Players.Select(x => x.ConnectionId)).SendAsync("LobbyPlayerLeft", new Entities.Transport.GameLobby
                {
                    Host = lobby.Host,
                    Id = lobby.Id,
                    Name = lobby.Name,
                    Password = lobby.Password,
                    Players = lobby.Players,
                    GameRunning = false
                });
            }
        }
    }
}

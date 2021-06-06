using FinalE.Entities.Game;
using FinalE.Entities.Transport;
using IdGen;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalE.WS.Hubs
{
    public partial class SocketHub : Hub
    {
        public async Task InitializeGame(long lobbyId)
        {
            var lobby = this._lobbies[lobbyId];
            if (lobby.Host != this.Context.ConnectionId)
                return;
            await lobby.StartGame();
            foreach (var item in lobby.Players)
            {
                var p = await lobby.Game.AddPlayer(item.ConnectionId, item.Username);
                await this.Clients.Client(item.ConnectionId).SendAsync("GameInitialized", p, lobby.Game.Cards.Peek());
            }
            await this.Clients.Client(lobby.Game.CurrentPlayer).SendAsync("TurnNotification");
            await this.Clients.AllExcept(lobby.Players.Select(x => x.ConnectionId)).SendAsync("LobbyUpdated", new Entities.Transport.GameLobby
            {
                Host = lobby.Host,
                Id = lobby.Id,
                Name = lobby.Name,
                Password = lobby.Password == "" ? "" : null,
                Players = lobby.Players,
                GameRunning = true
            });
        }

        public async Task PlayCard(long lobbyId, Card card)
        {
            var lobby = this._lobbies[lobbyId];
            if (lobby.Game.CurrentPlayer != this.Context.ConnectionId)
                return;

            await lobby.Game.PushCard(this.Context.ConnectionId, card);
            await lobby.Game.SetNextPlayer();
            await this.Clients.Clients(lobby.Game.Players
                .Select(x => x.Value.ConnectionId))
                .SendAsync("CardPlayed", new CardEvent
                {
                    Card = card,
                    ConnectionId = this.Context.ConnectionId
                });
            if (lobby.Game.Players.Any(x => x.Value.Cards.Count == 0))
            {
                await this.Clients.Clients(lobby.Game.Players
                .Select(x => x.Value.ConnectionId))
                .SendAsync("GameEnded", lobby.Players.First(x => x.ConnectionId == this.Context.ConnectionId));
                lobby.Game = null;
                return;
            }
            await this.Clients.Client(lobby.Game.CurrentPlayer).SendAsync("TurnNotification");
        }

        public async Task DrawCard(long lobbyId)
        {
            var lobby = this._lobbies[lobbyId];
            if (lobby.Game.CurrentPlayer != this.Context.ConnectionId)
                return;
            var card = await lobby.Game.DrawCard(this.Context.ConnectionId);
            await this.Clients.Clients(lobby.Game.Players
                .Where(x => x.Value.ConnectionId != this.Context.ConnectionId)
                .Select(x => x.Value.ConnectionId))
                .SendAsync("DrawnCard", new CardEvent
                {
                    Card = null,
                    ConnectionId = this.Context.ConnectionId
                });
            await this.Clients.Caller.SendAsync("DrawnCard", new CardEvent
            {
                Card = card,
                ConnectionId = this.Context.ConnectionId
            });
        }
    }
}

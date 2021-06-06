using FinalE.Entities.Lobby;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalE.Entities
{
    public class GameLobby
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public List<Player> Players { get; set; } = new();
        public CardGame Game { get; set; }

        public GameLobby(long id, string host, string name, string password = default)
        {
            this.Id = id;
            this.Host = host;
            this.Name = name;
            this.Password = password;
        }

        public Task<Player> AddPlayer(string connectionId, string username)
        {
            var p = new Player
            {
                ConnectionId = connectionId,
                Username = username
            };
            this.Players.Add(p);
            return Task.FromResult(p);
        }

        public Task<bool> RemovePlayer(string connectionId)
        {
            var p = this.Players.FirstOrDefault(x => x.ConnectionId == connectionId);
            if (p == default) 
                return Task.FromResult(true);
            this.Players.Remove(p);
            if (connectionId == this.Host)
                this.Host = this.Players[0].ConnectionId;
            return Task.FromResult(true);
        }

        public Task<bool> StartGame()
        {
            this.Game = new CardGame(this.Id);
            return Task.FromResult(true);
        }

    }
}

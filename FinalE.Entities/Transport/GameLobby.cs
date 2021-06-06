using FinalE.Entities.Lobby;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalE.Entities.Transport
{
    public class GameLobby
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public bool GameRunning { get; set; }
        public List<Player> Players { get; set; } = new();
    }
}

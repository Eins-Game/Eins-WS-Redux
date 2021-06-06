using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalE.Entities.Game
{
    public class Player
    {
        public string Username { get; set; }
        public string ConnectionId { get; set; }
        public List<Card> Cards { get; set; } = new();

    }
}

using FinalE.Entities.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalE.Entities.Transport
{
    public class CardEvent
    {
        public string ConnectionId { get; set; }
        public Card Card { get; set; }
    }
}

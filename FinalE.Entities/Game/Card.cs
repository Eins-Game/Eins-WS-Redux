using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalE.Entities.Game
{
    public class Card
    {
        public int Value { get; set; }

        public CardColor Color { get; set; }
    }

    [Flags]
    public enum CardColor
    {
        Red = 1,
        Green = 2,
        Yellow = 4,
        Blue = 8
    }
}

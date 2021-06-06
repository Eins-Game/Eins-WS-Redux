using FinalE.Entities.Game;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalE.Entities
{
    public class CardGame
    {
        private readonly Random rnd = new();

        public long Id { get; set; }
        public Dictionary<int, Player> Players { get; set; } = new();
        public string CurrentPlayer { get; set; }
        public Stack<Card> Cards { get; set; } = new();
        public bool Ready { get; set; } = false;

        public CardGame(long id)
        {
            this.Id = id;
            this.Cards.Push(GetRandomCard());
        }

        public Task<Player> AddPlayer(string connectionId, string username)
        {
            var p = new Player
            {
                ConnectionId = connectionId,
                Username = username
            };
            for (int i = 0; i < 7; i++)
            {
                p.Cards.Add(this.GetRandomCard());
            }
            this.Players.Add(this.Players.Count, p);
            if (this.Players.Count == 1)
                this.CurrentPlayer = connectionId;
            return Task.FromResult(p);
        }

        public Task<bool> PushCard(string connectionId, Card card)
        {
            if (this.CurrentPlayer != connectionId)
                return Task.FromResult(false);
            var topCard = this.Cards.Peek();
            var player = this.Players.First(x => x.Value.ConnectionId == connectionId);
            var playerCard = player.Value.Cards.FirstOrDefault(x => x.Color == card.Color && x.Value == card.Value);
            if (playerCard == default || this.CurrentPlayer != connectionId)
                return Task.FromResult(false);
            if (card.Color != topCard.Color && card.Value != topCard.Value)
                return Task.FromResult(false);
            this.Cards.Push(card);
            player.Value.Cards.Remove(playerCard);
            return Task.FromResult(true);
        }

        public Task<Card> DrawCard(string connectionId)
        {
            var card = GetRandomCard();
            var player = this.Players.First(x => x.Value.ConnectionId == connectionId);
            player.Value.Cards.Add(card);
            return Task.FromResult(card);
        }

        public Task<Player> SetNextPlayer() 
        {
            var current = this.Players.First(x => x.Value.ConnectionId == CurrentPlayer);
            var index = Array.IndexOf(this.Players.ToArray(), current);
            index++;
            if (index == this.Players.Count)
                index = 0;
            this.CurrentPlayer = this.Players.ElementAt(index).Value.ConnectionId;
            return Task.FromResult(this.Players.ElementAt(index).Value);
        }


        private Card GetRandomCard()
        {
            var values = Enum.GetValues(typeof(CardColor));
            var color = values.GetValue(this.rnd.Next(0,4));
            var cardvalue = rnd.Next(0, 10);
            return new Card
            {
                Color = (CardColor)color,
                Value = cardvalue
            };
        }
    }
}

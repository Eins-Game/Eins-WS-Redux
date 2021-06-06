using FinalE.Entities.Game;
using FinalE.Entities.Transport;
using FinalE.UI_Test.Views;
using Microsoft.AspNetCore.SignalR.Client;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalE.UI_Test.ViewModels
{
    public class GameViewWindowViewModel : ViewModelBase
    {
        private readonly HubConnection _hubConnection;
        private readonly GameViewWindow _gameViewWindow;
        private readonly Player _player;
        private readonly long _lobbyId;
        private CardColor _topColor;

        public CardColor TopColor
        {
            get { return _topColor; }
            set { _topColor = this.RaiseAndSetIfChanged(ref _topColor, value); }
        }

        private int _topValue;

        public int TopValue
        {
            get { return _topValue; }
            set { _topValue = this.RaiseAndSetIfChanged(ref _topValue, value); }
        }


        private ObservableCollection<Card> _cards = new ObservableCollection<Card>();

        public ObservableCollection<Card> Cards
        {
            get { return _cards; }
            set { _cards = this.RaiseAndSetIfChanged(ref _cards, value); }
        }

        public GameViewWindowViewModel(HubConnection hubConnection, GameViewWindow gameViewWindow, Player player, Card topcard, long lobbyId)
        {
            this._hubConnection = hubConnection;
            this._gameViewWindow = gameViewWindow;
            this._player = player;
            this._lobbyId = lobbyId;
            this.Cards = new ObservableCollection<Card>(_player.Cards);
            this.TopColor = topcard.Color;
            this.TopValue = topcard.Value;
            this._hubConnection.On<CardEvent>("CardPlayed", CardPlayed);
            this._hubConnection.On<CardEvent>("DrawnCard", CardDrawn);
        }

        private void CardDrawn(CardEvent obj)
        {
            if (this._hubConnection.ConnectionId == obj.ConnectionId)
                this.Cards.Add(obj.Card);
        }

        private void CardPlayed(CardEvent obj)
        {
            if (this._hubConnection.ConnectionId == obj.ConnectionId)
            {
                var myCard = this.Cards.First(x => x.Color == obj.Card.Color && x.Value == obj.Card.Value);
                this.Cards.Remove(myCard);
            }
            this.TopColor = obj.Card.Color;
            this.TopValue = obj.Card.Value;
        }

        public async Task PlayCard(Card card)
        {
            await this._hubConnection.SendAsync("PlayCard", this._lobbyId, card);
        }

        public async Task DrawCard()
        {
            await this._hubConnection.SendAsync("DrawCard", this._lobbyId);
        }
    }
}

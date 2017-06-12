using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _21BlackJack.Cards_FrameWork
{
    public class CardEvents : EventArgs
    {
        public TraditionalCard Card { get; set; } //dá erro porque falta o file Traditional.cs
    }
    public class CardPacket
    {
        #region Field Property Indexer
        protected List<TraditionalCard> cards { get; set; }

        public event EventHandler<CardEvents> LostCard;
        public int Count { get { return cards.Count; } }

        protected CardPacket()
        {
            cards = new List<TraditionalCard>();
        }
        public TraditionalCard this[int index]
        {
            get { return cards[index]; }
        }
        #endregion

        #region Inicializações
        public CardPacket(int numberOfDecks, int jokersInDeck, CardSuit suits, CardValue cardValues)
        {
            cards = new List<TraditionalCard>();

            for (int deckIndex = 0; deckIndex < numberOfDecks; deckIndex++)
            {
                AddSuit(suits, cardValues);

                for (int j = 0; j < jokersInDeck / 2; j++)
                {
                    cards.Add(new TraditionalCard(CardSuit.Club, CardValue.FirstJoker, this));
                    cards.Add(new TraditionalCard(CardSuit.Club, CardValue.SecondJoker, this));
                }

                if (jokersInDeck % 2 == 1)
                {
                    cards.Add(new TraditionalCard(CardSuit.Club, CardValue.FirstJoker, this));
                }
            }
        }
        #endregion

        #region Private Methods
        private void AddSuit(CardSuit suits, CardValue cardValues)
        {
            if ((suits & CardSuit.Club) == CardSuit.Club)
            {
                AddCards(CardSuit.Club, cardValues);
            }

            if ((suits & CardSuit.Diamond) == CardSuit.Diamond)
            {
                AddCards(CardSuit.Diamond, cardValues);
            }

            if ((suits & CardSuit.Heart) == CardSuit.Heart)
            {
                AddCards(CardSuit.Heart, cardValues);
            }

            if ((suits & CardSuit.Spade) == CardSuit.Spade)
            {
                AddCards(CardSuit.Spade, cardValues);
            }
        }

        private void AddCards(CardSuit suit, CardValue cardValues)
        {
            if ((cardValues & CardValue.Ace) == CardValue.Ace)
                cards.Add(new TraditionalCard(suit, CardValue.Ace, this));
            if ((cardValues & CardValue.Two) == CardValue.Two)
                cards.Add(new TraditionalCard(suit, CardValue.Two, this));
            if ((cardValues & CardValue.Three) == CardValue.Three)
                cards.Add(new TraditionalCard(suit, CardValue.Three, this));
            if ((cardValues & CardValue.Four) == CardValue.Four)
                cards.Add(new TraditionalCard(suit, CardValue.Four, this));
            if ((cardValues & CardValue.Five) == CardValue.Five)
                cards.Add(new TraditionalCard(suit, CardValue.Five, this));
            if ((cardValues & CardValue.Six) == CardValue.Six)
                cards.Add(new TraditionalCard(suit, CardValue.Six, this));
            if ((cardValues & CardValue.Seven) == CardValue.Seven)
                cards.Add(new TraditionalCard(suit, CardValue.Seven, this));
            if ((cardValues & CardValue.Eight) == CardValue.Eight)
                cards.Add(new TraditionalCard(suit, CardValue.Eight, this));
            if ((cardValues & CardValue.Nine) == CardValue.Nine)
                cards.Add(new TraditionalCard(suit, CardValue.Nine, this));
            if ((cardValues & CardValue.Ten) == CardValue.Ten)
                cards.Add(new TraditionalCard(suit, CardValue.Ten, this));
            if ((cardValues & CardValue.Jack) == CardValue.Jack)
                cards.Add(new TraditionalCard(suit, CardValue.Jack, this));
            if ((cardValues & CardValue.Queen) == CardValue.Queen)
                cards.Add(new TraditionalCard(suit, CardValue.Queen, this));
            if ((cardValues & CardValue.King) == CardValue.King)
                cards.Add(new TraditionalCard(suit, CardValue.King, this));
        }
        #endregion

        #region Public Methods
        public void Shuffle()
        {
            Random random = new Random();
            List<TraditionalCard> shuffleDeck = new List<TraditionalCard>();

            while (cards.Count > 0)
            {
                TraditionalCard card = cards[random.Next(0, cards.Count)];
                cards.Remove(card);
                shuffleDeck.Add(card);
            }

            cards = shuffleDeck;
        }

        internal TraditionalCard Remove(TraditionalCard card)
        {
            if (cards.Contains(card))
            {
                cards.Remove(card);

                if (LostCard != null)
                {
                    LostCard(this, new CardEvents() { Card = card });
                }
                return card;
            }
            return null;
        }

        internal List<TraditionalCard> Remove()
        {
            List<TraditionalCard> cards = this.cards;
            this.cards = new List<TraditionalCard>();
            return cards;
        }

        public TraditionalCard DealCardToHand(Hand destinationHand)
        {
            TraditionalCard firstCard = cards[0];
            firstCard.MoveToHand(destinationHand);
            return firstCard;
        }
        public List<TraditionalCard> DealCardsToHand(Hand destinationHand, int count)
        {
            List<TraditionalCard> dealtCards = new List<TraditionalCard>();

            for (int cardIndex = 0; cardIndex < count; cardIndex++)
            {
                dealtCards.Add(DealCardToHand(destinationHand));
            }

            return dealtCards;
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _21BlackJack.Cards_FrameWork
{
    public class Hand : CardPacket
    {
        public event EventHandler<CardEvents> ReceivedCard;

        #region Internal Methods
        internal void Add(TraditionalCard card)
        {
            cards.Add(card);
            if(ReceivedCard != null)
            {
                ReceivedCard(this, new CardEvents() { Card = card });
            }
        }

        internal void Add(IEnumerable<TraditionalCard> cards)
        {
            this.cards.AddRange(cards);
        }
        #endregion
    }
}

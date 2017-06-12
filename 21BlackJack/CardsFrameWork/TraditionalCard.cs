using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _21BlackJack.Cards_FrameWork
{
    [Flags]
    public enum CardSuit
    {
        Heart = 0x01,
        Diamond = 0x02,
        Club=0x04,
        Spade=0x08,
        //Sets
        AllSuits=Heart|Diamond|Club|Spade
    }

    [Flags]
    public enum CardValue
    {
        Ace = 0x01,
        Two = 0x02,
        Three=0x04,
        Four=0x08,
        Five=0x010,
        Six=0x020,
        Seven=0x40,
        Eight=0x80,
        Nine=0x100,
        Ten=0x200,
        Jack=0x400,
        Queen=0x800,
        King=0x1000,
        FirstJoker=0x2000,
        SecondJoker=0x4000,
        //Sets
        AllNumbers = 0x3FF,
        NonJokers = 0x1FF,
        Jokers = FirstJoker | SecondJoker,
        AllFigures = Jack | Queen | King,
    }

    public class TraditionalCard
    {
        public CardSuit Type { get; set; }
        public CardValue Value { get; set; }
        public CardPacket HoldingCardCollection;

        #region Inicialização
        internal TraditionalCard(CardSuit type, CardValue value, CardPacket holdingCardCollection)
        {
            switch (type)
            {
                case CardSuit.Club:
                case CardSuit.Diamond:
                case CardSuit.Heart:
                case CardSuit.Spade:
                    break;

                default:
                    {
                        throw new ArgumentException(
                            "type must be single value", "type");
                    }
            }

            switch (value)
            {
                case CardValue.Ace:
                case CardValue.Two:
                case CardValue.Three:
                case CardValue.Four:
                case CardValue.Five:
                case CardValue.Six:
                case CardValue.Seven:
                case CardValue.Eight:
                case CardValue.Nine:
                case CardValue.Ten:
                case CardValue.Jack:
                case CardValue.Queen:
                case CardValue.King:
                case CardValue.FirstJoker:
                case CardValue.SecondJoker:
                    break;
                default:
                    {
                        throw new ArgumentException(
                            "value must be single value", "value");
                    }
            }

            Type = type;
            Value = value;
            HoldingCardCollection = holdingCardCollection;
        }
        #endregion

        public void MoveToHand(Hand hand) 
        {
            HoldingCardCollection.Remove(this);
            HoldingCardCollection = hand;
            hand.Add(this);
        }
    }
}

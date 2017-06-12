using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _21BlackJack.Cards_FrameWork;

namespace _21BlackJack.CardsGame.Players
{
    public enum HandTypes
    {
        First,
        Second
    }

    public class BlackjackPlayer : Cards_FrameWork.Player.Player
    {
        #region Fields/Properties

        private int firstValue;
        private bool firstValueConsiderAce;

        private int secondValue;
        private bool secondValueConsiderAce;

        public bool Bust { get; set; }
        public bool SecondBust { get; set; }
        public bool BlackJack { get; set; }
        public bool SecondBlackJack { get; set; }
        public bool Double { get; set; }
        public bool SecoundDouble { get; set; }

        public bool IsSplit { get; set; }
        public Hand SecondHand { get; private set; }
        public HandTypes CurrentHandType { get; set; }

        public Hand CurrentHand
        {
            get
            {
                switch (CurrentHandType)
                {
                    case HandTypes.First:
                        return Hand;
                    case HandTypes.Second:
                        return SecondHand;
                    default:
                        throw new Exception("No hand to return");
                }
            }
        }

        public int FirstValue
        {
            get { return firstValue; }
        }
        public bool FirstValueConsiderAce
        {
            get { return firstValueConsiderAce; }
        }

        public int SecondValue
        {
            get { return secondValue; }
        }
        public bool SecondValueConsiderAce
        {
            get { return secondValueConsiderAce; }
        }

        public bool MadeBet { get { return BetAmount > 0; } }
        public bool IsDoneBetting { get; set; }
        public float Balance { get; set; }
        public float BetAmount { get; private set; }
        public bool IsInsurance { get; set; }
        #endregion

        public BlackjackPlayer(string name, Cards_FrameWork.Game.CardsGame game)
            :base(name, game)
        {
            Balance = 500;
            CurrentHandType = HandTypes.First;
        }

        private static void CalculateValue(Hand hand, Cards_FrameWork.Game.CardsGame game, out int value, out bool considerAce)
        {
            value = 0;
            considerAce = false;

            for(int cardIndex =0; cardIndex < hand.Count; cardIndex++)
            {
                value += game.CardValue(hand[cardIndex]);

                if (hand[cardIndex].Value == CardValue.Ace)
                    considerAce = true;

                if (considerAce && value + 10 > 21)
                    considerAce = false;
            }
        }

        #region Public Methods
        public bool Bet(float amount)
        {
            if (amount > Balance)
                return false;

            BetAmount += amount;
            Balance -= amount;
            return true;
        }

        public void ClearBet()
        {
            Balance += BetAmount;
            BetAmount = 0;
        }

        public void CalculateValues()
        {
            CalculateValue(Hand, Game, out firstValue, out firstValueConsiderAce);

            if (SecondHand != null)
                CalculateValue(SecondHand, Game, out secondValue, out secondValueConsiderAce);
        }

        public void ResetValues()
        {
            BlackJack = false;
            SecondBlackJack = false;
            Bust = false;
            Double = false;
            SecoundDouble = false;
            firstValue = 0;
            firstValueConsiderAce = false;
            IsSplit = false;
            secondValue = 0;
            secondValueConsiderAce = false;
            BetAmount = 0;
            IsDoneBetting = false;
            IsInsurance = false;
            CurrentHandType = HandTypes.First;
        }

        public void InitializeSecondHand()
        {
            SecondHand = new Hand();
        }

        public void SplitHand()
        {
            if (SecondHand == null)
                throw new InvalidOperationException("Second hand is not initialized.");
            if (IsSplit == true)
                throw new InvalidOperationException("A hand cannot be split more than once.");
            if (Hand.Count != 2)
                throw new InvalidOperationException("You must have 2 cards to perform a split.");
            if (Hand[0].Value != Hand[1].Value)
                throw new InvalidOperationException("You can only split when both cards are of identical value.");
            IsSplit = true;
            Hand[1].MoveToHand(SecondHand);
        }
        #endregion
    }
}

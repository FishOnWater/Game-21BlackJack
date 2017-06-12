using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _21BlackJack.CardsGame.Players
{
    class BlackJackAIPlayer : BlackjackPlayer
    {
        static Random random = new Random();

        public event EventHandler Hit;
        public event EventHandler Stand;

        public BlackJackAIPlayer(string name, Cards_FrameWork.Game.CardsGame game)
            :base(name, game) { }

        public void AIPlay()
        {
            int value = FirstValue;
            if(FirstValueConsiderAce && value + 10 <= 21)
            {
                value += 10;
            }
            if (value < 17 && Hit != null)
                Hit(this, EventArgs.Empty);
            else if (Stand != null)
                Stand(this, EventArgs.Empty);
        }

        public int AIBet()
        {
            int[] chips = { 0, 5, 25, 100, 500 };
            int bet = chips[random.Next(0, chips.Length)];

            if (bet < Balance)
                return bet;
            return 0;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _21BlackJack.Cards_FrameWork;

namespace _21BlackJack.CardsGame.Rules
{
    public class BlackJackGameEventArgs : EventArgs
    {
        public Cards_FrameWork.Player.Player Player { get; set;}
        public _21BlackJack.CardsGame.Players.HandTypes Hand { get; set; }
    }
}

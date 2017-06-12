using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _21BlackJack.Cards_FrameWork.Player
{
    public class Player
    {
        #region Property
        public string Name { get; set; }
        public Cards_FrameWork.Game.CardsGame Game { get; set; } //erro couse no CardsGame.cs
        public Hand Hand { get; set; }
        #endregion

        public Player(string name, Cards_FrameWork.Game.CardsGame game)
        {
            Name = name;
            Game = game;
            Hand = new Hand();
        }
    }
}

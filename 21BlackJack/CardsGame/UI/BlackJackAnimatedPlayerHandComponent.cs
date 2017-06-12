using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using _21BlackJack.Cards_FrameWork;

namespace _21BlackJack.CardsGame.UI
{
    public class BlackJackAnimatedPlayerHandComponent : _21BlackJack.CardsFrameWork.UserInterface.AnimatedHandGameComponent
    {
        Vector2 offSet;

        public BlackJackAnimatedPlayerHandComponent(int place, Hand hand, Cards_FrameWork.Game.CardsGame cardGame)
            :base (place, hand, cardGame)
        {
            this.offSet = Vector2.Zero;
        }

        public BlackJackAnimatedPlayerHandComponent(int place, Vector2 offSet, Hand hand, Cards_FrameWork.Game.CardsGame cardGame)
            :base(place, hand, cardGame)
        {
            this.offSet = offSet;
        }
        public override Vector2 GetCardRelativePosition(int cardLocationInHand)
        {
            return new Vector2(25 * cardLocationInHand, -30 * cardLocationInHand) + offSet;
        }
    }
}

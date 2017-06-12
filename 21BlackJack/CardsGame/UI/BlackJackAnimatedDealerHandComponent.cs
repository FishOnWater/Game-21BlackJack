using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _21BlackJack.Cards_FrameWork;
using Microsoft.Xna.Framework;

namespace _21BlackJack.CardsGame.UI
{
    public class BlackJackAnimatedDealerHandComponent : _21BlackJack.CardsFrameWork.UserInterface.AnimatedHandGameComponent
    {
        public BlackJackAnimatedDealerHandComponent(int place, Hand hand, Cards_FrameWork.Game.CardsGame cardGame)
            :base(place, hand, cardGame)
        {
        }

        public override Vector2 GetCardRelativePosition(int cardLocationInHand)
        {
            return new Vector2(30 * cardLocationInHand, 0);
        }
    }
}

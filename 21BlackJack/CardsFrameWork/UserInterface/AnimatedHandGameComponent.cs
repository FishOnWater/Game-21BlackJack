using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _21BlackJack.Cards_FrameWork;
using BlackJack21.CardFramework;
using Microsoft.Xna.Framework;

namespace _21BlackJack.CardsFrameWork.UserInterface
{
    public class AnimatedHandGameComponent : AnimatedGameComponent
    {
        public int Place { get; private set; }
        public readonly Hand hand;

        List<AnimatedCardsGameComponent> heldAnimationCards = new List<AnimatedCardsGameComponent>();

        public override bool IsAnimating
        {
            get
            {
                for (int animationIndex = 0; animationIndex < heldAnimationCards.Count; animationIndex++)
                {
                    if (heldAnimationCards[animationIndex].IsAnimating)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public IEnumerable<AnimatedGameComponent> AnimateCards
        {
            get
            {
                return heldAnimationCards.AsReadOnly();
            }
        }

        public AnimatedHandGameComponent(int place, Hand hand, Cards_FrameWork.Game.CardsGame cardGame)
            : base(cardGame, null)
        {
            Place = place;
            hand.ReceivedCard += Hand_ReceiveCard;
            hand.LostCard += Hand_LostCard;

            if (place == -1)
            {
                CurrentPosition = CardGame.GameTable.DealerPosition;
            }
            else
            {
                CurrentPosition = CardGame.GameTable[place];
            }

            for (int cardIndex = 0; cardIndex < hand.Count; cardIndex++)
            {
                AnimatedCardsGameComponent animatedCardGameComponent = new AnimatedCardsGameComponent(hand[cardIndex], CardGame)
                {
                    CurrentPosition = CurrentPosition + new Vector2(30 * cardIndex, 0)
                };
                heldAnimationCards.Add(animatedCardGameComponent);
                Game.Components.Add(animatedCardGameComponent);
            }
            Game.Components.ComponentRemoved += Components_ComponentRemoved;
        }

        public override void Update(GameTime gameTime)
        {
            for(int animationIndex=0; animationIndex < heldAnimationCards.Count; animationIndex++)
            {
                if (!heldAnimationCards[animationIndex].IsAnimating)
                {
                    heldAnimationCards[animationIndex].CurrentPosition = CurrentPosition + GetCardRelativePosition(animationIndex);
                }
            }
            base.Update(gameTime);
        }

        public virtual Vector2 GetCardRelativePosition(int cardLocationInHand)
        {
            return default(Vector2);
        }

        public int GetCardLocationInHand(TraditionalCard card)
        {
            for(int animationIndex =0; animationIndex < heldAnimationCards.Count; animationIndex++)
            {
                if (heldAnimationCards[animationIndex].Card == card)
                {
                    return animationIndex;
                }
            }
            return -1;
        }

        public AnimatedCardsGameComponent GetCardGameComponent(TraditionalCard card, int value)
        {
            int location = GetCardLocationInHand(card);
            if (location == -1)
                return null;

            return heldAnimationCards[location];
        }

        void Components_ComponentRemoved(object sender, GameComponentCollectionEventArgs e)
        {
            if (e.GameComponent == this)
                Dispose();
        }

        void Hand_LostCard(object sender, CardEvents e)
        {
            for(int animationIndex=0; animationIndex<heldAnimationCards.Count; animationIndex++)
            {
                if (heldAnimationCards[animationIndex].Card == e.Card)
                {
                    Game.Components.Remove(heldAnimationCards[animationIndex]);
                    heldAnimationCards.RemoveAt(animationIndex);
                    return;
                }
            }
        }

        void Hand_ReceiveCard(object sender, CardEvents e)
        {
            AnimatedCardsGameComponent animatedCardGameComponent =
                new AnimatedCardsGameComponent(e.Card, CardGame) { Visible = false };

            heldAnimationCards.Add(animatedCardGameComponent);
            Game.Components.Add(animatedCardGameComponent);
        }

        public override TimeSpan EstimatedTimeForAnimationsCompletion()
        {
            TimeSpan result = TimeSpan.Zero;

            if (IsAnimating)
            {
                for(int animationIndex=0; animationIndex<heldAnimationCards.Count; animationIndex++)
                {
                    if (heldAnimationCards[animationIndex].EstimatedTimeForAnimationsCompletion() > result)
                        result = heldAnimationCards[animationIndex].EstimatedTimeForAnimationsCompletion();
                }
            }
            return result;
        }

        protected override void Dispose(bool disposing)
        {
            hand.ReceivedCard -= Hand_ReceiveCard;
            hand.LostCard -= Hand_LostCard;
            base.Dispose(disposing);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using _21BlackJack.Cards_FrameWork;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace BlackJack21.CardFramework
{
    public class AnimatedCardsGameComponent : AnimatedGameComponent
    {
        public _21BlackJack.Cards_FrameWork.TraditionalCard Card { get; private set; }

        public AnimatedCardsGameComponent(TraditionalCard card, _21BlackJack.Cards_FrameWork.Game.CardsGame cardGame) : base(cardGame, null)
        {
            Card = card;
        }

        public override void Update(GameTime gametime)
        {
            base.Update(gametime);
            CurrentFrame = IsFaceDown ? CardGame.cardsAssets["CardBack_" + CardGame.Theme] : CardGame.cardsAssets[_21BlackJack.Cards_FrameWork.Utility.UIUtility.GetCardAssetName(Card)];
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            CardGame.SpriteBatch.Begin();
            if (CurrentFrame != null)
            {
                if (CurrentDestination.HasValue)
                {
                    CardGame.SpriteBatch.Draw(CurrentFrame,CurrentDestination.Value, Color.White);
                }
                else
                {
                    CardGame.SpriteBatch.Draw(CurrentFrame,CurrentPosition, Color.White);
                }
            }
            CardGame.SpriteBatch.End();
        }
    }
}

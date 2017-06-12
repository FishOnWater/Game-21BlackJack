using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BlackJack21.CardFramework
{
    public class AnimatedGameComponent : DrawableGameComponent
    {
        public Texture2D CurrentFrame { get; set; }
        public Rectangle? CurrentSegment { get; set; }
        public string Text { get; set; }
        public Color TextColor { get; set; }
        public bool IsFaceDown = true;
        public Vector2 CurrentPosition { get; set; }
        public Rectangle? CurrentDestination { get; set; }

        List<AnimatedGameComponentAnimation> runningAnimations = new List<AnimatedGameComponentAnimation>();

        public virtual bool IsAnimating { get { return runningAnimations.Count > 0; } }
        public _21BlackJack.Cards_FrameWork.Game.CardsGame CardGame { get; private set; }

        public AnimatedGameComponent(Game game) : base(game)
        {
            TextColor = Color.Black;
        }

        public AnimatedGameComponent(Game game, Texture2D currentFrame) : this(game)
        {
            CurrentFrame = currentFrame;
        }

        public AnimatedGameComponent(_21BlackJack.Cards_FrameWork.Game.CardsGame cardGame, Texture2D currentFrame) : this(cardGame.Game)
        {
            CardGame = cardGame;
            CurrentFrame = currentFrame;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            for (int animationIndex = 0; animationIndex < runningAnimations.Count; animationIndex++)
            {
                runningAnimations[animationIndex].AccumulateElapsedTime(gameTime.ElapsedGameTime);
                runningAnimations[animationIndex].Run(gameTime);
                if (runningAnimations[animationIndex].IsDone())
                {
                    runningAnimations.RemoveAt(animationIndex);
                    animationIndex--;
                }

            }
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            SpriteBatch spriteBatch;

            if (CardGame != null)
            {
                spriteBatch = CardGame.SpriteBatch;
            }
            else
            {
                spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            }

            spriteBatch.Begin();

            if (CurrentDestination.HasValue)
            {
                if (CurrentFrame != null)
                {
                    spriteBatch.Draw(CurrentFrame, CurrentDestination.Value, CurrentSegment, Color.White);
                    if (Text != null)
                    {
                        Vector2 size = CardGame.Font.MeasureString(Text);
                        Vector2 textPosition = new Vector2(CurrentDestination.Value.X + CurrentDestination.Value.Width / 2 - size.X / 2, CurrentDestination.Value.Y + CurrentDestination.Value.Height / 2 - size.Y / 2);
                        spriteBatch.DrawString(CardGame.Font, Text, textPosition, TextColor);
                    }
                }
            }
            else
            {
                if (CurrentFrame != null)
                {
                    spriteBatch.Draw(CurrentFrame, CurrentPosition, CurrentSegment, Color.White);
                    if (Text != null)
                    {
                        Vector2 size = CardGame.Font.MeasureString(Text);
                        Vector2 textPosition = new Vector2(CurrentPosition.X + CurrentFrame.Bounds.Width / 2 - size.X / 2,CurrentPosition.Y + CurrentFrame.Bounds.Height / 2 - size.Y / 2);
                        spriteBatch.DrawString(CardGame.Font, Text, textPosition, TextColor);
                    }
                }
            }
            spriteBatch.End();
        }

        public void AddAnimation(AnimatedGameComponentAnimation animation)
        {
            animation.Component = this;
            runningAnimations.Add(animation);
        }

        public virtual TimeSpan EstimatedTimeForAnimationsCompletion()
        {
            TimeSpan result = TimeSpan.Zero;

            if (IsAnimating)
            {
                for (int animationIndex = 0; animationIndex < runningAnimations.Count; animationIndex++)
                {
                    if (runningAnimations[animationIndex].EstimatedTimeForAnimationCompletion > result)
                    {
                        result = runningAnimations[animationIndex].EstimatedTimeForAnimationCompletion;
                    }
                }
            }

            return result;
        }
    }
}
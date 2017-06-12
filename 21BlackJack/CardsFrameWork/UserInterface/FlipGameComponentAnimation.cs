using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BlackJack21.CardFramework
{
    public class FlipGameComponentAnimation : AnimatedGameComponentAnimation
    {
        protected int percent = 0;
        public bool IsFromFaceDownToFaceUp = true;

        public override void Run(GameTime gameTime)
        {
            Texture2D texture;
            if (IsStarted())
            {
                if (IsDone())
                {
                    Component.IsFaceDown = !IsFromFaceDownToFaceUp;
                    Component.CurrentDestination = null;
                }
                else
                {
                    texture = Component.CurrentFrame;
                    if (texture != null)
                    {
                        percent += (int)(((gameTime.ElapsedGameTime.TotalMilliseconds / (Duration.TotalMilliseconds / AnimationCycles)) * 100));

                        if (percent >= 100)
                        {
                            percent = 0;
                        }

                        int currentPercent;

                        if (percent < 50)
                        {
                            currentPercent = percent;
                            Component.IsFaceDown = IsFromFaceDownToFaceUp;
                        }
                        else
                        {
                            currentPercent = 100 - percent;
                            Component.IsFaceDown = !IsFromFaceDownToFaceUp;
                        }
                        Component.CurrentDestination =
                            new Rectangle((int)(Component.CurrentPosition.X + texture.Width * currentPercent / 100),
                                (int)Component.CurrentPosition.Y,
                                (int)(texture.Width - texture.Width * currentPercent / 100 * 2), texture.Height);
                    }
                }
            }
        }
    }
}

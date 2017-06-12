using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace BlackJack21.CardFramework
{
    public class ScaleGameComponentAnimation : AnimatedGameComponentAnimation
    {
        float percent = 0;
        float beginFactor;
        float factorDelta;

        public ScaleGameComponentAnimation(float beginFactor, float endFactor)
        {
            this.beginFactor = beginFactor;
            factorDelta = endFactor - beginFactor;
        }

        public override void Run(GameTime gameTime)
        {
            Texture2D texture;
            if (IsStarted())
            {
                texture = Component.CurrentFrame;
                if (texture != null)
                {
                    percent += (float)(gameTime.ElapsedGameTime.TotalSeconds / Duration.TotalSeconds);
                    Rectangle bounds = texture.Bounds;
                    bounds.X = (int)Component.CurrentPosition.X;
                    bounds.Y = (int)Component.CurrentPosition.Y;
                    float currentFactor = beginFactor + factorDelta * percent - 1;
                    bounds.Inflate((int)(bounds.Width * currentFactor), (int)(bounds.Height * currentFactor));
                    Component.CurrentDestination = bounds;
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace BlackJack21.CardFramework
{
    public class TransitionGameComponentAnimation : AnimatedGameComponentAnimation
    {
        Vector2 sourcePosition;
        Vector2 positionDelta;
        Vector2 destinationPosition;
        float percent = 0;

        public TransitionGameComponentAnimation(Vector2 sourcePosition, Vector2 destinationPosition)
        {

            this.destinationPosition = destinationPosition;
            this.sourcePosition = sourcePosition;
            positionDelta = destinationPosition - sourcePosition;
        }

        public override void Run(GameTime gameTime)
        {
            if(IsStarted())
            {
                percent += (float)(gameTime.ElapsedGameTime.TotalSeconds / Duration.TotalSeconds);
                Component.CurrentPosition = sourcePosition + positionDelta * percent;
                if(IsDone())
                {
                    Component.CurrentPosition = destinationPosition;
                }
            }
        }
    }
}

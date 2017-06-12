using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BlackJack21.CardFramework
{
    public class FramesetGameComponentAnimation : AnimatedGameComponentAnimation
    {
        Texture2D framesTexture;
        int numberOfFrames;
        int numberOfFramePerRow;
        Vector2 frameSize;
        private double percent = 0;

        public FramesetGameComponentAnimation(Texture2D framesTexture, int numberOfFrames, int numberOfFramePerRow, Vector2 frameSize)
        {
            this.framesTexture = framesTexture;
            this.numberOfFrames = numberOfFrames;
            this.numberOfFramePerRow = numberOfFramePerRow;
            this.frameSize = frameSize;
        }

        public override void Run(GameTime gameTime)
        {
            if (IsStarted())
            {
                percent += (((gameTime.ElapsedGameTime.TotalMilliseconds / (Duration.TotalMilliseconds / AnimationCycles)) * 100));

                if (percent >= 100)
                {
                    percent = 0;
                }
                int animationIndex = (int)(numberOfFrames * percent / 100);
                Component.CurrentSegment =new Rectangle((int)frameSize.X * (animationIndex % numberOfFramePerRow), (int)frameSize.Y * (animationIndex / numberOfFramePerRow), (int)frameSize.X, (int)frameSize.Y);
                Component.CurrentFrame = framesTexture;

            }
            else
            {
                Component.CurrentFrame = null;
                Component.CurrentSegment = null;
            }
            base.Run(gameTime);
        }
    }
}

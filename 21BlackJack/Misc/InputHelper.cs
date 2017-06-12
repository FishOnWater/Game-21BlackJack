using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace _21BlackJack.Misc
{
    class InputHelper : DrawableGameComponent
    {
        #region Fields

        public bool IsEscape;
        public bool IsPressed;

        Vector2 drawPosition;
        Texture2D texture;
        SpriteBatch spriteBatch;
        float maxVelocity;
        #endregion

        #region Inicializações

        public InputHelper(Game game)
            : base(game)
        {
            texture = Game.Content.Load<Texture2D>(@"Images\GamePadCursor");
            spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            maxVelocity = (float)(Game.GraphicsDevice.Viewport.Width + Game.GraphicsDevice.Viewport.Height) / 3000f;
            drawPosition = new Vector2(Game.GraphicsDevice.Viewport.Width / 2, Game.GraphicsDevice.Viewport.Height / 2);
        }
        #endregion

        #region Properties
        public Vector2 PointPosition
        {
            get
            {
                return drawPosition + new Vector2(texture.Width / 2f, texture.Height / 2f);
            }
        }
        #endregion

        #region Update and Render
        public override void Update(GameTime gameTime)
        {
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);

            IsPressed = gamePadState.Buttons.A == ButtonState.Pressed;
            IsEscape = gamePadState.Buttons.Back == ButtonState.Pressed;

            drawPosition += gamePadState.ThumbSticks.Left
                * new Vector2(1, -1)
                * gameTime.ElapsedGameTime.Milliseconds
                * maxVelocity;
            drawPosition += Vector2.Clamp(drawPosition, Vector2.Zero, new Vector2(Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height) - new Vector2(texture.Bounds.Width, texture.Bounds.Height));
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(texture, drawPosition, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            base.Draw(gameTime);
        }
        #endregion
    }
}

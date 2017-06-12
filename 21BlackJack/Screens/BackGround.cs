using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using _21BlackJack.ScreenManager;

namespace _21BlackJack.Screens
{
    class BackGround : GameScreen
    {
        Texture2D background;
        Rectangle safeArea;

        public BackGround()
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.0);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }

        #region Loading
        public override void LoadContent()
        {
            background = ScreenManager.Game.Content.Load<Texture2D>(@"Images\titlescreen");
            safeArea = ScreenManager.Game.GraphicsDevice.Viewport.TitleSafeArea;
            base.LoadContent();
        }
        #endregion

        #region Update and Render
        public override void Update(GameTime gameTime, bool otherScreeHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreeHasFocus, false);
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.SpriteBatch.Draw(background, ScreenManager.GraphicsDevice.Viewport.Bounds, Color.White * TransitionAlpha);
            ScreenManager.SpriteBatch.End();
            base.Draw(gameTime);
        }
        #endregion
    }
}
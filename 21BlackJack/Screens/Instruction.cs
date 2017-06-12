using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace _21BlackJack.Screens
{
    class Instruction : GameplayScreen
    {
        #region Fields
        Texture2D background;
        SpriteFont font;
        GameplayScreen gameplayScreen;
        string theme;
        bool isExit = false;
        bool isExited = false;
        #endregion

        #region Initialization
        public Instruction(string theme) : base("")
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.0);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
            this.theme = theme;
        }
        #endregion

        #region Loading
        public override void LoadContent()
        {
            background = Load<Texture2D>(@"Images\instructions");
            font = Load<SpriteFont>(@"Fonts\MenuFont");
            gameplayScreen = new GameplayScreen(theme);
        }
        #endregion

        #region Update and Rende
        private void HandleInput(MouseState mouseState, GamePadState padState)
        {
            if (!isExit)
            {
                PlayerIndex result;
                if (mouseState.LeftButton == ButtonState.Pressed)
                {
                    isExit = true;
                }
                else if (ScreenManager.input.IsNewButtonPress(Buttons.A, null, out result) ||ScreenManager.input.IsNewButtonPress(Buttons.Start, null, out result))
                {
                    isExit = true;
                }
            }
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            if (isExit && !isExited)
            {
                foreach (ScreenManager.GameScreen screen in ScreenManager.GetScreen())screen.ExitScreen();
                gameplayScreen.ScreenManager = ScreenManager;
                ScreenManager.AddScreen(gameplayScreen, null);
                isExited = true;
            }
            HandleInput(Mouse.GetState(), GamePad.GetState(PlayerIndex.One));
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            spriteBatch.Begin();
            spriteBatch.Draw(background, ScreenManager.GraphicsDevice.Viewport.Bounds, Color.White * TransitionAlpha);

            if (isExit)
            {
                Rectangle safeArea = ScreenManager.SafeArea;
                string text = "Loading...";
                Vector2 measure = font.MeasureString(text);
                Vector2 textPosition = new Vector2(safeArea.Center.X - measure.X / 2, safeArea.Center.Y - measure.Y / 2);
                spriteBatch.DrawString(font, text, textPosition, Color.Black);
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
        #endregion
    }
}
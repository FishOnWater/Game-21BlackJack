using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using _21BlackJack.Cards_FrameWork;

namespace _21BlackJack.Screens
{
    class OptionsMenu : ScreenManager.MenuScreen
    {
        Dictionary<string, Texture2D> themes = new Dictionary<string, Texture2D>();
        BlackJack21.CardFramework.AnimatedGameComponent card;
        Texture2D background;
        Rectangle safeArea;

        #region Initializations
        public OptionsMenu() : base("")
        {
        }
        #endregion

        public override void LoadContent()
        {
            safeArea = ScreenManager.SafeArea;
            ScreenManager.MenuEntry themeGameMenuEntry = new ScreenManager.MenuEntry("Deck");
            ScreenManager.MenuEntry returnMenuEntry = new ScreenManager.MenuEntry("Return");

            themeGameMenuEntry.Selected += ThemeGameMenuEntrySelected;
            returnMenuEntry.Selected += OnCancel;

            MenuEntries.Add(themeGameMenuEntry);
            MenuEntries.Add(returnMenuEntry);

            themes.Add("Red", ScreenManager.Game.Content.Load<Texture2D>(@"Images\Cards\CardBack_Red"));
            themes.Add("Blue", ScreenManager.Game.Content.Load<Texture2D>(@"Images\Cards\CardBack_Blue"));
            background = ScreenManager.Game.Content.Load<Texture2D>(@"Images\UI\table");

            card = new BlackJack21.CardFramework.AnimatedGameComponent(ScreenManager.Game, themes[MainMenuScreen.Theme])
            {
                CurrentPosition = new Vector2(safeArea.Center.X, safeArea.Center.Y - 50)
            };
            ScreenManager.Game.Components.Add(card);

            base.LoadContent();
        }

        #region Update and Render
        void ThemeGameMenuEntrySelected(object sender, EventArgs e)
        {
            if (MainMenuScreen.Theme == "Red")
            {
                MainMenuScreen.Theme = "Blue";
            }
            else
            {
                MainMenuScreen.Theme = "Red";
            }
            card.CurrentFrame = themes[MainMenuScreen.Theme];
        }

        protected override void OnCancel(PlayerIndex playerIndex)
        {
            ScreenManager.Game.Components.Remove(card);
            ExitScreen();
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.SpriteBatch.Begin();
            ScreenManager.SpriteBatch.Draw(background, ScreenManager.GraphicsDevice.Viewport.Bounds, Color.White * TransitionAlpha);

            ScreenManager.SpriteBatch.End();
            base.Draw(gameTime);
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _21BlackJack.Screens
{
    class MainMenuScreen : ScreenManager.MenuScreen
    {
        public static string Theme = "Red";

        #region Initializations
        public MainMenuScreen(): base("")
        {
        }
        #endregion

        public override void LoadContent()
        {
            ScreenManager.MenuEntry startGameMenuEntry = new ScreenManager.MenuEntry("Play");
            ScreenManager.MenuEntry themeGameMenuEntry = new ScreenManager.MenuEntry("Theme");
            ScreenManager.MenuEntry exitMenuEntry = new ScreenManager.MenuEntry("Exit");

            startGameMenuEntry.Selected += StartGameMenuEntrySelected;
            themeGameMenuEntry.Selected += ThemeGameMenuEntrySelected;
            exitMenuEntry.Selected += OnCancel;

            MenuEntries.Add(startGameMenuEntry);
            MenuEntries.Add(themeGameMenuEntry);
            MenuEntries.Add(exitMenuEntry);

            base.LoadContent();
        }

        #region Update
        void StartGameMenuEntrySelected(object sender, EventArgs e)
        {
            foreach (ScreenManager.GameScreen screen in ScreenManager.GetScreen())screen.ExitScreen();
            ScreenManager.AddScreen(new GameplayScreen(Theme), null);
        }

        void ThemeGameMenuEntrySelected(object sender, EventArgs e)
        {
            ScreenManager.AddScreen(new Screens.OptionsMenu(), null);
        }

        protected override void OnCancel(PlayerIndex playerIndex)
        {
            ScreenManager.Game.Exit();
        }
        #endregion
    }
}

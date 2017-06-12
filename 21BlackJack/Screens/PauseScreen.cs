using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace _21BlackJack.Screens
{
    class PauseScreen : ScreenManager.MenuScreen
    {
        #region Initializations
        public PauseScreen() : base("")
        {

        }
        #endregion

        public override void LoadContent()
        {
            ScreenManager.MenuEntry returnGameMenuEntry = new ScreenManager.MenuEntry("Back");
            ScreenManager.MenuEntry exitMenuEntry = new ScreenManager.MenuEntry("Quit");

            returnGameMenuEntry.Selected += ReturnGameMenuEntrySelected;
            exitMenuEntry.Selected += OnCancel;

            MenuEntries.Add(returnGameMenuEntry);
            MenuEntries.Add(exitMenuEntry);

            base.LoadContent();
        }

        #region Update
        void ReturnGameMenuEntrySelected(object sender, EventArgs e)
        {
            ScreenManager.GameScreen[] screens = ScreenManager.GetScreen();
            GameplayScreen gameplayScreen = null;
            List<ScreenManager.GameScreen> res = new List<ScreenManager.GameScreen>();

            for (int screenIndex = 0; screenIndex < screens.Length; screenIndex++)
            {
                if (screens[screenIndex] is GameplayScreen)
                {
                    gameplayScreen = (GameplayScreen)screens[screenIndex];
                }
                else
                {
                    res.Add(screens[screenIndex]);
                }
            }

            foreach (ScreenManager.GameScreen screen in res)screen.ExitScreen();

            gameplayScreen.ReturnFromPause();
        }

        protected override void OnCancel(PlayerIndex playerIndex)
        {
            for (int componentIndex = 0; componentIndex < ScreenManager.Game.Components.Count; componentIndex++)
            {
                if (!(ScreenManager.Game.Components[componentIndex] is ScreenManager.ScreenManager))
                {
                    if (ScreenManager.Game.Components[componentIndex] is DrawableGameComponent)
                    {
                        (ScreenManager.Game.Components[componentIndex] as IDisposable).Dispose();
                        componentIndex--;
                    }
                    else
                    {
                        ScreenManager.Game.Components.RemoveAt(componentIndex);
                        componentIndex--;
                    }
                }
            }

            foreach (ScreenManager.GameScreen screen in ScreenManager.GetScreen()) screen.ExitScreen();

            ScreenManager.AddScreen(new Screens.BackGround(), null);
            ScreenManager.AddScreen(new MainMenuScreen(), null);
        }
        #endregion
    }
}

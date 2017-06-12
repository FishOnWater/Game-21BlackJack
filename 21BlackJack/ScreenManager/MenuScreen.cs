using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace _21BlackJack.ScreenManager
{
    abstract class MenuScreen : GameScreen
    {
        #region Fields

        const int menuEntryPadding = 35;

        List<MenuEntry> menuEntries = new List<MenuEntry>();
        int selectedEntry = 0;
        string menuTitle;
#if WINDOWS || MACOS || LINUX
        bool isMouseDown = false;
#endif
        Rectangle bounds;
        #endregion

        #region Properties

        protected IList<MenuEntry> MenuEntries
        {
            get { return menuEntries; }
        }
        #endregion

        #region Inicializações

        public MenuScreen(string menuTitle)
        {
            this.menuTitle = menuTitle;

            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }
        #endregion

        #region Handle Input

        protected virtual Rectangle GetMenuEntryHitBounds(MenuEntry entry)
        {
            return new Rectangle(0, (int)entry.Destination.Y - menuEntryPadding,
                ScreenManager.GraphicsDevice.Viewport.Width,
                entry.GetHeight(this) + (menuEntryPadding * 2));
        }

        public override void HandleInput(InputState input)
        {
            base.HandleInput(input);

            PlayerIndex player;
            if(input.IsNewButtonPress(Buttons.Back, ControllingPlayer, out player))
            {
                OnCancel(player);
            }

#if WINDOWS || MACOS || LINUX
            if (input.IsMenuUp(ControllingPlayer))
            {
                selectedEntry--;

                if (selectedEntry < 0)
                    selectedEntry = menuEntries.Count - 1;
            }
            else if (input.IsMenuDown(ControllingPlayer))
            {
                selectedEntry++;

                if (selectedEntry >= menuEntries.Count)
                    selectedEntry = 0;
            }
            else if(input.IsNewKeyPress(Keys.Enter, ControllingPlayer, out player) || input.IsNewKeyPress(Keys.Space, ControllingPlayer, out player))
            {
                OnSelectEntry(selectedEntry, player);
            }

            MouseState state = Mouse.GetState();
            if (state.LeftButton == ButtonState.Released)
            {
                if (isMouseDown)
                {
                    isMouseDown = false;

                    Point clickLocation = new Point(state.X, state.Y);

                    for (int i = 0; i < menuEntries.Count; i++)
                    {
                        MenuEntry menuEntry = menuEntries[i];

                        if (menuEntry.Destination.Contains(clickLocation))
                        {
                            OnSelectEntry(i, PlayerIndex.One);
                        }
                    }
                }
            }
            else if(state.LeftButton == ButtonState.Pressed)
            {
                isMouseDown = true;

                Point clickLocation = new Point(state.X, state.Y);

                for(int i=0; i<menuEntries.Count; i++)
                {
                    MenuEntry menuEntry = menuEntries[i];

                    if (menuEntry.Destination.Contains(clickLocation))
                        selectedEntry = 1;
                }
            }
            if (input.IsMenuUp(ControllingPlayer))
            {
                selectedEntry--;

                if (selectedEntry < 0)
                    selectedEntry = menuEntries.Count - 1;
            }
            else if (input.IsMenuDown(ControllingPlayer))
            {
                selectedEntry++;

                if (selectedEntry >= menuEntries.Count)
                    selectedEntry = 0;
            }
            else if (input.IsNewButtonPress(Buttons.A, ControllingPlayer, out player))
                OnSelectEntry(selectedEntry, player);
        }
#endif
        protected virtual void OnSelectEntry(int entryIndex, PlayerIndex playerIndex)
        {
            menuEntries[entryIndex].OnSelectEntry(playerIndex);
        }

        protected virtual void OnCancel(PlayerIndex playerIndex)
        {
            ExitScreen();
        }

        protected void OnCancel(object sender, PlayerIndexEvent e)
        {
            OnCancel(e.PlayerIndex);
        }
        #endregion

        #region Loading
        public override void LoadContent()
        {
            base.LoadContent();
            bounds = ScreenManager.SafeArea;
        }
        #endregion

        #region Update and Draw
        protected virtual void UpdateMenuEntryLocations()
        {
            float transitionOffSet = (float)Math.Pow(TransitionPosition, 2);

            Vector2 position = new Vector2(0f, ScreenManager.Game.Window.ClientBounds.Height / 2 -
                (menuEntries[0].GetHeight(this)) + (menuEntryPadding * 2) * MenuEntries.Count);

            for (int i=0; i<menuEntries.Count; i++)
            {
                MenuEntry menuEntry = menuEntries[i];

                position.X = ScreenManager.GraphicsDevice.Viewport.Width / 2 - menuEntry.GetWidth(this) / 2;

                if (ScreenState == ScreenState.TransitionOn)
                    position.X -= transitionOffSet * 256;
                else
                    position.X += transitionOffSet * 512;

                position.Y += menuEntry.GetHeight(this) + (menuEntryPadding * 2);
            }
        }

        public override void Update(GameTime gameTime, bool otherScreeHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreeHasFocus, coveredByOtherScreen);

            for(int i=0; i < menuEntries.Count; i++)
            {
                bool isSelected = IsActive && (i == selectedEntry);
                UpdateMenuEntryLocations();
                menuEntries[i].Update(this, isSelected, gameTime);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            GraphicsDevice graphics = ScreenManager.GraphicsDevice;
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ScreenManager.Font;

            spriteBatch.Begin();

            for(int i=0; i < menuEntries.Count; i++)
            {
                MenuEntry menuEntry = menuEntries[i];
                bool isSelected = IsActive && (i == selectedEntry);
                menuEntry.Draw(this, isSelected, gameTime);
            }

            float transitionOffSet = (float)Math.Pow(TransitionPosition, 2);

            Vector2 titlePosition = new Vector2(graphics.Viewport.Width / 2, 375);
            Vector2 titleOrigin = font.MeasureString(menuTitle) / 2;
            Color titleColor = new Color(192, 192, 192) * TransitionAlpha;
            float titleScale = 1.25f;

            titlePosition.Y -= transitionOffSet * 100;

            spriteBatch.DrawString(font, menuTitle, titlePosition, titleColor, 0, titleOrigin, titleScale, SpriteEffects.None, 0);
            spriteBatch.End();
        }
        #endregion

        #region Public Functions
        public void UpdateMenuEntryDestination()
        {
            Rectangle bounds = ScreenManager.SafeArea;

            Rectangle textureSize = ScreenManager.ButtonBackGround.Bounds;
            int xStep = bounds.Width / (menuEntries.Count + 2);
            int maxWidth = 0;

            for (int i=0; i < menuEntries.Count; i++)
            {
                int width = menuEntries[i].GetWidth(this);
                if (width > maxWidth)
                    maxWidth = width;
            }
            maxWidth += 20;

            for(int i =0; i<menuEntries.Count; i++)
            {
                menuEntries[i].Destination =
                    new Rectangle(
                        bounds.Left + (xStep - textureSize.Width) / 2 + (i + 1) * xStep,
                bounds.Bottom - textureSize.Height * 2, maxWidth, 50);
            }
        }
        #endregion
    }
}

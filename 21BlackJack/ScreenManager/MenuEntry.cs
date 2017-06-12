using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _21BlackJack.ScreenManager
{
    class MenuEntry
    {
        #region Fields

        string text;
        float selectionFade;
        Rectangle destination;

        #endregion

        #region Properties
        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        public Rectangle Destination
        {
            get { return destination; }
            set { destination = value; }
        }

        public float Scale { get; set; }

        public float Rotation { get; set; }
        #endregion

        #region Events
        public event EventHandler<PlayerIndexEvent> Selected;

        protected internal virtual void OnSelectEntry(PlayerIndex playerIndex)
        {
            if (Selected != null)
                Selected(this, new PlayerIndexEvent(playerIndex)); //ver o que se passa
        }
        #endregion

        #region Inicializações
        public MenuEntry(string text)
        {
            this.text = text;
            Scale = 1f;
            Rotation = 0f;
        }
        #endregion

        #region Update and Draw
        public virtual void Update(MenuScreen scree, bool isSelected, GameTime gameTime)
        {
            float fadeSpeed = (float)gameTime.ElapsedGameTime.TotalSeconds * 4;

            if (isSelected)
                selectionFade = Math.Min(selectionFade + fadeSpeed, 1);
            else
                selectionFade = Math.Max(selectionFade - fadeSpeed, 0);
        }

        public virtual void Draw(MenuScreen screen, bool isSelected, GameTime gameTime)
        {
            Color textColor = isSelected ? Color.White : Color.Black;
            Color tintColor = isSelected ? Color.White : Color.Gray;

            ScreenManager screenManager = screen.ScreenManager;
            SpriteBatch spriteBatch = screenManager.SpriteBatch;
            SpriteFont spriteFont = screenManager.Font;

            spriteBatch.Draw(screenManager.ButtonBackGround, destination, tintColor);
            spriteBatch.DrawString(screenManager.Font, text, getTextPosition(screen), textColor, Rotation, Vector2.Zero, Scale, SpriteEffects.None, 0);
        }

        public virtual int GetHeight(MenuScreen screen)
        {
            return screen.ScreenManager.Font.LineSpacing;
        }

        public virtual int GetWidth(MenuScreen screen)
        {
            return (int)screen.ScreenManager.Font.MeasureString(Text).X;
        }

        private Vector2 getTextPosition(MenuScreen screen)
        {
            Vector2 textPosition = Vector2.Zero;
            if (Scale == 1f)
            {
                textPosition = new Vector2((int)destination.X + destination.Width / 2 - GetWidth(screen) / 2, (int)destination.Y);
            }
            else
            {
                textPosition = new Vector2((int)destination.X + (destination.Width / 2 - ((GetWidth(screen) / 2) * Scale)), (int)destination.Y + (GetHeight(screen) - GetHeight(screen) * Scale / 2));
            }

            return textPosition;
        }
        #endregion
    }
}

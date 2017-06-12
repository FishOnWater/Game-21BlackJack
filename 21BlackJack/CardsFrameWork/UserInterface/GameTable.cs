using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _21BlackJack.CardsFrameWork.UserInterface
{
    public class GameTable : DrawableGameComponent
    {
        public string Theme { get; private set; }
        public Texture2D TableTexture { get; private set; }
        public Vector2 DealerPosition { get; private set; }
        public SpriteBatch SpriteBatch { get; private set; }
        public Func<int, Vector2> PlaceOrder { get; private set; }
        public Rectangle TableBounds { get; private set; }
        public int Places { get; private set; }

        public Vector2 this[int index]
        {
            get
            {
                return new Vector2(TableBounds.Left, TableBounds.Top) + PlaceOrder(index);
            }
        }

        public GameTable(Rectangle tableBounds, Vector2 dealerPosition, int places, Func<int, Vector2> placeOrder, string theme, Game game) : base(game)
        {
            TableBounds = tableBounds;
            DealerPosition = dealerPosition + new Vector2(tableBounds.Left, tableBounds.Top);
            Places = places;
            PlaceOrder = placeOrder;
            Theme = theme;
            SpriteBatch = new SpriteBatch(game.GraphicsDevice);
        }

        protected override void LoadContent()
        {
            string assetName = string.Format(@"Images\UI\table");
            TableTexture = Game.Content.Load<Texture2D>(assetName);
            base.LoadContent();
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch.Begin();
            SpriteBatch.Draw(TableTexture, TableBounds, Color.White);
            SpriteBatch.End();
            base.Draw(gameTime);
        }
    }
}

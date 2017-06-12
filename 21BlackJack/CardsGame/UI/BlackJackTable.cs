using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _21BlackJack.Cards_FrameWork;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _21BlackJack.CardsGame.UI
{
    class BlackJackTable : CardsFrameWork.UserInterface.GameTable
    {
        public Texture2D RingTexture { get; private set; }
        public Vector2 RingOffSet { get; private set; }

        public BlackJackTable(Vector2 ringOffSet, Rectangle tableBounds, Vector2 dealerPosition, int places, Func<int, Vector2> placeOrder, string theme, Game game)
            :base(tableBounds, dealerPosition, places, placeOrder, theme, game)
        {
            RingOffSet = ringOffSet;
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }
    }
}

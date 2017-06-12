using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using _21BlackJack.Cards_FrameWork;

namespace _21BlackJack.CardsGame.UI
{
    public class Button : BlackJack21.CardFramework.AnimatedGameComponent
    {
        bool isKeyDown = false;
        bool isPressed = false;
        SpriteBatch spriteBatch;

        public Texture2D RegularTexture { get; set; }
        public Texture2D PressedTexture { get; set; }
        public SpriteFont Font { get; set; }
        public Rectangle Bounds { get; set; }

        string regularTexture;
        string pressedTexture;

        public event EventHandler Click;
        ScreenManager.InputState input;

        _21BlackJack.Misc.InputHelper inputHelper;

        public Button(string regularTexture, string pressedTexture, ScreenManager.InputState input, Cards_FrameWork.Game.CardsGame cardGame)
            :base(cardGame, null){
            this.input = input;
            this.regularTexture = regularTexture;
            this.pressedTexture = pressedTexture;
        }
        public override void Initialize()
        {
            inputHelper = null;
            for(int componentIndex =0; componentIndex<Game.Components.Count; componentIndex++)
            {
                if(Game.Components[componentIndex] is _21BlackJack.Misc.InputHelper)
                {
                    inputHelper = (_21BlackJack.Misc.InputHelper)Game.Components[componentIndex];
                    break;
                }
            }
            spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            if(regularTexture != null)
            {
                RegularTexture = Game.Content.Load<Texture2D>(@"Images\" + regularTexture);
            }
            if (pressedTexture != null)
                PressedTexture = Game.Content.Load<Texture2D>(@"Images\" + pressedTexture);
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            if (RegularTexture != null)
                HandleInput(Mouse.GetState());
            base.Update(gameTime);
        }

        private void HandleInput(MouseState mouseState)
        {
            bool pressed = false;
            Vector2 position = Vector2.Zero;

            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                pressed = true;
                position = new Vector2(mouseState.X, mouseState.Y);   
            }
            else if (inputHelper.IsPressed)
            {
                pressed = true;
                position = inputHelper.PointPosition;
            }
            else
            {
                if (pressed)
                {
                    if (IntersectWith(new Vector2(mouseState.X, mouseState.Y)) || IntersectWith(inputHelper.PointPosition))
                    {
                        FireClick();
                        isPressed = false;
                    }
                    else
                        isPressed = false;
                }
                isKeyDown = false;
            }
            if (pressed)
            {
                if (!isKeyDown)
                {
                    if (IntersectWith(position))
                    {
                        isPressed = true;
                    }
                    isKeyDown = true;
                }
            }
            else
            {
                isKeyDown = false;
            }
        }

        private bool IntersectWith(Vector2 position)
        {
            Rectangle touchTap = new Rectangle((int)position.X - 1, (int)position.Y - 1, 2, 2);
            return Bounds.Intersects(touchTap);
        }

        public void FireClick()
        {
            if (Click != null)
                Click(this, EventArgs.Empty);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            spriteBatch.Draw(isPressed ? PressedTexture : RegularTexture, Bounds, Color.White);
            if(Font != null)
            {
                Vector2 textposition = Font.MeasureString(Text);
                textposition = new Vector2(Bounds.Width - textposition.X, Bounds.Height - textposition.Y);
                textposition /= 2;
                textposition.X += Bounds.X;
                textposition.Y += Bounds.Y;
                spriteBatch.DrawString(Font, Text, textposition, Color.White);
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }

        protected override void Dispose(bool disposing)
        {
            Click = null;
            base.Dispose(disposing);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using _21BlackJack.Cards_FrameWork;
using _21BlackJack.Cards_FrameWork.Game;
using _21BlackJack.Cards_FrameWork.Player;
using _21BlackJack.Cards_FrameWork.Rules;
using _21BlackJack.Cards_FrameWork.Utility;

namespace _21BlackJack.CardsGame
{
    class BlackJackGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        ScreenManager.ScreenManager screenManager;

        public static float HeightScale = 1.0f;
        public static float WidthScale = 1.0f;

        public BlackJackGame()
        {
            graphics = new GraphicsDeviceManager(this);
            screenManager = new ScreenManager.ScreenManager(this);
            Content.RootDirectory = "Content";

            screenManager.AddScreen(new Screens.BackGround(), null);
            screenManager.AddScreen(new Screens.MainMenuScreen(), null); //falta

            Components.Add(screenManager);
#if WINDOWS || MACOS || LINUX
            IsMouseVisible = true;
#endif
            _21BlackJack.Misc.AudioManager.Initialize(this);
        }

        protected override void Initialize()
        {
            base.Initialize();

#if WINDOWS || MACOS || LINUX
            graphics.PreferredBackBufferHeight = 480;
            graphics.PreferredBackBufferWidth = 800;
#endif
            graphics.ApplyChanges();

            Rectangle bounds = graphics.GraphicsDevice.Viewport.TitleSafeArea;
            HeightScale = bounds.Height / 480f;
            WidthScale = bounds.Width / 800f;
        }

        protected override void LoadContent()
        {
            _21BlackJack.Misc.AudioManager.LoadSounds();
            base.LoadContent();
        }
    }
}

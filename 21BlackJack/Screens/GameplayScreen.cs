using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _21BlackJack.ScreenManager;
using _21BlackJack.Cards_FrameWork;
using _21BlackJack.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System.IO;

namespace _21BlackJack.Screens
{
    class GameplayScreen : GameScreen
    {
        #region Fields and properties
        _21BlackJack.Game1 blackjackGame;

        InputHelper inputHelper;

        string theme;
        List<DrawableGameComponent> pauseEnabledComponents = new List<DrawableGameComponent>();
        List<DrawableGameComponent> pauseVisibleComponents = new List<DrawableGameComponent>();
        Rectangle safeArea;

        static Vector2[] playerCardOffSet = new Vector2[]
        {
            new Vector2(100f*_21BlackJack.CardsGame.BlackJackGame.WidthScale, 190f * _21BlackJack.CardsGame.BlackJackGame.HeightScale),
            new Vector2(336f*_21BlackJack.CardsGame.BlackJackGame.WidthScale, 210f * _21BlackJack.CardsGame.BlackJackGame.HeightScale),
            new Vector2(570f*_21BlackJack.CardsGame.BlackJackGame.WidthScale, 190f*_21BlackJack.CardsGame.BlackJackGame.HeightScale)
        };
        #endregion

        #region Inicializações
        public GameplayScreen(string theme)
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.0);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
            this.theme = theme;
        }
        #endregion

        #region Loading
        public override void LoadContent()
        {
            safeArea = ScreenManager.SafeArea;

            inputHelper = new InputHelper(ScreenManager.Game);
            inputHelper.DrawOrder = 1000;
            ScreenManager.Game.Components.Add(inputHelper);

            blackjackGame = new Game1(ScreenManager.GraphicsDevice.Viewport.Bounds, new Vector2(safeArea.Left * safeArea.Width / 2 - 50, safeArea.Top + 20), GetPlayerCardPosition, ScreenManager, theme);

            InitializeGame();
            base.LoadContent();
        }

        public override void UnloadContent()
        {
            ScreenManager.Game.Components.Remove(inputHelper);
            base.UnloadContent();
        }
        #endregion

        #region Update and Render
        public override void HandleInput(InputState input)
        {
            if (input.IsPausedGame(null))
                PauseCurrentGame();
            base.HandleInput(input);
        }

        public override void Update(GameTime gameTime, bool otherScreeHasFocus, bool coveredByOtherScreen)
        {
            if (blackjackGame != null && !coveredByOtherScreen)
                blackjackGame.Update(gameTime);
            base.Update(gameTime, otherScreeHasFocus, coveredByOtherScreen);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if (blackjackGame != null)
                blackjackGame.Draw(gameTime);
        }
        #endregion

        #region Private Methods
        private void InitializeGame()
        {
            blackjackGame.Initialize();
            blackjackGame.AddPlayer(new CardsGame.Players.BlackjackPlayer("Abe", blackjackGame));

            CardsGame.Players.BlackJackAIPlayer player = new CardsGame.Players.BlackJackAIPlayer("Benny", blackjackGame);
            blackjackGame.AddPlayer(player);
            player.Hit += player_Hit;
            player.Stand += player_Stand;

            string[] assets = { "blackjack", "bust", "lose", "push", "win", "pass", "shuffle_" + theme };

            for(int chipIndex =0; chipIndex<assets.Length; chipIndex++)
            {
                blackjackGame.LoadUITexture("UI", assets[chipIndex]);
            }
            blackjackGame.StartRound();
        }
        private Vector2 GetPlayerCardPosition(int player)

        {

            switch (player)

            {

                case 0:

                case 1:

                case 2:

                    return new Vector2(ScreenManager.SafeArea.Left,

                        ScreenManager.SafeArea.Top + 200 * (CardsGame.BlackJackGame.HeightScale - 1)) +

                        playerCardOffSet[player];

                default:

                    throw new ArgumentException(

                        "Player index should be between 0 and 2", "player");

            }

        }



        /// <summary>

        /// Pause the game.

        /// </summary>

        private void PauseCurrentGame()

        {

            // Move to the pause screen

            ScreenManager.AddScreen(new Screens.BackGround(), null);

            ScreenManager.AddScreen(new Screens.PauseScreen(), null);



            // Hide and disable all components which are related to the gameplay screen

            pauseEnabledComponents.Clear();

            pauseVisibleComponents.Clear();

            foreach (IGameComponent component in ScreenManager.Game.Components)

            {

                if (component is CardsGame.Misc.BetGameComponent ||

                    component is BlackJack21.CardFramework.AnimatedGameComponent ||

                    component is _21BlackJack.CardsFrameWork.UserInterface.GameTable ||

                    component is InputHelper)

                {

                    DrawableGameComponent pauseComponent = (DrawableGameComponent)component;

                    if (pauseComponent.Enabled)

                    {

                        pauseEnabledComponents.Add(pauseComponent);

                        pauseComponent.Enabled = false;

                    }

                    if (pauseComponent.Visible)

                    {

                        pauseVisibleComponents.Add(pauseComponent);

                        pauseComponent.Visible = false;

                    }

                }

            }

        }



        /// <summary>

        /// Returns from pause.

        /// </summary>

        public void ReturnFromPause()

        {

            // Reveal and enable all previously hidden components

            foreach (DrawableGameComponent component in pauseEnabledComponents)

            {

                component.Enabled = true;

            }

            foreach (DrawableGameComponent component in pauseVisibleComponents)

            {

                component.Visible = true;

            }

        }

        #endregion



        #region Event Handler

        /// <summary>

        /// Responds to the event sent when AI player's choose to "Stand".

        /// </summary>

        /// <param name="sender">The source of the event.</param>

        /// <param name="e">The 

        /// <see cref="System.EventArgs"/> instance containing the event data.</param>

        void player_Stand(object sender, EventArgs e)

        {

            blackjackGame.Stand();

        }



        /// <summary>

        /// Responds to the event sent when AI player's choose to "Split".

        /// </summary>

        /// <param name="sender">The source of the event.</param>

        /// <param name="e">The 

        /// <see cref="System.EventArgs"/> instance containing the event data.</param>

        void player_Split(object sender, EventArgs e)

        {

            blackjackGame.Split();

        }



        /// <summary>

        /// Responds to the event sent when AI player's choose to "Hit".

        /// </summary>

        /// <param name="sender">The source of the event.</param>

        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>

        void player_Hit(object sender, EventArgs e)

        {

            blackjackGame.Hit();

        }



        /// <summary>

        /// Responds to the event sent when AI player's choose to "Double".

        /// </summary>

        /// <param name="sender">The source of the event.</param>

        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>

        void player_Double(object sender, EventArgs e)

        {

            blackjackGame.Double();

        }

        #endregion

    }
}

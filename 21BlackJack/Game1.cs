using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Reflection;
using _21BlackJack.Cards_FrameWork;
using _21BlackJack.CardsFrameWork;
using _21BlackJack.Cards_FrameWork.Game;
using _21BlackJack.Cards_FrameWork.Player;

namespace _21BlackJack
{
    public enum BlackjackGameState
    {
        Shuffling,
        Betting,
        Playing,
        Dealing,
        RoundEnd,
        GameOver,
    }
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Cards_FrameWork.Game.CardsGame
    {
        Dictionary<Cards_FrameWork.Player.Player, string> playerHandValueTexts = new Dictionary<Cards_FrameWork.Player.Player, string>();
        Dictionary<Cards_FrameWork.Player.Player, string> playerSecondHandValueTexts = new Dictionary<Cards_FrameWork.Player.Player, string>();
        private Hand deadCards = new Hand();
        private CardsGame.Players.BlackjackPlayer dealerPLayer;
        bool[] turnFinishedByPlayer;
        TimeSpan dealDuration = TimeSpan.FromMilliseconds(500);

        CardsFrameWork.UserInterface.AnimatedHandGameComponent[] animatedHands;
        CardsFrameWork.UserInterface.AnimatedHandGameComponent[] animatedSecondHands;

        CardsGame.Misc.BetGameComponent betGameComponent;
        CardsFrameWork.UserInterface.AnimatedHandGameComponent dealerHandComponent;
        Dictionary<string, CardsGame.UI.Button> buttons = new Dictionary<string, CardsGame.UI.Button>();
        CardsGame.UI.Button newGame;
        bool showInsurance;

        Vector2 secondHandOffSet = new Vector2(100 * CardsGame.BlackJackGame.WidthScale, 25 * CardsGame.BlackJackGame.HeightScale);
        static Vector2 ringOffSet = new Vector2(0, 110);
        Vector2 frameSize = new Vector2(100, 180);

        public BlackjackGameState State { get; set; }
        ScreenManager.ScreenManager screenManager;
        const int maxPlayers = 3;
        const int minPlayers = 1;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public Game1(Rectangle tableBounds, Vector2 dealerPosition, Func<int, Vector2> placeOrder, ScreenManager.ScreenManager screenManager, string theme)
            :base (6,0,CardSuit.AllSuits,Cards_FrameWork.CardValue.NonJokers, minPlayers, maxPlayers, new CardsGame.UI.BlackJackTable(ringOffSet, tableBounds, dealerPosition, maxPlayers, placeOrder, theme, screenManager.Game), theme, screenManager.Game)
        {
            dealerPLayer = new CardsGame.Players.BlackjackPlayer("Dealer", this);
            turnFinishedByPlayer = new bool[MaximumPlayers];
            this.screenManager = screenManager;

            if (animatedHands == null)
                animatedHands = new CardsFrameWork.UserInterface.AnimatedHandGameComponent[maxPlayers];
            if (animatedSecondHands == null)
                animatedSecondHands = new CardsFrameWork.UserInterface.AnimatedHandGameComponent[maxPlayers];
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        public void Initialize()
        {
            base.LoadContent();
            betGameComponent =
                new CardsGame.Misc.BetGameComponent(players, screenManager.input, Theme, this);
            Game.Components.Add(betGameComponent);

            string[] buttonsText = { "Hit", "Stand", "Double", "Split", "Insurance" };
            for(int buttonIndex =0; buttonIndex < buttonsText.Length; buttonIndex++)
            {
                CardsGame.UI.Button button = new CardsGame.UI.Button("ButtonRegular", "ButtonPressed", screenManager.input, this)
                {
                    Text = buttonsText[buttonIndex],
                    Bounds = new Rectangle(screenManager.SafeArea.Left + 10 + buttonIndex * 110, screenManager.SafeArea.Bottom - 60, 100, 50),
                    Font = this.Font,
                    Visible = false,
                    Enabled = false
                };
                buttons.Add(buttonsText[buttonIndex], button);
                Game.Components.Add(button);
            }

            newGame = new CardsGame.UI.Button("ButtonRegular", "ButtonPressed", screenManager.input, this)
            {
                Text = "New Hand",
                Bounds = new Rectangle(screenManager.SafeArea.Left+10, screenManager.SafeArea.Bottom-60, 200, 50),
                Font = this.Font,
                Visible=false,
                Enabled=false
            };

            Rectangle insuranceBounds = buttons["Insurance"].Bounds;
            insuranceBounds.Width = 200;
            buttons["Insurance"].Bounds = insuranceBounds;

            newGame.Click += newGame_Click;
            Game.Components.Add(newGame);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        //protected override LoadContent()
        //{
        //    // Create a new SpriteBatch, which can be used to draw textures.
        //    spriteBatch = new SpriteBatch(GraphicsDevice);

        //    // TODO: use this.Content to load your game content here
        //}

        ///// <summary>
        ///// UnloadContent will be called once per game and is the place to unload
        ///// game-specific content.
        ///// </summary>
        //protected override void UnloadContent()
        //{
        //    // TODO: Unload any non ContentManager content here
        //}

        ///// <summary>
        ///// Allows the game to run logic such as updating the world,
        ///// checking for collisions, gathering input, and playing audio.
        ///// </summary>
        ///// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Update(GameTime gameTime)
        {
            switch (State)
            {
                case BlackjackGameState.Shuffling:
                    {
                        ShowShuffleAnimation();
                    } break;
                case BlackjackGameState.Betting:
                    {
                        EnableButtons(false);
                    }break;
                case BlackjackGameState.Dealing:
                    {
                        State = BlackjackGameState.Playing;
                        Deal();
                        StartPlaying();
                    }break;
                case BlackjackGameState.Playing:
                    {
                        for(int playerIndex=0; playerIndex<players.Count; playerIndex++)
                        {
                            ((CardsGame.Players.BlackjackPlayer)players[playerIndex]).CalculateValues();
                        }
                        dealerPLayer.CalculateValues();

                        if (!CheckForRunningAnimations<BlackJack21.CardFramework.AnimatedCardsGameComponent>())
                        {
                            CardsGame.Players.BlackjackPlayer player = (CardsGame.Players.BlackjackPlayer)GetCurrentPlayer();
                            if (player is CardsGame.Players.BlackJackAIPlayer)
                            {
                                ((CardsGame.Players.BlackJackAIPlayer)player).AIPlay();
                            }
                            //CheckRules();

                            if (State == BlackjackGameState.Playing && GetCurrentPlayer() == null)
                                EndRound();
                            SetButtonAvailability();
                        }
                        else
                            EnableButtons(false);
                    }break;
                case BlackjackGameState.RoundEnd:
                    {
                        if (dealerHandComponent.EstimatedTimeForAnimationsCompletion() == TimeSpan.Zero)
                        {
                            betGameComponent.CalculateBalance(dealerPLayer);
                            if (((CardsGame.Players.BlackjackPlayer)players[0]).Balance < 5)
                                EndGame();
                            else
                            {
                                newGame.Enabled = true;
                                newGame.Visible = true;
                            }
                        }
                    }break;
                case BlackjackGameState.GameOver:
                    {

                    }break;
                default: break;
            }
        }

        private void ShowShuffleAnimation()
        {
            BlackJack21.CardFramework.AnimatedGameComponent animationComponent = new BlackJack21.CardFramework.AnimatedGameComponent(this.Game)
            {
                CurrentPosition = GameTable.DealerPosition,
                Visible = false
            };
            Game.Components.Add(animationComponent);

            animationComponent.AddAnimation(new BlackJack21.CardFramework.FramesetGameComponentAnimation(cardsAssets["Shuffle_" + Theme], 32, 11, frameSize)
            {
                Duration = TimeSpan.FromSeconds(1.5f),
                PerformBeforeStart = ShowComponent,
                PerformBeforSartArgs = animationComponent,
                PerformWhenDone = PlayShuffleAndRemoveComponent,
                PerformWhenDoneArgs = animationComponent
            });
            State = BlackjackGameState.Betting;
        }

        void ShowComponent(object obj)
        {
            ((BlackJack21.CardFramework.AnimatedGameComponent)obj).Visible = true;
        }
        void PlayShuffleAndRemoveComponent(object obj)
        {
            _21BlackJack.Misc.AudioManager.PlaySound("Shuffle");
            Game.Components.Remove((BlackJack21.CardFramework.AnimatedGameComponent)obj);
        }
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Draw(GameTime gameTime)
        {
            SpriteBatch.Begin();

            switch (State)
            {
                case BlackjackGameState.Playing:
                    ShowPlayerValues();
                    break;
                case BlackjackGameState.GameOver:
                    break;
                case BlackjackGameState.RoundEnd:
                    {
                        if (dealerHandComponent.EstimatedTimeForAnimationsCompletion() == TimeSpan.Zero)
                            ShowDealerValue();
                        ShowPlayerValues();
                    }break;
                default: break;
            }
            SpriteBatch.End();
        }

        private void ShowDealerValue()
        {
            string dealerValue = dealerPLayer.FirstValue.ToString();
            if (dealerPLayer.FirstValueConsiderAce)
            {
                if (dealerPLayer.FirstValue + 10 == 21)
                    dealerValue = "21";
                else
                    dealerValue += @"\" + (dealerPLayer.FirstValue + 10).ToString();
            }

            Vector2 measure = Font.MeasureString(dealerValue);
            Vector2 position = GameTable.DealerPosition - new Vector2(measure.X + 20, 0);

            SpriteBatch.Draw(screenManager.BlankTexture, new Rectangle((int)position.X - 4, (int)position.Y, (int)measure.X + 8, (int)measure.Y), Color.Black);
            SpriteBatch.DrawString(Font, dealerValue, position, Color.White);
        }

        private void ShowPlayerValues()
        {
            Color color = Color.Black;
            Cards_FrameWork.Player.Player currentPlayer = GetCurrentPlayer();

            for(int playerIndex =0; playerIndex < players.Count; playerIndex++)
            {
                CardsGame.Players.BlackjackPlayer player = (CardsGame.Players.BlackjackPlayer)players[playerIndex];
                if (player == currentPlayer)
                    color = Color.Red;
                else
                    color = Color.White;

                string playerHandValueText;
                string playerSecondHandValueText = null;
                if (!animatedHands[playerIndex].IsAnimating)
                {
                    if (player.FirstValue > 0)
                    {
                        playerHandValueText = player.FirstValue.ToString();
                        if (player.FirstValueConsiderAce)
                        {
                            if (player.FirstValue + 10 == 21)
                                playerHandValueText = "21";
                            else
                                playerHandValueText += @"\" + (player.FirstValue + 10).ToString();
                        }
                        playerHandValueTexts[player] = playerHandValueText;
                    }
                    else
                        playerHandValueText = null;

                    if (player.IsSplit)
                    {
                        if (player.SecondValue > 0)
                        {
                            playerSecondHandValueText = player.SecondValue.ToString();
                            if (player.SecondValueConsiderAce)
                            {
                                if (player.SecondValue + 10 == 21)
                                    playerSecondHandValueText = "21";
                                else
                                    playerSecondHandValueText += @"\" + (player.SecondValue + 10).ToString();
                            }
                            playerSecondHandValueTexts[player] = playerSecondHandValueText;
                        }
                        else
                            playerSecondHandValueText = null;
                    }
                }
                else
                {
                    playerHandValueTexts.TryGetValue(player, out playerHandValueText);
                    playerSecondHandValueTexts.TryGetValue(player, out playerSecondHandValueText);
                }

                if (player.IsSplit)
                {
                    color = player.CurrentHandType == CardsGame.Players.HandTypes.First && player == currentPlayer ? Color.Red : Color.White;

                    if(playerHandValueText != null)
                    {
                        DrawValue(animatedHands[playerIndex], playerIndex, playerHandValueText, color);
                    }

                    color = player.CurrentHandType == CardsGame.Players.HandTypes.Second && player == currentPlayer ? Color.Red : Color.White;
                    if(playerSecondHandValueText != null)
                    {
                        DrawValue(animatedHands[playerIndex], playerIndex, playerSecondHandValueText, color);
                    }
                }
                else
                {
                    if (playerHandValueText != null)
                        DrawValue(animatedHands[playerIndex], playerIndex, playerHandValueText, color);
                }
            }
        }
        private void DrawValue(_21BlackJack.CardsFrameWork.UserInterface.AnimatedHandGameComponent animateHand, int place, string value, Color valueColor)
        {
            Hand hand = animateHand.hand;

            Vector2 position = GameTable.PlaceOrder(place) + animateHand.GetCardRelativePosition(hand.Count - 1);
            Vector2 measure = Font.MeasureString(value);

            position.X += (cardsAssets["CardBack_" + Theme].Bounds.Width - measure.X) / 2;
            position.Y -= measure.Y + 5;

            SpriteBatch.Draw(screenManager.BlankTexture, new Rectangle((int)position.X - 4, (int)position.Y, (int)measure.X + 8, (int)measure.Y), Color.Black);
            SpriteBatch.DrawString(Font, value, position, valueColor);
        }

        public override void AddPlayer(Player player)
        {
            if (player is CardsGame.Players.BlackjackPlayer && players.Count < MaximumPlayers)
                players.Add(player);
        }

        public override Player GetCurrentPlayer()
        {
            for(int playerIndex =0; playerIndex < players.Count; playerIndex++)
            {
                if (((CardsGame.Players.BlackjackPlayer)players[playerIndex]).MadeBet && turnFinishedByPlayer[playerIndex] == false)
                    return players[playerIndex];
            }
            return null;
        }

        public override int CardValue(TraditionalCard card)
        {
            return Math.Min(base.CardValue(card), 10);
        }

        public override void Deal()
        {
            if (State == BlackjackGameState.Playing)
            {
                TraditionalCard card;
                for(int dealIndex = 0; dealIndex < 2; dealIndex++)
                {
                    for(int playerIndex =0; playerIndex < players.Count; playerIndex++)
                    {
                        if (((CardsGame.Players.BlackjackPlayer)players[playerIndex]).MadeBet)
                        {
                            card = dealer.DealCardToHand(players[playerIndex].Hand);

                            AddDealAnimation(card, animatedHands[playerIndex], true, dealDuration, DateTime.Now + TimeSpan.FromSeconds(dealDuration.TotalSeconds * (dealIndex * players.Count + playerIndex)));
                        }
                    }
                    card = dealer.DealCardToHand(dealerPLayer.Hand);
                    AddDealAnimation(card, dealerHandComponent, dealIndex == 0, dealDuration, DateTime.Now);
                }
            }
        }

        public override void StartPlaying()
        {
            if((MinimumPLayers <= players.Count && players.Count <= MaximumPlayers))
            {
                Cards_FrameWork.Rules.GameRule gameRule = new CardsGame.Rules.BustRule(players); //needs checking
                rules.Add(gameRule);
                gameRule.RuleMatch += BustGameRule;

                gameRule = new CardsGame.Rules.BlackjackRule(players);
                rules.Add(gameRule);
                gameRule.RuleMatch += InsuranceGameRule;

                for(int playerIndex = 0; playerIndex<players.Count; playerIndex++)
                {
                    if (((CardsGame.Players.BlackjackPlayer)players[playerIndex]).MadeBet)
                    {
                        animatedHands[playerIndex].Visible = false;
                    }
                    else
                    {
                        animatedHands[playerIndex].Visible = true;
                    }
                }
            }
        }

        public void AddDealAnimation(TraditionalCard card, _21BlackJack.CardsFrameWork.UserInterface.AnimatedHandGameComponent animatedHand, bool flipCard, TimeSpan duration, DateTime startTime)
        {
            int cardLocationInHand = animatedHand.GetCardLocationInHand(card);
            BlackJack21.CardFramework.AnimatedCardsGameComponent cardComponent = animatedHand.GetCardGameComponent(card, cardLocationInHand);

            cardComponent.AddAnimation(
                new BlackJack21.CardFramework.TransitionGameComponentAnimation(GameTable.DealerPosition,
                animatedHand.CurrentPosition +
                animatedHand.GetCardRelativePosition(cardLocationInHand))
                {
                    StartTime = startTime,
                    PerformBeforeStart = ShowComponent,
                    PerformBeforSartArgs = cardComponent,
                    PerformWhenDone = PlayDealSound
                });
            if (flipCard)
            {
                cardComponent.AddAnimation(new BlackJack21.CardFramework.FlipGameComponentAnimation
                {
                    IsFromFaceDownToFaceUp = true,
                    Duration = duration,
                    StartTime = startTime + duration,
                    PerformWhenDone = PlayFlipSound
                });
            }
        }

        void PlayDealSound(object obj)
        {
            _21BlackJack.Misc.AudioManager.PlaySound("Deal");
        }

        void PlayFlipSound(object obj)
        {
            _21BlackJack.Misc.AudioManager.PlaySound("Flip");
        }

        void CueOverPlayerHand(CardsGame.Players.BlackjackPlayer player, string assetName, CardsGame.Players.HandTypes animationHand, CardsFrameWork.UserInterface.AnimatedHandGameComponent waitforHand)
        {
            int playerIndex = players.IndexOf(player);
            CardsFrameWork.UserInterface.AnimatedHandGameComponent currentAnimatedHand;
            Vector2 currentPosition;
            if(playerIndex >= 0)
            {
                switch (animationHand)
                {
                    case CardsGame.Players.HandTypes.First:
                        currentAnimatedHand = animatedHands[playerIndex];
                        currentPosition = currentAnimatedHand.CurrentPosition;
                        break;
                    case CardsGame.Players.HandTypes.Second:
                        currentAnimatedHand = animatedSecondHands[playerIndex];
                        currentPosition = currentAnimatedHand.CurrentPosition + secondHandOffSet;
                        break;
                    default:
                        throw new Exception("Player has an unsupported hand type");
                }
            }
            else
            {
                currentAnimatedHand = dealerHandComponent;
                currentPosition = currentAnimatedHand.CurrentPosition;
            }

            BlackJack21.CardFramework.AnimatedGameComponent animationComponent =
                new BlackJack21.CardFramework.AnimatedGameComponent(this, cardsAssets[assetName])
                {
                    CurrentPosition = currentPosition,
                    Visible = false,
                };
            Game.Components.Add(animationComponent);

            TimeSpan estimatedTimeToCompleteAnimations;
            if (waitforHand != null)
                estimatedTimeToCompleteAnimations = waitforHand.EstimatedTimeForAnimationsCompletion();
            else
                estimatedTimeToCompleteAnimations = currentAnimatedHand.EstimatedTimeForAnimationsCompletion();

            animationComponent.AddAnimation(new BlackJack21.CardFramework.ScaleGameComponentAnimation(2.0f, 1.0f)
            {
                StartTime = DateTime.Now + estimatedTimeToCompleteAnimations,
                Duration = TimeSpan.FromSeconds(1f),
                PerformBeforeStart = ShowComponent,
                PerformBeforSartArgs = animationComponent
            });
        }
        private void EndRound()

        {

            RevealDealerFirstCard();

            DealerAI();

            ShowResults();

            State = BlackjackGameState.RoundEnd;

        }



        /// <summary>

        /// Causes the dealer's hand to be displayed.

        /// </summary>

        private void ShowDealerHand()

        {

            dealerHandComponent =

                new CardsGame.UI.BlackJackAnimatedDealerHandComponent(-1, dealerPLayer.Hand, this);

            Game.Components.Add(dealerHandComponent);

        }



        /// <summary>

        /// Reveal's the dealer's hidden card.

        /// </summary>

        private void RevealDealerFirstCard()

        {

            // Iterate over all dealer cards expect for the last

            BlackJack21.CardFramework.AnimatedCardsGameComponent cardComponent = dealerHandComponent.GetCardGameComponent(null, 1);

            cardComponent.AddAnimation(new BlackJack21.CardFramework.FlipGameComponentAnimation()

            {

                Duration = TimeSpan.FromSeconds(0.5),

                StartTime = DateTime.Now

            });

        }



        /// <summary>

        /// Present visual indication as to how the players fared in the current round.

        /// </summary>

        private void ShowResults()

        {

            // Calculate the dealer's hand value

            int dealerValue = dealerPLayer.FirstValue;



            if (dealerPLayer.FirstValueConsiderAce)

            {

                dealerValue += 10;

            }



            // Show each player's result

            for (int playerIndex = 0; playerIndex < players.Count; playerIndex++)

            {

                ShowResultForPlayer((CardsGame.Players.BlackjackPlayer)players[playerIndex], dealerValue, CardsGame.Players.HandTypes.First);

                if (((CardsGame.Players.BlackjackPlayer)players[playerIndex]).IsSplit)

                {

                    ShowResultForPlayer((CardsGame.Players.BlackjackPlayer)players[playerIndex], dealerValue, CardsGame.Players.HandTypes.Second);

                }

            }

        }



        /// <summary>

        /// Display's a player's status after the turn has ended.

        /// </summary>

        /// <param name="player">The player for which to display the status.</param>

        /// <param name="dealerValue">The dealer's hand value.</param>

        /// <param name="currentHandType">The player's hand to take into 

        /// account.</param>

        private void ShowResultForPlayer(CardsGame.Players.BlackjackPlayer player, int dealerValue,

            CardsGame.Players.HandTypes currentHandType)

        {

            // Calculate the player's hand value and check his state (blackjack/bust)

            bool blackjack, bust;

            int playerValue;

            switch (currentHandType)

            {

                case CardsGame.Players.HandTypes.First:

                    blackjack = player.BlackJack;

                    bust = player.Bust;



                    playerValue = player.FirstValue;



                    if (player.FirstValueConsiderAce)

                    {

                        playerValue += 10;

                    }

                    break;

                case CardsGame.Players.HandTypes.Second:

                    blackjack = player.SecondBlackJack;

                    bust = player.SecondBust;



                    playerValue = player.SecondValue;



                    if (player.SecondValueConsiderAce)

                    {

                        playerValue += 10;

                    }

                    break;

                default:

                    throw new Exception(

                        "Player has an unsupported hand type.");

            }

            // The bust or blackjack state are animated independently of this method,

            // so only trigger different outcome indications

            if (player.MadeBet &&

                (!blackjack || (dealerPLayer.BlackJack && blackjack)) && !bust)

            {

                string assetName = GetResultAsset(player, dealerValue, playerValue);



                CueOverPlayerHand(player, assetName, currentHandType, dealerHandComponent);

            }

        }



        /// <summary>

        /// Return the asset name according to the result.

        /// </summary>

        /// <param name="player">The player for which to return the asset name.</param>

        /// <param name="dealerValue">The dealer's hand value.</param>

        /// <param name="playerValue">The player's hand value.</param>

        /// <returns>The asset name</returns>

        private string GetResultAsset(CardsGame.Players.BlackjackPlayer player, int dealerValue, int playerValue)

        {

            string assetName;

            if (dealerPLayer.Bust)

            {

                assetName = "win";

            }

            else if (dealerPLayer.BlackJack)

            {

                if (player.BlackJack)

                {

                    assetName = "push";

                }

                else

                {

                    assetName = "lose";

                }

            }

            else if (playerValue < dealerValue)

            {

                assetName = "lose";

            }

            else if (playerValue > dealerValue)

            {

                assetName = "win";

            }

            else

            {

                assetName = "push";

            }

            return assetName;

        }



        /// <summary>

        /// Have the dealer play. The dealer hits until reaching 17+ and then 

        /// stands.

        /// </summary>

        private void DealerAI()

        {

            // The dealer may have not need to draw additional cards after his first

            // two. Check if this is the case and if so end the dealer's play.

            dealerPLayer.CalculateValues();

            int dealerValue = dealerPLayer.FirstValue;



            if (dealerPLayer.FirstValueConsiderAce)

            {

                dealerValue += 10;

            }



            if (dealerValue > 21)

            {

                dealerPLayer.Bust = true;

                CueOverPlayerHand(dealerPLayer, "bust", CardsGame.Players.HandTypes.First, dealerHandComponent);

            }

            else if (dealerValue == 21)

            {

                dealerPLayer.BlackJack = true;

                CueOverPlayerHand(dealerPLayer, "blackjack", CardsGame.Players.HandTypes.First, dealerHandComponent);

            }



            if (dealerPLayer.BlackJack || dealerPLayer.Bust)

            {

                return;

            }



            // Draw cards until 17 is reached, or the dealer gets a blackjack or busts

            int cardsDealed = 0;

            while (dealerValue <= 17)

            {

                TraditionalCard card = dealer.DealCardToHand(dealerPLayer.Hand);

                AddDealAnimation(card, dealerHandComponent, true, dealDuration,

                    DateTime.Now.AddMilliseconds(1000 * (cardsDealed + 1)));

                cardsDealed++;

                dealerPLayer.CalculateValues();

                dealerValue = dealerPLayer.FirstValue;



                if (dealerPLayer.FirstValueConsiderAce)

                {

                    dealerValue += 10;

                }



                if (dealerValue > 21)

                {

                    dealerPLayer.Bust = true;

                    CueOverPlayerHand(dealerPLayer, "bust", CardsGame.Players.HandTypes.First, dealerHandComponent);

                }

            }

        }



        /// <summary>

        /// Displays the hands currently in play.

        /// </summary>

        private void DisplayPlayingHands()

        {

            for (int playerIndex = 0; playerIndex < players.Count; playerIndex++)

            {

                _21BlackJack.CardsFrameWork.UserInterface.AnimatedHandGameComponent animatedHandGameComponent =

                    new CardsGame.UI.BlackJackAnimatedPlayerHandComponent(playerIndex, players[playerIndex].Hand, this);

                Game.Components.Add(animatedHandGameComponent);

                animatedHands[playerIndex] = animatedHandGameComponent;

            }



            ShowDealerHand();

        }



        /// <summary>

        /// Starts a new game round.

        /// </summary>

        public void StartRound()

        {

            playerHandValueTexts.Clear();

            Misc.AudioManager.PlaySound("Shuffle");

            dealer.Shuffle();

            DisplayPlayingHands();

            State = BlackjackGameState.Shuffling;

        }



        /// <summary>

        /// Sets the button availability according to the options available to the 

        /// current player.

        /// </summary>

        private void SetButtonAvailability()

        {

            CardsGame.Players.BlackjackPlayer player = (CardsGame.Players.BlackjackPlayer)GetCurrentPlayer();

            // Hide all buttons if no player is in play or the player is an AI player

            if (player == null || player is CardsGame.Players.BlackJackAIPlayer)

            {

                EnableButtons(false);

                ChangeButtonsVisiblility(false);

                return;

            }



            // Show all buttons

            EnableButtons(true);

            ChangeButtonsVisiblility(true);



            // Set insurance button availability

            buttons["Insurance"].Visible = showInsurance;

            buttons["Insurance"].Enabled = showInsurance;



            if (player.IsSplit == false)

            {

                // Remember that the bet amount was already reduced from the balance,

                // so we only need to check if the player has more money than the

                // current bet when trying to double/split



                // Set double button availability

                if (player.BetAmount > player.Balance || player.Hand.Count != 2)

                {

                    buttons["Double"].Visible = false;

                    buttons["Double"].Enabled = false;

                }



                if (player.Hand.Count != 2 ||

                    player.Hand[0].Value != player.Hand[1].Value ||

                    player.BetAmount > player.Balance)

                {

                    buttons["Split"].Visible = false;

                    buttons["Split"].Enabled = false;

                }

            }

            else

            {

                // We've performed a split. Get the initial bet amount to check whether

                // or not we can double the current bet.

                float initialBet = player.BetAmount /

                    ((player.Double ? 2f : 1f) + (player.SecoundDouble ? 2f : 1f));



                // Set double button availability.

                if (initialBet > player.Balance || player.CurrentHand.Count != 2)

                {

                    buttons["Double"].Visible = false;

                    buttons["Double"].Enabled = false;

                }



                // Once you've split, you can't split again

                buttons["Split"].Visible = false;

                buttons["Split"].Enabled = false;

            }

        }



        /// <summary>

        /// Checks for running animations.

        /// </summary>

        /// <typeparam name="T">The type of animation to look for.</typeparam>

        /// <returns>True if a running animation of the desired type is found and

        /// false otherwise.</returns>

        internal bool CheckForRunningAnimations<T>() where T : BlackJack21.CardFramework.AnimatedGameComponent

        {

            T animationComponent;

            for (int componentIndex = 0; componentIndex < Game.Components.Count; componentIndex++)

            {

                animationComponent = Game.Components[componentIndex] as T;

                if (animationComponent != null)

                {

                    if (animationComponent.IsAnimating)

                        return true;

                }

            }

            return false;

        }



        /// <summary>

        /// Ends the game.

        /// </summary>

        private void EndGame()

        {

            // Calculate the estimated time for all playing animations to end

            long estimatedTime = 0;

            BlackJack21.CardFramework.AnimatedGameComponent animationComponent;

            for (int componentIndex = 0; componentIndex < Game.Components.Count; componentIndex++)

            {

                animationComponent = Game.Components[componentIndex] as BlackJack21.CardFramework.AnimatedGameComponent;

                if (animationComponent != null)

                {

                    estimatedTime = Math.Max(estimatedTime,

                        animationComponent.EstimatedTimeForAnimationsCompletion().Ticks);

                }

            }



            // Add a component for an empty stalling animation. This actually acts

            // as a timer.

            Texture2D texture = this.Game.Content.Load<Texture2D>(@"Images\youlose");

            animationComponent = new BlackJack21.CardFramework.AnimatedGameComponent(this, texture)

            {

                CurrentPosition = new Vector2(

                    this.Game.GraphicsDevice.Viewport.Bounds.Center.X - texture.Width / 2,

                    this.Game.GraphicsDevice.Viewport.Bounds.Center.Y - texture.Height / 2),

                Visible = false

            };

            this.Game.Components.Add(animationComponent);



            // Add a button to return to the main menu

            Rectangle bounds = this.Game.GraphicsDevice.Viewport.Bounds;

            Vector2 center = new Vector2(bounds.Center.X, bounds.Center.Y);

            CardsGame.UI.Button backButton = new CardsGame.UI.Button("ButtonRegular", "ButtonPressed",

                screenManager.input, this)

            {

                Bounds = new Rectangle((int)center.X - 100, (int)center.Y + 80, 200, 50),

                Font = this.Font,

                Text = "Main Menu",

                Visible = false,

                Enabled = true,

            };



            backButton.Click += backButton_Click;



            // Add stalling animation

            animationComponent.AddAnimation(new BlackJack21.CardFramework.AnimatedGameComponentAnimation()

            {

                Duration = TimeSpan.FromTicks(estimatedTime) + TimeSpan.FromSeconds(1),

                PerformWhenDone = ResetGame,

                PerformWhenDoneArgs = new object[] { animationComponent, backButton }

            });

            Game.Components.Add(backButton);

        }



        /// <summary>

        /// Helper method to reset the game

        /// </summary>

        /// <param name="obj"></param>

        void ResetGame(object obj)

        {

            object[] arr = (object[])obj;

            State = BlackjackGameState.GameOver;

            ((BlackJack21.CardFramework.AnimatedGameComponent)arr[0]).Visible = true;

            ((CardsGame.UI.Button)arr[1]).Visible = true;



            // Remove all unnecessary game components

            for (int compontneIndex = 0; compontneIndex < Game.Components.Count;)

            {

                if ((Game.Components[compontneIndex] != ((BlackJack21.CardFramework.AnimatedGameComponent)arr[0]) &&

                    Game.Components[compontneIndex] != ((CardsGame.UI.Button)arr[1])) &&

                    (Game.Components[compontneIndex] is CardsGame.Misc.BetGameComponent ||

                    Game.Components[compontneIndex] is BlackJack21.CardFramework.AnimatedGameComponent ||

                    Game.Components[compontneIndex] is CardsGame.UI.Button))

                {

                    Game.Components.RemoveAt(compontneIndex);

                }

                else

                    compontneIndex++;

            }

        }



        /// <summary>

        /// Finishes the current turn.

        /// </summary>

        private void FinishTurn()

        {

            // Remove all unnecessary components

            for (int componentIndex = 0; componentIndex < Game.Components.Count; componentIndex++)

            {

                if (!(Game.Components[componentIndex] is _21BlackJack.CardsFrameWork.UserInterface.GameTable ||

                    Game.Components[componentIndex] is Game1 ||

                    Game.Components[componentIndex] is CardsGame.Misc.BetGameComponent ||

                    Game.Components[componentIndex] is CardsGame.UI.Button ||

                    Game.Components[componentIndex] is ScreenManager.ScreenManager ||

                    Game.Components[componentIndex] is _21BlackJack.Misc.InputHelper))

                {

                    if (Game.Components[componentIndex] is BlackJack21.CardFramework.AnimatedCardsGameComponent)

                    {

                        BlackJack21.CardFramework.AnimatedCardsGameComponent animatedCard =

                            (Game.Components[componentIndex] as BlackJack21.CardFramework.AnimatedCardsGameComponent);

                        animatedCard.AddAnimation(

                            new BlackJack21.CardFramework.TransitionGameComponentAnimation(animatedCard.CurrentPosition,

                            new Vector2(animatedCard.CurrentPosition.X, this.Game.GraphicsDevice.Viewport.Height))

                            {

                                Duration = TimeSpan.FromSeconds(0.40),

                                PerformWhenDone = RemoveComponent,

                                PerformWhenDoneArgs = animatedCard

                            });

                    }

                    else

                    {

                        Game.Components.RemoveAt(componentIndex);

                        componentIndex--;

                    }

                }

            }



            // Reset player values

            for (int playerIndex = 0; playerIndex < players.Count; playerIndex++)

            {

                (players[playerIndex] as CardsGame.Players.BlackjackPlayer).ResetValues();

                players[playerIndex].Hand.DealCardsToHand(deadCards, players[playerIndex].Hand.Count);

                turnFinishedByPlayer[playerIndex] = false;

                animatedHands[playerIndex] = null;

                animatedSecondHands[playerIndex] = null;

            }



            // Reset the bet component

            betGameComponent.Reset();

            betGameComponent.Enabled = true;



            // Reset dealer

            dealerPLayer.Hand.DealCardsToHand(deadCards, dealerPLayer.Hand.Count);

            dealerPLayer.ResetValues();



            // Reset rules

            rules.Clear();

        }



        /// <summary>

        /// Helper method to remove component

        /// </summary>

        /// <param name="obj"></param>

        void RemoveComponent(object obj)

        {

            Game.Components.Remove((BlackJack21.CardFramework.AnimatedGameComponent)obj);

        }



        /// <summary>

        /// Performs the "Stand" move for the current player.

        /// </summary>

        public void Stand()

        {

            CardsGame.Players.BlackjackPlayer player = (CardsGame.Players.BlackjackPlayer)GetCurrentPlayer();

            if (player == null)

                return;



            // If the player only has one hand, his turn ends. Otherwise, he now plays

            // using his next hand

            if (player.IsSplit == false)

            {

                turnFinishedByPlayer[players.IndexOf(player)] = true;

            }

            else

            {

                switch (player.CurrentHandType)

                {

                    case CardsGame.Players.HandTypes.First:

                        if (player.SecondBlackJack)

                        {

                            turnFinishedByPlayer[players.IndexOf(player)] = true;

                        }

                        else

                        {

                            player.CurrentHandType = CardsGame.Players.HandTypes.Second;

                        }

                        break;

                    case CardsGame.Players.HandTypes.Second:

                        turnFinishedByPlayer[players.IndexOf(player)] = true;

                        break;

                    default:

                        throw new Exception(

                            "Player has an unsupported hand type.");

                }

            }

        }



        /// <summary>

        /// Performs the "Split" move for the current player.

        /// This includes adding the animations which shows the first hand splitting

        /// into two.

        /// </summary>

        public void Split()

        {

            CardsGame.Players.BlackjackPlayer player = (CardsGame.Players.BlackjackPlayer)GetCurrentPlayer();



            int playerIndex = players.IndexOf(player);



            player.InitializeSecondHand();



            Vector2 sourcePosition = animatedHands[playerIndex].GetCardGameComponent(null, 1).CurrentPosition;

            Vector2 targetPosition = animatedHands[playerIndex].GetCardGameComponent(null, 0).CurrentPosition +

                secondHandOffSet; ;

            // Create an animation moving the top card to the second hand location

            BlackJack21.CardFramework.AnimatedGameComponentAnimation animation = new BlackJack21.CardFramework.TransitionGameComponentAnimation(sourcePosition,

                    targetPosition)

            {

                StartTime = DateTime.Now,

                Duration = TimeSpan.FromSeconds(0.5f)

            };



            // Actually perform the split

            player.SplitHand();



            // Add additional chip stack for the second hand

            betGameComponent.AddChips(playerIndex, player.BetAmount,

                false, true);



            // Initialize visual representation of the second hand

            animatedSecondHands[playerIndex] =

                new CardsGame.UI.BlackJackAnimatedPlayerHandComponent(playerIndex, secondHandOffSet,

                    player.SecondHand, this);

            Game.Components.Add(animatedSecondHands[playerIndex]);



            BlackJack21.CardFramework.AnimatedCardsGameComponent animatedGameComponet = animatedSecondHands[playerIndex].GetCardGameComponent(null, 0);

            animatedGameComponet.IsFaceDown = false;

            animatedGameComponet.AddAnimation(animation);



            // Deal an additional cards to each of the new hands

            TraditionalCard card = dealer.DealCardToHand(player.Hand);

            AddDealAnimation(card, animatedHands[playerIndex], true, dealDuration,

                DateTime.Now + animation.EstimatedTimeForAnimationCompletion);

            card = dealer.DealCardToHand(player.SecondHand);

            AddDealAnimation(card, animatedSecondHands[playerIndex], true, dealDuration,

                DateTime.Now + animation.EstimatedTimeForAnimationCompletion +

                dealDuration);

        }



        /// <summary>

        /// Performs the "Double" move for the current player.

        /// </summary>

        public void Double()

        {

            CardsGame.Players.BlackjackPlayer player = (CardsGame.Players.BlackjackPlayer)GetCurrentPlayer();



            int playerIndex = players.IndexOf(player);



            switch (player.CurrentHandType)

            {

                case CardsGame.Players.HandTypes.First:

                    player.Double = true;

                    float betAmount = player.BetAmount;



                    if (player.IsSplit)

                    {

                        betAmount /= 2f;

                    }



                    betGameComponent.AddChips(playerIndex, betAmount, false, false);

                    break;

                case CardsGame.Players.HandTypes.Second:

                    player.SecoundDouble = true;

                    if (player.Double == false)

                    {

                        // The bet is evenly spread between both hands, add one half

                        betGameComponent.AddChips(playerIndex, player.BetAmount / 2f,

                            false, true);

                    }

                    else

                    {

                        // The first hand's bet is double, add one third of the total

                        betGameComponent.AddChips(playerIndex, player.BetAmount / 3f,

                            false, true);

                    }

                    break;

                default:

                    throw new Exception(

                        "Player has an unsupported hand type.");

            }

            Hit();

            Stand();

        }



        /// <summary>

        /// Performs the "Hit" move for the current player.

        /// </summary>

        public void Hit()

        {

            CardsGame.Players.BlackjackPlayer player = (CardsGame.Players.BlackjackPlayer)GetCurrentPlayer();

            if (player == null)

                return;



            int playerIndex = players.IndexOf(player);



            // Draw a card to the appropriate hand

            switch (player.CurrentHandType)

            {

                case CardsGame.Players.HandTypes.First:

                    TraditionalCard card = dealer.DealCardToHand(player.Hand);

                    AddDealAnimation(card, animatedHands[playerIndex], true,

                        dealDuration, DateTime.Now);

                    break;

                case CardsGame.Players.HandTypes.Second:

                    card = dealer.DealCardToHand(player.SecondHand);

                    AddDealAnimation(card, animatedSecondHands[playerIndex], true,

                        dealDuration, DateTime.Now);

                    break;

                default:

                    throw new Exception(

                        "Player has an unsupported hand type.");

            }

        }



        /// <summary>

        /// Changes the visiblility of most game buttons.

        /// </summary>

        /// <param name="visible">True to make the buttons visible, false to make

        /// them invisible.</param>

        void ChangeButtonsVisiblility(bool visible)

        {

            buttons["Hit"].Visible = visible;

            buttons["Stand"].Visible = visible;

            buttons["Double"].Visible = visible;

            buttons["Split"].Visible = visible;

            buttons["Insurance"].Visible = visible;

        }



        /// <summary>

        /// Enables or disable most game buttons.

        /// </summary>

        /// <param name="enabled">True to enable the buttons , false to 

        /// disable them.</param>

        void EnableButtons(bool enabled)

        {

            buttons["Hit"].Enabled = enabled;

            buttons["Stand"].Enabled = enabled;

            buttons["Double"].Enabled = enabled;

            buttons["Split"].Enabled = enabled;

            buttons["Insurance"].Enabled = enabled;

        }



        /// <summary>

        /// Add an indication that the player has passed on the current round.

        /// </summary>

        /// <param name="indexPlayer">The player's index.</param>

        public void ShowPlayerPass(int indexPlayer)

        {

            // Add animation component

            BlackJack21.CardFramework.AnimatedGameComponent passComponent = new BlackJack21.CardFramework.AnimatedGameComponent(this, cardsAssets["pass"])

            {

                CurrentPosition = GameTable.PlaceOrder(indexPlayer),

                Visible = false

            };

            Game.Components.Add(passComponent);



            // Hide insurance button only when the first payer passes

            Action<object> performWhenDone = null;

            if (indexPlayer == 0)

            {

                performWhenDone = HideInshurance;

            }

            // Add scale animation for the pass "card"

            passComponent.AddAnimation(new BlackJack21.CardFramework.ScaleGameComponentAnimation(2.0f, 1.0f)

            {

                AnimationCycles = 1,

                PerformBeforeStart = ShowComponent,

                PerformBeforSartArgs = passComponent,

                StartTime = DateTime.Now,

                Duration = TimeSpan.FromSeconds(1),

                PerformWhenDone = performWhenDone

            });

        }



        /// <summary>

        /// Helper method to hide insurance

        /// </summary>

        /// <param name="obj"></param>

        void HideInshurance(object obj)

        {

            showInsurance = false;

        }



        #region Event Handlers

        /// <summary>

        /// Shows the insurance button if the first player can afford insurance.

        /// </summary>

        /// <param name="sender">The sender.</param>

        /// <param name="e">The <see cref="System.EventArgs"/> instance containing 

        /// the event data.</param>

        void InsuranceGameRule(object sender, EventArgs e)

        {

            CardsGame.Players.BlackjackPlayer player = (CardsGame.Players.BlackjackPlayer)players[0];

            if (player.Balance >= player.BetAmount / 2)

            {

                showInsurance = true;

            }

        }



        /// <summary>

        /// Shows the bust visual cue after the bust rule has been matched.

        /// </summary>

        /// <param name="sender">The sender.</param>

        /// <param name="e">The <see cref="System.EventArgs"/> instance containing 

        /// the event data.</param>

        void BustGameRule(object sender, EventArgs e)

        {

            showInsurance = false;

            CardsGame.Rules.BlackJackGameEventArgs args = (e as CardsGame.Rules.BlackJackGameEventArgs);

            CardsGame.Players.BlackjackPlayer player = (CardsGame.Players.BlackjackPlayer)args.Player;



            CueOverPlayerHand(player, "bust", args.Hand, null);



            switch (args.Hand)

            {

                case CardsGame.Players.HandTypes.First:

                    player.Bust = true;



                    if (player.IsSplit && !player.SecondBlackJack)

                    {

                        player.CurrentHandType = CardsGame.Players.HandTypes.Second;

                    }

                    else

                    {

                        turnFinishedByPlayer[players.IndexOf(player)] = true;

                    }

                    break;

                case CardsGame.Players.HandTypes.Second:

                    player.SecondBust = true;

                    turnFinishedByPlayer[players.IndexOf(player)] = true;

                    break;

                default:

                    throw new Exception(

                        "Player has an unsupported hand type.");

            }

        }



        /// <summary>

        /// Shows the blackjack visual cue after the blackjack rule has been matched.

        /// </summary>

        /// <param name="sender">The sender.</param>

        /// <param name="e">The <see cref="System.EventArgs"/> instance containing 

        /// the event data.</param>

        void BlackJackGameRule(object sender, EventArgs e)

        {

            showInsurance = false;

            CardsGame.Rules.BlackJackGameEventArgs args = (e as CardsGame.Rules.BlackJackGameEventArgs);

            CardsGame.Players.BlackjackPlayer player = (CardsGame.Players.BlackjackPlayer)args.Player;



            CueOverPlayerHand(player, "blackjack", args.Hand, null);



            switch (args.Hand)

            {

                case CardsGame.Players.HandTypes.First:

                    player.BlackJack = true;



                    if (player.IsSplit)

                    {

                        player.CurrentHandType = CardsGame.Players.HandTypes.Second;

                    }

                    else

                    {

                        turnFinishedByPlayer[players.IndexOf(player)] = true;

                    }

                    break;

                case CardsGame.Players.HandTypes.Second:

                    player.SecondBlackJack = true;

                    if (player.CurrentHandType == CardsGame.Players.HandTypes.Second)

                    {

                        turnFinishedByPlayer[players.IndexOf(player)] = true;

                    }

                    break;

                default:

                    throw new Exception(

                        "Player has an unsupported hand type.");

            }

        }



        /// <summary>

        /// Handles the Click event of the insurance button.

        /// </summary>

        /// <param name="sender">The source of the event.</param>

        /// <param name="e">The 

        /// <see cref="System.EventArgs"/> instance containing the event data.</param>

        void Insurance_Click(object sender, EventArgs e)

        {

            CardsGame.Players.BlackjackPlayer player = (CardsGame.Players.BlackjackPlayer)GetCurrentPlayer();

            if (player == null)

                return;

            player.IsInsurance = true;

            player.Balance -= player.BetAmount / 2f;

            betGameComponent.AddChips(players.IndexOf(player), player.BetAmount / 2, true, false);

            showInsurance = false;

        }



        /// <summary>

        /// Handles the Click event of the new game button.

        /// </summary>

        /// <param name="sender">The source of the event.</param>

        /// <param name="e">The 

        /// <see cref="System.EventArgs"/> instance containing the event data.</param>

        void newGame_Click(object sender, EventArgs e)

        {

            FinishTurn();

            StartRound();

            newGame.Enabled = false;

            newGame.Visible = false;

        }



        /// <summary>

        /// Handles the Click event of the hit button.

        /// </summary>

        /// <param name="sender">The source of the event.</param>

        /// <param name="e">The 

        /// <see cref="System.EventArgs"/> instance containing the event data.</param>

        void Hit_Click(object sender, EventArgs e)

        {

            Hit();

            showInsurance = false;

        }



        /// <summary>

        /// Handles the Click event of the stand button.

        /// </summary>

        /// <param name="sender">The source of the event.</param>

        /// <param name="e">The 

        /// <see cref="System.EventArgs"/> instance containing the event data.</param>

        void Stand_Click(object sender, EventArgs e)

        {

            Stand();

            showInsurance = false;

        }



        /// <summary>

        /// Handles the Click event of the double button.

        /// </summary>

        /// <param name="sender">The source of the event.</param>

        /// <param name="e">The 

        /// <see cref="System.EventArgs"/> instance containing the event data.</param>

        void Double_Click(object sender, EventArgs e)

        {

            Double();

            showInsurance = false;

        }



        /// <summary>

        /// Handles the Click event of the split button.

        /// </summary>

        /// <param name="sender">The source of the event.</param>

        /// <param name="e">The 

        /// <see cref="System.EventArgs"/> instance containing the event data.</param>

        void Split_Click(object sender, EventArgs e)

        {

            Split();

            showInsurance = false;

        }



        /// <summary>

        /// Handles the Click event of the back button.

        /// </summary>

        /// <param name="sender">The source of the event.</param>

        /// <param name="e">>The 

        /// <see cref="System.EventArgs"/> instance containing the event data.</param>

        void backButton_Click(object sender, EventArgs e)

        {

            // Remove all unnecessary components

            for (int componentIndex = 0; componentIndex < Game.Components.Count; componentIndex++)

            {

                if (!(Game.Components[componentIndex] is ScreenManager.ScreenManager))

                {

                    Game.Components.RemoveAt(componentIndex);

                    componentIndex--;

                }

            }



            foreach (ScreenManager.GameScreen screen in screenManager.GetScreen())

                screen.ExitScreen();



            screenManager.AddScreen(new Screens.BackGround(), null);

            screenManager.AddScreen(new Screens.MainMenuScreen(), null);

        }

        #endregion

    }

}

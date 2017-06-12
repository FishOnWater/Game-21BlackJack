using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using _21BlackJack.Cards_FrameWork;
using _21BlackJack.Cards_FrameWork.Game;
using _21BlackJack.Cards_FrameWork.Player;
using _21BlackJack.Cards_FrameWork.Rules;
using _21BlackJack.Cards_FrameWork.Utility;


namespace _21BlackJack.CardsGame.Misc
{
    public class BetGameComponent :DrawableGameComponent
    {
        #region Fields and Properties
        List<Player> players;
        string theme;
        int[] assetNames = { 5, 25, 100, 500 };
        Dictionary<int, Texture2D> chipAssets;
        Texture2D blankChip;
        Vector2[] positions;
        Cards_FrameWork.Game.CardsGame cardGame;
        SpriteBatch spriteBatch;

        bool isKeyDown = false;

        CardsGame.UI.Button bet;
        CardsGame.UI.Button clear;

        Vector2 ChipOffset { get; set; }
        static float insurancePosition = 120 * CardsGame.BlackJackGame.HeightScale;
        static Vector2 secondHandOffSet = new Vector2(25 * CardsGame.BlackJackGame.WidthScale, 30 * CardsGame.BlackJackGame.HeightScale);

        List<BlackJack21.CardFramework.AnimatedGameComponent> currentChipComponent = new List<BlackJack21.CardFramework.AnimatedGameComponent>();
        int currentBet = 0;
        ScreenManager.InputState input;
        _21BlackJack.Misc.InputHelper inputHelper;
        #endregion

        public BetGameComponent(List<Cards_FrameWork.Player.Player> players, ScreenManager.InputState input, string theme, Cards_FrameWork.Game.CardsGame cardGame)
            :base(cardGame.Game)
        {
            this.players = players;
            this.theme = theme;
            this.cardGame = cardGame;
            this.input = input;
            chipAssets = new Dictionary<int, Texture2D>();
        }

        public override void Initialize()
        {
            inputHelper = null;
            for(int componentIndex=0; componentIndex<Game.Components.Count; componentIndex++)
            {
                if(Game.Components[componentIndex] is _21BlackJack.Misc.InputHelper)
                {
                    inputHelper = (_21BlackJack.Misc.InputHelper)Game.Components[componentIndex];
                    break;
                }
            }
            Game.IsMouseVisible = true;
            base.Initialize();

            spriteBatch = new SpriteBatch(Game.GraphicsDevice);

            Rectangle size = chipAssets[assetNames[0]].Bounds;
            Rectangle bounds = spriteBatch.GraphicsDevice.Viewport.TitleSafeArea;

            positions[chipAssets.Count - 1] = new Vector2(bounds.Left + 10, bounds.Bottom - size.Height - 80);
            for(int chipIndex=2; chipIndex<=chipAssets.Count; chipIndex++)
            {
                size = chipAssets[assetNames[chipAssets.Count - chipIndex]].Bounds;
                positions[chipAssets.Count - chipIndex] = positions[chipAssets.Count - (chipIndex - 1)] - new Vector2(0, size.Height + 10);
            }

            bet = new CardsGame.UI.Button("Button Regular", "ButtonPressed",input, cardGame)
            {
                Bounds = new Rectangle(bounds.Left + 10, bounds.Bottom - 60, 100, 50),
                Font = cardGame.Font,
                Text = "Deal",
            };
            bet.Click += Bet_Click;
            Game.Components.Add(bet);

            clear = new CardsGame.UI.Button("Button Regular", "Button Pressed",input, cardGame)
            {
                Bounds = new Rectangle(bounds.Left + 120, bounds.Bottom - 60, 100, 50),
                Font = cardGame.Font,
                Text = "Clear",
            };
            clear.Click += Clear_Click;
            Game.Components.Add(clear);
            ShowAndEnableButtons(false);
        }

        protected override void LoadContent()
        {
            blankChip = Game.Content.Load<Texture2D>(string.Format(@"Images\Chips\chip{0}", "White"));

            int[] assetNames = { 5, 25, 100, 500 };
            for(int chipIndex=0; chipIndex < assetNames.Length; chipIndex++)
            {
                chipAssets.Add(assetNames[chipIndex], Game.Content.Load<Texture2D>(string.Format(@"Images\Chips\chip{0}", assetNames[chipIndex])));
            }
            positions = new Vector2[assetNames.Length];
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            if (players.Count > 0)
            {
                if(((Game1)cardGame).State == BlackjackGameState.Betting && !((CardsGame.Players.BlackjackPlayer)players[players.Count-1]).IsDoneBetting)
                {
                    int playerIndex = GetCurrentPlayer();

                    CardsGame.Players.BlackjackPlayer player = (CardsGame.Players.BlackjackPlayer)players[playerIndex];
                    //se for necessário adiciona-se aqui os aiplayers
                }
            }

            if (((CardsGame.Players.BlackjackPlayer)players[players.Count - 1]).IsDoneBetting)
            {
                Game1 blackjackGame = ((Game1)cardGame);

                if (!blackjackGame.CheckForRunningAnimations<BlackJack21.CardFramework.AnimatedGameComponent>())
                {
                    ShowAndEnableButtons(false);
                    blackjackGame.State = BlackjackGameState.Dealing;

                    Enabled = false;
                }
            }
            base.Update(gameTime);
        }

        private int GetCurrentPlayer()
        {
            for(int playerIndex = 0; playerIndex < players.Count; playerIndex++)
            {
                if (!((CardsGame.Players.BlackjackPlayer)players[playerIndex]).IsDoneBetting)
                {
                    return playerIndex;
                }
            }
            return -1;
        }

        private void HandleInput(MouseState mouseState)
        {
            bool isPressed = false;
            Vector2 position = Vector2.Zero;

            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                isPressed = true;
                position = new Vector2(mouseState.X, mouseState.Y);
            }
            else if (inputHelper.IsPressed)
            {
                isPressed = true;
                position = inputHelper.PointPosition;
            }
            if (isPressed)
            {
                if (isKeyDown)
                {
                    int chipvalue = GetIntersectingChipValue(position);
                    if (chipvalue != 0)
                    {
                        AddChip(GetCurrentPlayer(), chipvalue, false);
                    }
                    isKeyDown = true;
                }
            }
            else
                isKeyDown = false;
        }

        private int GetIntersectingChipValue(Vector2 position)
        {
            Rectangle size;
            Rectangle touchTap = new Rectangle((int)position.X - 1, (int)position.Y - 1, 2, 2);
            for(int chipIndex=0; chipIndex<chipAssets.Count; chipIndex++)
            {
                size = chipAssets[assetNames[chipIndex]].Bounds;
                size.X = (int)positions[chipIndex].X;
                size.Y = (int)positions[chipIndex].Y;
                if (size.Intersects(touchTap))
                    return assetNames[chipIndex];
            }
            return 0;
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            for(int chipIndex =0; chipIndex<chipAssets.Count; chipIndex++)
            {
                spriteBatch.Draw(chipAssets[assetNames[chipIndex]], positions[chipIndex], Color.White);
            }

            CardsGame.Players.BlackjackPlayer player;

            for(int playerIndex =0; playerIndex<players.Count; playerIndex++)
            {
                CardsGame.UI.BlackJackTable table = (CardsGame.UI.BlackJackTable)cardGame.GameTable;
                Vector2 position = table.RingOffSet + new Vector2(table.RingTexture.Bounds.Width, 0);
                player = (CardsGame.Players.BlackjackPlayer)players[playerIndex];
                spriteBatch.DrawString(cardGame.Font, "$" + player.BetAmount.ToString(), position, Color.White);
                spriteBatch.DrawString(cardGame.Font, "$" + player.Balance.ToString(), position + new Vector2(0, 30), Color.White);
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }

        public void AddChip(int playerIndex, int chipValue, bool secondHand)
        {
            if (((CardsGame.Players.BlackjackPlayer)players[playerIndex]).Bet(chipValue))
            {
                currentBet += chipValue;

                BlackJack21.CardFramework.AnimatedGameComponent chipComponent = new BlackJack21.CardFramework.AnimatedGameComponent(cardGame, chipAssets[chipValue])
                {
                    Visible = false
                };
                Game.Components.Add(chipComponent);

                Vector2 position;
                position = cardGame.GameTable[playerIndex] + ChipOffset + new Vector2(-currentChipComponent.Count * 2, currentChipComponent.Count * 1);

                int currentChipIndex = 0;
                for(int chipIndex=0; chipIndex<chipAssets.Count; chipIndex++)
                {
                    if (assetNames[chipIndex] == chipValue)
                    {
                        currentChipIndex = chipIndex;
                        break;
                    }
                }

                chipComponent.AddAnimation(new BlackJack21.CardFramework.TransitionGameComponentAnimation(positions[currentChipIndex], position)
                {
                    Duration = TimeSpan.FromSeconds(1f),
                    PerformBeforeStart = ShowComponent,
                    PerformBeforSartArgs = chipComponent,
                    PerformWhenDone = PlayBetSound
                });

                chipComponent.AddAnimation(new BlackJack21.CardFramework.FlipGameComponentAnimation()
                {
                    Duration = TimeSpan.FromSeconds(1f),
                    AnimationCycles = 3
                });
            }
        }

        void ShowComponent(object obj)
        {
            ((BlackJack21.CardFramework.AnimatedGameComponent)obj).Visible = true;
        }

        void PlayBetSound(object obj)
        {
            _21BlackJack.Misc.AudioManager.PlaySound("Bet");  
        }

        public void AddChips(int playerIndex, float amount, bool insurance, bool secondHand)
        {
            if (insurance)
            {
                AddInsuranceChipAnimation(amount);
            }
            else
            {
                AddChips(playerIndex, amount, secondHand);
            }
        }

        public void Reset()
        {
            ShowAndEnableButtons(true);
            currentChipComponent.Clear();
        }

        public void CalculateBalance(CardsGame.Players.BlackjackPlayer dealerPlayer)
        {
            for(int playerIndex=0; playerIndex<players.Count; playerIndex++)
            {
                CardsGame.Players.BlackjackPlayer player = (CardsGame.Players.BlackjackPlayer)players[playerIndex];
                float factor = CalculateFactorFromHand(dealerPlayer, player, CardsGame.Players.HandTypes.First);
                if (player.IsSplit)
                {
                    float factor2 = CalculateFactorFromHand(dealerPlayer, player, CardsGame.Players.HandTypes.Second);
                    float initialBet = player.BetAmount / ((player.Double ? 2f : 1f) + (player.SecoundDouble ? 2f : 1f));

                    float bet1 = initialBet * (player.Double ? 2f : 1f);
                    float bet2 = initialBet * (player.SecoundDouble ? 2f : 1f);

                    player.Balance += bet1 * factor + bet2 * factor2;

                    if (player.IsInsurance && dealerPlayer.BlackJack)
                        player.Balance += initialBet;
               }
                else
                {
                    if (player.IsInsurance && dealerPlayer.BlackJack)
                        player.Balance += player.BetAmount;
                }
                player.ClearBet();
            }
        }

        private void AddChips(int playerIndex, float amount, bool secondHand)
        {
            int[] assetnames = { 5, 25, 100, 500 };

            while (amount > 0)
            {
                if(amount >= 5)
                {
                    for(int chipIndex =0;chipIndex < assetNames.Length; chipIndex++)
                    {
                        while (assetNames[chipIndex - 1] <= amount)
                        {
                            AddChip(playerIndex, assetNames[chipIndex - 1], secondHand);
                            amount -= assetNames[chipIndex - 1];
                        }
                    }
                }
                else
                {
                    amount = 0;
                }
            }
        }

        private void AddInsuranceChipAnimation(float amount)
        {
            BlackJack21.CardFramework.AnimatedGameComponent chipComponent = new BlackJack21.CardFramework.AnimatedGameComponent(cardGame, blankChip)
            {
                TextColor = Color.Black,
                Enabled = true,
                Visible = false
            };
            Game.Components.Add(chipComponent);

            chipComponent.AddAnimation(new BlackJack21.CardFramework.TransitionGameComponentAnimation(positions[0], new Vector2(GraphicsDevice.Viewport.Width / 2, insurancePosition))
            {
                PerformBeforeStart = ShowComponent,
                PerformBeforSartArgs = chipComponent,
                PerformWhenDone = ShowChipAmountAndPlayBetSound,
                PerformWhenDoneArgs = new object[] { chipComponent, amount },
                Duration = TimeSpan.FromSeconds(1),
                StartTime = DateTime.Now
            });

            chipComponent.AddAnimation(new BlackJack21.CardFramework.FlipGameComponentAnimation()
            {
                Duration = TimeSpan.FromSeconds(1f),
                AnimationCycles = 3
            });
        }

        void ShowChipAmountAndPlayBetSound(object obj)
        {
            object[] arr = (object[])obj;
            ((BlackJack21.CardFramework.AnimatedGameComponent)arr[0]).Text = arr[1].ToString();
            _21BlackJack.Misc.AudioManager.PlaySound("Bet");
        }

        private Vector2 GetChipOffSet(int playerIndex, bool secondHand)
        {
            Vector2 offSet = Vector2.Zero;

            CardsGame.UI.BlackJackTable table = ((CardsGame.UI.BlackJackTable)cardGame.GameTable);
            offSet = table.RingOffSet + new Vector2(table.RingTexture.Bounds.Width - blankChip.Bounds.Width, table.RingTexture.Bounds.Height - blankChip.Bounds.Height) / 2f;

            if (secondHand == true)
                offSet += secondHandOffSet;
            return offSet;
        }

        private void ShowAndEnableButtons(bool visibleEnabled)
        {
            bet.Visible = visibleEnabled;
            bet.Enabled = visibleEnabled;
            clear.Enabled = visibleEnabled;
            clear.Visible = visibleEnabled;
        }

        private float CalculateFactorFromHand(CardsGame.Players.BlackjackPlayer dealerPlayer, CardsGame.Players.BlackjackPlayer player, CardsGame.Players.HandTypes currentHand)
        {
            float factor;

            bool blackjack, bust, considerAce;
            int playerValue;
            player.CalculateValues();

            switch (currentHand)
            {
                case Players.HandTypes.First:
                    blackjack = player.BlackJack;
                    bust = player.Bust;
                    playerValue = player.FirstValue;
                    considerAce = player.FirstValueConsiderAce;
                    break;
                case Players.HandTypes.Second:
                    blackjack = player.BlackJack;
                    bust = player.SecondBust;
                    playerValue = player.SecondValue;
                    considerAce = player.SecondValueConsiderAce;
                    break;
                default:
                    throw new Exception("Player has an unsupported hand type.");
            }

            if (considerAce)
                playerValue += 10;
            if (bust)
                factor = -1;
            else if (dealerPlayer.Bust)
            {
                if (blackjack)
                {
                    factor = 1.5f;
                }
                else
                {
                    factor = 1;
                }
            }
            else if (dealerPlayer.BlackJack)
            {
                if (blackjack)
                {
                    factor = 0;
                }
                else
                {
                    factor = -1;
                }
            }
            else if (blackjack)
            {
                factor = 1.5f;
            }
            else
            {
                int dealerValue = dealerPlayer.FirstValue;

                if (dealerPlayer.FirstValueConsiderAce)
                    dealerValue += 10;
                if (playerValue > dealerValue)
                    factor = 1;
                else if (playerValue < dealerValue)
                    factor = -1;
                else
                    factor = 0;
            }
            return 0;
        }

        void Clear_Click(object sender, EventArgs e)
        {
            currentBet = 0;
            ((CardsGame.Players.BlackjackPlayer)players[GetCurrentPlayer()]).ClearBet();
            for(int chipComponentIndex =0; chipComponentIndex < currentChipComponent.Count; chipComponentIndex++)
            {
                Game.Components.Remove(currentChipComponent[chipComponentIndex]);
            }
            currentChipComponent.Clear();
        }

        void Bet_Click(object sender, EventArgs e)
        {
            int playerIndex = GetCurrentPlayer();
            if (currentBet == 0)
                ((Game1)cardGame).ShowPlayerPass(playerIndex);
            ((CardsGame.Players.BlackjackPlayer)players[playerIndex]).IsDoneBetting = true;
            currentChipComponent.Clear();
            currentBet = 0;
        }
    }
}

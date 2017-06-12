using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using _21BlackJack.Cards_FrameWork.Rules;

namespace _21BlackJack.Cards_FrameWork.Game
{
    public abstract class CardsGame
    {
        #region Fields and Properties
        protected List<GameRule> rules;
        protected List<Player.Player> players;
        protected CardPacket dealer;

        public int MinimumPLayers { get; protected set; }
        public int MaximumPlayers { get; protected set; }

        public string Theme { get; protected set; }
        protected internal Dictionary<string, Texture2D> cardsAssets;
        public _21BlackJack.CardsFrameWork.UserInterface.GameTable GameTable { get; protected set; } //erro cuz no stuff
        public SpriteFont Font { get; set; }
        public SpriteBatch SpriteBatch { get; set; }
        public Microsoft.Xna.Framework.Game Game { get; set; }
        #endregion

        #region Inicializações
        public CardsGame(int decks, int jokersInDeck, CardSuit suits, CardValue cardValues, int minimumPlayers, int maximumPlayers, _21BlackJack.CardsFrameWork.UserInterface.GameTable gameTable, string theme, Microsoft.Xna.Framework.Game game)
        {
            rules = new List<GameRule>();
            players = new List<Player.Player>();
            dealer = new CardPacket(decks, jokersInDeck, suits, cardValues);
            Game = game;
            MinimumPLayers = minimumPlayers;
            MaximumPlayers = maximumPlayers;
            this.Theme = theme;
            cardsAssets = new Dictionary<string, Texture2D>();
            GameTable = gameTable;
            GameTable.DrawOrder = -1000;
            Game.Components.Add(GameTable);
        }
        #endregion

        #region Virtual Methods
        public virtual int CardValue(TraditionalCard card)
        {
            switch (card.Value)
            {
                case Cards_FrameWork.CardValue.Ace:
                    return 1;
                case Cards_FrameWork.CardValue.Two:
                    return 2;
                case Cards_FrameWork.CardValue.Three:
                    return 3;
                case Cards_FrameWork.CardValue.Four:
                    return 4;
                case Cards_FrameWork.CardValue.Five:
                    return 5;
                case Cards_FrameWork.CardValue.Six:
                    return 6;
                case Cards_FrameWork.CardValue.Seven:
                    return 7;
                case Cards_FrameWork.CardValue.Eight:
                    return 8;
                case Cards_FrameWork.CardValue.Nine:
                    return 9;
                case Cards_FrameWork.CardValue.Ten:
                    return 10;
                case Cards_FrameWork.CardValue.Jack:
                    return 11;
                case Cards_FrameWork.CardValue.Queen:
                    return 12;
                case Cards_FrameWork.CardValue.King:
                    return 13;
                default:
                    throw new ArgumentException("Ambiguous card value");
            }
        }
        #endregion

        #region Abstract Methods
        public abstract void AddPlayer(Player.Player player);
        public abstract Player.Player GetCurrentPlayer();
        public abstract void Deal();
        public abstract void StartPlaying();

        public void LoadContent()
        {
            SpriteBatch = new SpriteBatch(Game.GraphicsDevice);
            //Inicia um deck
            CardPacket fullDeck = new CardPacket(1, 2, CardSuit.AllSuits, Cards_FrameWork.CardValue.NonJokers | Cards_FrameWork.CardValue.Jokers);
            string assetName;

            for (int cardIndex = 0; cardIndex < 54; cardIndex++)
            {
                assetName = _21BlackJack.Cards_FrameWork.Utility.UIUtility.GetCardAssetName(fullDeck[cardIndex]);
            }
            LoadUITexture("Cards", "CardBack_" + Theme);

                Font = Game.Content.Load<SpriteFont>(string.Format(@"Fonts\Regular"));

            GameTable.Initialize();
        }

        public void LoadUITexture(string folder, string assetName)
        {
            cardsAssets.Add(assetName, Game.Content.Load<Texture2D>(string.Format(@"Images\{0}\{1}", folder, assetName)));
        }
        #endregion
    }
}

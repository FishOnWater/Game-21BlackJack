using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using System.IO;
using System.IO.IsolatedStorage;

namespace _21BlackJack.ScreenManager
{
    public class ScreenManager : DrawableGameComponent
    {
        #region Fields
        List<GameScreen> screens = new List<GameScreen>();
        List<GameScreen> screensToUpdate = new List<GameScreen>();

        public InputState input = new InputState();  //trocar depois pelo nosso InputWrapper

        SpriteBatch spriteBatch;
        SpriteFont font;
        Texture2D blankTexture;
        Texture2D buttonBackgroung;

        bool isInitialized;
        bool traceEnabled;
        #endregion

        #region Properties

        public SpriteBatch SpriteBatch
        {
            get { return spriteBatch; }
        }
        public Texture2D ButtonBackGround
        {
            get { return buttonBackgroung; }
        }
        public Texture2D BlankTexture
        {
            get { return blankTexture; }
        }
        public SpriteFont Font
        {
            get { return font; }
        }
        public bool TraceEnabled
        {
            get { return traceEnabled; }
            set { traceEnabled = value; }
        }
        public Rectangle SafeArea
        {
            get
            {
                return Game.GraphicsDevice.Viewport.TitleSafeArea;
            }
        }
        #endregion

        #region Inicializações
        public ScreenManager(Game Game)
            : base(Game){}

        public override void Initialize()
        {
            base.Initialize();
            isInitialized = true;
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            ContentManager content = Game.Content;

            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = content.Load<SpriteFont>("Fonts/Menu");
            blankTexture = content.Load<Texture2D>("Images/blank");
            buttonBackgroung = content.Load<Texture2D>("Images/ButtonRegular");

            foreach (GameScreen screen in screens)
                screen.LoadContent();
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
            foreach (GameScreen screen in screens)
            {
                screen.UnloadContent();
            }
        }
        #endregion

        #region Update and Draw
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            input.Update();
            screensToUpdate.Clear();

            foreach (GameScreen screen in screens)
                screensToUpdate.Add(screen);

            bool otherScreenHasFocus = !Game.IsActive;
            bool coveredByOtherScreen = false;

            //Loop as long as there are screens waiting to be updated
            while (screensToUpdate.Count > 0)
            {
                GameScreen screen = screensToUpdate[screensToUpdate.Count - 1];
                screensToUpdate.RemoveAt(screensToUpdate.Count - 1);

                screen.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

                if(screen.ScreenState == ScreenState.TransitionOn || screen.ScreenState == ScreenState.Active)
                {
                    if (!otherScreenHasFocus)
                    {
                        screen.HandleInput(input);
                        otherScreenHasFocus = true;
                    }

                    if (!screen.IsPopUp)
                        coveredByOtherScreen = true;
                }
            }

            if (traceEnabled)
                TraceScreens();
        }

        void TraceScreens()
        {
            List<string> screenNames = new List<string>();

            foreach (GameScreen screen in screens)
                screenNames.Add(screen.GetType().Name);

            Debug.WriteLine(string.Join(",", screenNames.ToArray()));
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            foreach (GameScreen screen in screens)
            {
                if (screen.ScreenState == ScreenState.Hidden)
                    continue;

                screen.Draw(gameTime);
            }
        }
        #endregion

        #region Public Methods
        public void AddScreen(GameScreen screen, PlayerIndex? controllingPlayer)
        {
            screen.ControllingPlayer = controllingPlayer;
            screen.ScreenManager = this;
            screen.IsExiting = false;

            if (isInitialized)
            {
                screen.LoadContent();
            }
            screens.Add(screen);
        }

        public void RemoveScreen(GameScreen screen)
        {
            if (isInitialized)
            {
                screen.UnloadContent(); //não tem if de smartphone
            }
            screens.Remove(screen);
            screensToUpdate.Remove(screen);
        }

        public GameScreen[] GetScreen()
        {
            return screens.ToArray();
        }

        public void FadeBackBufferToBlack(float alpha)
        {
            Viewport viewport = GraphicsDevice.Viewport;
            spriteBatch.Draw(blankTexture, new Rectangle(0, 0, viewport.Width, viewport.Height), Color.Black * alpha);
            spriteBatch.End();
        }

        public void SerializeState()
        {
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (storage.DirectoryExists("ScreenManager"))
                {
                    DeleteState(storage);
                }
                else
                {
                    storage.CreateDirectory("ScreenManager");
                }

                using (IsolatedStorageFileStream stream = storage.CreateFile("ScreenManager\\ScreenList.dat"))
                {
                    using (BinaryWriter writer=new BinaryWriter(stream))
                    {
                        foreach (GameScreen screen in screens)
                        {
                            if (screen.IsSerializable)
                            {
                                writer.Write(screen.GetType().AssemblyQualifiedName);
                            }
                        }
                    }
                }
                int screenIndex = 0;
                foreach (GameScreen screen in screens)
                {
                    if (screen.IsSerializable)
                    {
                        string fileName = string.Format("ScreenManager\\Screen{0}.dat", screenIndex);

                        using (IsolatedStorageFileStream stream = storage.CreateFile(fileName))
                        {
                            screen.Serialize(stream);
                        }
                        screenIndex++;
                    }
                }
            }
        }

        public bool DeserializeState()
        {
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (storage.DirectoryExists("ScreenManager"))
                {
                    try
                    {
                        if (storage.FileExists("ScreenManager\\ScreenList.dat"))
                        {
                            using (IsolatedStorageFileStream stream = storage.OpenFile("ScreenManager\\ScreenList.dat", FileMode.Open, FileAccess.Read))
                            {
                                using (BinaryReader reader = new BinaryReader(stream))
                                {
                                    while (reader.BaseStream.Position < reader.BaseStream.Length)
                                    {
                                        string line = reader.ReadString();

                                        if (!string.IsNullOrEmpty(line))
                                        {
                                            Type screenType = Type.GetType(line);
                                            GameScreen screen = Activator.CreateInstance(screenType) as GameScreen;
                                            AddScreen(screen, PlayerIndex.One);
                                        }
                                    }
                                }
                            }
                        }

                        for (int i=0; i< screens.Count; i++)
                        {
                            string filename = string.Format("ScreenManager\\Screen{0}.dat", i);
                            using (IsolatedStorageFileStream stream = storage.OpenFile(filename, FileMode.Open, FileAccess.Read))
                            {
                                screens[i].Deserialize(stream);
                            }
                        }
                        return true;
                    }
                    catch (Exception)
                    {
                        DeleteState(storage);
                    }
                }
            }
            return false;
        }

        private void DeleteState(IsolatedStorageFile storage)
        {
            string[] files = storage.GetFileNames("ScreenManager\\*");
            foreach (string file in files)
            {
                storage.DeleteFile(Path.Combine("ScreenManger", file));
            }
        }
        #endregion
    }
}

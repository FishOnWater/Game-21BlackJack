﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using System.IO;

namespace _21BlackJack.ScreenManager
{
    public enum ScreenState
    {
        TransitionOn,
        Active,
        TranstionOff,
        Hidden,
    }

    public abstract class GameScreen
    {
        public bool IsPopUp
        {
            get { return IsPopUp; }
            protected set { IsPopUp = value; }
        }

        bool isPopUp = false;

        public TimeSpan TransitionOnTime
        {
            get { return transitionOnTime; }
            protected set { transitionOnTime = value; }
        }

        TimeSpan transitionOnTime = TimeSpan.Zero;

        public TimeSpan TransitionOffTime
        {
            get { return transitionOffTime; }
            protected set { transitionOffTime = value; }
        }

        TimeSpan transitionOffTime = TimeSpan.Zero;

        public float TransitionPosition
        {
            get { return transitionPosition; }
            protected set { transitionPosition = value; }
        }

        float transitionPosition = 1;

        public float TransitionAlpha
        {
            get { return 1f - TransitionPosition; }
        }

        public ScreenState ScreenState
        {
            get { return screenState; }
            protected set { screenState = value; }
        }

        ScreenState screenState = ScreenState.TransitionOn;

        public bool IsExiting
        {
            get { return isExiting; }
            protected internal set { isExiting = value; }
        }
        bool isExiting = false;

        public bool IsActive
        {
            get
            {
                return !otherScreenHasFocus && (screenState == ScreenState.TransitionOn || screenState == ScreenState.Active);
            }
        }

        bool otherScreenHasFocus;

        public ScreenManager ScreenManager
        {
            get { return screenManager; }
            internal set { screenManager = value; }
        }

        ScreenManager screenManager;

        public PlayerIndex? ControllingPlayer
        {
            get { return controllingPlayer; }
            internal set { controllingPlayer = value; }
        }
        PlayerIndex? controllingPlayer;

        public bool IsSerializable
        {
            get { return isSerializable; }
            protected set { isSerializable = value; }
        }
        bool isSerializable = true;

        #region Inicialização

        public virtual void LoadContent() { }

        public virtual void UnloadContent() { }
        #endregion

        #region Update and Draw

        public virtual void Update(GameTime gameTime, bool otherScreeHasFocus, bool coveredByOtherScreen)
        {
            this.otherScreenHasFocus = otherScreeHasFocus;

            if (isExiting)
            {
                screenState = ScreenState.TranstionOff;

                if(!UpdateTransition(gameTime, transitionOffTime, 1))
                {
                    ScreenManager.RemoveScreen(this);
                }
                else if (coveredByOtherScreen)
                {
                    if(UpdateTransition(gameTime, transitionOffTime, 1))
                    {
                        screenState = ScreenState.TranstionOff;
                    }
                    else
                    {
                        screenState = ScreenState.Hidden;
                    }
                }
                else
                {
                    if(UpdateTransition(gameTime, transitionOnTime, -1))
                    {
                        screenState = ScreenState.TransitionOn;
                    }
                    else
                    {
                        screenState = ScreenState.Active;
                    }
                }
            }
        }

        bool UpdateTransition(GameTime gameTime, TimeSpan time, int direction)
        {
            float transitionDelta;

            if (time == TimeSpan.Zero)
                transitionDelta = 1;
            else
                transitionDelta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds / time.TotalMilliseconds);

            transitionPosition += transitionDelta * direction;

            if(((direction<0)&&(transitionPosition<=0)) || ((direction > 0)&&(transitionPosition >= 1)))
            {
                transitionPosition = MathHelper.Clamp(transitionPosition, 0, 1);
                return false;
            }

            return true;
        }

        public virtual void HandleInput(InputState input) { }

        public virtual void Draw(GameTime gameTime) { }
        #endregion

        #region Public Methods

        public virtual void Serialize(Stream stream) { }
        public virtual void Deserialize(Stream stream) { }
        public void ExitScreen()
        {
            if (TransitionOffTime == TimeSpan.Zero)
            {
                ScreenManager.RemoveScreen(this);
            }
            else
                isExiting = true;
        }
        #endregion

        #region Helper Methods
        public T Load<T> (string assetName)
        {
            return ScreenManager.Game.Content.Load<T>(assetName);
        }
        #endregion
    }
}

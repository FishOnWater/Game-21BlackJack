using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace BlackJack21.CardFramework
{
   public class AnimatedGameComponentAnimation
    {
        protected TimeSpan Elapsed { get; set; }
        public AnimatedGameComponent Component { get; internal set; }
        public Action<object> PerformBeforeStart;
        public object PerformBeforSartArgs { get; set; }
        public Action<object> PerformWhenDone;
        public object PerformWhenDoneArgs { get; set; }
        uint animationCycles = 1;

        public uint AnimationCycles
        {
            get
            {
                return animationCycles;
            }
            set
            {
                if (value > 0)
                {
                    animationCycles = value;
                }
            }
        }

        public DateTime StartTime { get; set; }
        public TimeSpan Duration { get; set; }

        public TimeSpan EstimatedTimeForAnimationCompletion
        {
            get
            {
                if (isStarted)
                {
                    return (Duration - Elapsed);
                }
                else
                {
                    return StartTime - DateTime.Now + Duration;
                }
            }
        }

        public bool IsLooped { get; set; }
        private bool isDone = false;
        private bool isStarted = false;

        public AnimatedGameComponentAnimation()
        {
            StartTime = DateTime.Now;
            Duration = TimeSpan.FromMilliseconds(150);
        }

        public bool IsDone()
        {
            if (!isDone)
            {
                isDone = !IsLooped && (Elapsed >= Duration);
                if (isDone && PerformWhenDone != null)
                {
                    PerformWhenDone(PerformWhenDoneArgs);
                    PerformWhenDone = null;
                }
            }
            return isDone;
        }

        public bool IsStarted()
        {
            if (!isStarted)
            {
                if (StartTime <= DateTime.Now)
                {
                    if (PerformBeforeStart != null)
                    {
                        PerformBeforeStart(PerformBeforSartArgs);
                        PerformBeforeStart = null;
                    }
                    StartTime = DateTime.Now;
                    isStarted = true;
                }
            }
            return isStarted;
        }

        internal void AccumulateElapsedTime(TimeSpan elapsedTime)
        {
            if (isStarted)
            {
                Elapsed += elapsedTime;
            }
        }

        public virtual void Run(GameTime gameTime)
        {
            bool isStarted = IsStarted();
        }
    }
}

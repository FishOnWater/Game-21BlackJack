using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace _21BlackJack.ScreenManager
{
    class PlayerIndexEvent : EventArgs
    {
        public PlayerIndexEvent(PlayerIndex playerIndex)
        {
            this.playerIndex = PlayerIndex;
        }

        public PlayerIndex PlayerIndex
        {
            get { return playerIndex; }
        }

        PlayerIndex playerIndex;
    }
}

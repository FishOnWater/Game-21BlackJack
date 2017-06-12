using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _21BlackJack.Cards_FrameWork.Utility
{
    public static class UIUtility
    {
        public static string GetCardAssetName(TraditionalCard card)
        {
            return string.Format("{0}{1}", ((card.Value | CardValue.FirstJoker) ==
                CardValue.FirstJoker || (card.Value | CardValue.SecondJoker) == CardValue.SecondJoker) ? "" : card.Type.ToString(), card.Value);
        }
    }
}

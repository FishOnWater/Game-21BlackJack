using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _21BlackJack.Cards_FrameWork;

namespace _21BlackJack.CardsGame.Rules
{
    class InsuranceRule : Cards_FrameWork.Rules.GameRule
    {
        Hand dealerHand;
        bool done = false;

        public InsuranceRule(Hand dealerHand)
        {
            this.dealerHand = dealerHand;
        }

        public override void Check()
        {
            if (!done)
            {
                if(dealerHand.Count > 0)
                {
                    if (dealerHand[0].Value == CardValue.Ace)
                    {
                        FireRuleMatch(EventArgs.Empty);
                    }
                    done = true;
                }
            }
        }
    }
}

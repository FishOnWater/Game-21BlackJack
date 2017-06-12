using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _21BlackJack.Cards_FrameWork.Rules
{
    public abstract class GameRule
    {
        public event EventHandler RuleMatch;

        public abstract void Check();

        protected void FireRuleMatch (EventArgs e)
        {
            if (RuleMatch != null)
                RuleMatch(this, e);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _21BlackJack.Cards_FrameWork;

namespace _21BlackJack.CardsGame.Rules
{
    public class BlackjackRule : Cards_FrameWork.Rules.GameRule
    {
        List<CardsGame.Players.BlackjackPlayer> players;

        public BlackjackRule(List<Cards_FrameWork.Player.Player> players)
        {
            this.players = new List<Players.BlackjackPlayer>();
            for (int playerIndex = 0; playerIndex < players.Count; playerIndex++)
                this.players.Add((Players.BlackjackPlayer)players[playerIndex]);
        }

        public override void Check()
        {
            for (int playerIndex =0; playerIndex < players.Count; playerIndex++)
            {
                players[playerIndex].CalculateValues();

                if (!players[playerIndex].BlackJack)
                {
                    if(((players[playerIndex].FirstValue==21) ||
                        (players[playerIndex].FirstValueConsiderAce && players[playerIndex].FirstValue+10 == 21)) && 
                        players[playerIndex].SecondHand.Count == 2)
                    {
                        FireRuleMatch(new BlackJackGameEventArgs()
                        {
                            Player = players[playerIndex],
                            Hand = Players.HandTypes.Second
                        });
                    }
                }
            }
        }
    }
}

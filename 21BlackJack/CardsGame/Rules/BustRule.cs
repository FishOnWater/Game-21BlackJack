using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _21BlackJack.CardsGame;

namespace _21BlackJack.CardsGame.Rules
{
    public class BustRule : Cards_FrameWork.Rules.GameRule
    {
        List<CardsGame.Players.BlackjackPlayer> players;

        public BustRule(List<Cards_FrameWork.Player.Player> players)
        {
            this.players = new List<Players.BlackjackPlayer>();
            for(int playerIndex = 0; playerIndex < players.Count; playerIndex++)
            {
                this.players.Add((Players.BlackjackPlayer)players[playerIndex]);
            }
        }
        public override void Check()
        {
            for (int playerIndex = 0; playerIndex < players.Count; playerIndex++)
            {
                players[playerIndex].CalculateValues();

                if (!players[playerIndex].Bust)
                {
                    if(!players[playerIndex].FirstValueConsiderAce && players[playerIndex].FirstValue > 21)
                    {
                        FireRuleMatch(new BlackJackGameEventArgs()
                        {
                            Player = players[playerIndex],
                            Hand = Players.HandTypes.First
                        });
                    }
                }
                if (!players[playerIndex].SecondBust)
                {
                    if((players[playerIndex].IsSplit &&
                        !players[playerIndex].SecondValueConsiderAce && 
                        players[playerIndex].SecondValue > 21))
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

using System;

namespace _21BlackJack
{
    static class Program
    {
        static void Main(string[] args)
        {
            try
            {
                using (CardsGame.BlackJackGame game = new CardsGame.BlackJackGame())
                {
                    game.Run();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.ReadLine();
            }
        }
    }
}

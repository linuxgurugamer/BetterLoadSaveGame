using BetterLoadSaveGame;
using System;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            foreach(var save in SaveGameManager.GetAllSaves(@"G:\Games\kerbal\1.2.2_mod_dev\saves\default"))
            {
                Console.WriteLine(save);
            }
        }
    }
}

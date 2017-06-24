using System.Collections.Generic;
using System.IO;

namespace BetterLoadSaveGame
{
    public static class SaveGameManager
    {
        public static IEnumerable<string> GetSaveFiles(string gameDir, string saveFolder)
        {
            return Directory.GetFiles(gameDir + "saves\\" + saveFolder, "*.sfs");
        }
    }
}

using System.Collections.Generic;
using System.IO;

namespace BetterLoadSaveGame
{
    public static class SaveGameManager
    {
        public static List<SaveGameInfo> GetAllSaves(string gameDir, string saveFolder)
        {
            var allSavesDir = Path.Combine(gameDir, "saves");
            var currentSavesDir = Path.Combine(allSavesDir, saveFolder);
            var result = new List<SaveGameInfo>();
            foreach (var saveFile in Directory.GetFiles(currentSavesDir, "*.sfs"))
            {
                result.Add(new SaveGameInfo(saveFile));
            }
            result.Sort((a, b) => b.SaveFile.CreationTime.CompareTo(a.SaveFile.CreationTime));
            return result;
        }
    }
}

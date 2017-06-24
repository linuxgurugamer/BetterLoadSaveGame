using System.Collections.Generic;
using System.IO;

namespace BetterLoadSaveGame
{
    public static class SaveGameManager
    {
        public static List<SaveGameInfo> GetAllSaves(string saveDir)
        {
            var result = new List<SaveGameInfo>();
            foreach (var saveFile in Directory.GetFiles(saveDir, "*.sfs"))
            {
                result.Add(new SaveGameInfo(saveFile));
            }
            result.Sort((a, b) => b.SaveFile.LastWriteTime.CompareTo(a.SaveFile.LastWriteTime));
            return result;
        }
    }
}

using System.IO;

namespace BetterLoadSaveGame
{
    static class Util
    {
        public static string SaveDir
        {
            get
            {
                return Path.GetFullPath(Path.Combine(Path.Combine(KSPUtil.ApplicationRootPath, "saves"), HighLogic.SaveFolder));
            }
        }
    }
}

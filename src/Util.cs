using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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

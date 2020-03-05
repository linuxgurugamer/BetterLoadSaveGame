using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace BetterLoadSaveGame
{
    class ManageOldSaves
    {
        private static SaveGameCollection _saveGameCollection;
        internal const string ARCHIVEDIR = "/Archive";
        static double AgeInMinutes(System.DateTime dt)
        {

            TimeSpan timeSpan = DateTime.Now.Subtract(dt);
            return timeSpan.TotalMinutes;
        }
        static internal void ManageSaves()
        {
            double fileAgeLimit = HighLogic.CurrentGame.Parameters.CustomParams<BLSG2>().fileAge;
            if (HighLogic.CurrentGame.Parameters.CustomParams<BLSG2>().hourUnit)
                fileAgeLimit *= 60;
            if (HighLogic.CurrentGame.Parameters.CustomParams<BLSG2>().dayUnit)
                fileAgeLimit *= 1440;

            Log.Info("ManageSaves, fileAgeLimit: " + fileAgeLimit);

            _saveGameCollection = new SaveGameCollection(null);

            foreach (SaveGameInfo save in _saveGameCollection.Saves)
            {
                if (AgeInMinutes(save.SaveFile.LastWriteTime) > fileAgeLimit)
                {
                    if (HighLogic.CurrentGame.Parameters.CustomParams<BLSG2>().archiveSaves)
                        MoveSaveToArchive(save);
                    if (HighLogic.CurrentGame.Parameters.CustomParams<BLSG2>().deleteSaves)
                        DeleteSaveGame(save);
                }
            }
            if (HighLogic.CurrentGame.Parameters.CustomParams<BLSG2>().archiveSaves)
                Main.fetch.RefreshSaves();
        }

        internal static void MoveSaveToArchive(SaveGameInfo sgi)
        {
            var name = Path.GetFileNameWithoutExtension(sgi.SaveFile.Name);
            string path = sgi.SaveFile.DirectoryName + ARCHIVEDIR;
            string suffix = "";
            if (File.Exists(path + "/" + name + suffix + ".sfs"))
            {
                int cnt = 0;
                while (File.Exists(path + "/" + name + suffix + ".sfs"))
                {
                    cnt++;
                    suffix = "-" + cnt;
                }
                Log.Error("Archive path exists as a file, renaming: " + path + "/" + name + suffix + ".sfs");
            }
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            if (!Directory.Exists(path))
            {
                Log.Error("Archive directory doesn't exist, and unable to create it: " + path);
                return;
            }

            MoveFile(sgi.SaveFile.DirectoryName, name ,  ".sfs", suffix, path);
            MoveFile(sgi.SaveFile.DirectoryName, name ,  ".loadmeta", suffix, path);
            MoveFile(sgi.SaveFile.DirectoryName, name ,  "-thumb.png", suffix, path);
        }

        static void MoveFile(string dirName, string fname, string suffix, string cntsuffix, string path)
        {
            Log.Info("MoveFile, old: " + dirName + "/" + fname + suffix);
            Log.Info("MoveFile, new: " + path + "/" + fname + cntsuffix + suffix);
            if (File.Exists(dirName + "/" + fname + suffix))
                File.Move(dirName + "/" + fname+suffix, path + "/" + fname+cntsuffix+suffix);
        }

        internal static void DeleteSaveGame(SaveGameInfo sgi)
        {
            var name = Path.GetFileNameWithoutExtension(sgi.SaveFile.Name);
            var game = GamePersistence.LoadGame(name, HighLogic.SaveFolder, true, false);

            DeleteFile(HighLogic.SaveFolder + "/" + name + ".sfs");
            DeleteFile(HighLogic.SaveFolder + "/" + name + ".loadmeta");
            DeleteFile(HighLogic.SaveFolder + "/" + name + "-thumb.png");
            Main.fetch.RefreshSaves();
        }
        static void DeleteFile(string str)
        {
            File.Delete(KSPUtil.ApplicationRootPath + "saves/" + str);
        }
    }
}

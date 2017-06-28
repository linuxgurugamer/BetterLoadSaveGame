using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace BetterLoadSaveGame
{
    public class SaveGameInfo
    {
        public FileInfo SaveFile { get; private set; }
        public Dictionary<string, string> MetaData { get; private set; }

        public SaveGameInfo(string saveFile)
        {
            SaveFile = new FileInfo(saveFile);
            MetaData = new Dictionary<string, string>();

            var metaFile = Path.ChangeExtension(saveFile, "loadmeta");

            if (File.Exists(metaFile))
            {
                var content = File.ReadAllLines(metaFile);
                foreach(var line in content)
                {
                    var idx = line.IndexOf("=");
                    var key = line.Substring(0, idx).Trim();
                    var value = line.Substring(idx + 1).Trim();
                    MetaData[key] = value;
                }
            }

            string funds = "";
            if (MetaData.TryGetValue("funds", out funds))
            {
                double fundsAmount;
                if (double.TryParse(funds, out fundsAmount))
                {
                    fundsAmount /= 1000.0;
                    string suffix = "k";
                    if (fundsAmount > 1000)
                    {
                        fundsAmount /= 1000.0;
                        suffix = "m";
                    }
                    if (fundsAmount > 1000)
                    {
                        fundsAmount /= 1000.0;
                        suffix = "b";
                    }
                    funds = Math.Round(fundsAmount, 1).ToString() + suffix;
                }
            }

            string gameTime = "";
            if (MetaData.TryGetValue("UT", out gameTime))
            {
                double gameTimeVal;
                if (double.TryParse(gameTime, out gameTimeVal))
                {
                    GameTime = gameTimeVal;
                    GameTimeText = KSPUtil.PrintDate(GameTime, includeTime: true);
                }
            }

            ButtonText = String.Format("  {0}\n  {1}\n  {2} funds\n  {3}",
                SaveFile.LastWriteTime,
                Path.GetFileNameWithoutExtension(SaveFile.Name),
                funds,
                GameTimeText);
        }

        public string ButtonText { get; private set; }
        public double GameTime { get; private set; }
        public string GameTimeText { get; private set; }
    }
}

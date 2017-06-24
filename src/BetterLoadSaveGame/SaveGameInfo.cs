using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
                    MetaData.Add(key, value);
                }
            }
        }

        public override string ToString()
        {
            return SaveFile.FullName + " : " + String.Join(", ", MetaData.Select(r => r.Key + " = " + r.Value).ToArray());
        }
    }

    public class SaveData
    {
        public string Name;
        public IEnumerable<SaveData> Sections;
        public IEnumerable<KeyValuePair<string, string>> Values;
    }
}

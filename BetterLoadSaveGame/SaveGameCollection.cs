using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace BetterLoadSaveGame
{
    public enum SortModeEnum
    {
        FileTime,
        GameTime,
        Name
    }

    class SaveGameCollection
    {
        internal List<SaveGameInfo> _saves = new List<SaveGameInfo>();
        private SortModeEnum _sortMode = SortModeEnum.FileTime;

        internal string ArchiveDir { set; get; }

        public SaveGameCollection(SaveWatcher saveWatcher)
        {
            if (saveWatcher != null)
                saveWatcher.OnSave += OnSave;
            ArchiveDir = "";
            LoadAllGames();
        }

        public void LoadAllGames()
        {
            _saves.Clear();
            foreach (var file in Directory.GetFiles(Util.SaveDir + ArchiveDir))
            {
                if (file.EndsWith(".sfs"))
                {
                    if (!file.EndsWith("persistent.sfs") || !HighLogic.CurrentGame.Parameters.CustomParams<BLSG1>().hidePersistent)
                    {
                        var fullPath = Path.Combine(Util.SaveDir, file);
                        _saves.Add(new SaveGameInfo(file));
                    }
                }
            }
            UpdateSort();

        }
        public SortModeEnum SortMode
        {
            get
            {
                return _sortMode;
            }
            set
            {
                _sortMode = value;
                UpdateSort();
            }
        }

        public IEnumerable<SaveGameInfo> Saves
        {
            get { return _saves; }
        }

        double lastTimeSaved = 0;

        private void OnSave(object sender, System.IO.FileSystemEventArgs e)
        {
            if (SystemInfo.operatingSystemFamily != OperatingSystemFamily.Windows)
            {
                //
                // The following is because of behaviour on OSX 
                // That sends multiple OnSave messages for a single save
                //

                if (Time.realtimeSinceStartup - lastTimeSaved < 2)
                    return;
                lastTimeSaved = Time.realtimeSinceStartup;
            }
            if (e.FullPath.EndsWith(".sfs"))
            {
                var newSave = new SaveGameInfo(e.FullPath);
                var existingSave = _saves.Find(r => r.SaveFile.Name == newSave.SaveFile.Name);
                if (existingSave != null)
                {
                    _saves.Remove(existingSave);
                }
                _saves.Add(newSave);
                UpdateSort();
            }
        }

        private void UpdateSort()
        {
            switch (_sortMode)
            {
                case SortModeEnum.FileTime:
                    _saves.Sort((a, b) => b.SaveFile.LastWriteTime.CompareTo(a.SaveFile.LastWriteTime));
                    break;

                case SortModeEnum.GameTime:
                    _saves.Sort((a, b) => b.GameTime.CompareTo(a.GameTime));
                    break;

                case SortModeEnum.Name:
                    _saves.Sort((a, b) => a.SaveFile.Name.CompareTo(b.SaveFile.Name));
                    break;
            }
        }
    }
}

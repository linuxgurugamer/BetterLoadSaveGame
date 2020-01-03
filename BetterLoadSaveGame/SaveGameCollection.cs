using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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

        public SaveGameCollection(SaveWatcher saveWatcher)
        {
            saveWatcher.OnSave += OnSave;

            foreach(var file in Directory.GetFiles(Util.SaveDir))
            {
                if (file.EndsWith(".sfs"))
                {
                    var fullPath = Path.Combine(Util.SaveDir, file);
                    _saves.Add(new SaveGameInfo(file));
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

        private void OnSave(object sender, System.IO.FileSystemEventArgs e)
        {
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
                    Log.Info("Sorting by file time");
                    _saves.Sort((a, b) => b.SaveFile.LastWriteTime.CompareTo(a.SaveFile.LastWriteTime));
                    break;

                case SortModeEnum.GameTime:
                    Log.Info("Sorting by game time");
                    _saves.Sort((a, b) => b.GameTime.CompareTo(a.GameTime));
                    break;

                case SortModeEnum.Name:
                    Log.Info("Sorting by file time");
                    _saves.Sort((a, b) => a.SaveFile.Name.CompareTo(b.SaveFile.Name));
                    break;
            }
        }
    }
}

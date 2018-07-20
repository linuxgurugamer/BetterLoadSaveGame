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
        private List<SaveGameInfo> _saves = new List<SaveGameInfo>();
        private SortModeEnum _sortMode = SortModeEnum.FileTime;

        public void Start()
        {
            SaveWatcher.OnSave += OnSave;

            foreach(var file in Directory.GetFiles(SaveWatcher.SaveDir))
            {
                if (file.EndsWith(".sfs"))
                {
                    var fullPath = Path.Combine(SaveWatcher.SaveDir, file);
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
                _saves.Add(new SaveGameInfo(e.FullPath));
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

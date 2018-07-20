using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BetterLoadSaveGame
{
    static class SaveWatcher
    {
        private static FileSystemWatcher _watcher;
        private static string _saveDir;

        public static void Start()
        {
            _saveDir = Path.Combine(Path.Combine(KSPUtil.ApplicationRootPath, "saves"), HighLogic.SaveFolder);

            Log.Info("Watching save dir: " + _saveDir);

            _watcher = new FileSystemWatcher(_saveDir);

            _watcher.Created += _watcher_Created;

            _watcher.EnableRaisingEvents = true;
        }

        private static void _watcher_Created(object sender, FileSystemEventArgs e)
        {
            Log.Info("watcher created: " + e.FullPath);
        }

        public static void Stop()
        {
            if (_watcher != null)
            {
                _watcher.Dispose();
                _watcher = null;
            }
        }

        public static string SaveDir
        {
            get { return _saveDir; }
        }

        public static event FileSystemEventHandler OnSave
        {
            add
            {
                Log.Info("Adding a file watcher");
                _watcher.Changed += value;
                _watcher.Created += value;
            }
            remove
            {
                Log.Info("Removing a file watcher");

                _watcher.Changed -= value;
                _watcher.Created -= value;
            }
        }
    }
}

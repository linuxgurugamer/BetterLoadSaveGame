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
            var saveDir = Path.Combine(KSPUtil.ApplicationRootPath, "saves");

            _saveDir = Path.Combine(saveDir, HighLogic.SaveFolder);
            _watcher = new FileSystemWatcher(_saveDir);
            _watcher.EnableRaisingEvents = true;
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
                _watcher.Changed += value;
                _watcher.Created += value;
            }
            remove
            {
                _watcher.Changed -= value;
                _watcher.Created -= value;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BetterLoadSaveGame
{
    class SaveWatcher : IDisposable
    {
        private FileSystemWatcher _watcher;

        public event FileSystemEventHandler OnSave;

        public SaveWatcher()
        {
            _watcher = new FileSystemWatcher(Util.SaveDir);
            _watcher.Created += FileCreated;
            _watcher.Changed += FileCreated;
            _watcher.EnableRaisingEvents = true;
        }

        private void FileCreated(object sender, FileSystemEventArgs e)
        {
            if (OnSave != null)
            {
                OnSave(sender, e);
            }
        }

        public void Dispose()
        {
            _watcher.Dispose();
        }
    }
}

using System;
using UnityEngine;

namespace BetterLoadSaveGame
{
    [KSPAddon(KSPAddon.Startup.FlightAndKSC, false)]
    public class Main : MonoBehaviour
    {
        static public Main fetch;
        private SaveWatcher _saveWatcher;
        private ScreenshotManager _screenshotManager;
        private SaveGameCollection _saveGameCollection;
        private LoadGameDialog _loadGameDialog;

        public void Start()
        {
            try
            {
                fetch = this;
                _saveWatcher = new SaveWatcher();

                _screenshotManager = new ScreenshotManager(_saveWatcher);
                _saveGameCollection = new SaveGameCollection(_saveWatcher);

                _loadGameDialog = new LoadGameDialog(_saveWatcher, _saveGameCollection, _screenshotManager, GetInstanceID());
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }

        public void RefreshSaves()
        {
            _saveGameCollection.LoadAllGames();
        }

        public void LateUpdate()
        {
            try
            {
                _screenshotManager.LateUpdate();
                _loadGameDialog.LateUpdate();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }

        public void OnGUI()
        {
            try
            {
                _loadGameDialog.OnGUI();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }

        public void OnDisable()
        {
            if (_saveWatcher != null)
            {
                _saveWatcher.Dispose();
            }
        }

        public bool Visible {  get { return _loadGameDialog.Visible; } }
        public bool EnableDialog()
        {
            _loadGameDialog.Visible = !_loadGameDialog.Visible;
            return _loadGameDialog.Visible;
        }
    }
}

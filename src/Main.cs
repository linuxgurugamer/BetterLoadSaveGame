using System;
using UnityEngine;

namespace BetterLoadSaveGame
{
    [KSPAddon(KSPAddon.Startup.FlightAndKSC, false)]
    public class Main : MonoBehaviour
    {
        private SaveWatcher _saveWatcher;
        private ScreenshotManager _screenshotManager;
        private SaveGameCollection _saveGameCollection;
        private LoadGameDialog _loadGameDialog;

        public void Start()
        {
            try
            {
                _saveWatcher = new SaveWatcher();

                _screenshotManager = new ScreenshotManager(_saveWatcher);
                _saveGameCollection = new SaveGameCollection(_saveWatcher);

                _loadGameDialog = new LoadGameDialog(_saveGameCollection, _screenshotManager, GetInstanceID());
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }

        public void Update()
        {
            try
            {
                _screenshotManager.Update();
                _loadGameDialog.Update();
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
    }
}

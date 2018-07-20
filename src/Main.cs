using System;
using UnityEngine;

namespace BetterLoadSaveGame
{
    [KSPAddon(KSPAddon.Startup.FlightAndKSC, false)]
    public class Main : MonoBehaviour
    {
        private ScreenshotManager _screenshotManager = new ScreenshotManager();
        private SaveGameCollection _saveGameCollection = new SaveGameCollection();
        private LoadGameDialog _loadGameDialog;

        public void Start()
        {
            try
            {
                SaveWatcher.Start();

                _screenshotManager.Start();
                _saveGameCollection.Start();

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
            SaveWatcher.Stop();
        }
    }
}

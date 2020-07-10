using System;
using System.Collections;
using System.Collections.Generic;
using ToolbarControl_NS;
using UnityEngine;

using static BetterLoadSaveGame.Main;


namespace BetterLoadSaveGame
{

    [KSPAddon(KSPAddon.Startup.Instantly, true)]
    public class InitLog : MonoBehaviour
    {
        protected void Awake()
        {
            Main.Log = new KSP_Log.Log("AQSS"
#if DEBUG
                , KSP_Log.Log.LEVEL.INFO
#endif
                );
        }
    }


    [KSPAddon(KSPAddon.Startup.AllGameScenes, false)]
    public class ManageSaves : MonoBehaviour
    {
        public void start()
        {
            InvokeRepeating("Delay", 60f, 60f);
        }

        void Delay()
        {
            Log.Info("Calling ManageSaves");
            ManageOldSaves.ManageSaves();
        }
    }

    [KSPAddon(KSPAddon.Startup.FlightAndKSC, false)]
    public class Main : MonoBehaviour
    {
        static public Main fetch;
        private SaveWatcher _saveWatcher;
        private ScreenshotManager _screenshotManager;
        private SaveGameCollection _saveGameCollection;
        private LoadGameDialog _loadGameDialog;

        public static KSP_Log.Log Log;
        internal static Texture2D clearBtn = new Texture2D(2, 2);

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
            if (!ToolbarControl.LoadImageFromFile(ref clearBtn, "GameData/" + InstallChecker.FOLDERNAME+"/PluginData/clear-30"))
            {
                Log.Error("Error loading clear-30 image");
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

        public bool Visible { get { return _loadGameDialog.Visible; } }
        public bool EnableDialog()
        {
            _loadGameDialog.Visible = !_loadGameDialog.Visible;
            return _loadGameDialog.Visible;
        }
    }
}

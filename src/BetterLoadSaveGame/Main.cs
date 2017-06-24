using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace BetterLoadSaveGame
{
    [KSPAddon(KSPAddon.Startup.FlightAndKSC, false)]
    public class Main : MonoBehaviour
    {
        private const string SCREENSHOT_FILENAME = "blsgss.png";

        private SaveGameInfo _saveToLoad;
        private List<SaveGameInfo> _saves;
        private Rect _windowRect = new Rect(100, 100, 300, 400);
        private Vector2 _scrollPos;
        private bool _visible = false;
        private bool _toggleVisibility = false;
        private FileSystemWatcher _watcher;
        private string _saveDir;

        public void Start()
        {
            _saveDir = Path.Combine(Path.Combine(KSPUtil.ApplicationRootPath, "saves"), HighLogic.SaveFolder);
            _watcher = new FileSystemWatcher(_saveDir);
            _watcher.Changed += OnSave;
            _watcher.Created += OnSave;
            _watcher.EnableRaisingEvents = true;
            LoadExistingSaveGames();
        }

        private void OnSave(object sender, FileSystemEventArgs e)
        {
            Application.CaptureScreenshot(SCREENSHOT_FILENAME);
            LoadExistingSaveGames();
        }

        private void LoadExistingSaveGames()
        {
            _saves = SaveGameManager.GetAllSaves(_saveDir);
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.F7))
            {
                if (!_toggleVisibility)
                {
                    _toggleVisibility = true;
                    _visible = !_visible;
                }
            }
            else
            {
                _toggleVisibility = false;
            }

            if (_saveToLoad != null)
            {
                _visible = false;
                LoadSaveGame(_saveToLoad);
                _saveToLoad = null;
            }
        }

        public void OnGUI()
        {
            if (_visible)
            {
                var buttonStyle = new GUIStyle(GUI.skin.button);
                buttonStyle.alignment = TextAnchor.MiddleLeft;

                _windowRect = GUILayout.Window(GetInstanceID(), _windowRect, (windowID) =>
                {
                    _scrollPos = GUILayout.BeginScrollView(_scrollPos);

                    foreach (var save in _saves)
                    {
                        if (GUILayout.Button(save.ToString(), buttonStyle))
                        {
                            // KSP seems to crash if we load the game here, but works ok in Update.
                            _saveToLoad = save;
                        }
                    }

                    GUILayout.EndScrollView();
                    GUI.DragWindow();
                }, "Load Game");
            }
        }

        private void LoadSaveGame(SaveGameInfo save)
        {
            var name = Path.GetFileNameWithoutExtension(save.SaveFile.Name);
            var game = GamePersistence.LoadGame(name, HighLogic.SaveFolder, true, false);
            game.Start();
        }
    }
}

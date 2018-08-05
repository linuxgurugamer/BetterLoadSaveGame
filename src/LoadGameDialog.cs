using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace BetterLoadSaveGame
{
    class LoadGameDialog : ActionGenerator
    {
        private const int WIDTH = 500;
        private const int HEIGHT = 600;
        private bool _visible = false;
        private bool _toggleVisibility = false;
        private Rect _windowRect;
        private string _filterText = "";
        private Vector2 _scrollPos;
        private SaveGameCollection _saveGameCollection;
        private ScreenshotManager _screenshotManager;
        private int _instanceID;
        private SaveWatcher _saveWatcher;

        public LoadGameDialog(SaveWatcher saveWatcher, SaveGameCollection saveGameCollection, ScreenshotManager screenshotManager, int instanceID)
        {
            _saveWatcher = saveWatcher;
            _saveGameCollection = saveGameCollection;
            _screenshotManager = screenshotManager;
            _instanceID = instanceID;

            _windowRect = new Rect((Screen.width - WIDTH) / 2, (Screen.height - HEIGHT) / 2, WIDTH, HEIGHT);
        }

        public override void Update()
        {
            base.Update();

            if (Input.GetKeyDown(KeyCode.F7))
            {
                if (!_toggleVisibility)
                {
                    _toggleVisibility = true;
                    Visible = !Visible;
                    if (Visible)
                    {
                        _scrollPos = new Vector2();
                    }
                    else
                    {
                        _screenshotManager.ClearScreenshots();
                    }
                }
            }
            else if (_toggleVisibility)
            {
                _toggleVisibility = false;
            }
        }

        public void OnGUI()
        {
            if (_visible)
            {
                _windowRect = GUILayout.Window(_instanceID, _windowRect, (windowID) =>
                {
                    RenderSortButtonsPanel();
                    RenderFilterPanel();
                    RenderGameList();

                    GUI.DragWindow();
                }, "Load Game", HighLogic.Skin.window);
            }
        }

        private void RenderGameList()
        {
            var gameButtonStyle = new GUIStyle(GUI.skin.button);
            gameButtonStyle.alignment = TextAnchor.MiddleLeft;

            _scrollPos = GUILayout.BeginScrollView(_scrollPos, HighLogic.Skin.scrollView);

            int saveIndex = 0;
            foreach (var save in _saveGameCollection.Saves)
            {
                // Determine if the button is visible.
                // This is not very accurate...
                bool isVisible = (saveIndex * 100 - _scrollPos.y) < HEIGHT;

                var name = Path.GetFileNameWithoutExtension(save.SaveFile.Name);

                if (_filterText == "" || name.Contains(_filterText))
                {
                    var content = new GUIContent();
                    content.text = save.ButtonText;

                    if (isVisible)
                    {
                        content.image = _screenshotManager.GetScreenshot(save);
                    }

                    if (GUILayout.Button(content, gameButtonStyle))
                    {
                        Log.Info("Clicked save: {0}", save.SaveFile.Name);
                        EnqueueAction(() =>
                        {
                            Visible = false;
                            LoadSaveGame(save);
                        });
                    }
                    saveIndex++;
                }
            }

            GUILayout.EndScrollView();
        }

        private void RenderSortButtonsPanel()
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label("Sort:", GUILayout.ExpandWidth(false));

            RenderSortButton(SortModeEnum.FileTime, "File Time");
            RenderSortButton(SortModeEnum.GameTime, "Game Time");
            RenderSortButton(SortModeEnum.Name, "Name");

            GUILayout.EndHorizontal();
        }

        private void RenderFilterPanel()
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label("Filter:", GUILayout.ExpandWidth(false));
            _filterText = GUILayout.TextField(_filterText);

            GUILayout.EndHorizontal();
        }

        private void RenderSortButton(SortModeEnum buttonSort, string text)
        {
            var currentSort = _saveGameCollection.SortMode;

            if (GUILayout.Toggle(currentSort == buttonSort, text, GUILayout.ExpandWidth(false)) && currentSort != buttonSort)
            {
                _saveGameCollection.SortMode = buttonSort;
            }
        }

        private bool Visible
        {
            get { return _visible; }
            set
            {
                _visible = value;

                Log.Info("Changing visibility to: {0}", _visible);
                FlightDriver.SetPause(_visible);
            }
        }

        private void LoadSaveGame(SaveGameInfo save)
        {
            Log.Info("Loading save: {0}", save.SaveFile.Name);
            var name = Path.GetFileNameWithoutExtension(save.SaveFile.Name);
            var game = GamePersistence.LoadGame(name, HighLogic.SaveFolder, true, false);

            // For some reason, loading games that start at the space center
            // just loads the latest persistent save instead. Copying the save
            // files over the persistent save seems to fix the problem.
            // Interestingly from my testing, this is similar to what the stock
            // load game dialog does as well, which updates the persistent save
            // when loading a game that starts in the space center, but not one
            // that starts in flight. I'm sure there's a better way to solve this
            // but this will do until I find it.
            if (name != "persistent" && game != null && game.startScene == GameScenes.SPACECENTER)
            {
                _saveWatcher.Enable(false);

                CopySaveFile(name + ".sfs", "persistent.sfs");
                CopySaveFile(name + ".loadmeta", "persistent.loadmeta");
                CopySaveFile(name + "-thumb.png", "persistent-thumb.png");

                _saveWatcher.Enable(true);
            }

            if (game != null)
            {
                game.Start();
            }
        }

        private void CopySaveFile(string from, string to)
        {
            var sourceFile = Path.Combine(Util.SaveDir, from);
            if (File.Exists(sourceFile))
            {
                var destFile = Path.Combine(Util.SaveDir, to);
                File.Copy(sourceFile, destFile, overwrite: true);
                File.SetLastWriteTime(destFile, DateTime.Now);
            }
        }
    }
}

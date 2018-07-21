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

        public LoadGameDialog(SaveGameCollection saveGameCollection, ScreenshotManager screenshotManager, int instanceID)
        {
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
            game.Start();
        }
    }
}

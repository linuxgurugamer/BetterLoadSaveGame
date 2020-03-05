using ClickThroughFix;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;



namespace BetterLoadSaveGame
{
    class CloseLoadGameDialog : MonoBehaviour
    {
        public void CloseIt()
        {

            StartCoroutine(DoIt());
        }
        IEnumerator DoIt()
        {
            bool found = false;
            double start = Time.realtimeSinceStartup;
            while (!found && Time.realtimeSinceStartup - start < 0.1)
            {
                List<GameObject> GameObjects = new List<GameObject>(FindObjectsOfType<GameObject>());
                foreach (var go in GameObjects)
                {
                    if (go.name == "LoadSavedGame")
                    {
                        Destroy(go);
                        InputLockManager.ClearControlLocks();
                        found = true;
                        break;
                    }
                }
                yield return null;
            }
            yield return null;
            LoadGameDialog.Instance.ToggleVisibility(true);
        }
    }

    class LoadGameDialog : ActionGenerator
    {
        internal static LoadGameDialog Instance;

        private const int WIDTH = 500;
        private const int HEIGHT = 600;
        const int DELWIDTH = 300;
        const int DELHEIGHT = 100;

        private bool _visibleLoadScreen = false;
        private bool _visibleDeleteDialog = false;
        private bool _toggleVisibility = false;
        private Rect _windowRect;
        private Rect _deleteRect;
        private string _filterText = "";
        private Vector2 _scrollPos;
        private SaveGameCollection _saveGameCollection;
        private ScreenshotManager _screenshotManager;
        private int _instanceID;
        private SaveWatcher _saveWatcher;
        bool openedViaF9 = false;

        public LoadGameDialog(SaveWatcher saveWatcher, SaveGameCollection saveGameCollection, ScreenshotManager screenshotManager, int instanceID)
        {
            Instance = this;
            _saveWatcher = saveWatcher;
            _saveGameCollection = saveGameCollection;
            _screenshotManager = screenshotManager;
            _instanceID = instanceID;

            _windowRect = new Rect((Screen.width - WIDTH) / 2, (Screen.height - HEIGHT) / 2, WIDTH, HEIGHT);
            _deleteRect = new Rect((Screen.width - DELWIDTH) / 2, (Screen.height - DELHEIGHT) / 2, DELWIDTH, DELHEIGHT);
        }

        public override void LateUpdate()
        {
            if (_toggleVisibility && (Input.GetKeyUp(KeyCode.Pause) || Input.GetKeyUp(KeyCode.Escape)))
            {
                Visible = false;
            }
            base.LateUpdate();

            if (HighLogic.CurrentGame.Parameters.CustomParams<BLSG1>().replaceStock)
            {
                bool quickloadKeyDown = GameSettings.QUICKLOAD.GetKeyDown(false);
                if (InputLockManager.IsUnlocked(ControlTypes.QUICKLOAD) && quickloadKeyDown)
                {
                    GameObject obj = new GameObject();
                    var move = obj.AddComponent<CloseLoadGameDialog>();
                    move.CloseIt();
                }
                else
                {
                    if (_toggleVisibility)
                        _toggleVisibility = false;
                    if (quickloadKeyDown)
                        Visible = false;
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.F7))
                {
                    ToggleVisibility();
                }
                else if (_toggleVisibility)
                {
                    _toggleVisibility = false;
                    InputLockManager.RemoveControlLock("gamePause");
                }
            }
        }

        internal void ToggleVisibility(bool F9 = false)
        {
            openedViaF9 = F9;
            if (!_toggleVisibility)
            {
                InputLockManager.SetControlLock(~(ControlTypes.UI | ControlTypes.PAUSE), "gamePause");

                _toggleVisibility = true;
                Visible = !Visible;
                if (Visible)
                {
                    _scrollPos = new Vector2();
                    Main.fetch.RefreshSaves();
                }
                else
                {
                    _screenshotManager.ClearScreenshots();
                }
            }
        }

        Color HtmlToColor(string htmlValue)
        {
            Color newCol = Color.gray;

            if (ColorUtility.TryParseHtmlString(htmlValue, out newCol))
            {
                return newCol;
            }
            return newCol;
        }


        bool firstTime = true;
        GUIStyle gameButtonStyle = null;
        GUIStyle buttonStyleYellow;
        GUIStyle buttonStyleOrange;
        public void OnGUI()
        {
            if (!HighLogic.CurrentGame.Parameters.CustomParams<BLSG1>().useAlternateSkin)
                GUI.skin = HighLogic.Skin;

            if (firstTime)
            {
                gameButtonStyle = new GUIStyle(GUI.skin.button);
                gameButtonStyle.alignment = TextAnchor.MiddleLeft;
                buttonStyleYellow = new GUIStyle(GUI.skin.button);
                buttonStyleYellow.normal.textColor = Color.yellow;
                buttonStyleOrange = new GUIStyle(GUI.skin.button);
                buttonStyleOrange.normal.textColor = HtmlToColor("#fdb915");

                firstTime = false;
            }

            if (Visible)
            {
                string title = "Load Game";
                if (useArchives)
                    title += " from Archives";
                _windowRect = ClickThruBlocker.GUILayoutWindow(_instanceID, _windowRect, (windowID) =>
                {
                    GUI.enabled = !_visibleDeleteDialog;

                    GUILayout.BeginVertical();
                    RenderSortButtonsPanel();
                    RenderFilterPanel();
                    RenderGameList();

                    GUILayout.FlexibleSpace();

                    GUILayout.BeginHorizontal();
                    if (lastButtonclicked == "" || lastButtonclicked == "persistent")
                        GUI.enabled = false;
                    if (GUILayout.Button("Delete", buttonStyleOrange, GUILayout.Width(90), GUILayout.Height(30)))
                    {
                        _visibleDeleteDialog = true;
                    }
                    if (!useArchives)
                    {
                        if (GUILayout.Button("Archive", GUILayout.Width(90), GUILayout.Height(30)))
                        {
                            ManageOldSaves.MoveSaveToArchive(selectedSave);
                            Main.fetch.RefreshSaves();
                            lastButtonclicked = "";
                        }
                        GUILayout.FlexibleSpace();
                    }
                    GUI.enabled = !_visibleDeleteDialog;

#if false
                    if (!useArchives)
                    {
                        GUILayout.FlexibleSpace();
                        string s = "Archive";
                        if (HighLogic.CurrentGame.Parameters.CustomParams<BLSG2>().deleteSaves)
                            s = "Delete";
                        if (GUILayout.Button(s + " old saves"))
                        {
                            ManageOldSaves.ManageSaves();
                        }
                    }
                    GUILayout.FlexibleSpace();
#endif
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Cancel", GUI.skin.button, GUILayout.Width(90), GUILayout.Height(30)))
                    {
                        Visible = false;
#if false
                        if (openedViaF9)
                        {
                            DisplayPauseMenu();
                        }
                        else
#endif
                        FlightDriver.SetPause(false, false);
                    }

                    if (lastButtonclicked == "" || _visibleDeleteDialog)
                        GUI.enabled = false;
                    if (GUILayout.Button("Load", buttonStyleYellow, GUILayout.Width(90), GUILayout.Height(30)))
                        Visible = false;
                    GUI.enabled = true;

                    GUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                    GUI.DragWindow();
                }, title, GUI.skin.window);

            }
            if (lastButtonclicked != "" && !Visible)
            {
                // Do load here
                //Visible = false;

                EnqueueAction(() =>
                {
                    Visible = false;
                    LoadSaveGame(selectedSave);
                });
            }
            if (_visibleDeleteDialog)
            {

                _deleteRect = ClickThruBlocker.GUILayoutWindow(_instanceID + 1, _deleteRect, (winId) =>
                {
                    GUILayout.Label("Are you sure you want to delete this save?\nYou will lose all progress and ships in it.");
                    if (GUILayout.Button("Delete Save"))
                    {
                        ManageOldSaves.DeleteSaveGame(selectedSave);
                        _visibleDeleteDialog = false;
                        // Visible = false;
                        lastButtonclicked = "";
                    }
                    if (GUILayout.Button("Cancel"))
                    {
                        _visibleDeleteDialog = false;
                    }
                    GUI.DragWindow();
                }, "Confirmation Needed", GUI.skin.window);

            }
        }

        bool clicked = false;
        string lastButtonclicked = "";
        SaveGameInfo selectedSave;
        bool useArchives = false;

        private void RenderGameList()
        {
            GUILayout.BeginHorizontal();
            _scrollPos = GUILayout.BeginScrollView(_scrollPos, GUI.skin.scrollView);
            for (int saveIndex = 0; saveIndex < _saveGameCollection._saves.Count; saveIndex++)
            {
                SaveGameInfo save = _saveGameCollection._saves[saveIndex];
                // Determine if the button is visible.
                // This is not very accurate...
                bool isVisible = (saveIndex * 100 - _scrollPos.y) < HEIGHT;

                var name = Path.GetFileNameWithoutExtension(save.SaveFile.Name);

                if (_filterText == "" || name.Contains(_filterText) || (save.ButtonText != null && save.ButtonText.Contains(_filterText)))
                {

                    if (save.buttonContent == null)
                    {
                        save.buttonContent = new GUIContent();
                        save.buttonContent.text = save.ButtonText;

                        //if (isVisible)
                        {
                            save.buttonContent.image = _screenshotManager.GetScreenshot(save);
                        }
                    }

                    bool value = false;
                    if (lastButtonclicked == name && clicked)
                        value = true;

                    var rc = GUILayout.Toggle(value, save.buttonContent, gameButtonStyle);
                    if (rc != value)
                    {
                        if (Event.current.clickCount >= 2)
                        {
                            Log.Info("Clicked save: {0}", save.SaveFile.Name);
                            EnqueueAction(() =>
                            {
                                Visible = false;
                                LoadSaveGame(save);
                            });
                        }
                        else
                        {
                            if (lastButtonclicked == name)
                            {
                                lastButtonclicked = "";
                            }
                            else
                            {
                                lastButtonclicked = name;
                                clicked = true;
                                selectedSave = save;
                            }
                        }
                    }
                }
            }
            GUILayout.EndScrollView();
            GUILayout.EndHorizontal();
        }

        private void RenderSortButtonsPanel()
        {
            if (HighLogic.CurrentGame.Parameters.CustomParams<BLSG2>().archiveSaves)
            {
                string t = "Use Archives";
                GUIContent c = new GUIContent(t);
                Vector2 s = (HighLogic.CurrentGame.Parameters.CustomParams<BLSG1>().useAlternateSkin ? GUI.skin.toggle.CalcSize(c) : GUI.skin.textField.CalcSize(c));

                bool b = useArchives;
                useArchives = GUI.Toggle(new Rect(2, 2, s.x, s.y), useArchives, "Use Archives");
                if (b != useArchives)
                {
                    if (useArchives)
                    {
                        _saveGameCollection.ArchiveDir = ManageOldSaves.ARCHIVEDIR;
                    }
                    else
                    {
                        _saveGameCollection.ArchiveDir = "";
                    }
                    Main.fetch.RefreshSaves();
                }
            }
            GUILayout.BeginHorizontal();
            GUILayout.Label("Sort:", GUILayout.ExpandWidth(false));
            RenderSortButton(SortModeEnum.FileTime, "File Time ");
            RenderSortButton(SortModeEnum.GameTime, "Game Time ");
            RenderSortButton(SortModeEnum.Name, "Name ");

            GUILayout.FlexibleSpace();

            string text = "Use alt. skin";
            GUIContent content = new GUIContent(text);
            Vector2 size = (HighLogic.CurrentGame.Parameters.CustomParams<BLSG1>().useAlternateSkin ? GUI.skin.toggle.CalcSize(content) : GUI.skin.textField.CalcSize(content));

            bool newVal = GUILayout.Toggle(HighLogic.CurrentGame.Parameters.CustomParams<BLSG1>().useAlternateSkin, text, GUILayout.ExpandWidth(false), GUILayout.Width(size.x + 30));
            if (newVal != HighLogic.CurrentGame.Parameters.CustomParams<BLSG1>().useAlternateSkin)
            {
                HighLogic.CurrentGame.Parameters.CustomParams<BLSG1>().useAlternateSkin = newVal;
                firstTime = true;
            }

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
            GUIContent content = new GUIContent(text);
            Vector2 size = (HighLogic.CurrentGame.Parameters.CustomParams<BLSG1>().useAlternateSkin ? GUI.skin.toggle.CalcSize(content) : GUI.skin.textField.CalcSize(content));

            if (GUILayout.Toggle(currentSort == buttonSort, text, GUILayout.ExpandWidth(false), GUILayout.Width(size.x + 20)) && currentSort != buttonSort)
            {
                _saveGameCollection.SortMode = buttonSort;
            }
        }

        internal bool Visible
        {
            get { return _visibleLoadScreen; }
            set
            {
                _visibleLoadScreen = value;

                Log.Info("Changing visibility to: {0}", _visibleLoadScreen);
                FlightDriver.SetPause(_visibleLoadScreen, false);
            }
        }

        private void LoadSaveGame(SaveGameInfo save)
        {
            Log.Info("Loading save: {0}", save.SaveFile.Name);
            var name = Path.GetFileNameWithoutExtension(save.SaveFile.Name);
            var game = GamePersistence.LoadGame(name, HighLogic.SaveFolder + _saveGameCollection.ArchiveDir, true, false);

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
            var sourceFile = Path.Combine(Util.SaveDir + _saveGameCollection.ArchiveDir, from);
            Log.Info("CopySaveFile, Util.SaveDir: " + Util.SaveDir + ", from: " + from + ", sourceFile: " + sourceFile);
            if (File.Exists(sourceFile))
            {
                var destFile = Path.Combine(Util.SaveDir, to);
                File.Copy(sourceFile, destFile, overwrite: true);
                File.SetLastWriteTime(destFile, DateTime.Now);
            }
        }
    }
}

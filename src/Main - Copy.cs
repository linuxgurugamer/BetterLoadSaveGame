using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;

namespace BetterLoadSaveGame
{
    [KSPAddon(KSPAddon.Startup.FlightAndKSC, false)]
    public class Main : MonoBehaviour
    {
        private enum SortMode
        {
            FileTime,
            GameTime,
            Name
        }

        private const int WIDTH = 500;
        private const int HEIGHT = 600;
        private SaveGameInfo _saveToLoad;
        private List<SaveGameInfo> _saves;
        private Rect _windowRect;
        private Vector2 _scrollPos;
        private bool _visible = false;
        private bool _toggleVisibility = false;
        private FileSystemWatcher _watcher;
        private string _saveDir;
        private Queue<string> _saveScreenshots = new Queue<string>();
        private Queue<string> _loadScreenshots = new Queue<string>();
        private List<string> _shownScreenshots = new List<string>();
        private Queue<KeyValuePair<string, byte[]>> _screenshotData = new Queue<KeyValuePair<string, byte[]>>();
        private Dictionary<string, Texture2D> _screenshots = new Dictionary<string, Texture2D>();
        private Texture2D _placeholder;
        private Texture2D _loadingPlaceholder;
        private string _filterText = "";
        private SortMode _sortMode = SortMode.FileTime;
        private object _screenshotLoaderLock = new object();
        private AutoResetEvent _screenshotLoaderEvent = new AutoResetEvent(false);
        private Thread _screenshotLoader;
        private bool _exiting = false;

        private Texture2D LoadPNG(string filePath)
        {
            if (File.Exists(filePath))
            {
                return CreateTexture(File.ReadAllBytes(filePath));
            }
            else
            {
                Log.Error("File not found: " + filePath);
            }
            return null;
        }

        private Texture2D CreateTexture(byte[] fileData)
        {
            var tex = new Texture2D(2, 2);
            tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
            TextureScale.Bilinear(tex, 150, 94);
            return tex;
        }

        public void Start()
        {
            try
            {
                _saveDir = Path.Combine(Path.Combine(KSPUtil.ApplicationRootPath, "saves"), HighLogic.SaveFolder);
                _watcher = new FileSystemWatcher(_saveDir);
                _watcher.Changed += OnSave;
                _watcher.Created += OnSave;
                _watcher.EnableRaisingEvents = true;

                _windowRect = new Rect((Screen.width - WIDTH) / 2, (Screen.height - HEIGHT) / 2, WIDTH, HEIGHT);

                // Supposedly should be able to load the texture using GameDatabase.Instance.GetTexture but I can't get it to work :(
                _placeholder = LoadPNG(Path.GetFullPath("GameData/BetterLoadSaveGame/placeholder.png"));
                _loadingPlaceholder = LoadPNG(Path.GetFullPath("GameData/BetterLoadSaveGame/loading.png"));

                _screenshotLoader = new Thread(ScreenshotLoader);
                _screenshotLoader.Start();

                Log.Info("Started");
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        /// <summary>
        /// Thread to handle loading screenshots. This is done in a thread to avoid lagging out the game when
        /// scrolling through the list and loading lots of images. Unfortunately, creating a texture from the
        /// image still has to be done on the main thread and takes some time, so there's still some lag.
        /// 
        /// Shared:
        ///     _loadScreenshots    - Queue of screenshots to load
        ///     _screenshotData     - File data of loaded screenshots
        ///     _exiting            - Exit condition, triggered on OnDisable (don't bother to lock on this one)
        /// 
        /// Main thread:
        ///     _screenshots        - Textures created from screenshot data
        ///     _shownScreenshots   - List of all screenshots that have been requested to load so far.
        /// </summary>
        private void ScreenshotLoader()
        {
            while(!_exiting)
            {
                string filename = null;
                lock (_screenshotLoaderLock)
                {
                    if (_loadScreenshots.Count > 0)
                    {
                        filename = _loadScreenshots.Dequeue();
                    }
                }
                if (filename == null)
                {
                    _screenshotLoaderEvent.WaitOne();
                }
                else
                {
                    var data = File.ReadAllBytes(filename);
                    lock (_screenshotLoaderLock)
                    {
                        _screenshotData.Enqueue(new KeyValuePair<string, byte[]>(Path.GetFileNameWithoutExtension(filename), data));
                    }
                }
            }
        }

        private void OnSave(object sender, FileSystemEventArgs e)
        {
            try
            {
                Log.Info("Detected file change: {0}", e.Name);
                if (e.FullPath.EndsWith(".sfs"))
                {
                    _saveScreenshots.Enqueue(Path.ChangeExtension(e.FullPath, ".png"));
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        private void LoadExistingSaveGames()
        {
            Log.Info("Loading existing save games");
            _saves = new List<SaveGameInfo>();
            foreach (var saveFile in Directory.GetFiles(_saveDir, "*.sfs"))
            {
                _saves.Add(new SaveGameInfo(saveFile));
            }
            UpdateSort(SortMode.FileTime);
        }

        private void LoadScreenshots()
        {
            lock (_screenshotLoaderLock)
            {
                foreach (var file in Directory.GetFiles(_saveDir, "*.png"))
                {
                    _loadScreenshots.Enqueue(file);
                }
            }
            _screenshotLoaderEvent.Set();
        }

        private void UpdateSort(SortMode mode)
        {
            _sortMode = mode;

            switch (mode)
            {
                case SortMode.FileTime:
                    Log.Info("Sorting by file time");
                    _saves.Sort((a, b) => b.SaveFile.LastWriteTime.CompareTo(a.SaveFile.LastWriteTime));
                    break;

                case SortMode.GameTime:
                    Log.Info("Sorting by game time");
                    _saves.Sort((a, b) => b.GameTime.CompareTo(a.GameTime));
                    break;

                case SortMode.Name:
                    Log.Info("Sorting by file time");
                    _saves.Sort((a, b) => a.SaveFile.Name.CompareTo(b.SaveFile.Name));
                    break;
            }
        }

        private void SetVisible(bool visible)
        {
            Log.Info("Changing visibility to: {0}", visible);
            _visible = visible;
            FlightDriver.SetPause(visible);

            if (_visible)
            {
                _scrollPos = new Vector2();
                LoadExistingSaveGames();
            }
            else
            {
                // Clear screenshots when UI not shown to save memory
                _screenshots.Clear();
                _shownScreenshots.Clear();
                lock(_screenshotLoaderLock)
                {
                    _loadScreenshots.Clear();
                }
            }
        }

        public void Update()
        {
            try
            {
                if (Input.GetKeyDown(KeyCode.F7))
                {
                    if (!_toggleVisibility)
                    {
                        _toggleVisibility = true;
                        SetVisible(!_visible);
                    }
                }
                else
                {
                    _toggleVisibility = false;
                }

                if (_saveToLoad != null)
                {
                    SetVisible(false);
                    LoadSaveGame(_saveToLoad);
                    _saveToLoad = null;
                }

                if (_saveScreenshots.Count > 0)
                {
                    var filename = _saveScreenshots.Peek();
                    Log.Info("Capturing screenshot: {0}", filename);
                    try
                    {
                        ScreenCapture.CaptureScreenshot(filename);
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Failed to capture screenshot");
                        Log.Error(ex);
                    }
                    _saveScreenshots.Dequeue();
                }

                lock (_screenshotLoaderLock)
                {
                    if (_screenshotData.Count > 0)
                    {
                        if (_visible)
                        { 
                            var item = _screenshotData.Dequeue();
                            Log.Info("Creating texture for screenshot: {0}", item.Key);
                            _screenshots[item.Key] = CreateTexture(item.Value);
                        }
                        else
                        {
                            _screenshotData.Clear();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        private void SortButton(SortMode mode, string text)
        {
            if (GUILayout.Toggle(_sortMode == mode, text, GUILayout.ExpandWidth(false)) && _sortMode != mode)
            {
                UpdateSort(mode);
            }
        }

        public void OnGUI()
        {
            try
            {
                if (_visible)
                {
                    var buttonStyle = new GUIStyle(GUI.skin.button);
                    buttonStyle.alignment = TextAnchor.MiddleLeft;

                    _windowRect = GUILayout.Window(GetInstanceID(), _windowRect, (windowID) =>
                    {
                        GUILayout.BeginHorizontal();

                        GUILayout.Label("Sort:", GUILayout.ExpandWidth(false));
                        SortButton(SortMode.FileTime, "File Time");
                        SortButton(SortMode.GameTime, "Game Time");
                        SortButton(SortMode.Name, "Name");

                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();

                        GUILayout.Label("Filter:", GUILayout.ExpandWidth(false));
                        _filterText = GUILayout.TextField(_filterText);

                        GUILayout.EndHorizontal();

                        _scrollPos = GUILayout.BeginScrollView(_scrollPos, HighLogic.Skin.scrollView);

                        int saveIndex = 0;
                        foreach (var save in _saves)
                        {
                            // Determine if the button is visible.
                            // This is not very accurate...
                            bool isVisible = (saveIndex * 100 - _scrollPos.y) < HEIGHT;

                            var name = Path.GetFileNameWithoutExtension(save.SaveFile.Name);

                            if (_filterText == "" || name.Contains(_filterText))
                            {
                                var content = new GUIContent();
                                content.text = save.ButtonText;

                                Texture2D screenshot;
                                if (_screenshots.TryGetValue(name, out screenshot))
                                {
                                    content.image = screenshot;
                                }
                                else
                                {
                                    content.image = _loadingPlaceholder;

                                    // Load the screenshot for a button if it's visible
                                    if (isVisible)
                                    {
                                        var filename = Path.ChangeExtension(save.SaveFile.FullName, "png");
                                        if (File.Exists(filename))
                                        {
                                            if (!_shownScreenshots.Contains(filename))
                                            {
                                                _shownScreenshots.Add(filename);

                                                Log.Info("Will load screenshot: {0}", filename);
                                                lock (_screenshotLoaderLock)
                                                {
                                                    if (!_loadScreenshots.Contains(filename))
                                                    {
                                                        _loadScreenshots.Enqueue(filename);
                                                    }
                                                }
                                                _screenshotLoaderEvent.Set();
                                            }
                                        }
                                        else
                                        {
                                            content.image = _placeholder;
                                        }
                                    }
                                }

                                if (GUILayout.Button(content, buttonStyle))
                                {
                                    Log.Info("Clicked save: {0}", save.SaveFile.Name);
                                    _saveToLoad = save;
                                }
                                saveIndex++;
                            }
                        }

                        GUILayout.EndScrollView();
                        GUI.DragWindow();
                    }, "Load Game", HighLogic.Skin.window);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        private void LoadSaveGame(SaveGameInfo save)
        {
            Log.Info("Loading save: {0}", save.SaveFile.Name);
            var name = Path.GetFileNameWithoutExtension(save.SaveFile.Name);
            var game = GamePersistence.LoadGame(name, HighLogic.SaveFolder, true, false);
            game.Start();
        }

        public void OnDisable()
        {
            _exiting = true;
            _screenshotLoaderEvent.Set();
        }
    }
}

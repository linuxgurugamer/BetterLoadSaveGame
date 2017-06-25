using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace BetterLoadSaveGame
{
    [KSPAddon(KSPAddon.Startup.FlightAndKSC, false)]
    public class Main : MonoBehaviour
    {
        private const int WIDTH = 400;
        private const int HEIGHT = 500;
        private SaveGameInfo _saveToLoad;
        private List<SaveGameInfo> _saves;
        private Rect _windowRect;
        private Vector2 _scrollPos;
        private bool _visible = false;
        private bool _toggleVisibility = false;
        private FileSystemWatcher _watcher;
        private string _saveDir;
        private string _saveScreenshot;
        private string _loadScreenshot;
        private Dictionary<string, Texture2D> _screenshots = new Dictionary<string, Texture2D>();
        private Texture2D _placeholder;

        private Texture2D LoadPNG(string filePath)
        {
            Texture2D tex = null;
            byte[] fileData;

            if (File.Exists(filePath))
            {
                fileData = File.ReadAllBytes(filePath);
                tex = new Texture2D(2, 2);
                tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
                TextureScale.Bilinear(tex, 100, 62);
            }
            else
            {
                Log.Error("File not found: " + filePath);
            }
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

                foreach (var file in Directory.GetFiles(_saveDir, "*.png"))
                {
                    _screenshots[Path.GetFileNameWithoutExtension(file)] = LoadPNG(file);
                }

                _windowRect = new Rect((Screen.width - WIDTH) / 2, (Screen.height - HEIGHT) / 2, WIDTH, HEIGHT);

                // Supposedly should be able to load the texture using GameDatabase.Instance.GetTexture but I can't get it to work :(
                _placeholder = LoadPNG(Path.GetFullPath("GameData/BetterLoadSaveGame/placeholder.png"));

                Log.Info("Started");
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        private void OnSave(object sender, FileSystemEventArgs e)
        {
            try
            {
                Log.Info("Detected file change: {0}", e.Name);
                if (e.FullPath.EndsWith(".sfs"))
                {
                    _saveScreenshot = Path.ChangeExtension(e.FullPath, ".png");
                }
                else if (e.FullPath.EndsWith(".png"))
                {
                    _loadScreenshot = e.FullPath;
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
            _saves.Sort((a, b) => b.SaveFile.LastWriteTime.CompareTo(a.SaveFile.LastWriteTime));
        }

        private void SetVisible(bool visible)
        {
            Log.Info("Changing visibility to: {0}", visible);
            _visible = visible;
            FlightDriver.SetPause(visible);

            if (_visible)
            {
                LoadExistingSaveGames();
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

                if (_saveScreenshot != null)
                {
                    Log.Info("Capturing screenshot: {0}", _saveScreenshot);
                    Application.CaptureScreenshot(_saveScreenshot);
                    _saveScreenshot = null;
                }

                if (_loadScreenshot != null)
                {
                    Log.Info("Loading save image: {0}", _loadScreenshot);
                    _screenshots[Path.GetFileNameWithoutExtension(_loadScreenshot)] = LoadPNG(_loadScreenshot);
                    _loadScreenshot = null;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
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
                        _scrollPos = GUILayout.BeginScrollView(_scrollPos, HighLogic.Skin.scrollView);

                        foreach (var save in _saves)
                        {
                            var content = new GUIContent();
                            content.text = save.ButtonText;

                            var name = Path.GetFileNameWithoutExtension(save.SaveFile.Name);
                            Texture2D screenshot;
                            if (_screenshots.TryGetValue(name, out screenshot))
                            {
                                content.image = screenshot;
                            }
                            else
                            {
                                content.image = _placeholder;
                            }

                            if (GUILayout.Button(content, buttonStyle))
                            {
                                Log.Info("Clicked save: {0}", save.SaveFile.Name);
                                _saveToLoad = save;
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
    }
}

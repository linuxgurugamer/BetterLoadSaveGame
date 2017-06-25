using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace BetterLoadSaveGame
{
    [KSPAddon(KSPAddon.Startup.FlightAndKSC, false)]
    public class Main : MonoBehaviour
    {
        private SaveGameInfo _saveToLoad;
        private List<SaveGameInfo> _saves;
        private Rect _windowRect = new Rect(100, 100, 400, 500);
        private Vector2 _scrollPos;
        private bool _visible = false;
        private bool _toggleVisibility = false;
        private FileSystemWatcher _watcher;
        private string _saveDir;
        private string _saveScreenshot;
        private string _loadScreenshot;
        private Dictionary<string, Texture2D> _screenshots = new Dictionary<string, Texture2D>();

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
            return tex;
        }

        public void Start()
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
        }

        private void OnSave(object sender, FileSystemEventArgs e)
        {
            if (e.FullPath.EndsWith(".sfs"))
            {
                _saveScreenshot = Path.ChangeExtension(e.FullPath, ".png");
            }
            else if (e.FullPath.EndsWith(".png"))
            {
                _loadScreenshot = e.FullPath;
            }
        }

        private void LoadExistingSaveGames()
        {
            _saves = new List<SaveGameInfo>();
            foreach (var saveFile in Directory.GetFiles(_saveDir, "*.sfs"))
            {
                _saves.Add(new SaveGameInfo(saveFile));
            }
            _saves.Sort((a, b) => b.SaveFile.LastWriteTime.CompareTo(a.SaveFile.LastWriteTime));
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.F7))
            {
                if (!_toggleVisibility)
                {
                    _toggleVisibility = true;
                    _visible = !_visible;

                    if (_visible)
                    {
                        LoadExistingSaveGames();
                    }
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

            if (_saveScreenshot != null)
            {
                Application.CaptureScreenshot(_saveScreenshot);
                _saveScreenshot = null;
            }

            if (_loadScreenshot != null)
            {
                _screenshots[Path.GetFileNameWithoutExtension(_loadScreenshot)] = LoadPNG(_loadScreenshot);
                _loadScreenshot = null;
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
                        var content = new GUIContent();
                        content.text = save.ButtonText;

                        var name = Path.GetFileNameWithoutExtension(save.SaveFile.Name);
                        Texture2D screenshot;
                        if (_screenshots.TryGetValue(name, out screenshot))
                        { 
                            content.image = screenshot;
                        }

                        if (GUILayout.Button(content, buttonStyle))
                        {
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

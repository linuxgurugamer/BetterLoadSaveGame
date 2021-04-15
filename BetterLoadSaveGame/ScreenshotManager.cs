using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using ToolbarControl_NS;
using UnityEngine;

using static BetterLoadSaveGame.Main;

namespace BetterLoadSaveGame
{
    class ScreenshotManager : ActionGenerator
    {
        private const int THUMB_WIDTH = 150;
        private const int THUMB_HEIGHT = 94;

        private Dictionary<string, Texture2D> _loadedScreenshots; // = new Dictionary<string, Texture2D>();
        private Texture2D _placeholder;

        public ScreenshotManager(SaveWatcher saveWatcher)
        {
            _loadedScreenshots = new Dictionary<string, Texture2D>();
            try
            {
                saveWatcher.OnSave += OnSave;
                // Migrate old full screenshots to thumbnails
                foreach (var file in Directory.GetFiles(Util.SaveDir, "*.*"))
                {
                    var fullPath = Path.Combine(Util.SaveDir, file);
                    if (IsFullScreenshot(fullPath))
                    {
                        EnqueueAction(() => ResizeScreenshot(fullPath));
                    }
                }

                //_placeholder = GameDatabase.Instance.GetTexture("BetterLoadSaveGame/placeholder", false);
                _placeholder = new Texture2D(2, 2);
                ToolbarControl.LoadImageFromFile(ref _placeholder, "GameData/BetterLoadSaveGame/PluginData/placeholder");
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        //
        // The following is because of behaviour on OSX 
        // That sends multiple OnSave messages for a single save
        //
        double lastTimeSaved = 0;
        private void OnSave(object sender, FileSystemEventArgs e)
        {
            //
            // The following is because of behaviour on OSX 
            // That sends multiple OnSave messages for a single save
            //
            if (SystemInfo.operatingSystemFamily != OperatingSystemFamily.Windows)
            {
                if (Time.realtimeSinceStartup - lastTimeSaved <2)
                    return;
                lastTimeSaved = Time.realtimeSinceStartup;
            }
            //////// End of OSX patch //////////////////////////////////////////


            if (e.FullPath.EndsWith(".sfs"))
            {
                EnqueueAction(() => SaveScreenshot(Path.ChangeExtension(e.FullPath, ".png")));
            }
            else if (IsFullScreenshot(e.FullPath))
            {
                EnqueueAction(() => ResizeScreenshot(e.FullPath));
            }
        }

        private bool IsFullScreenshot(string filename)
        {
            return filename.EndsWith(".png") && !filename.EndsWith("-thumb.png") && File.Exists(filename) && File.Exists(Path.ChangeExtension(filename, ".sfs"));
        }

        private void SaveScreenshot(string filename)
        {
            ScreenCapture.CaptureScreenshot(filename);
        }

        private void ResizeScreenshot(string filename)
        {
            if (!File.Exists(filename))
                return;
            var fileData = File.ReadAllBytes(filename);
            var tex = new Texture2D(2, 2);
            tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
            TextureScale.Bilinear(tex, 150, 94);

            var outFile = Regex.Replace(filename, @"\.png", "-thumb.png");

            var bytes = tex.EncodeToPNG();
            File.WriteAllBytes(outFile, bytes);

            File.Delete(filename);
        }

        public Texture2D GetScreenshot(SaveGameInfo save)
        {
            Texture2D screenshot = null;

            var imageFile = save.SaveFile.FullName.Replace(".sfs", "-thumb.png");

            if (!_loadedScreenshots.TryGetValue(imageFile, out screenshot) && File.Exists(imageFile))
            {
                var data = File.ReadAllBytes(imageFile);
                screenshot = new Texture2D(2, 2);
                screenshot.LoadImage(data);
                _loadedScreenshots.Add(imageFile, screenshot);
            }

            if (screenshot != null)
            {
                return screenshot;
            }
            else
            {
                return _placeholder;
            }
        }

        public void ClearScreenshots()
        {
            _loadedScreenshots.Clear();
        }
    }
}

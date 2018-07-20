using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace BetterLoadSaveGame
{
    class ScreenshotManager : ActionGenerator
    {
        private const int THUMB_WIDTH = 150;
        private const int THUMB_HEIGHT = 94;

        public void Start()
        {
            try
            {
                SaveWatcher.OnSave += OnSave;

                // Migrate old full screenshots to thumbnails
                foreach (var file in Directory.GetFiles(SaveWatcher.SaveDir, "*.*"))
                {
                    var fullPath = Path.Combine(SaveWatcher.SaveDir, file);
                    if (IsFullScreenshot(fullPath))
                    {
                        EnqueueAction(() => ResizeScreenshot(fullPath));
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        private void OnSave(object sender, FileSystemEventArgs e)
        {
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
            return filename.EndsWith(".png") && !filename.EndsWith("-thumb.png") && File.Exists(Path.ChangeExtension(filename, ".sfs"));
        }

        private void SaveScreenshot(string filename)
        {
            Log.Info("Saving screenshot: " + filename);
            ScreenCapture.CaptureScreenshot(filename);
        }

        private void ResizeScreenshot(string filename)
        {
            Log.Info("Resizing screenshot: " + filename);

            var fileData = File.ReadAllBytes(filename);
            var tex = new Texture2D(2, 2);
            tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
            TextureScale.Bilinear(tex, 150, 94);

            var outFile = Regex.Replace(filename, @"\.png", "-thumb.png");

            var bytes = tex.EncodeToPNG();
            File.WriteAllBytes(outFile, bytes);

            File.Delete(filename);
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace BetterLoadSaveGame
{
    [KSPAddon(KSPAddon.Startup.FlightAndKSC, false)]
    public class Main : MonoBehaviour
    {
        private ScreenshotManager _screenshotManager = new ScreenshotManager();

        public void Start()
        {
            SaveWatcher.Start();
            _screenshotManager.Start();
        }

        public void Update()
        {
            _screenshotManager.Update();
        }

        public void OnDisable()
        {
            SaveWatcher.Stop();
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace BetterLoadSaveGame
{
    [KSPAddon(KSPAddon.Startup.FlightAndKSC, false)]
    public class AddOnMain : MonoBehaviour
    {
        private DateTime _started;
        private bool _triggered = false;
        private SaveGameInfo _saveToLoad;
        private List<SaveGameInfo> _saves;

        public void Start()
        {
            _started = DateTime.Now;
            LoadExistingSaveGames();
        }

        private void LoadExistingSaveGames()
        {
            _saves = SaveGameManager.GetAllSaves(KSPUtil.ApplicationRootPath, HighLogic.SaveFolder);
        }

        public void Update()
        {
            if (!_triggered)
            {
                var trigger = _started.AddSeconds(10);
                if (DateTime.Now > trigger)
                {
                    _triggered = true;
                    _saveToLoad = _saves[0];
                }
            }

            if (_saveToLoad != null)
            {
                LoadSaveGame(_saveToLoad);
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

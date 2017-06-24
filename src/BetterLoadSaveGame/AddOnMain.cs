using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace BetterLoadSaveGame
{
    [KSPAddon(KSPAddon.Startup.FlightAndKSC, false)]
    public class AddOnMain : MonoBehaviour
    {
        private SaveGameInfo _saveToLoad;
        private List<SaveGameInfo> _saves;
        private Rect _windowRect;

        public void Start()
        {
            LoadExistingSaveGames();
        }

        private void LoadExistingSaveGames()
        {
            _saves = SaveGameManager.GetAllSaves(KSPUtil.ApplicationRootPath, HighLogic.SaveFolder);
        }

        public void Update()
        {
            if (_saveToLoad != null)
            {
                LoadSaveGame(_saveToLoad);
                _saveToLoad = null;
            }
        }

        private void OnGUI()
        {
            _windowRect = GUILayout.Window(GetInstanceID(), _windowRect, (windowID) =>
            {
                foreach (var save in _saves)
                {
                    if (GUILayout.Button(save.ToString()))
                    {
                        // KSP seems to crash if we load the game here, but works ok in Update.
                        _saveToLoad = save;
                    }
                }
            }, "Load Game");
        }

        private void LoadSaveGame(SaveGameInfo save)
        {
            var name = Path.GetFileNameWithoutExtension(save.SaveFile.Name);
            var game = GamePersistence.LoadGame(name, HighLogic.SaveFolder, true, false);
            game.Start();
        }
    }
}

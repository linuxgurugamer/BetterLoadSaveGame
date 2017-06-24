using System;
using UnityEngine;

namespace BetterLoadSaveGame
{
    [KSPAddon(KSPAddon.Startup.FlightAndKSC, false)]
    public class AddOnMain : MonoBehaviour
    {
        private DateTime _started;
        private bool _triggered = false;

        public void Start()
        {
            _started = DateTime.Now;

            Debug.Log("SAVE GAMES");

            foreach(var save in SaveGameManager.GetAllSaves(KSPUtil.ApplicationRootPath, HighLogic.SaveFolder))
            {
                Debug.Log(save);
            }

            Debug.Log("DONE");
        }

        public void Update()
        {
            if (!_triggered)
            {
                var trigger = _started.AddSeconds(10);
                if (DateTime.Now > trigger)
                {
                    Debug.Log("TRIGGERING");
                    _triggered = true;
                    var game = GamePersistence.LoadGame("persistent", HighLogic.SaveFolder, true, false);
                    game.Start();
                }
            }
        }
    }
}

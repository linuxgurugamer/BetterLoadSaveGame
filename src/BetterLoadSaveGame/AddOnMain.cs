using UnityEngine;

namespace BetterLoadSaveGame
{
    [KSPAddon(KSPAddon.Startup.FlightAndKSC, false)]
    public class AddOnMain : MonoBehaviour
    {
        public void Start()
        {
            Debug.Log("SAVE GAMES");

            foreach(var save in SaveGameManager.GetAllSaves(KSPUtil.ApplicationRootPath, HighLogic.SaveFolder))
            {
                Debug.Log(save);
            }

            Debug.Log("DONE");
        }

        public void Update()
        {
            //Debug.Log("Hello world? " + Time.realtimeSinceStartup);
        }
    }
}

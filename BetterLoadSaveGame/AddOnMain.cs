using UnityEngine;

namespace BetterLoadSaveGame
{
    [KSPAddon(KSPAddon.Startup.MainMenu, false)]
    public class AddOnMain : MonoBehaviour
    {
        public void Start()
        {
            Debug.Log("***** " + SaveGameManager.GetSaveFiles(KSPUtil.ApplicationRootPath, HighLogic.SaveFolder));
        }

        public void Update()
        {
            //Debug.Log("Hello world? " + Time.realtimeSinceStartup);
        }
    }
}

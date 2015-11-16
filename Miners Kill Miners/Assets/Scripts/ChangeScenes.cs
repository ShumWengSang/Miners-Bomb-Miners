using UnityEngine;
using System.Collections;

namespace Roland
{
    public class ChangeScenes : Singleton<ChangeScenes>
    {
        private string Quit = "Quit";
        private string GameScene = "SelectPlayers";

        int gameLevel = 0;
        int GameLevel
        {
            get
            {
                return gameLevel;
            }
        }

        protected ChangeScenes() {}
        void Start()
        {
            DontDestroyOnLoad(this.gameObject); 
        }

        public void LoadScene(string LoadThisLevel)
        {
            if (System.String.Equals(LoadThisLevel, Quit))
            {
                Application.Quit();
            }
            if(System.String.Equals(LoadThisLevel,Application.loadedLevelName))
            {
                //This means we are reloading the scene. Hopefully it means we are in loading a different level, but the same scene.
                ++gameLevel;
                Application.LoadLevelAsync(LoadThisLevel);
            }
            else if (System.String.Equals(LoadThisLevel, GameScene))
            {
                //If the last check statement failed, this means we are loading game scene from somewhere else
                //Meaning we load it the first time. so we set gamelevel to 0.
                gameLevel = 0;
                Application.LoadLevelAsync(LoadThisLevel);
            }
            else
            {
                //We are not loading the game scene at all, but some misc scene
                Application.LoadLevelAsync(LoadThisLevel);
            }
        }
    }
}
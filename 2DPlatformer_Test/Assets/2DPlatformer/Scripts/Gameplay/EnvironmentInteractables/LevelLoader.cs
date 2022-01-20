namespace GSGD2.Gameplay
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using GSGD2.Player;

    public class LevelLoader : AEnvironmentInteractable
    {
        [SerializeField]
        private string _sceneName;
        
        public override void UseInteractable(PlayerReferences playerRefs)
        {
            LoadScene();
        }

        private void LoadScene()
        {
            
            SceneManager.LoadScene(_sceneName);
        }
    }
}
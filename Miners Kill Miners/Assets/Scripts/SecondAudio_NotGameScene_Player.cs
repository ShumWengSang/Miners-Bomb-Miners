using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SecondAudio_NotGameScene_Player : Singleton<SecondAudio_NotGameScene_Player> {

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelLoaded;
    }

    private void OnLevelLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.name == "MultiplayerGame")
        {
            Destroy(this.gameObject);
        }
    }
}

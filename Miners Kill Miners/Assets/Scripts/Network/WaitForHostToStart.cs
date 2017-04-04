using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DarkRift;
using Roland;
public class WaitForHostToStart : MonoBehaviour {
    ChangeScenes changeScene;
    private void Awake()
    {
        changeScene = GetGlobalObject.FindAndGetComponent<ChangeScenes>(this.gameObject, "Global");
    }
    // Use this for initialization
    void Start ()
    {
        DarkRiftAPI.onData += Receive;
    }

    private void OnDisable()
    {
        DarkRiftAPI.onData -= Receive;
    }

    // Update is called once per frame
    void Update () {
        DarkRiftAPI.Receive();
	}

    private void Receive(byte tag, ushort subject, object data)
    {
        if (tag == Roland.NetworkingTags.Server)
        {
            if (subject == Roland.NetworkingTags.ServerSubjects.StartGameFromHost)
            {
                changeScene.LoadScene("MultiplayerGame");
            }
        }
    }

    public void NetworkStartGame()
    {
        DarkRiftAPI.SendMessageToAll(Roland.NetworkingTags.Server, Roland.NetworkingTags.ServerSubjects.StartGameFromHost, "");
    }
}

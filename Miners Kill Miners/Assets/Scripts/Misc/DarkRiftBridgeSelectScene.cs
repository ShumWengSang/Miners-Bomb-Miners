using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DarkRift;
public class DarkRiftBridgeSelectScene : MonoBehaviour {

    public Text display_ip_address;
    public Text num_of_connected;


    public void StartServer()
    {
        ServerBridgeSingleton.Instance.StartProcess();
    }

    private void OnEnable()
    {
        //Network.player.ipAddress;
        string ip = Network.player.ipAddress;
        //Debug.Log(Network.player.ipAddress);
        display_ip_address.text += " " + ip;
        DarkRiftAPI.onData += Receive;
        DarkRiftAPI.Connect(ip);
        DarkRiftAPI.SendMessageToServer(Roland.NetworkingTags.Server, Roland.NetworkingTags.ServerSubjects.GetNumOfPlayers, 0);
    }

    private void OnDisable()
    {
        DarkRiftAPI.onData -= Receive;
    }

    private void Receive (byte tag, ushort subject, object data)
    {
        if(tag == Roland.NetworkingTags.Misc)
        {
            if(subject == Roland.NetworkingTags.MiscSubjects.RetNumOfPlayers)
            {
                num_of_connected.text += " " + data.ToString();
            }
        }
    }
}

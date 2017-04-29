using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DarkRift;
using System.Security.Cryptography.X509Certificates;
using System.Net;
using System.Net.Security;

public class DarkRiftBridgeSelectScene : MonoBehaviour {

    public Text display_ip_address;
    public Text num_of_connected;


    public void StartServer()
    {
        ServerBridgeSingleton.Instance.StartProcess();
    }

    private void OnEnable()
    {
        ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;
        //Network.player.ipAddress;
        string ip = GetIPAddress();
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

    private string GetIPAddress()
    {
        return new System.Net.WebClient().DownloadString("https://ipinfo.io/ip").Replace("\n", "");
}

    private void Receive (byte tag, ushort subject, object data)
    {
        if(tag == Roland.NetworkingTags.Misc)
        {
            if(subject == Roland.NetworkingTags.MiscSubjects.RetNumOfPlayers)
            {
                num_of_connected.text = "Number of players: " + data.ToString();
            }
            if(subject == Roland.NetworkingTags.MiscSubjects.FullSlotted)
            {

            }
        }
    }

    public bool MyRemoteCertificateValidationCallback(System.Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
        bool isOk = true;
        // If there are errors in the certificate chain, look at each error to determine the cause.
        if (sslPolicyErrors != SslPolicyErrors.None)
        {
            for (int i = 0; i < chain.ChainStatus.Length; i++)
            {
                if (chain.ChainStatus[i].Status != X509ChainStatusFlags.RevocationStatusUnknown)
                {
                    chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
                    chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                    chain.ChainPolicy.UrlRetrievalTimeout = new System.TimeSpan(0, 1, 0);
                    chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
                    bool chainIsValid = chain.Build((X509Certificate2)certificate);
                    if (!chainIsValid)
                    {
                        isOk = false;
                    }
                }
            }
        }
        return isOk;
    }
}

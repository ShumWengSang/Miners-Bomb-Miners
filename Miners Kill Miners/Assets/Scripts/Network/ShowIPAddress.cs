using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using UnityEngine.UI;
public class ShowIPAddress : MonoBehaviour
{
    public Text ui;

    void Start()
    {
        ui.text = getIP();
    }

    static string getIP()
    {
        IPHostEntry host;
        string localIP = "?";
        host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                localIP = ip.ToString();
            }
        }
        return localIP;
    }
}

using UnityEngine;
using System.Collections;


using DarkRift;
namespace Roland
{
    public class DarkRiftReceiverCustom : MonoBehaviour
    {

        void Update()
        {
            if (DarkRiftAPI.isConnected)
                DarkRiftAPI.Receive();
        }
    }
}
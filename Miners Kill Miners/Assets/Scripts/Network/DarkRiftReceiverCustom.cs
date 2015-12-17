using UnityEngine;
using System.Collections;


using DarkRift;
namespace Roland
{
    public class DarkRiftReceiverCustom : MonoBehaviour
    {

        void LateUpdate()
        {
            if (DarkRiftAPI.isConnected)
                DarkRiftAPI.Receive();
        }
    }
}
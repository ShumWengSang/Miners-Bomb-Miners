using UnityEngine;
using System.Collections;
using DarkRift;
public class TestSend : MonoBehaviour {


	
	// Update is called once per frame
	void Update () {
	    if(Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("Sending C");
            DarkRiftAPI.SendMessageToServer(Roland.NetworkingTags.Server, Roland.NetworkingTags.ServerSubjects.SendMeSomething, null);
        }
	}
}

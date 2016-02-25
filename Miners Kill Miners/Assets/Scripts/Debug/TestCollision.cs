using UnityEngine;
using System.Collections;

public class TestCollision : MonoBehaviour {
    static int sID = 0;
    public int ID;
    void Start()
    {
        ID = sID;
        sID++;
    }
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("I hit enter");
    }

    void OnTriggerExit(Collider other)
    {
        Debug.Log("I'm exiting");
    }
}

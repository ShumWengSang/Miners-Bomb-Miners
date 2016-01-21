﻿using UnityEngine;
using System.Collections;

//First get access to the DarkRift namespace
using DarkRift;

public class CubeNetworkManager : MonoBehaviour
{
	public string IP = "127.0.0.1";

	void Start()
	{
		DarkRiftAPI.workInBackground = false;
		DarkRiftAPI.Connect(IP);
	}

	void OnApplicationQuit()
	{
		DarkRiftAPI.Disconnect();
	}
}

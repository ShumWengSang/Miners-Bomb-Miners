﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
public class ServerBridgeSingleton : Singleton<ServerBridgeSingleton>
{
    Process server = null;

    public void StartProcess()
    {
        if (server == null)
        {
            try
            {
                //only open one instance of the server.
                if (Process.GetProcessesByName("DarkRiftServer").Length <= 0)
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo(Application.dataPath + "/StreamingAssets/DarkRiftServer - Free/DarkRiftServer.exe");
                    startInfo.Verb = "runas";
                    startInfo.UseShellExecute = false;
                    startInfo.RedirectStandardOutput = false;
                    startInfo.RedirectStandardError = false;
                    startInfo.CreateNoWindow = false;
                    server = Process.Start(startInfo);
                }
                //server = Process.Start(Application.dataPath + "/StreamingAssets/DarkRiftServer - Free/DarkRiftServer.exe");
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.Log("Caught E is " + e);
            }
        }
    }

    public override void OnDestroy()
    {
        applicationIsQuitting = true;
        if (server != null)
            server.CloseMainWindow();
    }
}

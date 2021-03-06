﻿using UnityEngine;
using System.Collections;
using DarkRift;
namespace Roland
{
    public enum MouseButtons
    {
        left = 0,
        right = 1,
        ScrollUp = 2,
        ScrollDown = 3
    }
    public class EventManager : MonoBehaviour
    {
        public delegate void KeyboardMovement(Direction theDirection, int player_id);
        public static event KeyboardMovement OnKeyboardButtonDown;

        public delegate void MouseKeyDown(MouseButtons MouseButton, int player_id, int theItemID);
        public static event MouseKeyDown OnMouseButtonDown;

        public delegate void DisplayKD();
        public static event DisplayKD OnDisplayKD;

        GameSceneController theSceneController;

        int client_id;

        void Awake()
        {
            theSceneController = GetComponent<GameSceneController>();
        }
        // Use this for initialization
        void Start()
        {
            client_id = DarkRiftAPI.id;
            DarkRiftAPI.onDataDetailed += ReceiveData;
        }
        void OnDesotry()
        {
            DarkRiftAPI.onDataDetailed -= ReceiveData;
        }

        void SendEventKeyboardDown(Direction theDir)
        {
            OnKeyboardButtonDown(theDir, client_id);
            if(DarkRiftAPI.isConnected)
                DarkRiftAPI.SendMessageToAll(NetworkingTags.Events, NetworkingTags.EventSubjects.KeyboardEvent, theDir);
        }

        void Update()
        {
            if (theSceneController.GameHasStarted)
            {
                if (Input.GetKeyDown(KeyCode.W))
                {
                    //up
                    SendEventKeyboardDown(Direction.Up);
                }
                else if (Input.GetKeyDown(KeyCode.A))
                {
                    //left
                    SendEventKeyboardDown(Direction.Left);
                   
                }
                else if (Input.GetKeyDown(KeyCode.S))
                {
                    //down
                    SendEventKeyboardDown(Direction.Down);
                }
                else if (Input.GetKeyDown(KeyCode.D))
                {
                    //right
                    SendEventKeyboardDown(Direction.Right);
                }
                if(Input.GetKeyDown(KeyCode.Backslash))
                {
                    //OnDisplayKD();
                }

                if (Input.GetMouseButtonDown(0))
                {
                    //We send 0 because if this is only for out client, so the Player class doesn't care
                    //what id we sends it since it keeps its own current item
                    OnMouseButtonDown(MouseButtons.left, client_id, -1);
                }
                else if(Input.GetMouseButtonDown(1))
                {
                    OnMouseButtonDown(MouseButtons.right, client_id, -1);
                }
                float d = Input.GetAxis("Mouse ScrollWheel");
                if(d < 0)
                {
                    //less then 0, scroll down
                    OnMouseButtonDown(MouseButtons.ScrollDown, client_id, -1);
                }
                else if(d > 0)
                {
                    OnMouseButtonDown(MouseButtons.ScrollUp, client_id, -1);
                    //greater than 0, scroll up
                }
            }
        }

        void ReceiveData(ushort senderID, byte tag, ushort subject, object data)
        {
            if (theSceneController.GameHasStarted)
            {
                if (tag == NetworkingTags.Events)
                {
                    switch (subject)
                    {
                        case NetworkingTags.EventSubjects.KeyboardEvent:
                            Direction theDirectionToGo = (Direction)data;
                            OnKeyboardButtonDown(theDirectionToGo, senderID);
                            break;
                        case NetworkingTags.EventSubjects.leftMouseButton:
                            OnMouseButtonDown(MouseButtons.left, senderID, (int)data);
                            break;
                        case NetworkingTags.EventSubjects.rightMouseButton:
                            OnMouseButtonDown(MouseButtons.right, senderID, 0);
                            break;
                        default:
                            Debug.LogWarning("No such subject found: " + subject);
                            break;
                    }
                }
            }
        }
    }
}
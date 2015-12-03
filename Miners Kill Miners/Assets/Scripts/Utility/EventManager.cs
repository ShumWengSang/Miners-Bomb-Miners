using UnityEngine;
using System.Collections;
using DarkRift;
namespace Roland
{

    public class EventManager : MonoBehaviour
    {
        public delegate void KeyboardMovement(Direction theDirection, int player_id);
        public static event KeyboardMovement OnKeyboardButtonDown;

        public delegate void MouseKeyDown(int MouseButton, int player_id, Items_e theItem);
        public static event MouseKeyDown OnMouseButtonDown;

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

        void Update()
        {
            if (theSceneController.GameHasStarted)
            {
                if (Input.GetKeyDown(KeyCode.W))
                {
                    //up
                    OnKeyboardButtonDown(Direction.Up, client_id);
                    DarkRiftAPI.SendMessageToOthers(NetworkingTags.Events, NetworkingTags.EventSubjects.KeyboardEvent, Direction.Up);
                }
                else if (Input.GetKeyDown(KeyCode.A))
                {
                    //left
                    OnKeyboardButtonDown(Direction.Left, client_id);
                    DarkRiftAPI.SendMessageToOthers(NetworkingTags.Events, NetworkingTags.EventSubjects.KeyboardEvent, Direction.Left);
                }
                else if (Input.GetKeyDown(KeyCode.S))
                {
                    //down
                    OnKeyboardButtonDown(Direction.Down, client_id);
                    DarkRiftAPI.SendMessageToOthers(NetworkingTags.Events, NetworkingTags.EventSubjects.KeyboardEvent, Direction.Down);
                }
                else if (Input.GetKeyDown(KeyCode.D))
                {
                    //right
                    OnKeyboardButtonDown(Direction.Right, client_id);
                    DarkRiftAPI.SendMessageToOthers(NetworkingTags.Events, NetworkingTags.EventSubjects.KeyboardEvent, Direction.Right);
                }

                if (Input.GetMouseButtonDown(0))
                {
                    OnMouseButtonDown(0, client_id, Items_e.SmallBomb);
                    DarkRiftAPI.SendMessageToOthers(NetworkingTags.Events, NetworkingTags.EventSubjects.leftMouseButton, Items_e.SmallBomb);
                }
                else if(Input.GetMouseButtonDown(1))
                {
                    OnMouseButtonDown(1, client_id, Items_e.SmallBomb);
                    DarkRiftAPI.SendMessageToOthers(NetworkingTags.Events, NetworkingTags.EventSubjects.rightMouseButton, Items_e.SmallBomb);
                }


            }
        }

        void ReceiveData(ushort senderID, byte tag, ushort subject, object data)
        {
            
            if(tag == NetworkingTags.Events)
            {
                switch (subject)
                {
                    case NetworkingTags.EventSubjects.KeyboardEvent:
                        Direction theDirectionToGo = (Direction)data;
                        OnKeyboardButtonDown(theDirectionToGo, senderID);
                        break;
                    case NetworkingTags.EventSubjects.leftMouseButton:
                        OnMouseButtonDown(0, senderID, (Items_e)data);
                        break;
                    case NetworkingTags.EventSubjects.rightMouseButton:
                        OnMouseButtonDown(1, senderID, (Items_e)data);
                        break;
                    default:
                        Debug.LogWarning("No such subject found: " + subject);
                        break;
                }
            }
        }
    }
}
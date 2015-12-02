using UnityEngine;
using System.Collections;
using DarkRift;
namespace Roland
{

    public class EventManager : MonoBehaviour
    {
        bool LeftMouseDown = false;
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
        }

        void Update()
        {
            if (theSceneController.GameHasStarted)
            {
                if (Input.GetKeyDown(KeyCode.W))
                {
                    //up
                    OnKeyboardButtonDown(Direction.Up, client_id);
                }
                else if (Input.GetKeyDown(KeyCode.A))
                {
                    //left
                    OnKeyboardButtonDown(Direction.Left, client_id);
                }
                else if (Input.GetKeyDown(KeyCode.S))
                {
                    //down
                    OnKeyboardButtonDown(Direction.Down, client_id);
                }
                else if (Input.GetKeyDown(KeyCode.D))
                {
                    //right
                    OnKeyboardButtonDown(Direction.Right, client_id);
                }

                if (Input.GetMouseButtonDown(0))
                {
                    if (!LeftMouseDown)
                    {
                        LeftMouseDown = true;
                        OnMouseButtonDown(0, client_id, Items_e.SmallBomb);
                    }
                }
                if (Input.GetMouseButtonUp(0))
                {
                    LeftMouseDown = false;
                }

            }
        }
    }
}
using UnityEngine;
using System.Collections;

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

        void Awake()
        {
            theSceneController = GetComponent<GameSceneController>();
        }
        // Use this for initialization
        void Start()
        {

        }

        void Update()
        {
            if (theSceneController.GameHasStarted)
            {
                if (Input.GetKeyDown(KeyCode.W))
                {
                    //up
                    OnKeyboardButtonDown(Direction.Up, 0);
                }
                else if (Input.GetKeyDown(KeyCode.A))
                {
                    //left
                    OnKeyboardButtonDown(Direction.Left, 0);
                }
                else if (Input.GetKeyDown(KeyCode.S))
                {
                    //down
                    OnKeyboardButtonDown(Direction.Down, 0);
                }
                else if (Input.GetKeyDown(KeyCode.D))
                {
                    //right
                    OnKeyboardButtonDown(Direction.Right, 0);
                }

                if (Input.GetMouseButtonDown(0))
                {
                    if (!LeftMouseDown)
                    {
                        LeftMouseDown = true;
                        OnMouseButtonDown(0, 0, Items_e.SmallBomb);
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
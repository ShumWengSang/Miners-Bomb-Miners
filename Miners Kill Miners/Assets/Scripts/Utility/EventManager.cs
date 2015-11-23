using UnityEngine;
using System.Collections;

namespace Roland
{
    public class EventManager : MonoBehaviour
    {
        public delegate void KeyboardMovement(Direction theDirection, int player_id);
        public static event KeyboardMovement OnKeyboardButtonDown;

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
            }
        }
    }
}
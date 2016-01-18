using UnityEngine;
using System.Collections;
namespace Roland
{
    public class SimpleSingleton : MonoBehaviour
    {

        public static GameObject obj;
        // Use this for initialization
        public void Awake()
        {
            if (obj == null)
            {
                DontDestroyOnLoad(gameObject);
                obj = this.gameObject;
            }
            else if (obj != this)
            {
                //Destroy(gameObject);
            }
        }

    }
}

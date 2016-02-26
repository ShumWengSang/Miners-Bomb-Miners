using UnityEngine;
using System.Collections;

namespace Roland
{
    public class GetGlobalObject
    {
        public static T FindAndGetComponent<T> (GameObject thisObject, string tag) where T : Component
        {
            T returnObject = null;
            GameObject[] ListOfGlobals = GameObject.FindGameObjectsWithTag(tag);
            for (int i = 0; i < ListOfGlobals.Length; i++)
            {
                //Search the entire global for our change scene object
                if (!(ListOfGlobals[i].GetComponent<ChangeScenes>() == null))
                {
                    returnObject = ListOfGlobals[i].GetComponent<T>();
                    break;
                }
            }

            //If no such object found, create our own.
            if (returnObject == null)
            {
                GameObject go = new GameObject();
                go.tag = tag;
                returnObject = go.AddComponent<T>();
            }

            return returnObject;
        }
    }
}
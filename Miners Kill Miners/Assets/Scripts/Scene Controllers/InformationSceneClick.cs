using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Roland
{
    public class InformationSceneClick : MonoBehaviour
    {
        ChangeScenes SceneChanger;
        void Start()
        {

            SceneChanger = GetGlobalObject.FindAndGetComponent<ChangeScenes>(this.gameObject, "Global");

        }

        public void OnMouseDown()
        {
            if (SceneChanger != null)
            {
                SceneChanger.LoadScene("MainMenu");
            }
        }
    }
}
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Roland
{
    public class InformationSceneClick : MonoBehaviour
    {
        int clickCounter = 0;
        public Image [] infoImage;

        ChangeScenes SceneChanger;
        void Start()
        {

            SceneChanger = GetGlobalObject.FindAndGetComponent<ChangeScenes>(this.gameObject, "Global");

            clickCounter = 0;
        }   

        public void OnMouseDown()
        {
            infoImage[clickCounter].gameObject.SetActive(false);
            clickCounter++;
            if(clickCounter == (infoImage.Length))
            {
                if(SceneChanger != null)
                {
                    SceneChanger.LoadScene("MainMenu");
                }
                clickCounter = 0;
            }
        }
    }
}
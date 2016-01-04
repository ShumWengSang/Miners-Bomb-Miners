using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Roland
{
    public class Fade : MonoBehaviour
    {

        WaitForSeconds wait;
        public Graphic [] theImage;
        public int timesToFade = 7;


        void Start()
        {
            wait = new WaitForSeconds(1f);
            StartCoroutine(FadeInAndOut(timesToFade));
        }
        void FadeIn()
        {
            for (int i = 0; i < theImage.Length; i++ )
                theImage[i].CrossFadeAlpha(1.0f, 1f, true);
        }

        void FadeOut()
        {
            for (int i = 0; i < theImage.Length; i++)
             theImage[i].CrossFadeAlpha(0.0f, 1f, true);
        }

        public IEnumerator FadeInAndOut(int times)
        {
            for(int i = 0; i < times; i++)
            {
                FadeIn();
                yield return wait;
                FadeOut();
                yield return wait;
            }
            for (int i = 0; i < theImage.Length; i++)
            {
                theImage[i].gameObject.SetActive(false);
            }
        }
    }
}

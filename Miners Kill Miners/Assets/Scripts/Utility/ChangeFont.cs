using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Roland
{
    [ExecuteInEditMode]


    public class ChangeFont : MonoBehaviour
    {
        public Font theFontToUse;
        Transform thisTransform;
        // Update is called once per frame
        void Update()
        {
            Text[] Texts = FindObjectsOfType(typeof(Text)) as Text[];
            for(int i = 0; i < Texts.Length; i++)
            {
                Texts[i].font = theFontToUse;
            }
        }
    }
}
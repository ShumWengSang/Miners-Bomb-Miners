using UnityEngine;
using System.Collections;
using UnityEngine.UI;
namespace Roland
{
    public class UnityInputFieldExtended : MonoBehaviour
    {
        InputField inputField;
        SelectModeScene selectmodescene;

        void Start()
        {
            inputField = this.GetComponent<InputField>();
            selectmodescene = GameObject.Find("SelectMode").GetComponent<SelectModeScene>();
            if(selectmodescene == null)
            {
                Debug.Log("Its null");
            }
            
        }

        void Update()
        {
            if (inputField.isFocused && inputField.text != "" && Input.GetKey(KeyCode.Return))
            {
                Debug.Log("Enter");
                selectmodescene.Connect(inputField.text);
                inputField.text = "";
                this.gameObject.SetActive(false);
            }
        }
    }
}
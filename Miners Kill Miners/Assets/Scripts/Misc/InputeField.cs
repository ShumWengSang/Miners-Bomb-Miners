using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Roland;
using DarkRift;
public class InputeField : MonoBehaviour {

    public InputField mainInputField;
    public GameObject notConnected;
    public GameObject connected;
    public GameObject connecting;
    public GameObject button;
    public GameObject enterIP;
    public GameObject ipaddressText;
    public GameObject PlaceHolder;
    // Checks if there is anything entered into the input field.
    public void LockInput(InputField input)
    {
        enterIP.SetActive(false);
        input.interactable = false;
        if (input.text.Length == 0)
        {
            Debug.Log("Text has been entered");
        }
        connecting.gameObject.SetActive(true);

        if(CustomNetworkManager.Instance.Connect(input.text))
        {
            //Debug.Log("Connection successful");
            connected.gameObject.SetActive(true);
            button.GetComponent<Button>().interactable = true;
            input.text = "";
            PlaceHolder.GetComponent<Text>().text = "";
        }
        else
        {
            //Debug.Log("Connection failed");
            notConnected.gameObject.SetActive(true);
        }
        connecting.gameObject.SetActive(false);

    }

    public void Start()
    {
        //Adds a listener that invokes the "LockInput" method when the player finishes editing the main input field.
        //Passes the main input field into the method when "LockInput" is invoked
        mainInputField.onEndEdit.AddListener(delegate { LockInput(mainInputField); });
    }
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Roland;
public class UIShowDescr : MonoBehaviour {

    public GameObject child;
    bool MouseOver;

    public Text Title;
    public Text Desc;
    public float WaitTime = 1.0f;
    WaitForSeconds waitSeconds;

    EquipmentBase currentEquipment;
    void Start()
    {
        waitSeconds = new WaitForSeconds(WaitTime);
    }

    public void OnMouseHover(GameObject theObject)
    {
        MouseOver = true;
        currentEquipment = theObject.GetComponent<EquipmentBase>();
        StartCoroutine(StartCountDown());
        Debug.Log("Starting coroutine");
    }

    IEnumerator StartCountDown()
    {
        yield return waitSeconds;
        Debug.Log("MouseOver is " + MouseOver);
        if(MouseOver)
        {
            child.SetActive(true);
            child.transform.position = currentEquipment.SetDescTo.position;
            Title.text = currentEquipment.BombName;
            Desc.text = currentEquipment.ItemDescription;
        }
    }

    public void OnMouseExit()
    {
        Debug.Log("Exit");
        MouseOver = false;
        child.SetActive(false);
        StopAllCoroutines();
    }
}

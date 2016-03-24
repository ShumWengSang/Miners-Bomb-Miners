using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Roland;
public class UIShowDescr : MonoBehaviour {
    public static UIShowDescr instance;
    public GameObject child;
    bool MouseOver;

    public Text Title;
    public Text Desc;
    public float WaitTime = 1.0f;
    WaitForSeconds waitSeconds;

    EquipmentBase currentEquipment;
    void Start()
    {
        instance = this;
        waitSeconds = new WaitForSeconds(WaitTime);
    }

    void OnDestroy()
    {
        instance = null;
    }

    public void OnMouseHover(GameObject theObject)
    {
        MouseOver = true;
        currentEquipment = theObject.GetComponent<EquipmentBase>();
        StartCoroutine(StartCountDown());
        Title.text = "";
    }

    public void UpdateText()
    {
        Title.text = currentEquipment.Amount.ToString() + " ";
        Title.text += currentEquipment.BombName;
    }

    IEnumerator StartCountDown()
    {
        yield return waitSeconds;
        if(MouseOver)
        {
            child.SetActive(true);
            child.transform.position = currentEquipment.SetDescTo.position;
            Title.text = currentEquipment.Amount.ToString() + " ";
            Title.text += currentEquipment.BombName;
            Desc.text = currentEquipment.ItemDescription;
        }
    }

    public void OnMouseExit()
    {
        MouseOver = false;
        child.SetActive(false);
        StopAllCoroutines();
    }
}

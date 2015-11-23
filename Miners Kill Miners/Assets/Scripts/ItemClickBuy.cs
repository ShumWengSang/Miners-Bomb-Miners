using UnityEngine;
using System.Collections;
using UnityEngine.UI;
namespace Roland
{
    public class ItemClickBuy : MonoBehaviour
    {
        Text numberOfItemText;
        int numberOfItem_i = 0;
        public Items_e thisItem;
        public string AmountOfItemStringName = "Item Number";
        void Start()
        {
            numberOfItemText = transform.FindChild(AmountOfItemStringName).GetComponent<Text>();
            if(numberOfItemText == null)
            {
                Debug.LogError("Cannot find Number Of Text Item. Creating new one.");
                GameObject obj = new GameObject(AmountOfItemStringName);
                obj.AddComponent<Text>();
                obj.transform.SetParent(this.transform);
                obj.transform.localPosition = new Vector3(0, -10, 0);
                numberOfItemText = transform.FindChild(AmountOfItemStringName).GetComponent<Text>();
            }
            UpdateItemNumber();
        }
        
        public void BuyItem()
        {
            numberOfItem_i++;
            UpdateItemNumber();
        }

        void UpdateItemNumber()
        {
            numberOfItemText.text = numberOfItem_i.ToString();
            CurrentPlayer.Instance.ThePlayer.AmountOfItems[thisItem] = numberOfItem_i;
            //Debug.Log("I have " + numberOfItem_i + " amount of " + thisItem);
        }
    }
}
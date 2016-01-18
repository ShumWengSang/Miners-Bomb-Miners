using UnityEngine;
using System.Collections;
using UnityEngine.UI;
namespace Roland
{
    [ExecuteInEditMode]
    public class ItemClickBuy : MonoBehaviour
    {
        public EquipmentBase theEquipment;

        void Start()
        {
            if(theEquipment != null)
                theEquipment.Init();
        }
        public void BuyItem()
        {
            theEquipment.BuyItem();
        }
    }
}
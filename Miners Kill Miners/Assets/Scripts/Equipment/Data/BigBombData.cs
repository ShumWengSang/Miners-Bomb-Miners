using UnityEngine;
using System.Collections;

namespace Roland
{
    public class BigBombData : EquipmentBase
    {

        void Awake()
        {
            OrderID = 1;
            AddItemToList();
        }

        void OnDestroy()
        {
            Debug.Log("Destroying");
        }
    }
}

using UnityEngine;
using System.Collections;

namespace Roland
{
    public class MineBombData : EquipmentBase
    {
        void Awake()
        {
            OrderID = 6;
            AddItemToList();
        }
    }
}

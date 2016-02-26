using UnityEngine;
using System.Collections;

namespace Roland
{
    public class NapalmBombData : EquipmentBase
    {
        void Awake()
        {
            OrderID = 2;
            AddItemToList();
        }
    }
}

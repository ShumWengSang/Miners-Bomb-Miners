using UnityEngine;
using System.Collections;

namespace Roland
{
    public class RemoteBombData : EquipmentBase
    {
        void Awake()
        {
            OrderID = 7;
            AddItemToList();
        }
    }
}

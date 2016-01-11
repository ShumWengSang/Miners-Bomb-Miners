using UnityEngine;
using System.Collections;
namespace Roland
{
    public class BigRemoteBombData : EquipmentBase
    {
        void Awake()
        {
            OrderID = 8;
            AddItemToList();
        }
    }
}

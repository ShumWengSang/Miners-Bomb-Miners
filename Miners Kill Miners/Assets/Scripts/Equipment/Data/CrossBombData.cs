using UnityEngine;
using System.Collections;

namespace Roland
{
    public class CrossBombData : EquipmentBase
    {
        void Awake()
        {
            OrderID = 5;
            AddItemToList();
            BombName = "CrossBomb";
        }
    }
}

using UnityEngine;
using System.Collections;

namespace Roland
{
    public class TNTBombData : EquipmentBase
    {
        void Awake()
        {
            OrderID = 3;
            AddItemToList();
            BombName = "TNTBomb";
        }
    }
}

using UnityEngine;
using System.Collections;

namespace Roland
{
    public class NuclearBombData : EquipmentBase
    {
        void Awake()
        {
            OrderID = 4;
            AddItemToList();
            BombName = "NuclearBomb";
        }
    }
}

using UnityEngine;
using System.Collections;

namespace Roland
{
    public class NuclearBombData : EquipmentBase
    {
        void Awake()
        {
            Init();
            OrderID = 4;
            AddItemToList();
            BombName = "NuclearBomb";
        }
    }
}

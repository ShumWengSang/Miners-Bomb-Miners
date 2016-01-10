using UnityEngine;
using System.Collections;

namespace Roland
{
    public class NapalmBombData : EquipmentBase
    {
        void Awake()
        {
            Init();
            OrderID = 2;
            AddItemToList();
            BombName = "NapalmBomb";
        }
    }
}

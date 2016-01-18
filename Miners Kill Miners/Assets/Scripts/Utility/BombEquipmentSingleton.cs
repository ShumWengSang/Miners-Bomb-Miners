using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Roland
{

    public class BombEquipmentSingleton : Singleton<BombEquipmentSingleton>
    {
        protected BombEquipmentSingleton() { }
        protected List<EquipmentBase> Bombs;

        public void AddItem(EquipmentBase item)
        {

        }
    }
}

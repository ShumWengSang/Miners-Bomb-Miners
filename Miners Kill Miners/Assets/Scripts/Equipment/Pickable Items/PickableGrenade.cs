using UnityEngine;
using System.Collections;
namespace Roland
{
    public class PickableGrenade : PickItem
    {
        public int amount;
        
        protected override void PickItems()
        {
            CurrentPlayer.Instance.AmountOfEquipments[9].Amount += amount;
            Lean.LeanPool.Despawn(this.gameObject);
        }
    }
}

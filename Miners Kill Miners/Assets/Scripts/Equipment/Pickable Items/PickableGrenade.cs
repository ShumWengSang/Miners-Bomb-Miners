using UnityEngine;
using System.Collections;
using UnityEngine.UI;
namespace Roland
{
    public class PickableGrenade : PickItem
    {
        public int amount;
        Text InGameUINumberOfItem;
        protected override void PickItems()
        {
            CurrentPlayer.Instance.AmountOfEquipments[9].Amount += amount;
            if(InGameUINumberOfItem == null)
                {
                    InGameUINumberOfItem = GameObject.Find("NumberOfBombs").GetComponent<Text>();
                }
                if(Object.CurrentPlayer.Instance.ThePlayer.TheCurrentItem.GetType() is CurrentPlayer.Instance.AmountOfEquipments[9].GetType())
                InGameUINumberOfItem.text = amount.ToString();
            Lean.LeanPool.Despawn(this.gameObject);
        }
    }
}

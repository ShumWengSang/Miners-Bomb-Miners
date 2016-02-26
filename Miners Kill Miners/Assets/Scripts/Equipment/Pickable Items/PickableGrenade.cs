using UnityEngine;
using System.Collections;
using UnityEngine.UI;
namespace Roland
{
    public class PickableGrenade : PickItem
    {
        public int amount;
        Text InGameUINumberOfItem;

        void Start()
        {
            Init();
        }

        protected override void PickItems()
        {
            CurrentPlayer.Instance.AmountOfEquipments[9].Amount += amount;
            if (InGameUINumberOfItem == null)
            {
                InGameUINumberOfItem = GameObject.Find("NumberOfBombs").GetComponent<Text>();
            }
            if(CurrentPlayer.Instance.ThePlayer.TheCurrentItem.GetType() == CurrentPlayer.Instance.AmountOfEquipments[9].GetType())
            InGameUINumberOfItem.text = CurrentPlayer.Instance.AmountOfEquipments[9].Amount.ToString();
        }
    }
}

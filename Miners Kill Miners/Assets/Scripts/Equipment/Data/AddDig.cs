using UnityEngine;
using System.Collections;
namespace Roland
{
    public class AddDig : EquipmentBase
    {
        public int addDigAmount = 1;
        public override void BuyItem()
        {
            if (CurrentPlayer.Instance.BuyThings(cost))
            {
                CurrentPlayer.Instance.DigPower += addDigAmount;
                //numberOfItemText.text = CurrentPlayer.Instance.AddedDig.ToString();
            }
        }

        public override void Init()
        {
            if (CurrentPlayer.Instance != null)
            {
                if(CurrentPlayer.Instance.DigPowerUI != null)
                    CurrentPlayer.Instance.DigPowerUI.text = CurrentPlayer.Instance.DigPower.ToString();
            }
        }
    }
}

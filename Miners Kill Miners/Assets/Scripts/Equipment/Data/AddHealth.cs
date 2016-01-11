using UnityEngine;
using System.Collections;
namespace Roland
{
    public class AddHealth : EquipmentBase
    {
        public override void BuyItem()
        {
            if (CurrentPlayer.Instance.BuyThings(cost))
            {
                CurrentPlayer.Instance.AddedHealth++;
                numberOfItemText.text = CurrentPlayer.Instance.AddedHealth.ToString();
            }
        }
    }
}

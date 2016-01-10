using UnityEngine;
using System.Collections;

namespace Roland
{
    public class DamagePlayer : MonoBehaviour
    {
        public int Damage = 50;

        void OnTriggerEnter2D(Collider2D theCollider)
        {
            if(theCollider.CompareTag("Player"))
            {
                theCollider.GetComponent<Player>().MinusHealthPoints(Damage);
            }
        }
    }
}
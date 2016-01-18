using UnityEngine;
using System.Collections;
namespace Roland
{
    public class HideFog : MonoBehaviour
    {
        string Fog = "Fog";
        void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.CompareTag(Fog))
            {
                Lean.LeanPool.Despawn(collider.gameObject);
            }
        }
    }
}

using UnityEngine;
using System.Collections;
namespace Roland
{
    public class RevealFogExplosion : MonoBehaviour
    {
        string explosion = "Explosion";
        string EndExplosion = "EndExplosion";

        public static bool Trigger = false;
        void OnTriggerEnter2D(Collider2D collider)
        {
            if (Trigger)
            {
                if (collider.CompareTag(explosion) || collider.CompareTag(EndExplosion))
                {
                    Explosion exp = collider.GetComponentInParent<Explosion>();
                    if (exp.ID == DarkRift.DarkRiftAPI.id)
                        this.gameObject.SetActive(false);
                }
            }
        }
    }
}

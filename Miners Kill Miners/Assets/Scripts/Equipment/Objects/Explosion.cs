using UnityEngine;
using System.Collections;

namespace Roland
{
    public class Explosion : MonoBehaviour
    {
        Animator ChildAnimator;
        public float time;
        public AudioClip explosion;
        WaitForSeconds wait;
        void Init()
        {
            wait = new WaitForSeconds(time);
            StartCoroutine(destroyself());
            AudioSource src = gameObject.AddComponent<AudioSource>();
            src.clip = explosion;
            src.loop = false;
            src.Play();
        }

        protected virtual void OnSpawn()
        {
            Init();
        }

        IEnumerator destroyself()
        {
            yield return wait;
            Lean.LeanPool.Despawn(this.gameObject);
        }
    }
}
using UnityEngine;
using System.Collections;

public class ExplosionParticleEffect : Singleton<ExplosionParticleEffect>
{
    protected ExplosionParticleEffect()
    {
    }

    void Start()
    {
        ParentParticle = GameObject.Find("ParticleParent").transform;
        ParentParticle.gameObject.SetActive(false);
        Child = ParentParticle.GetChild(0).GetComponent<ParticleSystem>() ;
    }
    Transform ParentParticle;
    ParticleSystem Child;

    public void PositionParticleAndExplode( Transform HealthToExplode)
    {
        if(ParentParticle == null)
        {
            ParentParticle = GameObject.Find("ParticleParent").transform;
            ParentParticle.gameObject.SetActive(false);
        }
        Vector3 ResultPos = Camera.main.ScreenToWorldPoint(HealthToExplode.position);
        ParentParticle.gameObject.SetActive(false);
        ParentParticle.position = new Vector3(ResultPos.x, ResultPos.y, ParentParticle.position.z);
        ParentParticle.gameObject.SetActive(true);
    }

    void Update()
    {
        if(Child == null)
        {
            Start();
        }
        if(!Child.isPlaying)
        {
            ParentParticle.gameObject.SetActive(false);
        }
    }
}

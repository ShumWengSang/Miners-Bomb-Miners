﻿using UnityEngine;
using System.Collections;

namespace Roland
{
    public class Explosion : MonoBehaviour
    {
        Animator ChildAnimator;
        public float time;
   
        void Start()
        {
           // ChildAnimator = GetComponentInChildren<Animator>();
           // float time = ChildAnimator.GetCurrentAnimatorClipInfo(LayerMask.NameToLayer("Explosion")).Length;
           // Debug.Log("Time is + " + time);
            DestroyObject(this.gameObject, time);
        }
    }
}


using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Prop : MonoBehaviour
{
    [SerializeField]
    internal clssSetting hitBox;

    internal Rigidbody rigid;
    Animator animator;


    public bool isHit;
    internal bool isBreak = false;
    public float disableTime = 0;

    //OnHitInterection must be changeable to any.
    virtual internal void OnHit(hitDefParams hitParam, Vector3 hitPos)
    {

    }
    internal List<Renderer> renderers;

    //set rigidbody, animator, etc at start
    void Start()
    {
        renderers = GetComponentsInChildren<Renderer>().ToList();
        animator = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
        hitBox.initClss(transform);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        AnimationSet();
        hitBox.clssPosUpdate();
        foreach (clssDef Clsses in hitBox.clssDefs)
        {
            Clsses.getGlobalPos();
            Clsses.DrawCapsule();
        }
        if (isHit == false)
        {
            disableTime -= 1f;
        }
        else
        {
            isHit = false;
        }
        SetStatus();
    }

    internal virtual void AnimationSet()
    {
        if(animator != null)
        {
            //animator.SetBool("isDestroyed", HP < 0);
        }
    }

    internal virtual void SetStatus()
    { 
        
    }

    internal virtual void OnHit()
    {
        
    }

    public bool isPausable()
    {
        return disableTime <= 0;
    }
}


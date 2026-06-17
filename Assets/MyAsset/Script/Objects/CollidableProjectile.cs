using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollidableProjectile : Projectile
{
    void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Collided! + " + collision.gameObject.layer);
        if (other.gameObject.layer == LayerMask.NameToLayer("Terrain"))
        {
            destroyEmit();
        }
    }

    override internal void OnEmit()
    {
        rb.velocity = Velocity;
    }

    override internal void OnProjUpdation()
    {
        
    }
    
    override internal void HitUpdate()
    {
        foreach (clssDef c in cSet.clssDefs)
        {
            //Debug.Log("drawing clssDefs");
            c.getGlobalPos();
            c.DrawCapsule();
        }
        if(gameState.self.ProvokeHitDef(this.hitDefParams, ref ColliderNums ,cSet))
        {
            //destroyEmit();
        }
    }

    override internal void destroyEmit()
    {
        if (EffectObject != null)
        { 
            Instantiate(EffectObject,transform.position,Quaternion.identity);
        }
        base.destroyEmit();
    }
}

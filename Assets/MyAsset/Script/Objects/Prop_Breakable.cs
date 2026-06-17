

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Prop_Breakable : Prop
{
    //BreakableProps Settings
    public GameObject onDestroyInstantiate;
    public float HP = 10f;
    public float DestroyTime = 1.0f;

    internal override void SetStatus()
    {
        base.SetStatus();
        //Ondestroy, starts coroutine.
        if (HP < 0)
        {
            if (!isBreak)
            {
                isBreak = true;
                StartCoroutine(Destroys());
            }
        }
    }

    internal override void OnHit(hitDefParams hitParam, Vector3 hitPos)
    {
        SetDamages(hitParam, hitPos);        
    }

    void SetDamages(hitDefParams hitParam, Vector3 hitPos)
    { 
        float muls = 3.0f;
        Transform selectedTrfm = hitParam.ownerEntity.transform;
        Vector3 AddVelocity =
        Vector3.Normalize(selectedTrfm.forward) * hitParam.velset.x +
        Vector3.Normalize(selectedTrfm.up) * hitParam.velset.y +
        Vector3.Normalize(selectedTrfm.right) * hitParam.velset.z;
        AddVelocity *= muls;
        //POWER!!
        rigid.AddForceAtPosition(AddVelocity, hitPos, ForceMode.Impulse);
        disableTime += hitParam.hitStopTime.y;
        HP -= hitParam.Damage.x;
    }


    IEnumerator Destroys()
    {
        if(onDestroyInstantiate != null)
        {
            //Pops off.
            GameObject instObj = Instantiate(onDestroyInstantiate,transform.position,Quaternion.identity);
            instObj.GetComponent<Rigidbody>().velocity = Vector3.up * 6.2f;
        }

        //and add time
        while( DestroyTime > 0)
        {
            DestroyTime -= Time.deltaTime;
            if(renderers.Count > 0)
            {
                foreach(Renderer rend in renderers)
                {
                    rend.enabled = !rend.enabled;
                }
            }
            yield return null;
        }
        Destroy(gameObject);
    }
}


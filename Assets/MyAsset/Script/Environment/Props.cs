using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Prop : MonoBehaviour
{
    [SerializeField]
    internal clssSetting hitBox;
    Rigidbody rigid;

    public bool isHit;
    bool isBreak = false;

    public float disableTime = 0;

    public GameObject onDestroyInstantiate;

    public float HP = 10f;

    Animator animator;

    

    internal void OnHit(hitDefParams hitParam,Vector3 hitPos)
    {
        Transform selectedTrfm = hitParam.ownerEntity.transform;
        Vector3 AddVelocity = 
        Vector3.Normalize(selectedTrfm.forward) * hitParam.velset.x +
        Vector3.Normalize(selectedTrfm.up) * hitParam.velset.y +
        Vector3.Normalize(selectedTrfm.right) * hitParam.velset.z;
        //POWER!!
        rigid.AddForceAtPosition(AddVelocity,hitPos, ForceMode.Impulse);
        disableTime += hitParam.hitStopTime.y;
        HP -= hitParam.Damage.x;
    }
    List<Renderer> renderers;

    // Start is called before the first frame update
    void Start()
    {
        renderers = GetComponentsInChildren<Renderer>().ToList();
        animator = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
        hitBox.initClss(transform);
    }

    // Update is called once per frame
    void Update()
    {
        hitBox.clssPosUpdate();
        foreach(clssDef Clsses in hitBox.clssDefs)
        {
            Clsses.getGlobalPos();
            Clsses.DrawCapsule();
        }        
        if(isHit == false)
        {
            disableTime -= 1f;
        }
        //Ondestroy, starts coroutine.
        if(HP < 0)
        {
            if(!isBreak)
            { 
                isBreak = true;
                StartCoroutine(Destroys());
            }
        }
        else
        {            
            isHit = false;
        }
        AnimationSet();
    }

    void AnimationSet()
    {
        if(animator != null)
        {
            animator.SetBool("isDestroyed", HP < 0);
        }
    }

    public bool isPausable()
    {
        return disableTime <= 0;
    }

    float destroTime_Current = 0f;
    float destroTime_Max = 1f;

    IEnumerator Destroys()
    {
        if(onDestroyInstantiate != null)
        {
            //Pops off.
            GameObject instObj = Instantiate(onDestroyInstantiate,transform.position,Quaternion.identity);
            instObj.GetComponent<Rigidbody>().velocity = Vector3.up * 6.2f;
        }

        //and add time
        while( destroTime_Max > destroTime_Current)
        {
            destroTime_Current += Time.deltaTime;
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Props : MonoBehaviour
{
    [SerializeField]
    internal clssSetting hitBox;
    Rigidbody rigid;

    public bool isHit;

    public float disableTime = 0;

    public float HP = 10f;

    

    internal void OnHit(hitDefParams hitParam,Vector3 hitPos)
    {
        Transform selectedTrfm = hitParam.hitEntity.transform;
        Vector3 AddVelocity = 
        Vector3.Normalize(selectedTrfm.forward) * hitParam.velset.x +
        Vector3.Normalize(selectedTrfm.up) * hitParam.velset.y +
        Vector3.Normalize(selectedTrfm.right) * hitParam.velset.z;
        //POWER!!
        rigid.AddForceAtPosition(AddVelocity,hitPos, ForceMode.Impulse);
        disableTime += hitParam.hitStopTime.y;
    }

    // Start is called before the first frame update
    void Start()
    {
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
        isHit = false;
    }

    public bool isPausable()
    {
        return disableTime <= 0;
    }
}

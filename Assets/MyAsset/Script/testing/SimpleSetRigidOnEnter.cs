using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SimpleSetRigidOnEnter : MonoBehaviour
{
    public Vector3 setVect;
    // Start is called before the first frame update
    void OnTriggerEnter(Collider c)
    {
        Entity ent = c.gameObject.GetComponent<Entity>();
        if(ent != null)
        {
            ent.rigid.velocity += setVect;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

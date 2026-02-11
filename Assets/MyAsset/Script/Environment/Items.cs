using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

[RequireComponent(typeof(CapsuleCollider))]
public class Items : MonoBehaviour
{
    float TimeForInit = 1f;
    float currentTime = 0f;

    Rigidbody rigid;
    CapsuleCollider capCol;

    public GameObject getEffect;

    //trigger detections, life up etcs.
    void OnTriggerEnter(Collider c)
    {
        if(currentTime >= TimeForInit && c.gameObject.tag == "Player")
        {
            if(getEffect != null)
            {
                Instantiate(getEffect, transform.position + Vector3.up * 0.5f, Quaternion.identity);
            }
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        capCol = GetComponent<CapsuleCollider>();
        rigid.AddTorque(Vector3.up * 500f);
    }

    // Update is called once per frame
    void Update()
    {
        float minimumLength = 0.001f;
        Vector3 norm =
            new Vector3
            (capCol.direction == 0 ? 1 : 0,
            capCol.direction == 1 ? 1 : 0,
            capCol.direction == 2 ? 1 : 0);

        Vector3 bottom_pos =
            transform.position + Vector3.up * (minimumLength) + transform.rotation * (capCol.center - norm * (capCol.height / 2f - capCol.radius));
        Ray ray = new Ray(bottom_pos,Vector3.down);
        LayerMask mask = LayerMask.GetMask("Default", "Terrain");
        float HitDist = capCol.radius + Mathf.Max(0f, -rigid.velocity.y * Time.deltaTime);
        Debug.DrawLine(bottom_pos, bottom_pos + Vector3.down * HitDist);
        if(Physics.Raycast(ray,out RaycastHit hit,HitDist,mask) && rigid.velocity.y <= 0)
        {
            rigid.velocity = Vector3.zero;
            rigid.position = hit.point;
        }
        currentTime += Time.deltaTime;
    }
}

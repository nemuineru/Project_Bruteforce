using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GaugeComponent : MonoBehaviour
{
    internal Shapes.ShapesAssets Component;
    internal Entity valueEntity;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    //onUpdate, Set those gauges to what value.
    void Update()
    {        
        setValues();
    }

    virtual internal void setValues()
    {
    }
}

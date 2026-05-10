using System.Collections;
using System.Collections.Generic;
using Shapes;
using UnityEngine;

public class TargetComp : MonoBehaviour
{
    public Shapes.Disc[] Discs;
    public Shapes.Line[] Lines;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetColors(Color col)
    {
        foreach(Disc D in Discs)
        {
            D.Color = col;
        }
        foreach(Line L in Lines)
        {
            L.Color = col;
        }
    }
}

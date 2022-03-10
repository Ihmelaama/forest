using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpherifyTest : MonoBehaviour
{

    public float radius=1f;
    public List<Transform> targets;
    private List<Transform> childTargets=new List<Transform>();

    void Start()
    {

        for(int i=0; i<targets.Count; i++) {

            childTargets.Add(targets[i].Find("Child"));

        }
        
    }

    void Update()
    {

        Vector3 origin=transform.position;
        origin.y-=radius;

        for(int i=0; i<targets.Count; i++) {

            Vector3 a=targets[i].position;
            a.y=transform.position.y;

            Vector3 b=origin;

            Vector3 dir=a-b;
            childTargets[i].transform.position=origin+dir.normalized*radius;

        }
        
    }

}

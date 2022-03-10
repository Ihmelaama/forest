using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{

//---------------------------------
// VARIABLES

    public static Character instance;

    public Animator animator;
    public float walkSpeed=1f;
    public float walkTargetRadius=1f;

    private float walkDirectionLerp=0.1f;

    private bool isWalking=false;
    private List<Vector3> walkPath=new List<Vector3>();
    private float walkPathLength=0f;
    private float walkPathPosition=0f;

//---------------------------------
// EVENTS

    void Awake()
    {

        instance=this;

    }

//------------

    void Start()
    {
        
    }

//------------

    void Update()
    {

        if(isWalking) {

            WalkAlongPath();

        }
        
    }

//---------------------------------
// PUBLIC

    public void WalkTo(Vector3 point) 
    {

        animator.SetBool("isWalking", true);
        isWalking=true;

        GetWalkPath(point, ref walkPath, ref walkPathLength);
        walkPathPosition=0f;

        if(CameraRig.instance) {

            CameraRig.instance.LookTowards(point);

        }

    }

//---------------------------------
// PRIVATE

    private void StopWalking() {

        animator.SetBool("isWalking", false);
        isWalking=false;
        walkPath=new List<Vector3>();

    }

//------------

    private void WalkAlongPath() {

        if(walkPath.Count==0) return;

        // debug 
        Vector3 prevPos=transform.position;
        Quaternion prevRot=transform.rotation;

        float pos;
        Vector3 d;

    // get direction towards waypoint ---

        pos=walkPathPosition+walkSpeed*Time.deltaTime;
        d=FindPositionOnPath(walkPath, pos)-transform.position;

    // rotate towards waypoint ---

        float deg=Mathf.Atan2(d.x, d.z)*Mathf.Rad2Deg;
        Quaternion rot=Quaternion.Euler(0f, deg, 0f);
        transform.rotation=Quaternion.Lerp(transform.rotation, rot, walkDirectionLerp);

        Vector3 localDir=transform.InverseTransformDirection(d);
        animator.SetFloat("walkDirection", localDir.x);

    // move towards waypoint ---

        transform.position=transform.position+d*walkSpeed*Time.deltaTime;

    // check if path endpoint reached ---

        walkPathPosition=walkPathPosition+walkSpeed*Time.deltaTime;
        if(walkPathPosition>=walkPathLength) {
            StopWalking();
        }

    // debug ---

        DrawDebugPath(walkPath);

        Vector3 y=new Vector3(0f, 2f, 0f);
        Debug.DrawLine(prevPos+y, transform.position+y, Color.cyan, 3f);

    }

//------------

    private void GetWalkPath(Vector3 point, ref List<Vector3> path, ref float pathLength) {

        path=new List<Vector3>();
        path.Add(transform.position);
        path.Add(point);

    // sort obstacles by distance ---

        List<Obstacle> obstacles=new List<Obstacle>();
        obstacles.AddRange(Obstacle.obstacles);
        obstacles.Sort((Obstacle a, Obstacle b) => {

            Vector3 p1=a.transform.position;
            p1.y=transform.position.y;
            p1-=transform.position;

            Vector3 p2=b.transform.position;
            p2.y=transform.position.y;
            p2-=transform.position;

            return (int) Mathf.Sign(p1.magnitude-p2.magnitude);
            
        });

    // get valid path ---

        for(int i=1; i<path.Count; i++) {

            for(int j=0; j<obstacles.Count; j++) {

                Vector3 a=path[i-1];
                Vector3 b=path[i];

                Obstacle obstacle=obstacles[j];
                bool validPath=obstacle.IsPathValid(a, b);

                if(!validPath) {

                    List<Vector3> p=obstacle.FindBestPathAround(a, b, 1f);

                    // remove unnecessary waypoints
                    a=path[i-1];
                    while(p.Count>1 && obstacle.IsPathValid(a, p[1])) {
                    p.RemoveAt(0);
                    }

                    path.InsertRange(i, p);
                    i=path.Count-1;

                }

            }

        }

    // calculate path total length ---

        pathLength=0f;
        for(int i=1; i<path.Count; i++) {
            pathLength+=(path[i]-path[i-1]).magnitude;
        }

    }

//------------

    private Vector3 FindPositionOnPath(List<Vector3> path, float distance) {

        float segmentLen;
        float segmentPos;
        Vector3 pos;
        float pathPos=0f;

        for(int i=1; i<walkPath.Count; i++) {

            if(i<walkPath.Count-1) {

                segmentLen=(walkPath[i+1]-walkPath[i-1]).magnitude;
                segmentPos=(distance-pathPos)/segmentLen;

                if(distance>=pathPos && distance<pathPos+segmentLen) {

                    pos=GetQuadraticBezier(walkPath[i-1], walkPath[i], walkPath[i+1], segmentPos);
                    return pos;

                }

                pathPos+=segmentLen;
                i++;

            } else {

                segmentLen=(walkPath[i]-walkPath[i-1]).magnitude;
                segmentPos=(distance-pathPos)/segmentLen;

                if(distance>=pathPos && distance<pathPos+segmentLen) {

                    pos=Vector3.Lerp(walkPath[i-1], walkPath[i], segmentPos);
                    return pos;

                }

                pathPos+=segmentLen;

            }

        }
 
    return walkPath[walkPath.Count-1];
    }

//------------

    // copied this from: https://catlikecoding.com/unity/tutorials/curves-and-splines/
    public static Vector3 GetQuadraticBezier (Vector3 p0, Vector3 p1, Vector3 p2, float t) {

        t = Mathf.Clamp01(t);
        float oneMinusT = 1f - t;

        return oneMinusT * oneMinusT * p0 + 2f * oneMinusT * t * p1 + t * t * p2;

    }

//------------

    private void DrawDebugPath(List<Vector3> path) {

        for(int i=1; i<path.Count; i++) {

            Vector3 a=path[i-1];
            a.y+=1f;

            Vector3 b=path[i];
            b.y+=1f;

            Debug.DrawLine(a, b, Color.green, 3f);

        }

    }

}

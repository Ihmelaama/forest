using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{

    public static List<Obstacle> obstacles=new List<Obstacle>();

    public float avoidRadius=1f;
    public List<Vector3> navigationPoints;

    void Awake()
    {

        if(!Obstacle.obstacles.Contains(this)) {

            Obstacle.obstacles.Add(this);

        }

    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void OnDrawGizmos()
    {

        if(navigationPoints.Count==0) return;

        Gizmos.color = Color.red;

        for(int i=0; i<navigationPoints.Count; i++) {

            Vector3 a=GetWorldPoint(navigationPoints[i]);
            Vector3 b=GetWorldPoint( i==0 ? navigationPoints[navigationPoints.Count-1] : navigationPoints[i-1] );

            Gizmos.DrawSphere(a, 0.5f);
            Gizmos.DrawLine(a, b);

        }

        DrawDebugSphere(transform.position, avoidRadius, Color.red);

    }

    public bool IsWithinAvoidRadius(Vector3 point) {

        point.y=transform.position.y;
        Vector3 dif=point-transform.position;

        if(dif.magnitude<avoidRadius) return true;

    return false;
    }

    public bool IsPathValid(Vector3 start, Vector3 destination, float sphereCastRadius=0.5f)
    {

        Vector3 direction=(destination-start).normalized;
        float maxDistance=(destination-start).magnitude;

        RaycastHit[] hits;
        hits=Physics.SphereCastAll(start, sphereCastRadius, direction, maxDistance);

        foreach(RaycastHit hit in hits) {

            if(hit.collider.transform.IsChildOf(transform)) {

                return false;

            }

        }

        return true;

    }

    public Vector3 FindClosestNavigationPoint(Vector3 point) {

        int navPointNum=FindClosestNavigationPointNum(point);
        Vector3 navPoint=GetWorldPoint(navigationPoints[navPointNum]);

    return navPoint;
    }

    public List<Vector3> FindBestPathAround(Vector3 start, Vector3 destination, float sphereCastRadius=0.5f) 
    {

        int firstPointNum=FindClosestNavigationPointNum(start);
        Vector3 firstPointVec=GetWorldPoint(navigationPoints[firstPointNum]);

        int lastPointNum=FindClosestNavigationPointNum(destination);
        Vector3 lastPointVec=GetWorldPoint(navigationPoints[lastPointNum]);
        
        List<Vector3> path1=FindPath(firstPointNum, 1, destination, sphereCastRadius);
        List<Vector3> path2=FindPath(firstPointNum-1, -1, destination, sphereCastRadius);

        float dist1=GetPathDistance(start, destination, path1);
        float dist2=GetPathDistance(start, destination, path2);

        if(dist1<dist2) {
            return path1;
        }

    return path1;
    }

    private List<Vector3> FindPath(int firstNavPoint, int dir, Vector3 destination, float sphereCastRadius=0.5f) 
    {

        int i=firstNavPoint;
        if(i<0) i=navigationPoints.Count-1;
        if(i>=navigationPoints.Count) i=0;

        List<Vector3> path=new List<Vector3>();
        Vector3 worldPoint=GetWorldPoint(navigationPoints[i]);
        path.Add(worldPoint);

        int count=0;
        
        while(!IsPathValid(worldPoint, destination, sphereCastRadius)) {

            i+=dir;
            if(i<0) i=navigationPoints.Count-1;
            if(i>=navigationPoints.Count) i=0;
            worldPoint=GetWorldPoint(navigationPoints[i]);
            path.Add(worldPoint);

            count++;
            if(count>navigationPoints.Count) {
                return new List<Vector3>();
            }

        }

    return path;
    }

    private int FindClosestNavigationPointNum(Vector3 point) 
    {

        point.y=0f;

        int closest=-1;
        float dist=Mathf.Infinity;

        for(int i=0; i<navigationPoints.Count; i++) {

            Vector3 navPoint=GetWorldPoint(navigationPoints[i]);
            navPoint.y=0f;

            Vector3 dif=point-navPoint;

            if(dif.magnitude<dist) {
                
                closest=i;
                dist=dif.magnitude;

            }

        }

    return closest;
    }

    private float GetPathDistance(Vector3 start, Vector3 destination, List<Vector3> path) {

        if(path.Count==0) return Mathf.Infinity;

        float dist=0f;
        dist+=(path[0]-start).magnitude;

        for(int i=1; i<path.Count; i++) {

            dist+=(path[i]-path[i-1]).magnitude;

        }

        dist+=(destination-path[path.Count-1]).magnitude;

    return dist;
    }

    private Vector3 GetWorldPoint(Vector3 local) 
    {
    
        return transform.TransformPoint(local);

    }

    private void DrawDebugSphere(Vector3 center, float radius, Color color) 
    {

        for(int i=1; i<360; i++) {

            float rad1=(i-1)*Mathf.Deg2Rad;
            float x1=Mathf.Sin(rad1)*radius;
            float z1=Mathf.Cos(rad1)*radius;

            float rad2=i*Mathf.Deg2Rad;
            float x2=Mathf.Sin(rad2)*radius;
            float z2=Mathf.Cos(rad2)*radius;

            Vector3 a=center+new Vector3(x1, 0f, z1);
            Vector3 b=center+new Vector3(x2, 0f, z2);
            Debug.DrawLine(a, b, color, Time.deltaTime);

        }

    }

}

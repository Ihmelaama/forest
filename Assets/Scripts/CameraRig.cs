using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRig : MonoBehaviour
{

    public static CameraRig instance;

    public Transform followTarget;

    [Space(20f)]

    public float followLerp=0.01f;
    public float rotationLerp=0.01f;
    public float dragRotationLerp=0.1f;
    public float maxRotationSpeed=10f;
    public float rotationFrictionLerp=0.02f;

    [Space(20f)]

    public bool rotateTowardsTarget=true;

    private Vector3 followOffset;
    private Quaternion targetRotation;

    private Vector3 rotationSpeed=Vector3.zero;

    void Awake()
    {

        instance=this;

        followOffset=transform.position-followTarget.transform.position;

        targetRotation=transform.rotation;

    }

    void Start()
    {

    }

    void Update()
    {

        Vector3 targetPos=followTarget.position+followOffset;
        transform.position=Vector3.Lerp(transform.position, targetPos, followLerp);

        if(rotateTowardsTarget) {
        
            transform.rotation=Quaternion.Lerp(transform.rotation, targetRotation, rotationLerp);

        } else {

            Quaternion rot=Quaternion.Euler(rotationSpeed*Time.deltaTime);
            transform.rotation*=rot;

            rotationSpeed.y=Mathf.Lerp(rotationSpeed.y, 0f, rotationFrictionLerp);

        }

    }

    public void LookTowards(Vector3 point) {

        Vector3 a=transform.position;
        a.y=0f;
        
        Vector3 b=point;
        b.y=0f;

        Vector3 v=b-a;

        float deg=Mathf.Atan2(v.x, v.z)*Mathf.Rad2Deg;
        targetRotation=Quaternion.Euler(0f, deg, 0f);

    }

    public void Rotate(float speed) {

        rotationSpeed.y=Mathf.Lerp(rotationSpeed.y, speed, dragRotationLerp);

        if(Mathf.Abs(rotationSpeed.y)>maxRotationSpeed) {
        rotationSpeed.y=Mathf.Sign(rotationSpeed.y)*maxRotationSpeed;
        }

    }

}

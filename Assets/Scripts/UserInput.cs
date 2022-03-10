using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInput : MonoBehaviour
{

//---------------------------------
// VARIABLES

    public bool allowInput=false;

    private float dragThreshold=5f;

    private bool hasInput=false;
    private Vector3 firstTouchPos;
    private int floorLayerMask;

//---------------------------------
// EVENTS

    void Start() 
    {

        floorLayerMask=LayerMask.GetMask("Floor");

    }

//------------

    void Update()
    {

        if(!allowInput) return;

        Vector3 touchPos=firstTouchPos;

        #if UNITY_EDITOR || UNITY_STANDALONE_WIN

            if(Input.GetMouseButtonDown(0)) BeginTouch(Input.mousePosition);
            if(Input.GetMouseButtonUp(0)) EndTouch(Input.mousePosition);
            if(hasInput) touchPos=Input.mousePosition;

        #elif UNITY_IOS || UNITY_ANDROID

            if(Input.touches.Length>0 && Input.touches[0].phase==TouchPhase.Began) BeginTouch(Input.touches[0].position);
            if(Input.touches.Length==0 || Input.touches[0].phase==TouchPhase.Ended) EndTouch(Input.touches[0].position);
            if(hasInput && Input.touches.Length>0) touchPos=Input.touches[0].position;

        #endif

        if(hasInput) {

            Vector3 drag=touchPos-firstTouchPos;
            HandleDrag(drag);

        }

    }

//---------------------------------
// PUBLIC

    public void ToggleInput(bool b) {
        allowInput=b;
    }

//---------------------------------
// PRIVATE

    private void BeginTouch(Vector3 touchPos) {

        hasInput=true;
        firstTouchPos=touchPos;

    }

//------------

    private void EndTouch(Vector3 touchPos) {

        hasInput=false;

        Vector3 drag=touchPos-firstTouchPos;
        if(drag.magnitude<dragThreshold) {

            Ray ray=Camera.main.ScreenPointToRay(touchPos);
            RaycastHit hit;
            if(Physics.Raycast(ray.origin, ray.direction, out hit, Mathf.Infinity, floorLayerMask)) {
                
                if(Character.instance) {
                    
                    Vector3 destination=hit.point;
                    destination.y=0f;
                    Character.instance.WalkTo(destination);

                }

            }

        }

    }

//------------

    private void HandleDrag(Vector3 drag) {

        if(CameraRig.instance) {

            CameraRig.instance.Rotate(drag.x);

        }

    }

}

/*
//---------------------------------
// VARIABLES

    public Character character;
    public CameraRig cameraRig;

    private float dragThreshold=5f;

    private Vector3 firstTouchPos;

//---------------------------------
// EVENTS

    void Start() 
    {

    }

//------------

    void Update()
    {

        #if UNITY_EDITOR || UNITY_STANDALONE_WIN

            if(Input.GetMouseButtonDown(0)) BeginTouch(Input.mousePosition);
            if(Input.GetMouseButtonUp(0)) EndTouch(Input.mousePosition);

        #elif UNITY_IOS || UNITY_ANDROID

            if(Input.touches.Length>0 && Input.touches[0].phase==TouchPhase.Began) BeginTouch(Input.touches[0].position);
            if(Input.touches.Length==0 || Input.touches[0].phase==TouchPhase.Ended) EndTouch(Input.touches[0].position);

        #endif

    }

    private void BeginTouch(Vector3 touchPos) {

        firstTouchPos=touchPos;

    }

    private void EndTouch(Vector3 touchPos) {

        Vector3 drag=touchPos-firstTouchPos;
        if(drag.magnitude<dragThreshold) {

            MoveCharacter(touchPos);

        } else {

            Debug.Log("dragged");

        }

    }

    private void MoveCharacter(Vector3 touchPos) {

        Vector3 screenPos=Camera.main.WorldToScreenPoint(character.transform.position);
        screenPos.z=0f;

        Vector3 dir=touchPos-screenPos;

        Vector3 worldDir=dir.normalized;
        worldDir=new Vector3(worldDir.x, 0f, worldDir.y);
        worldDir=cameraRig.transform.TransformPoint(worldDir)-cameraRig.transform.position;

        float walkDistance=Mathf.Sqrt(Mathf.Pow(dir.x, 2f)+Mathf.Pow(dir.y, 2f));
        walkDistance/=30f;
        if(dir.y>0f) walkDistance*=2f;

        Vector3 destination=character.transform.position;
        destination+=worldDir*walkDistance;
        character.WalkTo(destination);

    }

}
*/

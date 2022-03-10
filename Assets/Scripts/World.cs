using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class World : MonoBehaviour
{

    public Transform worldCurvatureAnchor;
    public float worldCurvatureRadius=500f;
    
    [Space(20f)]

    public Transform fogCenter;
    public Color fogColor;
    public float fogDistance;
    public float fogStrength=1f;

    void Start()
    {
        
    }

    void Update()
    {

        Shader.SetGlobalVector("_WorldCurvatureAnchor", worldCurvatureAnchor ? worldCurvatureAnchor.position : Vector3.zero );
        Shader.SetGlobalFloat("_WorldCurvatureRadius", worldCurvatureRadius);

        Shader.SetGlobalVector("_FogCenter", fogCenter ? fogCenter.position : Vector3.zero );
        Shader.SetGlobalColor("_FogColor", fogColor);
        Shader.SetGlobalFloat("_FogDistance", fogDistance);
        Shader.SetGlobalFloat("_FogStrength", fogStrength);

        if(!Application.isPlaying) return;
        
    }

}

Shader "Custom/SpherifySurf"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _OffsetY ("Offset Y", Float) = 0.0
        [ToggleOff] _SpecularHighlights("Specular Highlights", Float) = 1.0
        [ToggleOff] _ApplyFog("Fog", Float) = 1.0
        [ToggleOff] _ApplyCurvature("Curvature", Float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows vertex:vert addshadow
        #pragma shader_feature_local _SPECULARHIGHLIGHTS_OFF

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
            float3 viewDir;
            float4 vertexWorldPos; 
            //float3 normal;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        float _OffsetY;

        float4 _WorldCurvatureAnchor=float4(0, 0, 0, 0);
        float _WorldCurvatureRadius=500;

        float4 _FogCenter=float4(0, 0, 0, 0);
        float4 _FogColor=float4(1, 0, 0, 0);
        float _FogDistance=100;
        float _FogStrength=1;

        float _ApplyFog;
        float _ApplyCurvature;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void vert (inout appdata_full v, out Input o) 
        {

            float4 vert = mul(unity_ObjectToWorld, v.vertex);

            float dir = sign(_WorldCurvatureRadius);
            float y = vert.y - _WorldCurvatureAnchor.y;

            float4 origin= _WorldCurvatureAnchor;
            origin.y -= _WorldCurvatureRadius;

            float4 surface= vert;
            surface.y = _WorldCurvatureAnchor.y;

            float3 dirFromOrigin= normalize(surface-origin);
            y = _WorldCurvatureRadius + y;

            vert.xyz = origin + ( dirFromOrigin * y ) * dir;

            vert = mul(unity_WorldToObject, vert);

            vert = lerp(v.vertex, vert, _ApplyCurvature);

            vert.y += _OffsetY;

            v.vertex = vert;

            UNITY_INITIALIZE_OUTPUT(Input, o);
            o.vertexWorldPos = mul(unity_ObjectToWorld, v.vertex);
            //o.normal= v.normal;
            //o.normal = mul(unity_ObjectToWorld, v.normal);

        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {

            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;

            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;

            float fog=length(IN.vertexWorldPos-_FogCenter);
            fog/=_FogDistance;
            fog=clamp(fog, 0, 1);

            _FogStrength*=_ApplyFog;

            fixed4 fogCol=lerp(float4(0, 0, 0, 0), _FogColor, fog*_FogStrength);
            o.Emission =fogCol;

            fixed4 albedoCol=lerp(c, _FogColor, fog*_FogStrength);
            o.Albedo = albedoCol;

            //o.Metallic=fog*_FogStrength;
            //o.Smoothness=fog*_FogStrength;

        }

        ENDCG
    }
    FallBack "Diffuse"
}

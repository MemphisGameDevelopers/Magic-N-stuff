Shader "AFS_v2/Advanced Grass Shader Groundlighting no Translucency" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_Cutoff ("Alpha cutoff", Range(0,1)) = 0.3
	_MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
}

SubShader {
	Tags {
		"Queue" = "Geometry"
		"IgnoreProjector"="True"
		"RenderType"="AfsGrassModelSingleSided"
	}
	Cull Off
	LOD 200
	ColorMask RGB

CGPROGRAM
#pragma surface surf Lambert vertex:AfsWavingGrassVert addshadow
#pragma exclude_renderers flash
#include "TerrainEngine.cginc"
#include "Includes/atsWavingGrassModel vertexAlpha.cginc"

#pragma multi_compile IN_EDITMODE IN_PLAYMODE
#pragma multi_compile GRASS_ANIMATE_COLOR GRASS_ANIMATE_NORMAL

sampler2D _MainTex;
float _Cutoff;
float4 _Color;

struct Input {
	float2 uv_MainTex;
	float4 color : COLOR;
};


void surf (Input IN, inout SurfaceOutput o) {
	half4 c = tex2D(_MainTex, IN.uv_MainTex);
	#ifdef IN_EDITMODE
	// in editmode do not add vertex colors but only AO stored in IN.color.b and use a normal according to the object's rotation
	o.Albedo = c.rgb * _Color.rgb * IN.color.b;
	o.Normal = normalize( mul( (float3x3)_Object2World, fixed3(0,1,0) ) );
	#else
	o.Albedo = c.rgb * _Color.rgb * IN.color.rgb;
	#endif
	o.Alpha = c.a * IN.color.a;
	clip (o.Alpha - _Cutoff);
}
ENDCG
}
	Fallback Off
}
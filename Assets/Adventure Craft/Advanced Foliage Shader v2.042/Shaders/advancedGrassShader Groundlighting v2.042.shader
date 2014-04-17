Shader "AFS_v2/Advanced Grass Shader Groundlighting" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_Cutoff ("Alpha cutoff", Range(0,1)) = 0.3
	_MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
	
	_TranslucencyColor ("Translucency Color", Color) = (0.73,0.85,0.41,1) // (187,219,106,255)
	_TranslucencyViewDependency ("View dependency", Range(0,1)) = 0.7
	_TranslucencyFactor ("Translucency Factor", Range(0,1)) = 0.5
	
	_ShadowStrength("Shadow Strength", Range(0,1)) = 0.8	
	_ShadowOffsetScale ("Shadow Offset Scale", Float) = 1
	
}

SubShader { 
	Tags {
		"Queue" = "Geometry+200"
		"IgnoreProjector"="True"
		"RenderType"="AfsGrassModel"
	}
	LOD 200

CGPROGRAM
#pragma surface surf Grass alphatest:_Cutoff vertex:AfsWavingGrassVertTrans nolightmap nodirlightmap addshadow

#pragma exclude_renderers flash
#pragma glsl_no_auto_normalization
#include "Includes/GrassLighting.cginc"

#include "TerrainEngine.cginc"
#include "Includes/atsWavingGrassModel vertexAlpha.cginc"

#pragma multi_compile IN_EDITMODE IN_PLAYMODE
#pragma multi_compile GRASS_ANIMATE_COLOR GRASS_ANIMATE_NORMAL

sampler2D _MainTex;
fixed4 _Color;
fixed _TranslucencyFactor;

struct Input {
	float2 uv_MainTex;
	fixed4 color : COLOR; // color.b = AO, color.a = Bending
};

void surf (Input IN, inout GrassSurfaceOutput o) {
	fixed4 c;
	c = tex2D(_MainTex, IN.uv_MainTex);
	#ifdef IN_EDITMODE
	// in editmode do not add vertex colors but only AO stored in IN.color.b and use a normal according to the object's rotation
	o.Albedo = c.rgb * IN.color.b *_Color.rgb;
	o.Normal = normalize( mul( (float3x3)_Object2World, fixed3(0,1,0) ) );
	#else
	// in play mode IN.color.rgb contain dry and healthy colors
	o.Albedo = c.rgb * _Color.rgb * IN.color.rgb;
	#endif
	// take the translucency from the green channel
	o.Translucency = c.g * _TranslucencyFactor;	
	o.Alpha = c.a; // * IN.color.a;
	
}
ENDCG

}

// vertexlit
SubShader {
	Tags {
		"Queue" = "Geometry+200"
		"IgnoreProjector"="True"
		"RenderType"="AfsGrassModel"
	}
	
	ColorMask RGB
	Lighting On
	
	Pass {
		CGPROGRAM
		#pragma vertex Grass
		#pragma exclude_renderers shaderonly
		#pragma glsl_no_auto_normalization

		#include "UnityCG.cginc"
		#include "Includes/GrassLighting.cginc"
		#include "Includes/atsWavingGrassModel vertexAlpha.cginc"

				
		struct v2f {
			float4 pos : SV_POSITION;
			fixed4 color : COLOR;
			float4 uv : TEXCOORD0;
		};
		
		v2f Grass (appdata_full v) {
			v2f o;
			AfsWavingGrassVertTrans (v);

			// this works in playmode as v.normal is tweaked by the combinechildren script
			o.color.rgb = ShadeVertexLights (v.vertex, v.normal);
			o.pos = mul (UNITY_MATRIX_MVP, v.vertex);	
			o.uv = v.texcoord;
			o.color.a = 1.0f;
			return o;
		}
		ENDCG

		AlphaTest Greater [_Cutoff]
		SetTexture [_MainTex] { combine texture * primary DOUBLE, texture }
		SetTexture [_MainTex] {
			ConstantColor [_Color]
			Combine previous * constant, previous
		} 
	}
}

SubShader {
	Tags { "RenderType"="AfsGrassModel" }
		
	Pass {
		ColorMask RGB
		
		Material {
			Diffuse (1,1,1,1)
			Ambient (1,1,1,1)
		}
		Lighting On
		
		AlphaTest Greater [_Cutoff]
		SetTexture [_MainTex] { Combine texture * primary DOUBLE, texture }
		SetTexture [_MainTex] {
			ConstantColor [_Color]
			Combine previous * constant, previous
		} 
	}
} 
}
Shader "AFS_v2/Advanced Foliage Shader TouchBending Atlas" {
Properties {
	_Cutoff ("Alpha cutoff", Range(0,1)) = 0.3
	_MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
	_Shininess("Shininess", Range(0.03,1)) = 0.2
	
	_TouchBendingPosition ("TouchBendingPosition", Vector) = (0,0,0,0)
	_TouchBendingForce ("TouchBendingForce", Vector) = (0,0,0,0)

	// These are here only to provide default values
	_TranslucencyColor ("Translucency Color", Color) = (0.73,0.85,0.41,1) // (187,219,106,255)
	_TranslucencyViewDependency ("View dependency", Range(0,1)) = 0.7
	_ShadowStrength("Shadow Strength", Range(0,1)) = 0.8
	_ShadowOffsetScale ("Shadow Offset Scale", Float) = 1
}


SubShader { 
	Tags {
		"IgnoreProjector"="True"
		"RenderType"="AtsFoliageTouchBending"
	}
	LOD 200
	//Offset -1,0
	
CGPROGRAM
#pragma surface surf TreeLeaf alphatest:_Cutoff vertex:TouchBending nolightmap nodirlightmap
#pragma exclude_renderers flash
#pragma glsl_no_auto_normalization

#include "Includes/Tree.cginc"
#include "TerrainEngine.cginc"
#include "Includes/TouchBending.cginc"

sampler2D _MainTex;
float _Shininess;
float _AfsRainamount;
float _AfsSpecPower;

struct Input {
	float2 uv_MainTex;
	fixed4 color : COLOR; // color.a = AO
};

void surf (Input IN, inout LeafSurfaceOutput o) {
	fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
	o.Albedo = c.rgb * IN.color.a;
	fixed4 trngls = tex2D(_MainTex, float2(IN.uv_MainTex.x + 0.5, IN.uv_MainTex.y));
	o.Translucency = trngls.r;
	o.Gloss = trngls.b * (1 + _AfsRainamount) * (1 + _AfsSpecPower *_AfsRainamount);
	o.Specular = _Shininess * (1 +_AfsRainamount);
	o.Alpha = c.a;
	o.Normal = UnpackNormalDXT5nm(trngls);
}
ENDCG

	// Pass to render object as a shadow caster
	Pass {
		Name "ShadowCaster"
		Tags { "LightMode" = "ShadowCaster" }
		
		Fog {Mode Off}
		ZWrite On ZTest LEqual Cull Off
		Offset 1, 1


		CGPROGRAM
		#pragma vertex vert_surf
		#pragma fragment frag_surf
		#pragma exclude_renderers noshadows flash
		#pragma glsl_no_auto_normalization
		#pragma fragmentoption ARB_precision_hint_fastest
		#pragma multi_compile_shadowcaster
		#include "HLSLSupport.cginc"
		#include "UnityCG.cginc"
		#include "Lighting.cginc"

		#define INTERNAL_DATA
		#define WorldReflectionVector(data,normal) data.worldRefl

		#include "Includes/Tree.cginc"
		#include "Includes/TouchBending.cginc"



		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		struct v2f_surf {
			V2F_SHADOW_CASTER;
			float2 hip_pack0 : TEXCOORD1;
		};
		
		float4 _MainTex_ST;
		v2f_surf vert_surf (appdata_full v) {
			v2f_surf o;
			TouchBending (v);
			o.hip_pack0.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
			TRANSFER_SHADOW_CASTER(o)
			return o;
		}
		fixed _Cutoff;
		float4 frag_surf (v2f_surf IN) : COLOR {
			half alpha = tex2D(_MainTex, IN.hip_pack0.xy).a;
			clip (alpha - _Cutoff);
			SHADOW_CASTER_FRAGMENT(IN)
		}
		ENDCG
	}	


		// Pass to render object as a shadow collector
	Pass {
		Name "ShadowCollector"
		Tags { "LightMode" = "ShadowCollector" }
		
		Fog {Mode Off}
		ZWrite On ZTest LEqual

		CGPROGRAM
		#pragma vertex vert_surf
		#pragma fragment frag_surf
		#pragma exclude_renderers noshadows flash
		#pragma fragmentoption ARB_precision_hint_fastest
		#pragma multi_compile_shadowcollector
		#pragma glsl_no_auto_normalization
		#include "HLSLSupport.cginc"
		#define SHADOW_COLLECTOR_PASS
		#include "UnityCG.cginc"
		#include "Lighting.cginc"

		//#define INTERNAL_DATA
		//#define WorldReflectionVector(data,normal) data.worldRefl

		#include "Includes/Tree.cginc"
		#include "Includes/TouchBending.cginc"



		sampler2D _MainTex;
		//sampler2D _BumpTransSpecMap;
		//float _ShadowOffsetScale;

		struct Input {
			float2 uv_MainTex;
		};



		struct v2f_surf {
			V2F_SHADOW_COLLECTOR;
			float2 hip_pack0 : TEXCOORD5;
			//float3 normal : TEXCOORD6;
		};
	

		
		float4 _MainTex_ST;
		
		v2f_surf vert_surf (appdata_full v) {
			v2f_surf o;
			TouchBending (v);
			o.hip_pack0.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
			
			//float3 worldN = mul((float3x3)_Object2World, SCALED_NORMAL);
			//o.normal = mul(_World2Shadow, half4(worldN, 0)).xyz;

			TRANSFER_SHADOW_COLLECTOR(o)
			return o;
		}		
		fixed _Cutoff;
		
		half4 frag_surf (v2f_surf IN) : COLOR {
			half alpha = tex2D(_MainTex, IN.hip_pack0.xy).a;

			//float3 shadowOffset = _ShadowOffsetScale * IN.normal * tex2D (_BumpTransSpecMap, IN.hip_pack0.xy).b;
			clip (alpha - _Cutoff);
		
				
			//IN._ShadowCoord0 += shadowOffset;
			//IN._ShadowCoord1 += shadowOffset;
			//IN._ShadowCoord2 += shadowOffset;
			//IN._ShadowCoord3 += shadowOffset;

			SHADOW_COLLECTOR_FRAGMENT(IN)
		}
		ENDCG
	}
}

// vertexlit
SubShader {
	Tags {
		"IgnoreProjector"="True"
		"RenderType"="AtsFoliageTouchBending"	
	}
	
	ColorMask RGB
	Lighting On
	
	Pass {
		CGPROGRAM
		#pragma vertex TreeVertLit
		#pragma exclude_renderers shaderonly
		
		#include "UnityCG.cginc"
		#include "Includes/Tree.cginc"
		#include "Includes/CustomBending.cginc"

		
		struct v2f {
			float4 pos : SV_POSITION;
			fixed4 color : COLOR;
			float4 uv : TEXCOORD0;
		};
		
		v2f TreeVertLit (appdata_full v) {
			v2f o;
			CustomBending (v);

			o.color.rgb = ShadeVertexLights (v.vertex, v.normal);
				
			o.pos = mul (UNITY_MATRIX_MVP, v.vertex);	
			o.uv = v.texcoord;
			o.color.a = 1.0f;
			return o;
		}
		ENDCG

		AlphaTest Greater [_Cutoff]
		SetTexture [_MainTex] { combine texture * primary DOUBLE, texture }
		//SetTexture [_MainTex] {
		//	ConstantColor [_Color]
		//	Combine previous * constant, previous
		//} 
	}
}

SubShader {
	Tags { "RenderType"="AtsFoliageTouchBending" }
		
	Pass {
		ColorMask RGB
		
		Material {
			Diffuse (1,1,1,1)
			Ambient (1,1,1,1)
		}
		Lighting On
		
		AlphaTest Greater [_Cutoff]
		SetTexture [_MainTex] { Combine texture * primary DOUBLE, texture }
		//SetTexture [_MainTex] {
		//	ConstantColor [_Color]
		//	Combine previous * constant, previous
		//} 
	}
} 
}
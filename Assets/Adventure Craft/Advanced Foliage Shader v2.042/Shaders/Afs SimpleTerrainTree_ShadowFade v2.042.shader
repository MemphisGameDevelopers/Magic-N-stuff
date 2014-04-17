Shader "AFS_v2/AFS Simple Terrain Tree ShadowFade" {
Properties {
	_MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
	_Cutoff ("Alpha cutoff", Range(0,1)) = 0.3
	_BumpTransSpecMap ("Normal (GA) Trans(R) Spec(B)", 2D) = "bump" {}	
	_Shininess("Shininess", Range(0.03,1)) = 0.2
	_TranslucencyColor ("Translucency Color", Color) = (0.73,0.85,0.41,1) // (187,219,106,255)
	_TranslucencyViewDependency ("View dependency", Range(0,1)) = 0.7
	_ShadowStrength("Shadow Strength", Range(0,1)) = 0.8
	_ShadowOffsetScale ("Shadow Offset Scale", Float) = 1
	
	_BillboardContrast ("Billboard Contrast", Float) = 1

	
	// These are here only to provide default values
	_Scale ("Scale", Vector) = (1,1,1,1)
	_SquashAmount ("Squash", Float) = 1
	
	_Color ("Do not Touch this", Color) = (1,1,1,1)
}

SubShader { 
	Tags {
		"IgnoreProjector"="True"
		"RenderType"="AtsFoliageTreeTerrain"
	}
	LOD 200
	//Offset -1,0
		
CGPROGRAM
#pragma surface surf TreeLeaf alphatest:_Cutoff vertex:SimpleTreeTerrain nolightmap nodirlightmap
#pragma exclude_renderers flash
#pragma glsl_no_auto_normalization

#pragma target 3.0

#include "Includes/Tree_ShadowFade.cginc"
#include "Includes/TreeBending.cginc"

#pragma multi_compile BILLBOARD_SHADOWS BILLBOARD_NO_SHADOWS

sampler2D _MainTex;
sampler2D _BumpTransSpecMap;
float _Shininess;
float _AfsRainamount;
float _AfsSpecPower;
fixed4 _AfsAmbientBillboardLight;

struct Input {
	float2 uv_MainTex;
	fixed4 color : COLOR; // color.a = AO
};


void surf (Input IN, inout LeafSurfaceOutput o) {
	fixed4 c = tex2D(_MainTex, IN.uv_MainTex);

//	add billboardShadows
	fixed3 shadowColor = lerp( ( _AfsAmbientBillboardLight.rgb ), fixed3(1,1,1), _Color.a);
	#ifdef BILLBOARD_SHADOWS
	o.Albedo = c.rgb * IN.color.a * _Color.rgb * lerp( shadowColor, fixed3(1,1,1), IN.color.r);
	#else
	o.Albedo = c.rgb * IN.color.a * _Color.rgb * lerp( shadowColor, fixed3(1,1,1), IN.color.b);
	#endif

//	store fadeState
	#ifdef BILLBOARD_SHADOWS
	o.ShadowCutOff = IN.color.r; // * _SquashAmount;
	#else
	o.ShadowCutOff = IN.color.b;
	#endif
	
	fixed4 trngls = tex2D (_BumpTransSpecMap, IN.uv_MainTex);
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
		#pragma multi_compile TREE_SHADOW_DISSOLVE TREE_SHADOW_NO_DISSOLVE
		#pragma multi_compile BILLBOARD_SHADOWS BILLBOARD_NO_SHADOWS
		#include "HLSLSupport.cginc"
		#include "UnityCG.cginc"
		#include "Lighting.cginc"

		#define INTERNAL_DATA
		#define WorldReflectionVector(data,normal) data.worldRefl

		#include "Includes/Tree.cginc"
		#include "Includes/TreeBending.cginc"

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		struct v2f_surf {
			V2F_SHADOW_CASTER;
			float2 hip_pack0 : TEXCOORD1;
			#ifdef TREE_SHADOW_DISSOLVE
//			dissolve
			float fade : TEXCOORD2;
			#endif
		};
		
		float4 _MainTex_ST;
		v2f_surf vert_surf (appdata_full v) {
			v2f_surf o;
			//SimpleTreeTerrain (v);
			SimpleTreeTerrainShadowCaster (v);
			#ifdef TREE_SHADOW_DISSOLVE
//			dissolve
			o.fade = v.color.b;
			#endif
			o.hip_pack0.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
			TRANSFER_SHADOW_CASTER(o)
			return o;
		}
		fixed _Cutoff;
		float4 frag_surf (v2f_surf IN) : COLOR {
			half alpha = tex2D(_MainTex, IN.hip_pack0.xy).a;
			#ifdef TREE_SHADOW_DISSOLVE
//			dissolve
			// contrast
			//alpha= lerp(0.5, alpha, 2 - IN.fade);
			clip (alpha * IN.fade - _Cutoff * (2 - IN.fade) * 0.5);
			#else
			clip (alpha - _Cutoff);
			#endif
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
		#pragma multi_compile BILLBOARD_SHADOWS BILLBOARD_NO_SHADOWS
		#pragma glsl_no_auto_normalization
		#include "HLSLSupport.cginc"
		#define SHADOW_COLLECTOR_PASS
		#include "UnityCG.cginc"
		#include "Lighting.cginc"

		#define INTERNAL_DATA
		#define WorldReflectionVector(data,normal) data.worldRefl

		#include "Includes/Tree.cginc"
		#include "Includes/TreeBending.cginc"


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
			SimpleTreeTerrain (v);
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
		"RenderType"="AtsFoliageTreeTerrain"
	}
	
	ColorMask RGB
	Lighting On
	
	Pass {
		CGPROGRAM
		#pragma vertex TreeVertLit
		#pragma exclude_renderers shaderonly
		
		#include "UnityCG.cginc"
		#include "Includes/Tree.cginc"
		#include "Includes/TreeBending.cginc"

		
		struct v2f {
			float4 pos : SV_POSITION;
			fixed4 color : COLOR;
			float4 uv : TEXCOORD0;
		};
		
		v2f TreeVertLit (appdata_full v) {
			v2f o;
			SimpleTreeTerrain (v);

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
	Tags { "RenderType"="AtsFoliageTreeTerrain" }
		
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
Dependency "BillboardShader" = "Hidden/Nature/AFS Simple Terrain Tree Rendertex"
}
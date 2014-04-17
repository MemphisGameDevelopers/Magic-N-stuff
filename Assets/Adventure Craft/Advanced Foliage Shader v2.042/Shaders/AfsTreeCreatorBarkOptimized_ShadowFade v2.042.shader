Shader "AFS_v2/AFS Tree Creator Bark Optimized ShadowFade" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
	_BumpSpecMap ("Normalmap (GA) Spec (R)", 2D) = "bump" {}
	_TranslucencyMap ("Trans (RGB) Gloss(A)", 2D) = "white" {}
	
	// These are here only to provide default values
	_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
	_Scale ("Scale", Vector) = (1,1,1,1)
	_SquashAmount ("Squash", Float) = 1
}

SubShader { 
	Tags { "RenderType"="AFSTreeBark" }
	LOD 200
	
CGPROGRAM
#pragma surface surf TreeBark vertex:MyTerrainTreeVertBark nolightmap
#pragma exclude_renderers flash
#pragma glsl_no_auto_normalization
#include "Includes/Tree_ShadowFade.cginc"
#include "Includes/TreeBending.cginc"

#pragma multi_compile BILLBOARD_SHADOWS BILLBOARD_NO_SHADOWS


sampler2D _MainTex;
sampler2D _BumpSpecMap;
sampler2D _TranslucencyMap;
fixed4 _AfsAmbientBillboardLight;

struct Input {
	float2 uv_MainTex;
	fixed4 color : COLOR;
};

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 c = tex2D(_MainTex, IN.uv_MainTex);

//	add billboardShadows
	fixed3 shadowColor = lerp( _AfsAmbientBillboardLight.rgb, fixed3(1,1,1), _Color.a);
	#ifdef BILLBOARD_SHADOWS
	o.Albedo = c.rgb * _Color.rgb * IN.color.a * lerp( shadowColor, fixed3(1,1,1), IN.color.r); // - _SquashAmount);
	#else
	o.Albedo = c.rgb * _Color.rgb * IN.color.a * lerp( shadowColor, fixed3(1,1,1), IN.color.b); // - _SquashAmount);
	#endif
	
	fixed4 trngls = tex2D (_TranslucencyMap, IN.uv_MainTex);
	o.Gloss = trngls.a * _Color.r;

//	store fadeState
	// as alpha is always 1 we can simply use it / 0 + IN.color.b --> make it compile with dx9/11
	//	store fadeState
	#ifdef BILLBOARD_SHADOWS
	o.Alpha = 0 + IN.color.r;
	#else
	o.Alpha = 0 + IN.color.b;
	#endif
	
	half4 norspc = tex2D (_BumpSpecMap, IN.uv_MainTex);
	o.Specular = norspc.r;
	o.Normal = UnpackNormalDXT5nm(norspc);
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

		float4 _CameraForwardVec;
		
		struct v2f_surf {
			V2F_SHADOW_CASTER;
			//#ifdef TREE_SHADOW_DISSOLVE
//			dissolve
			float2 fade : TEXCOORD2;
			//#endif
			
		};
		v2f_surf vert_surf (appdata_full v) {
			v2f_surf o;
			//MyTerrainTreeVertBark (v);
			MyTerrainTreeVertBarkShadowCaster (v);
			#ifdef TREE_SHADOW_DISSOLVE
//			dissolve
			o.fade.x = v.color.b;
			o.fade.y = (v.texcoord.x - 0.5);
			o.fade.y *= sign(o.fade.y);
			o.fade.y = 1 - o.fade.y * 1.25;
			//o.fade.y = v.texcoord.x * v.texcoord.x * (3 - 2 * v.texcoord.x);
			#endif
			TRANSFER_SHADOW_CASTER(o)
			return o;
		}
		fixed _Cutoff;
		float4 frag_surf (v2f_surf IN) : COLOR {
			#ifdef TREE_SHADOW_DISSOLVE
//			dissolve
			clip (IN.fade.x - IN.fade.y );
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

		struct v2f_surf {
			V2F_SHADOW_COLLECTOR;
			//float3 normal : TEXCOORD6;
		};
		
		v2f_surf vert_surf (appdata_full v) {
			v2f_surf o;
			MyTerrainTreeVertBark (v);
			//float3 worldN = mul((float3x3)_Object2World, SCALED_NORMAL);
			//o.normal = mul(_World2Shadow, half4(worldN, 0)).xyz;
			TRANSFER_SHADOW_COLLECTOR(o)
			return o;
		}
		half4 frag_surf (v2f_surf IN) : COLOR {
			SHADOW_COLLECTOR_FRAGMENT(IN)
		}
		ENDCG
	}





}

SubShader {
	Tags { "RenderType"="AFSTreeBark" }
	Pass {		
		Material {
			Diffuse (1,1,1,1)
			Ambient (1,1,1,1)
		} 
		Lighting On
		SetTexture [_MainTex] {
			Combine texture * primary DOUBLE, texture * primary
		}
		SetTexture [_MainTex] {
			ConstantColor [_Color]
			Combine previous * constant, previous
		} 
	}
}

Dependency "BillboardShader" = "Hidden/Nature/AFS Tree Creator Bark Rendertex"
}

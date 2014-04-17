Shader "Hidden/TerrainEngine/Details/Vertexlit" {
Properties {
	_MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
	
	// These are here only to provide default values
	_ShadowOffsetScale ("Shadow Offset Scale", Float) = 1
}


SubShader { 
	Tags {
		"IgnoreProjector"="True"
		"RenderType"="AtsFoliageTerrain"
	}
	LOD 200
		
CGPROGRAM
#pragma surface surf LeafTerrainEngine alphatest:_AfsAlphaCutOff vertex:CustomBending nolightmap nodirlightmap
#pragma exclude_renderers flash
#pragma glsl_no_auto_normalization

//#pragma multi_compile WINDOWS

#include "Includes/FoliageLightingTerrainEngine.cginc"
#include "TerrainEngine.cginc"
#include "Includes/CustomBendingVertexLit.cginc"

sampler2D _MainTex;
float _AfsShininess;
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
	o.Specular = _AfsShininess * (1 +_AfsRainamount);
	o.Normal = UnpackNormalDXT5nm(trngls);
	o.Alpha = c.a;
}
ENDCG

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
		#include "Includes/CustomBendingVertexLit.cginc"

		sampler2D _MainTex;
		
		float _ShadowOffsetScale;
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
			CustomBending (v);
			o.hip_pack0.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
			//float3 worldN = mul((float3x3)_Object2World, SCALED_NORMAL);
			//o.normal = mul(_World2Shadow, half4(worldN, 0)).xyz;
			TRANSFER_SHADOW_COLLECTOR(o)
			return o;
		}
		
		fixed _AfsAlphaCutOff;
		
		half4 frag_surf (v2f_surf IN) : COLOR {
			half alpha = tex2D(_MainTex, IN.hip_pack0).a;
			//float3 shadowOffset = _ShadowOffsetScale * IN.normal * tex2D(_MainTex, IN.hip_pack0).b;
			clip (alpha - _AfsAlphaCutOff);
			//IN._ShadowCoord0 += shadowOffset;
			//IN._ShadowCoord1 += shadowOffset;
			//IN._ShadowCoord2 += shadowOffset;
			//IN._ShadowCoord3 += shadowOffset;
			SHADOW_COLLECTOR_FRAGMENT(IN)
		}
		ENDCG
	}
}

SubShader {
	Tags { "RenderType"="AtsFoliageTerrain" }
		
	Pass {
		ColorMask RGB
		
		Material {
			Diffuse (1,1,1,1)
			Ambient (1,1,1,1)
		}
		Lighting On
		
		AlphaTest Greater [_AfsAlphaCutOff]
		SetTexture [_MainTex] { Combine texture * primary DOUBLE, texture }
		//SetTexture [_MainTex] {
		//	ConstantColor [_Color]
		//	Combine previous * constant, previous
		//} 
	}
} 
}
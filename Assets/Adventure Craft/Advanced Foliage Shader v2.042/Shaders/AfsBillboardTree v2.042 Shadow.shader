Shader "Hidden/TerrainEngine/BillboardTree" {
	Properties {
		_MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
	}
	
	
	SubShader {
		LOD 300
		Tags { "Queue" = "Transparent-100" "IgnoreProjector"="True" "RenderType"="AfsTreeBillboard" }
		
		Pass {
			ColorMask rgb
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite Off Cull Off
		
			CGPROGRAM
			#pragma vertex vert
			#include "UnityCG.cginc"
			#include "TerrainEngine.cginc"
			#include "Includes/AfsBillboardShadow.cginc"
			#pragma fragment frag

			struct v2f {
				float4 pos : POSITION;
				fixed4 color : COLOR0;
				float2 uv : TEXCOORD0;
				//
				float fade : TEXCOORD2;
			}   ;
			
			fixed4 _AfsTerrainTrees;

			v2f vert (appdata_tree_billboard v) {
				v2f o;
				float fade;
				AfsTerrainBillboardTree(v.vertex, v.texcoord1.xy, v.texcoord.y);
				// calculate fade
				float3 treePos = v.vertex.xyz - _TreeBillboardCameraPos.xyz;
				float treeDistanceSqr = dot(treePos, treePos);
				float fadeDistanceSqr = dot(_AfsTerrainTrees.z, _AfsTerrainTrees.z);
				o.fade = saturate( (_TreeBillboardDistances.x - fadeDistanceSqr - treeDistanceSqr ) / fadeDistanceSqr) ;
				
				o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
				o.uv.x = v.texcoord.x;
				o.uv.y = v.texcoord.y > 0;
				o.color = v.color;
				
				return o;
			}

			sampler2D _MainTex;
			fixed4 _AfsAmbientBillboardLight;
			
			fixed4 frag(v2f input) : COLOR
			{
				fixed4 col = tex2D( _MainTex, input.uv);
				col.rgb *= input.color.rgb * lerp( ( _AfsAmbientBillboardLight.rgb ), fixed3(1,1,1), input.color.a );
				// fade out billboards
				col.a *= input.fade;
				clip(col.a);
				return col;
			}
			ENDCG
		}
		
		// ///////////
		

		Pass {
		
			Name "ShadowCaster"
			Tags { "LightMode" = "ShadowCaster" }
		
			Fog {Mode Off}
			ColorMask rgb
			ZWrite On ZTest LEqual Cull Off
			Offset 1, 1
			
			CGPROGRAM
			#pragma vertex vert_surf
			#pragma fragment frag_surf
			#pragma exclude_renderers noshadows flash
			#pragma glsl_no_auto_normalization
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma multi_compile_shadowcaster
			#pragma multi_compile BILLBOARDSHADOW_EDGEFADE BILLBOARDSHADOW_NO_EDGEFADE
			
			#include "HLSLSupport.cginc"
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
	
			#define INTERNAL_DATA
			#define WorldReflectionVector(data,normal) data.worldRefl
	
			#include "TerrainEngine.cginc"
			#include "Includes/AfsBillboardShadow.cginc"
	
			sampler2D _MainTex;
			float4 _CameraForwardVec;
			
			float4 _AfsTerrainTrees;
	
			struct Input {
				float2 uv_MainTex;
			}   ;

			struct v2f_surf {
				V2F_SHADOW_CASTER;
				float2 hip_pack0 : TEXCOORD2;
				#ifdef BILLBOARDSHADOW_EDGEFADE
				float fade : TEXCOORD3;
				#endif
			}   ;
			
			float4 _MainTex_ST;
			v2f_surf vert_surf (appdata_full v) {
				v2f_surf o;
				AfsTerrainBillboardTreeShadow(v.vertex, v.texcoord1.xy, v.texcoord.y);
				#ifdef BILLBOARDSHADOW_EDGEFADE
				o.fade = dot (normalize (_WorldSpaceCameraPos.xyz - mul(_Object2World, v.vertex).xyz) * -1, _CameraForwardVec.xyz );
				o.fade = saturate( o.fade - _CameraForwardVec.w ) * .005;
				#endif
				o.hip_pack0.x = v.texcoord.x;
				o.hip_pack0.y = v.texcoord.y > 0;
				TRANSFER_SHADOW_CASTER(o)
				return o;
			}
			
			float4 frag_surf (v2f_surf IN) : COLOR {
				half4 alpha = tex2D(_MainTex, IN.hip_pack0.xy).rgba;
				#ifdef BILLBOARDSHADOW_EDGEFADE
				// fade out shadows towards screen edges
				alpha.a = lerp(0.0, alpha.a, IN.fade * (1 - alpha.g) * (1 - alpha.g) );
				#endif
				clip (alpha.a - 0.0001);
				SHADOW_CASTER_FRAGMENT(IN)
			}
			ENDCG
		}
		// ///////////
	}
	
	SubShader {
		LOD 200
		Tags { "Queue" = "Transparent-100" "IgnoreProjector"="True" "RenderType"="AfsTreeBillboard" }
		
		Pass {
			ColorMask rgb
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite Off Cull Off
		
			CGPROGRAM
			#pragma vertex vert
			#include "UnityCG.cginc"
			#include "TerrainEngine.cginc"
			#include "Includes/AfsBillboardShadow.cginc"
			#pragma fragment frag

			struct v2f {
				float4 pos : POSITION;
				fixed4 color : COLOR0;
				float2 uv : TEXCOORD0;
				//
				float fade : TEXCOORD2;
			}   ;
			
			fixed4 _AfsTerrainTrees;

			v2f vert (appdata_tree_billboard v) {
				v2f o;
				float fade;
				AfsTerrainBillboardTree(v.vertex, v.texcoord1.xy, v.texcoord.y);
				// calculate fade
				float3 treePos = v.vertex.xyz - _TreeBillboardCameraPos.xyz;
				float treeDistanceSqr = dot(treePos, treePos);
				float fadeDistanceSqr = dot(_AfsTerrainTrees.z, _AfsTerrainTrees.z);
				o.fade = saturate( (_TreeBillboardDistances.x - fadeDistanceSqr - treeDistanceSqr ) / fadeDistanceSqr) ;
				
				o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
				o.uv.x = v.texcoord.x;
				o.uv.y = v.texcoord.y > 0;
				o.color = v.color;
				
				return o;
			}

			sampler2D _MainTex;
			fixed4 _AfsAmbientBillboardLight;
			
			fixed4 frag(v2f input) : COLOR
			{
				fixed4 col = tex2D( _MainTex, input.uv);
				col.rgb *= input.color.rgb * lerp( ( _AfsAmbientBillboardLight.rgb ), fixed3(1,1,1), input.color.a );
				// fade out billboards
				col.a *= input.fade;
				clip(col.a);
				return col;
			}
			ENDCG
		}
		
		Pass {
			Name "ShadowCaster"
			Tags { "LightMode" = "ShadowCaster" }	
			// Clear pass
		}
		
	}
	

//	vertex lit
	SubShader {
		LOD 100
		Tags { "Queue" = "Transparent-100" "IgnoreProjector"="True" "RenderType"="AfsTreeBillboard" }
		
		Pass {

			CGPROGRAM
			#pragma vertex vert
			#pragma exclude_renderers shaderonly
			#include "UnityCG.cginc"
			#include "TerrainEngine.cginc"
			#include "Includes/AfsBillboard.cginc"

			struct v2f {
				float4 pos : POSITION;
				fixed4 color : COLOR0;
				float2 uv : TEXCOORD0;
			}   ;
			

			v2f vert (appdata_tree_billboard v) {
				v2f o;
				AfsTerrainBillboardTree(v.vertex, v.texcoord1.xy, v.texcoord.y);
				//TerrainBillboardTree(v.vertex, v.texcoord1.xy, v.texcoord.y);
				o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
				o.uv.x = v.texcoord.x;
				o.uv.y = v.texcoord.y > 0;
				o.color = v.color;
				return o;
			}
			ENDCG			

			ColorMask rgb
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite Off Cull Off
			AlphaTest Greater 0
			SetTexture [_MainTex] { combine texture * primary, texture }
		}
	}
	Fallback "Transparent/Cutout/Diffuse"
}



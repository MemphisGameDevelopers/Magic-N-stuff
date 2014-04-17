Shader "Hidden/Nature/AFS Simple Terrain Tree Rendertex" {
Properties {
	_TranslucencyColor ("Translucency Color", Color) = (0.73,0.85,0.41,1) // (187,219,106,255)
	_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
	_HalfOverCutoff ("0.5 / alpha cutoff", Range(0,1)) = 1.0
	_TranslucencyViewDependency ("View dependency", Range(0,1)) = 0.7
	
	_MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
	_BumpTransSpecMap ("Normal (GA) Trans(R) Spec(B)", 2D) = "white" {}
	_Shininess("Shininess", Range(0.03,1)) = 0.2
	
	_BillboardContrast ("Billboard Contrast", Float) = 1
}

SubShader {  
	Fog { Mode Off }
	
	Pass {
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "Includes/Tree.cginc"

struct v2f {
	float4 pos : SV_POSITION;
	float2 uv : TEXCOORD0;
	float3 color : TEXCOORD1; 
	float3 backContrib : TEXCOORD2;
	float3 nl : TEXCOORD3;
	float3 nh : TEXCOORD4;
};
#if defined(SHADER_API_D3D11) || defined(SHADER_API_D3D11_9X)
CBUFFER_START(UnityTerrainImposter)
#endif
	float3 _TerrainTreeLightDirections[4];
	float4 _TerrainTreeLightColors[4];
	float4 _AfsBillboardAdjustments;
#if defined(SHADER_API_D3D11) || defined(SHADER_API_D3D11_9X)
CBUFFER_END
#endif

v2f vert (appdata_full v) {	
	v2f o;
	//not needed as we do not have billboarded leaves...
	//ExpandBillboard (UNITY_MATRIX_IT_MV, v.vertex, v.normal, v.tangent);
	o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
	o.uv = v.texcoord.xy;
	float3 viewDir = normalize(ObjSpaceViewDir(v.vertex));
	
	for (int j = 0; j < 3; j++)
	{
		float3 lightDir = _TerrainTreeLightDirections[j];
		half nl = dot (v.normal, lightDir);
		// view dependent back contribution for translucency
		half backContrib = saturate(dot(viewDir, -lightDir));
		// normally translucency is more like -nl, but looks better when it's view dependent
		backContrib = lerp(saturate(-nl), backContrib, _TranslucencyViewDependency);
//	translucency		
		o.backContrib[j] = backContrib  * _AfsBillboardAdjustments.y;
		
		// wrap-around diffuse
		nl = max (0, nl * 0.6 + 0.4);
		o.nl[j] = nl;
		
		half3 h = normalize (lightDir + viewDir);
		float nh = max (0, dot (v.normal, h));
		o.nh[j] = nh;
	}
	
	o.color = v.color.a;
	return o;
}

sampler2D _MainTex;
sampler2D _BumpTransSpecMap;
fixed _Cutoff;
float _Shininess;
float _AfsRainamount;
float _AfsSpecPower;

float _BillboardContrast;

fixed4 frag (v2f i) : COLOR {
	fixed4 col = tex2D (_MainTex, i.uv);
	clip (col.a - _Cutoff);
	fixed3 albedo = col.rgb * i.color;
	half specular = _Shininess * (1 +_AfsRainamount) * 64.0; // * 128.0; //_Shininess * (1 +_AfsRainamount);
	fixed4 trngls = tex2D (_BumpTransSpecMap, i.uv);
	half gloss = trngls.b * (1 + _AfsRainamount) * (1 + _AfsSpecPower *_AfsRainamount);
	half3 light = UNITY_LIGHTMODEL_AMBIENT * albedo;
	half3 backContribs = i.backContrib * trngls.r;
	
	for (int j = 0; j < 3; j++)
	{
		half3 lightColor = _TerrainTreeLightColors[j].rgb;
		half3 translucencyColor = backContribs[j] * _TranslucencyColor;
		half nl = i.nl[j];		
		half nh = i.nh[j];
		half spec = pow (nh, specular) * gloss;
		light += (albedo * (translucencyColor + nl) + _SpecColor.rgb * spec) * lightColor;
	}
	
	fixed4 c;
	c.rgb = light * _AfsBillboardAdjustments.z;
	// brightness
	//c.rgb *= _AfsBillboardAdjustments.z;
	// contrast
	c.rgb = lerp(float3(0.5, 0.5, 0.5), c.rgb, _AfsBillboardAdjustments.w * _BillboardContrast);
	c.a = 1;
	return c;
}
ENDCG
	}
}

SubShader {
	Pass {		
		Fog { Mode Off }
		
		CGPROGRAM
		#pragma vertex vert
		#pragma exclude_renderers shaderonly
		#include "UnityCG.cginc"

		struct v2f {
			float4 pos : SV_POSITION;
			fixed4 color : COLOR;
			float2 uv : TEXCOORD0;
		};

		#if defined(SHADER_API_D3D11) || defined(SHADER_API_D3D11_9X)
		CBUFFER_START(UnityTerrainImposter)
		#endif
			float3 _TerrainTreeLightDirections[4];
			float4 _TerrainTreeLightColors[4];
		#if defined(SHADER_API_D3D11) || defined(SHADER_API_D3D11_9X)
		CBUFFER_END
		#endif
		float _HalfOverCutoff;

		v2f vert (appdata_full v) {
			v2f o;
			o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
			o.uv = v.texcoord.xy;

			float3 light = UNITY_LIGHTMODEL_AMBIENT.rgb;
			
			for (int j = 0; j < 3; j++)
			{
				half3 lightColor = _TerrainTreeLightColors[j].rgb;
				float3 lightDir = _TerrainTreeLightDirections[j];
			
				half nl = dot (v.normal, lightDir);
				light += lightColor * nl;
			}
			
			// lighting * AO
			o.color.rgb = light * v.color.a;
			
			// We want to do alpha testing on cutoff, but at the same
			// time write 1.0 into alpha. So we multiply alpha by 0.25/cutoff
			// and alpha test on alpha being greater or equal to 1.0.
			// That will work for cutoff values in range [0.25;1].
			// Remember that color gets clamped to [0;1].
			o.color.a = 0.5 * _HalfOverCutoff;
			return o;
		}
		ENDCG
		
		AlphaTest GEqual 1
		SetTexture [_MainTex] {
			Combine texture * primary DOUBLE, texture * primary QUAD
		} 
	}
}

FallBack Off
}

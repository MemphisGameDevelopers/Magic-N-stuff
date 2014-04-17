#ifndef TREE_CG_INCLUDED
#define TREE_CG_INCLUDED

#include "TerrainEngine.cginc"

fixed3 _TranslucencyColor;
fixed _TranslucencyViewDependency;
half _ShadowStrength;

struct GrassSurfaceOutput {
	fixed3 Albedo;
	fixed3 Normal;
	fixed3 Emission;
	half Specular;
	fixed Gloss;
	fixed Alpha;
	fixed Translucency;
};

inline half4 LightingGrass (GrassSurfaceOutput s, half3 lightDir, half3 viewDir, half atten)
{
	half3 h = normalize (lightDir + viewDir);
	half nl = dot (s.Normal, lightDir);
	// view dependent back contribution for translucency
	fixed backContrib = saturate(dot(viewDir, -lightDir));
	// normally translucency is more like -nl, but looks better when it's view dependent
	backContrib = lerp(saturate(-nl), backContrib, _TranslucencyViewDependency);
	fixed3 translucencyColor = backContrib * s.Translucency * _TranslucencyColor;
	fixed4 c;
	// clamp nl
	nl = saturate(nl);
	c.rgb = s.Albedo * (translucencyColor * 2 + nl);
	c.rgb = c.rgb * _LightColor0.rgb;
	// For directional lights, apply less shadow attenuation
	// based on shadow strength parameter.
	#if defined(DIRECTIONAL) || defined(DIRECTIONAL_COOKIE)
	c.rgb *= lerp(2, atten * 2, _ShadowStrength);
	#else
	c.rgb *= 2*atten;
	#endif
	return c;
}
#endif
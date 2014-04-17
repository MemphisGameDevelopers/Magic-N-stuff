#ifndef TREE_CG_INCLUDED
#define TREE_CG_INCLUDED

#include "TerrainEngine.cginc"

fixed4 _Color;
fixed3 _TranslucencyColor;
fixed _TranslucencyViewDependency;
half _ShadowStrength;


struct LeafSurfaceOutput {
	fixed3 Albedo;
	fixed3 Normal;
	fixed3 Emission;
	fixed Translucency;
	half Specular;
	fixed Gloss;
	fixed Alpha;
	fixed ShadowCutOff;
};

inline half4 LightingTreeLeaf (LeafSurfaceOutput s, half3 lightDir, half3 viewDir, fixed atten)
{
	half3 h = normalize (lightDir + viewDir);
	half nl = dot (s.Normal, lightDir);
	half nh = max (0, dot (s.Normal, h));
	// fading in spec value makes sense for shaded trees – but we keep going with the standard...
	half spec = pow (nh, s.Specular * 128.0) * s.Gloss; // * s.ShadowCutOff; //_SquashAmount; // fade out spec
	// view dependent back contribution for translucency
	fixed backContrib = saturate(dot(viewDir, -lightDir));
	// normally translucency is more like -nl, but looks better when it's view dependent
	backContrib = lerp(saturate(-nl), backContrib, _TranslucencyViewDependency);
	fixed3 translucencyColor = backContrib * s.Translucency * _TranslucencyColor;
	// wrap-around diffuse
	nl = max(0, nl * 0.6 + 0.4);
	fixed4 c;
	c.rgb = s.Albedo * (translucencyColor * 2 + nl);
	c.rgb = c.rgb * _LightColor0.rgb + spec;
	// For directional lights, apply less shadow attenuation
	// based on shadow strength parameter.
	#if defined(DIRECTIONAL) || defined(DIRECTIONAL_COOKIE)
	// fade in real time shadows
	atten = lerp(fixed(1), atten, 1 - (1 - s.ShadowCutOff)*(1 - s.ShadowCutOff) );
	c.rgb *= lerp(2, atten * 2, _ShadowStrength );
	#else
	// fade in point and spot lights
	//atten = lerp(fixed(0), 2*atten, _SquashAmount);
	atten = lerp(fixed(0), 2 * atten, s.ShadowCutOff);

	c.rgb *= atten;
	#endif
	
	return c;
}

inline fixed4 LightingTreeBark (SurfaceOutput s, fixed3 lightDir, half3 viewDir, fixed atten)
{
	half3 h = normalize (lightDir + viewDir);
	fixed diff = max (0, dot (s.Normal, lightDir));
	float nh = max (0, dot (s.Normal, h));
	// fading in spec value makes sense for shaded trees – but we keep going with the standard...
	float spec = pow (nh, s.Specular*128.0) * s.Gloss; // * s.Alpha; // _SquashAmount; // fade out spec
	fixed4 c;
	#if defined(DIRECTIONAL) || defined(DIRECTIONAL_COOKIE)
	// fade in real time shadows
	atten = lerp(fixed(1), atten, 1 - (1 - s.Alpha)*(1 - s.Alpha) );
	#else
	// fade in point and spot lights
	//atten = lerp(fixed(0), atten, _SquashAmount);
	atten = lerp(fixed(0), atten, s.Alpha);
	#endif
	c.rgb = (s.Albedo * _LightColor0.rgb * diff + _LightColor0.rgb * _SpecColor.rgb * spec) * (atten * 2);
	// c.a = s.Alpha + _LightColor0.a * _SpecColor.a * spec * atten;
	// Alpha is always 1
	c.a = 1.0 + _LightColor0.a * _SpecColor.a * spec * atten;
	return c;
}
#endif
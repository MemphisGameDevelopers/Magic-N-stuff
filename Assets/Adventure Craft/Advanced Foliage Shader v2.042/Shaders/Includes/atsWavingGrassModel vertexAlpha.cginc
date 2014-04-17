float4 _AfsWaveAndDistance;
float4 _AfsWavingTint;

void AfsWavingGrassVert (inout appdata_full v) {
	////////// start bending

	// doing the animation in worldspace will give us less contrast between manually placed grass
	// and grass within the terrain engine
	float4 worldPos = mul(_Object2World, v.vertex);

	float4 _waveXSizeMove = float4(0.048, 0.06, 0.24, 0.096) * _AfsWaveAndDistance.y;
	float4 _waveZSizeMove = float4 (0.024, .08, 0.08, 0.2) * _AfsWaveAndDistance.y;
	float4 waveSpeed = float4 (1.2, 2, 1.6, 4.8);
	float4 waves;
	
	waves = worldPos.x * _waveXSizeMove;
	waves += worldPos.z * _waveZSizeMove;
	_waveXSizeMove = float4(0.024, 0.04, -0.12, 0.096);
	_waveZSizeMove = float4 (0.006, .02, -0.02, 0.1);
	
	// Add in time to model them over time
	waves += _AfsWaveAndDistance.x * waveSpeed;
	float4 s, c;
	waves = frac (waves);
	FastSinCos (waves, s,c);
	float waveAmount = v.color.a * _AfsWaveAndDistance.z;

	// Faster winds move the grass more than slow winds 
	s *= normalize (waveSpeed);
	s = s * s;

	float lighting = dot (s, normalize (float4 (1,1,.4,.2))) * .7;
	s *= waveAmount;
	
	#ifdef GRASS_ANIMATE_COLOR
	fixed3 waveColor = lerp (fixed3(0.5,0.5,0.5), _AfsWavingTint.rgb, lighting);
	v.color.rgb = v.color.rgb * waveColor * 2;
	#endif

	float3 waveMove = float3 (0,0,0);
	waveMove.x = dot (s, _waveXSizeMove);
	waveMove.z = dot (s, _waveZSizeMove);
	worldPos.xz -= waveMove.xz * 8;
	
	#ifdef GRASS_ANIMATE_NORMAL
	v.normal.xyz = normalize(v.normal.xyz - waveMove.xyz * lighting);
	#endif
	
	////////// end bending
	
	// Fade the grass out before detail distance.
	// Saturate because Radeon HD drivers on OS X 10.4.10 don't saturate vertex colors properly.
	float3 offset = worldPos.xyz - _WorldSpaceCameraPos;
	v.color.a = saturate (2 * (_AfsWaveAndDistance.w - dot (offset, offset)) * ( 1 / _AfsWaveAndDistance.w) );

	v.vertex.xz = mul(_World2Object, worldPos).xz;

}

void AfsWavingGrassVertTrans (inout appdata_full v) {
	////////// start bending
	
	// doing the animation in worldspace will give us less contrast between manually placed grass
	// and grass within the terrain engine
	float4 worldPos = mul(_Object2World, v.vertex);
	
	float4 _waveXSizeMove = float4(0.048, 0.06, 0.24, 0.096) * _AfsWaveAndDistance.y;
	float4 _waveZSizeMove = float4 (0.024, .08, 0.08, 0.2) * _AfsWaveAndDistance.y;
	float4 waveSpeed = float4 (1.2, 2, 1.6, 4.8);
	float4 waves;
	
	
	waves = worldPos.x * _waveXSizeMove;
	waves += worldPos.z * _waveZSizeMove;
	_waveXSizeMove = float4(0.024, 0.04, -0.12, 0.096);
	_waveZSizeMove = float4 (0.006, .02, -0.02, 0.1);
	
	// Add in time to model them over time
	waves += _AfsWaveAndDistance.x * waveSpeed;
	float4 s, c;
	waves = frac (waves);
	FastSinCos (waves, s,c);
	float waveAmount = v.color.a * _AfsWaveAndDistance.z;

	// Faster winds move the grass more than slow winds 
	s *= normalize (waveSpeed);
	s = s * s;
	
	float lighting = dot (s, normalize (float4 (1,1,.4,.2))) * .7;
	s *= waveAmount;
	
	#ifdef GRASS_ANIMATE_COLOR
	fixed3 waveColor = lerp (fixed3(0.5,0.5,0.5), _AfsWavingTint.rgb, lighting);
	v.color.rgb = v.color.rgb * waveColor * 2;
	#endif

	float3 waveMove = float3 (0,0,0);
	waveMove.x = dot (s, _waveXSizeMove);
	waveMove.z = dot (s, _waveZSizeMove);
	
	worldPos.xz -= waveMove.xz * 8;
	
	#ifdef GRASS_ANIMATE_NORMAL
	v.normal.xyz = normalize(v.normal.xyz - waveMove.xyz * lighting);
	#endif
	
	////////// end bending
	
	v.vertex.xz = mul(_World2Object, worldPos).xz * unity_Scale.w;
	
	////////// decode compressed ground normal
	//	float3 tempnormal;
	//	tempnormal.xz = v.color.rg;
	//	tempnormal.y = sqrt(1 - dot(tempnormal.xz, tempnormal.xz));
	
	// so lets render some kind of per vertex lighting:
	//	float light = dot (tempnormal, WorldSpaceLightDir(v.vertex) );
	//	v.color.a = light;
}
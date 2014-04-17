float4 _AfsTerrainTrees;
float4 _AfsBillboardCameraForward;

inline float4 AfsSquash(in float4 pos)
{
	float3 planeNormal = _SquashPlaneNormal.xyz;
	float3 projectedVertex = pos.xyz - (dot(planeNormal, pos) + _SquashPlaneNormal.w) * planeNormal + _AfsBillboardCameraForward.xyz * 1.6 * pos.y;
	pos = float4(lerp(projectedVertex, pos.xyz, _SquashAmount), 1);
	
	return pos;
}

inline float4 AfsSquashNew(in float4 pos, float SquashNew)
{
	float3 planeNormal = _SquashPlaneNormal.xyz;
	float3 projectedVertex = pos.xyz - (dot(planeNormal, pos) + _SquashPlaneNormal.w) * planeNormal + _AfsBillboardCameraForward.xyz * 1.6 * pos.y;
	pos = float4(lerp(projectedVertex, pos.xyz, SquashNew), 1);
	return pos;
}

inline float4 MyAnimateVertexTreeTerrain(float4 pos, float3 normal, float4 animParams, float4 _myworldPos)
{	
//	fade in wind
	float _SquashTwo = _myworldPos.w * _myworldPos.w; //_SquashAmount * _SquashAmount;
	float _Wind_w = _Wind.w * _SquashTwo;

//	original wind bending parameters
	float fDetailAmp = 0.1f;
	float fBranchAmp = 0.3f;
	
//	Phases (object, vertex, branch)
	//float fObjPhase = dot(_Object2World[3].xyz, 1);
	float fObjPhase = dot(_myworldPos.xyz, 1);
	float fBranchPhase = fObjPhase + animParams.x;
	float fVtxPhase = dot(pos.xyz, animParams.y + fBranchPhase);
//	x is used for edges; y is used for branches
	float2 vWavesIn = _Time.yy + float2(fVtxPhase, fBranchPhase);
	// 1.975, 0.793, 0.375, 0.193 are good frequencies
	float4 vWaves = (frac( vWavesIn.xxyy * float4(1.975, 0.793, 0.375, 0.193) ) * 2.0 - 1.0);
	vWaves = SmoothTriangleWave( vWaves );
	float2 vWavesSum = vWaves.xz + vWaves.yw;
//	Edge (xz) and branch bending (y)
	float3 bend = animParams.y * fDetailAmp * normal.xyz * sign(normal.xyz);
	bend.y = animParams.w * fBranchAmp;
//	Secondary bending
	pos.xyz += ((vWavesSum.xyx * bend) + (_Wind.xyz * vWavesSum.y * animParams.w)) * _Wind_w;
//	Primary bending
	pos.xyz += animParams.z * _Wind.xyz * _SquashTwo;
	return pos;
}

void MyTerrainTreeVertLeaf (inout appdata_full v)
{
//	get pivot!
	float4 myworldPos = mul(_Object2World, float4(0,0,0,1) );
	ExpandBillboard (UNITY_MATRIX_IT_MV, v.vertex, v.normal, v.tangent);
	v.vertex.xyz *= _Scale.xyz;
//	calculate fadeState early as it might be needed by the wind aniamtion
	
	float fadeState = saturate( ( _AfsTerrainTrees.x - distance(_WorldSpaceCameraPos, myworldPos)) / _AfsTerrainTrees.y );
	myworldPos.w = clamp(fadeState * _SquashAmount, 0.01, 1.0); // must be greater 0.0 (unity 4.3)
//	animate vertices
	v.vertex = MyAnimateVertexTreeTerrain (v.vertex, v.normal, float4(v.color.xy, v.texcoord1.xy), myworldPos);
//	call Squash
	v.vertex = AfsSquashNew(v.vertex, myworldPos.w);
	v.normal = normalize(v.normal);
	v.tangent.xyz = normalize(v.tangent.xyz);
	
	v.color.b = fadeState;
//	calculate fadeState 2
	#ifdef BILLBOARD_SHADOWS
	v.color.r = _SquashAmount * saturate( ( _AfsTerrainTrees.x - _AfsTerrainTrees.y - distance(_WorldSpaceCameraPos, myworldPos)) / _AfsTerrainTrees.y );
	#endif
}

void MyTerrainTreeVertLeafShadowCaster (inout appdata_full v)
{
//	get pivot!
	float4 myworldPos = mul(_Object2World, float4(0,0,0,1) );
	ExpandBillboard (UNITY_MATRIX_IT_MV, v.vertex, v.normal, v.tangent);
	v.vertex.xyz *= _Scale.xyz;
//	calculate fadeState early as it might be needed by the wind aniamtion
	float fadeState = saturate( ( _AfsTerrainTrees.x - distance(_WorldSpaceCameraPos, myworldPos)) / _AfsTerrainTrees.y );
	myworldPos.w = clamp(fadeState * _SquashAmount, 0.01, 1.0); // must be greater 0.0 (unity 4.3)
//	animate vertices
	v.vertex = MyAnimateVertexTreeTerrain (v.vertex, v.normal, float4(v.color.xy, v.texcoord1.xy), myworldPos);
//	we do not have to call Squash: if Squash -> no shadows
	//v.vertex = AfsSquashNew(v.vertex, myworldPos.w);
	//v.normal = normalize(v.normal);
	//v.tangent.xyz = normalize(v.tangent.xyz);
	
	v.color.b = fadeState;
}

void MyTerrainTreeVertBark (inout appdata_full v)
{
//	get pivot!
	float4 myworldPos = mul(_Object2World, float4(0,0,0,1) );
	v.vertex.xyz *= _Scale.xyz;
//	calculate fadeState early as it might be needed by the wind aniamtion
	float fadeState = saturate( ( _AfsTerrainTrees.x - distance(_WorldSpaceCameraPos, myworldPos)) / _AfsTerrainTrees.y );
	myworldPos.w = clamp(fadeState * _SquashAmount, 0.01, 1.0); // must be greater 0.0 (unity 4.3)
//	animate vertices	
	v.vertex = MyAnimateVertexTreeTerrain (v.vertex, v.normal, float4(v.color.xy, v.texcoord1.xy), myworldPos);
//	call Squash
	v.vertex = AfsSquashNew(v.vertex, myworldPos.w);
	v.normal = normalize(v.normal);
	v.tangent.xyz = normalize(v.tangent.xyz);
	
	v.color.b = fadeState;
//	calculate fadeState 2
	#ifdef BILLBOARD_SHADOWS
	v.color.r = _SquashAmount * saturate( ( _AfsTerrainTrees.x - _AfsTerrainTrees.y - distance(_WorldSpaceCameraPos, myworldPos)) / _AfsTerrainTrees.y );
	#endif
}

void MyTerrainTreeVertBarkShadowCaster (inout appdata_full v)
{
//	get pivot!
	float4 myworldPos = mul(_Object2World, float4(0,0,0,1) );
	v.vertex.xyz *= _Scale.xyz;
//	calculate fadeState early as it might be needed by the wind aniamtion
	float fadeState = saturate( ( _AfsTerrainTrees.x - distance(_WorldSpaceCameraPos, myworldPos)) / _AfsTerrainTrees.y );
	myworldPos.w = clamp(fadeState * _SquashAmount, 0.01, 1.0); // must be greater 0.0 (unity 4.3)
	v.vertex = MyAnimateVertexTreeTerrain (v.vertex, v.normal, float4(v.color.xy, v.texcoord1.xy), myworldPos);
//	we do not have to call Squash: if Squash -> no shadows
	//v.normal = normalize(v.normal);
	//v.tangent.xyz = normalize(v.tangent.xyz);
	v.color.b = fadeState;
}

void SimpleTreeTerrain (inout appdata_full v)
{
//	get pivot!
	float4 myworldPos = mul(_Object2World, float4(0,0,0,1) );
	v.vertex.xyz *= _Scale.xyz;
//	calculate fadeState early as it might be needed by the wind aniamtion
	float fadeState = saturate( ( _AfsTerrainTrees.x - distance(_WorldSpaceCameraPos, myworldPos)) / _AfsTerrainTrees.y );
	myworldPos.w = clamp(fadeState * _SquashAmount, 0.01, 1.0); // must be greater 0.0 (unity 4.3)
//	animate vertices
	v.vertex = MyAnimateVertexTreeTerrain (v.vertex, v.normal, float4(v.color.xyzz), myworldPos);
//	call Squash
	v.vertex = AfsSquashNew(v.vertex, myworldPos.w);
	v.normal = normalize(v.normal);
	v.tangent.xyz = normalize(v.tangent.xyz);
//	Store fadeState
	v.color.b = fadeState;
//	calculate fadeState 2
	#ifdef BILLBOARD_SHADOWS
	v.color.r = _SquashAmount * saturate( ( _AfsTerrainTrees.x - _AfsTerrainTrees.y - distance(_WorldSpaceCameraPos, myworldPos)) / _AfsTerrainTrees.y );
	#endif
}

void SimpleTreeTerrainShadowCaster (inout appdata_full v)
{
//	get pivot!
	float4 myworldPos = mul(_Object2World, float4(0,0,0,1) );
	v.vertex.xyz *= _Scale.xyz;
//	calculate fadeState early as it might be needed by the wind aniamtion
	float fadeState = saturate( ( _AfsTerrainTrees.x - distance(_WorldSpaceCameraPos, myworldPos)) / _AfsTerrainTrees.y );
	myworldPos.w = clamp(fadeState * _SquashAmount, 0.01, 1.0); // must be greater 0.0 (unity 4.3)
//	animate vertices
	v.vertex = MyAnimateVertexTreeTerrain (v.vertex, v.normal, float4(v.color.xyzz), myworldPos);
//	we do not have to call Squash: if Squash -> no shadows
	//v.normal = normalize(v.normal);
	//v.tangent.xyz = normalize(v.tangent.xyz);
	v.color.b = fadeState;
}

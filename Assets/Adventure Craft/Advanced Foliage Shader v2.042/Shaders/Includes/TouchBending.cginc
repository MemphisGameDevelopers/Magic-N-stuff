// Detail bending

float _AfsWaveSize;

float4 _TouchBendingPosition;
float4 _TouchBendingForce;

float4x4 _RotMatrix;

void MyFastSin (float2 val, out float2 s) {
	val = val * 6.408849 - 3.1415927;
	// powers for taylor series
	float2 r5 = val * val;		// wavevec ^ 2
	float2 r6 = r5 * r5;		// wavevec ^ 4;
	float2 r7 = r6 * r5;		// wavevec ^ 6;
	float2 r8 = r6 * r5;		// wavevec ^ 8;

	float2 r1 = r5 * val;		// wavevec ^ 3
	float2 r2 = r1 * r5;		// wavevec ^ 5;
	float2 r3 = r2 * r5;		// wavevec ^ 7;

	//Vectors for taylor's series expansion of sin
	float4 sin7 = {1, -0.16161616, 0.0083333, -0.00019841};
	// sin
	s =  val + r1 * sin7.y + r2 * sin7.z + r3 * sin7.w;
}

inline float4 TouchAnimateVertex(float4 pos, float3 normal, float4 animParams)
{	
	// animParams stored in color
	// animParams.r = red = branch phase
	// animParams.g = green = edge flutter factor
	// animParams.b = blue = primary factor
	
	float3 myworldPos = mul(_Object2World, pos).xyz;
	
//	bring wind into local space (needed if static batching is disabled)
	// would be correct but produces shadow acne
	//_Wind.xyz = mul(UNITY_MATRIX_IT_MV, _Wind).xyz;
	
// original wind bending ////////////////	
	float fDetailAmp = 0.1; // changelog: was 0.2
	float fBranchAmp = 0.15; // changelog: was: 0.25
	
	// Phases (object, vertex, branch)
	//float fObjPhase = (myworldPos.x + myworldPos.z) * _AfsWaveSize; // changelog: was "* 0.5"
	
	float2 fObjPhase;
	float2 _sin = float2 ( frac( (myworldPos.x + myworldPos.z) * _AfsWaveSize ), frac( (myworldPos.x + myworldPos.z + _Time.y) * _AfsWaveSize ));
	MyFastSin ( _sin, fObjPhase);

	
	float fBranchPhase = fObjPhase + animParams.x; //---> fObjPhase + vertex color red
	float fVtxPhase = dot(myworldPos.xyz, animParams.yyy + fBranchPhase.xxx); // controled by vertex color green
	
	// x is used for edges; y is used for branches
	float2 vWavesIn = _Time.yy + float2(fVtxPhase, fBranchPhase);
	// 1.975, 0.793, 0.375, 0.193 are good frequencies
	//float4 vWaves = (frac( vWavesIn.xxyy * float4(1.975, 0.793, 0.375, 0.193) ) * 2.0 - 1.0);
	float4 vWaves = (frac( vWavesIn.xxyy * float4(1.975, 0.793, 0.375, 0.285) ) * 2.0 - 1.0);
	vWaves = SmoothTriangleWave( vWaves );
	float2 vWavesSum = vWaves.xz + vWaves.yw;
	// Edge (xz) and branch bending (y)
	// sign important to match normals of both faces!!! otherwise edge fluttering might be corrupted.
	float3 bend = animParams.g * fDetailAmp * normal.xyz * sign(normal.xyz); // controlled by vertex green
	bend.y = animParams.b * fBranchAmp; // controlled by vertex blue

//	Secondary bending
	pos.xyz += ((vWavesSum.xyx * bend) + (_Wind.xyz * vWavesSum.y * animParams.b)) * _Wind.w; // controlled by vertex blue

//	add variety
	//_Wind.xyz *= ( 0.25 + sin( (pos.x + pos.z + _Time.y) * _AfsWaveSize ) );
	// sin already stored in fObjPhase.y
//	Primary bending
	pos.xyz += animParams.b * _Wind.xyz * _Wind.w * ( 0.25 + fObjPhase.y);

//	touch bending
	// Primary displacement by touch bending
	pos.xyz += animParams.b * _TouchBendingForce.xyz * _TouchBendingForce.w;
	
	// Touch rotation
	pos.xyz = lerp( pos.xyz, mul(_RotMatrix, float4(pos.xyz - _TouchBendingPosition.xyz, 0)).xyz + _TouchBendingPosition.xyz, animParams.b * 10 * (1 + animParams.r ) );
	return pos;	
}


void TouchBending (inout appdata_full v)
{
	v.vertex = TouchAnimateVertex (v.vertex, v.normal, float4(v.color.xyzz));
	v.normal = normalize(v.normal);
	v.tangent.xyz = normalize(v.tangent.xyz);
}

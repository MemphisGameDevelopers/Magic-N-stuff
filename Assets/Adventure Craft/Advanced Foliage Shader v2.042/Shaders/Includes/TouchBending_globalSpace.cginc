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
	// animParams.x = red = branch phase
	// animParams.y = green = edge flutter factor
	// animParams.z = blue = primary factor
	// animParams.w = blue = secondary factor
	
// fix for unity 4.2: all computation has to be done in worldspace!
	pos.xyz = mul( _Object2World, pos).xyz;
	// only needed by forward rendering and dynamic batching? – nope: the problem is the "scale" of the normal
	// normal.xyz = mul( _Object2World, float4(normal, 0.0) ).xyz;
	
//	based on original wind bending
	float fDetailAmp = 0.1; // changelog: was 0.2
	float fBranchAmp = 0.15; // changelog: was: 0.25

//	Phases (object, vertex, branch)
//	float fObjPhase = sin( (pos.x + pos.z) * _AfsWaveSize );
	float2 fObjPhase;
	float2 sin = float2 ( frac( (pos.x + pos.z) * _AfsWaveSize ), frac( (pos.x + pos.z + _Time.y) * _AfsWaveSize ));
	MyFastSin ( sin, fObjPhase);
	
	float fBranchPhase = fObjPhase.x + animParams.x; //---> fObjPhase + vertex color red
	float fVtxPhase = dot(pos.xyz, animParams.y + fBranchPhase); // controled by vertex color green
	
	// x is used for edges; y is used for branches
	float2 vWavesIn = _Time.yy + float2(fVtxPhase, fBranchPhase);
	// 1.975, 0.793, 0.375, 0.193 are good frequencies
	//float4 vWaves = (frac( vWavesIn.xxyy * float4(1.975, 0.793, 0.375, 0.193) ) * 2.0 - 1.0);
	float4 vWaves = (frac( vWavesIn.xxyy * float4(1.975, 0.793, 0.375, 0.285) ) * 2.0 - 1.0);
	vWaves = SmoothTriangleWave( vWaves );
	float2 vWavesSum = vWaves.xz + vWaves.yw;

//	Edge (xz) controlled by vertex green and branch bending (y) controled by vertex blue
	// sign important to match normals of both faces!!! otherwise edge fluttering might be corrupted.
	float3 bend = animParams.g * fDetailAmp * normal.xyz * sign(normal.xyz); // * unity_Scale.w;
	bend.y = animParams.b * fBranchAmp;
//	Secondary bending / controlled by vertex blue
	pos.xyz += ( ((vWavesSum.xyx * bend) + (_Wind.xyz * vWavesSum.y * animParams.b)) * _Wind.w );

//	add variety
	//_Wind.xyz *= ( 0.25 + sin( (pos.x + pos.z + _Time.y) * _AfsWaveSize ) );
	// sin already stored in fObjPhase.y
//	Primary bending
	pos.xyz += animParams.b * _Wind.xyz * _Wind.w * ( 0.25 + fObjPhase.y);

// touch bending
	// Primary displacement by touch bending
	pos.xyz += animParams.b * _TouchBendingForce.xyz * _TouchBendingForce.w;
	
	// Touch rotation
	pos.xyz = lerp( pos.xyz, mul(_RotMatrix, float4(pos.xyz - _TouchBendingPosition.xyz, 0)).xyz + _TouchBendingPosition.xyz, animParams.b * 10 * (1 + animParams.r ) );
	
//	bring pos back to local space and scale	
	pos.xyz = mul( _World2Object, pos).xyz * unity_Scale.w;
	return pos;	
}


void TouchBending (inout appdata_full v)
{
	v.vertex = TouchAnimateVertex (v.vertex, v.normal, float4(v.color.xyzz));
	v.normal = normalize(v.normal);
	v.tangent.xyz = normalize(v.tangent.xyz);
}

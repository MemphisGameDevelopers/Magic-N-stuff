// Detail bending
// Use this include when compiling for forward rendering

// fix for unity 4.2: all computation has to be done in worldspace due to dynamic shadow collector/cater batching
// vertex.pos has to be brought to worldspace
// vertex normal has to be brought to worldspace
// final result has to be scaled by unity_Scale.w

float _AfsWaveSize;
float _GroundLightingAttunation;


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


inline float4 MyAnimateVertex (float4 pos, float3 normal, float4 animParams)
{	
	// animParams stored in color
	// animParams.r = red = branch phase
	// animParams.g = green = edge flutter factor
	// animParams.b = blue = primary factor
	
// fix for unity 4.2: all computation has to be done in worldspace!
	pos.xyz = mul( _Object2World, pos).xyz;
	// only needed by forward rendering and dynamic batching? – nope. problem is the "scaled" normal: it should be normal * 1.3 but only in the shadow passes
	// normal.xyz = mul( _Object2World, float4(normal, 0.0) ).xyz;
	
//	based on original wind bending
	float fDetailAmp = 0.1; // changelog: was 0.2
	float fBranchAmp = 0.15; // changelog: was: 0.25

//	Phases (object, vertex, branch)
	//float fObjPhase = sin( (pos.x + pos.z) * _AfsWaveSize );
	float2 fObjPhase;
	float2 _sin = float2 ( frac( (pos.x + pos.z) * _AfsWaveSize ), frac( (pos.x + pos.z + _Time.y) * _AfsWaveSize ));
	MyFastSin ( _sin, fObjPhase);

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
	float3 bend = animParams.g * fDetailAmp * normal.xyz * sign(normal.xyz);
	bend.y = animParams.b * fBranchAmp;
//	Secondary bending / controlled by vertex blue
	pos.xyz += ( ((vWavesSum.xyx * bend) + (_Wind.xyz * vWavesSum.y * animParams.b)) * _Wind.w );

//	add variety
	//_Wind.xyz *= ( 0.25 + sin( (pos.x + pos.z + _Time.y) * _AfsWaveSize ) );
	// sin already stored in fObjPhase.y
//	Primary bending
	pos.xyz += animParams.b * _Wind.xyz * _Wind.w * ( 0.25 + fObjPhase.y);
	
//	bring pos back to local space and scale
	pos.xyz = mul( _World2Object, pos).xyz * unity_Scale.w;
	return pos;	
}

void CustomBending (inout appdata_full v)
{
	// decode scale from alpha channel
	//float bendFactor = ( 32 * fmod(v.color.a * 255.0, 4.0)) / 255;
	//bendFactor *= bendFactor * 1.25 + 0.1;
	//v.vertex = MyAnimateVertex (v.vertex, v.normal, float4(v.color.xyzz), bendFactor);
	
	v.vertex = MyAnimateVertex (v.vertex, v.normal, float4(v.color.xyzz));
	v.normal = normalize(v.normal);
	v.tangent.xyz = normalize(v.tangent.xyz);
	
}

void CustomBendingGroundlighting (inout appdata_full v)
{
	// decode scale from alpha channel
	//float bendFactor = ( 32 * fmod(v.color.a * 255.0, 4.0)) / 255;
	//bendFactor *= bendFactor * 1.25 + 0.1;
	//v.vertex = MyAnimateVertex (v.vertex, v.normal, float4(v.color.xyzz), bendFactor);

	v.vertex = MyAnimateVertex (v.vertex, v.normal, float4(v.color.xyzz));
	v.normal = normalize(v.normal);
	v.tangent.xyz = normalize(v.tangent.xyz);
	
	// decode compressed ground normal
	float3 tempnormal;
	tempnormal.xz = v.texcoord1.xy;
	tempnormal.y = sqrt(1 - dot(tempnormal.xz, tempnormal.xz));
	// this would breaks spec lighting and translcency:
	// v.normal = lerp(v.normal, tempnormal, 0.95);
	
	// so lets render some kind of per vertex lighting:
	float light = dot (tempnormal, WorldSpaceLightDir(v.vertex) );
	//v.color.r = light; // simple term
	// lets take vertex.color.blue into account which should store the distance to ground – somehow
	v.color.r = saturate (light + ( 1 - light) * v.color.b * _GroundLightingAttunation);
}

void CustomBendingTreeTerrain (inout appdata_full v)
{
	// decode scale from alpha channel
	//float bendFactor = ( 32 * fmod(v.color.a * 255.0, 4.0)) / 255;
	//bendFactor *= bendFactor * 1.25 + 0.1;
	//v.vertex = MyAnimateVertex (v.vertex, v.normal, float4(v.color.xyzz), bendFactor);
	
	v.vertex.xyz *= _Scale.xyz;
	v.vertex = AnimateVertex (v.vertex, v.normal, float4(v.color.xyzz));
	v.vertex = Squash(v.vertex);
	v.normal = normalize(v.normal);
	v.tangent.xyz = normalize(v.tangent.xyz);
	
}

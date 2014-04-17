float4 _AfsBillboardCameraForward;
//
float4 _AfsSunDirection;

void AfsTerrainBillboardTree( inout float4 pos, float2 offset, float offsetz )
{
//	not needed here as we fade out the billboards
	//float3 treePos = pos.xyz - _TreeBillboardCameraPos.xyz;
	//float treeDistanceSqr = dot(treePos, treePos);
	//if( treeDistanceSqr > _TreeBillboardDistances.x )
	//	offset.xy = offsetz = 0.0;
		
//	positioning of billboard vertices horizontaly
	pos.xyz += _TreeBillboardCameraRight.xyz * offset.x;

	// _TreeBillboardCameraPos.w contains ImposterRenderTexture::billboardAngleFactor
	float billboardAngleFactor = _TreeBillboardCameraPos.w;
	
//
//		
//	this fixex the trunk on the terrain and aligns the billboard to the camera
	float copyoffset_y_factor = saturate ( floor(offset.y + 0.5));
	float copyoffset_y = offset.y * copyoffset_y_factor - 12.0 * billboardAngleFactor * (1 - copyoffset_y_factor);
	pos.xyz += _AfsBillboardCameraForward.xyz * copyoffset_y * (1.6 + billboardAngleFactor );
//	

	// The following line performs two things:
	// 1) peform non-uniform scale, see "3) incorrect compensating (using lerp)" above
	// 2) blend between vertical and horizontal billboard mode
	
	float radius = lerp(offset.y, offsetz, billboardAngleFactor);	
	// positioning of billboard vertices verticaly
	pos.xyz += _TreeBillboardCameraUp.xyz * radius;
		// would nearly do the trick...
		// pos.xyz += _TreeBillboardCameraUp.zyx * radius;
	
	// _TreeBillboardCameraUp.w contains ImposterRenderTexture::billboardOffsetFactor
	float billboardOffsetFactor = _TreeBillboardCameraUp.w;
	// Offsetting billboad from the ground, so it doesn't get clipped by ztest.
	// In theory we should use billboardCenterOffsetY instead of offset.x,
	// but we can't because offset.y is not the same for all 4 vertices, so 
	// we use offset.x which is the same for all 4 vertices (except sign). 
	// And it doesn't matter a lot how much we offset, we just need to offset 
	// it by some distance
	pos.xyz += _TreeBillboardCameraFront.xyz * abs(offset.x) * billboardOffsetFactor;	
}

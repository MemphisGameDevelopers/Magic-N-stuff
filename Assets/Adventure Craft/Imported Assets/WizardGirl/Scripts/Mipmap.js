var mipmap : float = -0.5;
function Start () {
	for(var i = 0; i < renderer.sharedMaterials.Length; i++){
		renderer.sharedMaterials[i].mainTexture.mipMapBias = mipmap;
		var bumpMapTexture : Texture = renderer.sharedMaterials[i].GetTexture("_BumpMap");
		bumpMapTexture.mipMapBias = mipmap;
		renderer.sharedMaterials[i].SetTexture("_BumpMap", bumpMapTexture);
	}
	//renderer.material.mainTexture.mipMapBias = mipmap;
}
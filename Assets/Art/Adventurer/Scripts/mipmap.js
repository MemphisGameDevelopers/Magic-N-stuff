var mipmap : float = -0.5;
function Start () {
	renderer.material.mainTexture.mipMapBias = mipmap;
}
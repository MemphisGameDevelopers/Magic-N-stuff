using UnityEngine;
using System.Collections;

[System.Serializable]
public class AABB {

	public XY center;
	public XY half;
	
	public AABB () : this (new XY(0,0), new XY(0,0)) // Constructor
	{
		
	}
	
	public AABB (XY _center, XY _half) // Constructor
	{
		center = _center;
		half = _half;
	}
	
	// Helper Methods
	public bool ContainsPoint(XY p)
	{
		if (p.x > center.x + half.x) return false;
		if (p.y > center.y + half.y) return false;
		if (p.x < center.x - half.x) return false;
		if (p.y < center.y - half.y) return false;
		return true;
	}
	
	public bool Intersects(AABB other)
	{
		bool _x = false;
		bool _y = false;

		XY distance = other.center - center;
		if (Mathf.Abs(distance.x) < (other.half.x + half.x)) _x = true;
		if (Mathf.Abs(distance.y) < (other.half.y + half.y)) _y = true;
		return _x&&_y;
	}
	
	public XY TopLeft()
	{
		return new XY(center.x - half.x, center.y + half.y);
	}
	
	public XY TopRight()
	{
		return new XY(center.x + half.x, center.y + half.y);
	}
	
	public XY BotLeft()
	{
		return new XY(center.x - half.x, center.y - half.y);
	}
	
	public XY BotRight()
	{
		return new XY(center.x + half.x, center.y - half.y);
	}
	
	public float Left()
	{
		return center.x - half.x;
	}
	
	public float Right()
	{
		return center.x + half.x;
	}
	
	public float Top()
	{
		return center.y + half.y;
	}
	
	public float Bottom()
	{
		return center.y - half.y;
	}
	
	public int LeftTile()
	{
		return Mathf.RoundToInt(Left ());
	}
	
	public int RightTile()
	{
		return Mathf.RoundToInt(Right ());
	}
	
	public int TopTile()
	{
		return Mathf.RoundToInt(Top ());
	}
	
	public int BottomTile()
	{
		return Mathf.RoundToInt(Bottom ());
	}
	
	public XY Size()
	{
		return new XY(half.x*2,half.y*2);
	}
}

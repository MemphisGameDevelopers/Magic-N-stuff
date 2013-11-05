using UnityEngine;
using System.Collections;

[System.Serializable]
public class XY {

	public float x;
	public float y;
	
	public XY () : this (0,0) // Constructor
	{
		
	}
	
	public XY (float _x, float _y) // Constructor
	{
		x = _x;
		y = _y;
	}
	
	// Helper Methods
	public static XY operator+(XY a, XY b)
	{
		return new XY(a.x+b.x, a.y+b.y);
	}
	
	public static XY operator-(XY a, XY b)
	{
		return new XY(a.x-b.x, a.y-b.y);
	}
	
	public static XY operator/(XY a, float b)
	{
		return new XY(a.x/b, a.y/b);
	}
	
	public static XY operator*(XY a, float b)
	{
		return new XY(a.x*b, a.y*b);
	}
	
	public static XY Round(XY xy) 
	{
		return new XY(Mathf.RoundToInt(xy.x), Mathf.RoundToInt(xy.y));	
	}
	
	public static float Distance(XY a, XY b)
	{
		float _x = b.x - a.x;
		float _y = b.y - a.y;
		return Mathf.Sqrt (_x*_x + _y*_y);
	}
}

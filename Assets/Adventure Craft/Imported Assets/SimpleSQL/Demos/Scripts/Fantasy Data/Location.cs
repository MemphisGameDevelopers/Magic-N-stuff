using System.Collections.Generic;
using SimpleSQL;

public class Location
{
	[PrimaryKey]
	public int LocationID { get; set; }
	
	public string LocationName { get; set; }
	
	// this isn't pulled directly from location database, but is built up
	// through a query in the 'Complex Query' example
	public List<Location> AdjacentLocations { get; set; }
	
	public Location()
	{
		AdjacentLocations = new List<Location>();
	}
}

using SimpleSQL;

public class StarShip
{
	// StarShipID is the primary key, which automatically gets the NotNull attribute
	[PrimaryKey]
	public int StarShipID { get; set; }
	
	// The starship name will have an index created in the database.
	// It's max length is set to 60 characters.
	// The name cannot be null.
	[Indexed, MaxLength(60), NotNull]
	public string StarShipName { get; set; }
	
	// The home planet name's maximum length is set to 100 characters.
	// The default value is set to Earth
	[MaxLength(100), Default("Earth")]
	public string HomePlanet { get; set; }
	
	// The range cannot be null.
	[NotNull]
	public float Range { get; set; }
	
	// The armor's default value is set to 120
	[Default(120.0f)]
	public float Armor { get; set; }
	
	// Firepower has no restrictions
	public float Firepower { get; set; }
}

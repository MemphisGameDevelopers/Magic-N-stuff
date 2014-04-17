using SimpleSQL;

public class PlayerStats
{
	// The PlayerID is the primary key and also autoincrements itself
	// the SQLite database so we reflect that here with these attributes.
	[PrimaryKey, AutoIncrement]
	public int PlayerID { get; set; }

	public string PlayerName { get; set; }
	
	public int TotalKills { get; set; } 
	
	public int Points { get; set; } 
}

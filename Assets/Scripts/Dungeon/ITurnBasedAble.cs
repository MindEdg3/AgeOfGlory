/// <summary>
/// Interface for objects that should have a turn in the game rounds.
/// </summary>
public interface ITurnBasedAble
{
	/// <summary>
	/// Gets or sets the rounds to pass.
	/// </summary>
	/// <value>
	/// The rounds to pass.
	/// </value>
	int RoundsToPass { get; set; }
	
	/// <summary>
	/// Starts turn of this client.
	/// </summary>
	void TakeTurn ();
	
	/// <summary>
	/// Ends the turn.
	/// </summary>
	void EndTurn ();
}

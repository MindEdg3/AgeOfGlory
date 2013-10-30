using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Handles general game processes:<br />
/// <ul>
/// <li>Turn-based system</li>
/// </ul>
/// </summary>
public class GameManager : MonoBehaviour
{
	#region Fields
	private ITurnBasedAble currentActiveClient;
	private List<ITurnBasedAble> turnBasedClients = new List<ITurnBasedAble> ();
	private Queue<ITurnBasedAble> roundQueue = new Queue<ITurnBasedAble> ();
	private float _roundsCount;
	#endregion
	
	#region Singletone
	private static GameManager _instance;

	public static GameManager Instance {
		get {
			if (_instance == null) {
				_instance = Component.FindObjectOfType (typeof(GameManager)) as GameManager;
			}
			return _instance;
		}
	}
	#endregion
	
	#region Properties
	#endregion
	
	#region Methods
	void Start ()
	{
		NextTurn ();
	}
	
	/// <summary>
	/// Registers the turn based client.
	/// </summary>
	/// <param name='newClient'>
	/// New client.
	/// </param>
	public void RegisterTurnBasedClient (ITurnBasedAble newClient)
	{
		turnBasedClients.Add (newClient);
	}
	
	/// <summary>
	/// Starts the new game round.
	/// </summary>
	void NextRound ()
	{
		for (int i = 0; i < turnBasedClients.Count; i++) {
			roundQueue.Enqueue (turnBasedClients [i]);
		}
		_roundsCount++;
		Debug.Log ("Round " + _roundsCount + " started!");
	}
	
	/// <summary>
	/// Passes turn to next client.
	/// </summary>
	public void NextTurn ()
	{
		if (roundQueue.Count == 0) {
			NextRound ();
		}
		ITurnBasedAble next = roundQueue.Dequeue ();
		next.TakeTurn ();
		Debug.Log ((next as MonoBehaviour).name + " started turn!");
	}
	
	/// <summary>
	/// Call this when client has ended his turn.
	/// </summary>
	public void ClientEndedTurn ()
	{
		NextTurn ();
	}
	#endregion
}

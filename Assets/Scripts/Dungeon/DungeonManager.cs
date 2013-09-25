using UnityEngine;
using System.Collections;

public class DungeonManager : MonoBehaviour
{
	#region Constants
	public const string WALLS_PATH = "Prefabs/DungeonWalls/";
	public const string ROADS_PATH = "Prefabs/Roads";
	#endregion
	
	#region Properties
	public string[] arr_wallTypes;

	public string[] WallTypes {
		get {
			return this.arr_wallTypes;
		}
		set {
			arr_wallTypes = value;
		}
	}
	
	private DungeonData _currentDungeon;

	public DungeonData CurrentDungeon {
		get {
			return this._currentDungeon;
		}
		set {
			if (_currentDungeon != value) {
				_currentDungeon = value;
				AssembleDungeon (value);
			}
		}
	}

	#endregion
	
	#region Singletone
	private static DungeonManager _instance;

	public static DungeonManager Instance {
		get {
			if (_instance == null) {
				_instance = Component.FindObjectOfType (typeof(DungeonManager)) as DungeonManager;
			}
			return _instance;
		}
	}
	#endregion
	
	// Use this for initialization
	void Start ()
	{
		DungeonData newDungeon = DungeonData.RandomizeNewDungeon ();
		CurrentDungeon = newDungeon;
		Debug.Log (newDungeon.ToString ());
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
	
	/// <summary>
	/// Assembles the dungeon.
	/// </summary>
	/// <param name='dungeon'>
	/// Dungeon.
	/// </param>
	void AssembleDungeon (DungeonData dungeon)
	{
		DungeonDataTile[,] tiles = dungeon.Tiles;
		
		for (int j = 0; j < dungeon.height; j++) {
			for (int i = 0; i < dungeon.width; i++) {
				if (tiles [i, j].room != null) {
					
				}
			}
		}
	}
}

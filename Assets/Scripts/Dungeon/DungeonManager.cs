using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DungeonManager : MonoBehaviour
{
	#region Constants
	public const string WALLS_PATH = "Prefabs/DungeonWalls/";
	public const string PASSAGES_PATH = "Prefabs/DungeonPassages/";
	public const string FLOORS_PATH = "Prefabs/DungeonFloors/";
	public const string CEILINGS_PATH = "Prefabs/DungeonCeilings/";
	public const string LIGHTS_PATH = "Prefabs/Lights/";
	#endregion
	
	#region Fields
	public int visibleTilesInFront;
	public int visibleTilesInBack;
	public int visibleTilesAtSides;
	public Dictionary<int, GameObject> walls = new Dictionary<int, GameObject> ();
	public Dictionary<int, GameObject> torches = new Dictionary<int, GameObject> ();
	private Dictionary<int, List<GameObject>> createdObjects = new Dictionary<int, List<GameObject>> ();
	#endregion
	
	#region Properties
	public string[] arr_wallTypes;

	public string[] WallTypes {
		get {
			return this.arr_wallTypes;
		}
		set {
			this.arr_wallTypes = value;
		}
	}
	
	public string[] arr_passagesTypes;

	public string[] PassagesTypes {
		get {
			return this.arr_passagesTypes;
		}
		set {
			this.arr_passagesTypes = value;
		}
	}

	public string[] arr_floorTypes;

	public string[] FloorsTypes {
		get {
			return this.arr_floorTypes;
		}
		set {
			this.arr_floorTypes = value;
		}
	}

	public string[] arr_ceilingsTypes;

	public string[] CeilingsTypes {
		get {
			return this.arr_ceilingsTypes;
		}
		set {
			this.arr_ceilingsTypes = value;
		}
	}
	
	private DungeonData _currentDungeon;

	public DungeonData CurrentDungeon {
		get {
			return this._currentDungeon;
		}
		set {
			if (this._currentDungeon != value) {
				this._currentDungeon = value;
			}
		}
	}
	
	private PlayerDungeonController _myPlayer;
	
	public PlayerDungeonController MyPlayer {
		get {
			if (_myPlayer == null) {
				_myPlayer = GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerDungeonController> ();
			}
			return _myPlayer;
		}
	}
	
	private Transform _myPlayerTr;
	
	public Transform MyPlayerTr {
		get {
			if (_myPlayerTr == null) {
				_myPlayerTr = MyPlayer.Tr;
			}
			return _myPlayerTr;
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
		
		// Init player
		MyPlayerTr.position = DungeonUtils.GetPositionByIndex (CurrentDungeon.entranceX, CurrentDungeon.entranceY);
		MyPlayer._currentI = CurrentDungeon.entranceX;
		MyPlayer._currentJ = CurrentDungeon.entranceY;
	}
	
	/// <summary>
	/// Updates the visible tiles.
	/// </summary>
	/// <param name='posI'>
	/// Position I index.
	/// </param>
	/// <param name='posJ'>
	/// Position J index.
	/// </param>
	public void UpdateVisibleTiles (int posI, int posJ)
	{
		Dictionary<int, List<GameObject>> newObjects = new Dictionary<int, List<GameObject>> ();
		
		//for every tile, that should be visible
		for (int j = -visibleTilesInFront; j <  visibleTilesInFront; j++) {
			for (int i = -visibleTilesInFront; i <  visibleTilesInFront; i++) {
				int objI = i + posI;
				int objJ = j + posJ;
				int code = DungeonUtils.GetCodeOfTileByIndex (objI, j + posJ);
				
				// If objects for this tile already instantiated
				if (createdObjects.ContainsKey (code)) {
					newObjects.Add (code, createdObjects [code]);
					// Remove this list from old dictionary, so this objects will not be destroyed
					createdObjects.Remove (code);
				} else { // else if objects hasn't been instantiated for this tile, create it
					if (CurrentDungeon.Tiles.ContainsKey (code)) {
						DungeonDataTile tile = CurrentDungeon.Tiles [code];
						// Create new list
						List<GameObject> tileObjects = new List<GameObject> ();
						newObjects.Add (code, tileObjects);
						
						// Instantiate every tile object
						for (int objIndex = 0; objIndex < tile.Objects.Count; objIndex++) {
							DungeonObject dungeonObject = tile.Objects [objIndex];
							GameObject newObject = Instantiate (
							ResourceManager.GetResource (dungeonObject.type, dungeonObject.prefabReference) as Object,
							DungeonUtils.GetPositionByIndex (objI, objJ) + dungeonObject.offset,
							dungeonObject.rotation
						) as GameObject;
							newObject.name = dungeonObject.prefabReference + "_" + code;
							tileObjects.Add (newObject);
						}
					}
				}
			}
		}
		
		// Remove objects from dictionaries differences
		foreach (List<GameObject> objectListToRemove in createdObjects.Values) {
			for (int i = 0; i < objectListToRemove.Count; i++) {
				Destroy (objectListToRemove [i]);
			}
		}
		
		createdObjects = newObjects;
	}
}

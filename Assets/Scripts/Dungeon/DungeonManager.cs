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
		MyPlayer.State = PlayerState.Idle;
		MyPlayer._currentX = CurrentDungeon.entranceX;
		MyPlayer._currentY = CurrentDungeon.entranceY;
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
	
	
	/// <summary>
	/// Decorates the roads.
	/// </summary>
	/// <param name='dungeon'>
	/// Dungeon.
	/// </param>
	public void DecorateRoads (DungeonData dungeon)
	{
		GameObject torchPrefab = (Resources.Load (DungeonManager.LIGHTS_PATH + "Torch") as GameObject).gameObject;
		
		// Torches
		List<DungeonRoad> roads = dungeon.Roads;
		for (int r = 0; r < roads.Count; r++) {
			DungeonRoad road = roads [r];
			
			// for every tile in road
			for (int j = road.y1; j <= road.y2; j++) {
				for (int i = road.x1; i <= road.x2; i++) {
					// 50% to create a torchlight
					if (Random.value > 0.5f) {
						//if it is a corridor
						if (dungeon.Tiles_old [i, j].room == null) {
							
							// slight randomiztion of first side to be checked
							int randomStart = Random.Range (0, 4);
							
							// check every side around a floor for a wall
							for (byte s = 0; s < 4; s++) {
								int x = 0, y = 0;
							
								// limit randomized index to maximum value of 4
								int startIndex = (s + randomStart) % 4;
								
								switch (startIndex) {
								case 0:
									x = i;
									y = j - 1;
									break;
								case 1:
									x = i + 1;
									y = j;
									break;
								case 2:
									x = i;
									y = j + 1;
									break;
								case 3:
									x = i - 1;
									y = j;
									break;
								}
							
								int code = DungeonUtils.GetCodeBetweenTilesByIndex (x, y, i, j);
							
								// if there are wall
								if (walls.ContainsKey (code)) {
									// and torch hasn't been attached already
									if (!torches.ContainsKey (code)) {
										// create it at wall and save to collection!
										Vector3 torchPosition = walls [code].transform.position;
										Quaternion torchRotation = Quaternion.Euler (0f, startIndex * 90f, 0f);
										GameObject newTorch = Instantiate (torchPrefab, torchPosition, torchRotation) as GameObject;
										newTorch.name = "torch_" + code;
										
										torches.Add (code, newTorch);
									}
									// anyway, we don't need any more torches for this tile..
									break;
								}
							}
						}
					}
				}
			}
		}
	}
}

﻿using UnityEngine;
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
	public Dictionary<int, GameObject> walls = new Dictionary<int, GameObject> ();
	public Dictionary<int, GameObject> torches = new Dictionary<int, GameObject> ();
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
				AssembleDungeon (value);
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
	public void AssembleDungeon (DungeonData dungeon)
	{	
		PlaceBaseGeometry (dungeon);
		DecorateRoads (dungeon);
//		DecorateRooms ();
	}
	
	/// <summary>
	/// Floor factory.
	/// </summary>
	/// <param name='floorName'>
	/// Floor prefab name.
	/// </param>
	/// <param name='i'>
	/// I position.
	/// </param>
	/// <param name='j'>
	/// J position.
	/// </param>
	public void AddFloor (string floorName, int i, int j)
	{
		GameObject floorPrefab = (Resources.Load (DungeonManager.FLOORS_PATH + floorName) as GameObject).gameObject;
		
		Vector3 pos = DungeonUtils.GetPositionByIndex (i, j);
		
		GameObject newFloor = Instantiate (floorPrefab, pos, Quaternion.identity) as GameObject;
	}
	
	/// <summary>
	/// Ceiling factory.
	/// </summary>
	/// <param name='ceilingName'>
	/// Ceiling prefab name.
	/// </param>
	/// <param name='i'>
	/// I position.
	/// </param>
	/// <param name='j'>
	/// J position.
	/// </param>
	public void AddCeiling (string ceilingName, int i, int j)
	{
		GameObject ceilingPrefab = (Resources.Load (DungeonManager.CEILINGS_PATH + ceilingName) as GameObject).gameObject;
		
		Vector3 pos = DungeonUtils.GetPositionByIndex (i, j);
		
		GameObject newCeiling = Instantiate (ceilingPrefab, pos, Quaternion.identity) as GameObject;
	}
	
	/// <summary>
	/// Passage factory.
	/// </summary>
	/// <param name='passageName'>
	/// Passage prefab name.
	/// </param>
	/// <param name='i1'>
	/// Entry I position.
	/// </param>
	/// <param name='j1'>
	/// Entry J position.
	/// </param>
	/// <param name='i2'>
	/// Exit I position.
	/// </param>
	/// <param name='j2'>
	/// Exit J position.
	/// </param>
	public void AddPassage (string passageName, int i1, int j1, int i2, int j2)
	{
		GameObject passagePrefab = (Resources.Load (DungeonManager.PASSAGES_PATH + passageName) as GameObject).gameObject;
		
		Vector3 pos = DungeonUtils.GetPositionBetweenTilesByIndex (i1, j1, i2, j2);
		Quaternion rot = Quaternion.identity;
		
		if (i1 == i2 && j1 < j2) {
			rot = Quaternion.identity;
		} else if (i2 < i1 && j1 == j2) {
			rot = Quaternion.Euler (0f, 90f, 0f);
		} else if (i1 == i2 && j2 < j1) {
			rot = Quaternion.Euler (0f, 180f, 0f);
		} else if (i1 < i2 && j1 == j2) {
			rot = Quaternion.Euler (0f, 270f, 0f);
		}
		
		GameObject newPassage = Instantiate (passagePrefab, pos, rot) as GameObject;
	}

	/// <summary>
	/// Wall factory.
	/// </summary>
	/// <param name='wallName'>
	/// Wall prefab name.
	/// </param>
	/// <param name='i1'>
	/// Entry I position.
	/// </param>
	/// <param name='j1'>
	/// Entry J position.
	/// </param>
	/// <param name='i2'>
	/// Exit I position.
	/// </param>
	/// <param name='j2'>
	/// Exit J position.
	/// </param>
	public void AddWall (string wallName, int i1, int j1, int i2, int j2)
	{
		GameObject wallPrefab = (Resources.Load (DungeonManager.WALLS_PATH + wallName) as GameObject).gameObject;
		
		Vector3 pos = DungeonUtils.GetPositionBetweenTilesByIndex (i1, j1, i2, j2);
		Quaternion rot = Quaternion.identity;
		
		if (i1 == i2 && j1 < j2) {
			rot = Quaternion.identity;
		} else if (i2 < i1 && j1 == j2) {
			rot = Quaternion.Euler (0f, 90f, 0f);
		} else if (i1 == i2 && j2 < j1) {
			rot = Quaternion.Euler (0f, 180f, 0f);
		} else if (i1 < i2 && j1 == j2) {
			rot = Quaternion.Euler (0f, 270f, 0f);
		}
		
		GameObject newWall = Instantiate (wallPrefab, pos, rot) as GameObject;
		
		int newWallCode = DungeonUtils.GetCodeBetweenTilesByIndex (i1, j1, i2, j2);
		newWall.name = "wall_" + newWallCode + "_" + wallName;
		
		walls.Add (newWallCode, newWall);
	}
	
	/// <summary>
	/// Places the base geometry: walls, floors, ceilings.
	/// </summary>
	/// <param name='dungeon'>
	/// Dungeon.
	/// </param>
	private void PlaceBaseGeometry (DungeonData dungeon)
	{
		DungeonDataTile[,] tiles = dungeon.Tiles;
		
		for (int j = 0; j < dungeon.height; j++) {
			for (int i = 0; i < dungeon.width; i++) {
				if (tiles [i, j].room != null) {
					AddFloor (arr_floorTypes [0], i, j);
					AddCeiling (arr_ceilingsTypes [0], i, j);
					
					// Left wall
					if (i == 0) {
						AddWall (arr_wallTypes [0], i - 1, j, i, j);
					}
					// Top wall
					if (j == 0) {
						AddWall (arr_wallTypes [0], i, j - 1, i, j);
					}
					
					if (i < dungeon.width - 1) {
						DungeonDataTile rightTile = tiles [i + 1, j];
						if (rightTile.room == null && rightTile.road != null) {
							AddPassage (arr_passagesTypes [0], i + 1, j, i, j);
						} else if (rightTile.room == null && rightTile.road == null) {
							AddWall (arr_wallTypes [0], i + 1, j, i, j);
						}
					} else {
						AddWall (arr_wallTypes [0], i + 1, j, i, j);
					}
					if (j < dungeon.height - 1) {
						DungeonDataTile bottomTile = tiles [i, j + 1];
						if (bottomTile.room == null && bottomTile.road != null) {
							AddPassage (arr_passagesTypes [0], i, j + 1, i, j);
						} else if (bottomTile.room == null && bottomTile.road == null) {
							AddWall (arr_wallTypes [0], i, j + 1, i, j);
						}
					} else {
						AddWall (arr_wallTypes [0], i, j + 1, i, j);
					}
				} else if (tiles [i, j].road != null) {
					AddFloor (arr_floorTypes [0], i, j);
					AddCeiling (arr_ceilingsTypes [0], i, j);
					
					if (i < dungeon.width - 1) {
						DungeonDataTile rightTile = tiles [i + 1, j];
						if (rightTile.room != null) {
							AddPassage (arr_passagesTypes [0], i, j, i + 1, j);
						} else if (rightTile.room == null && rightTile.road == null) {
							AddWall (arr_wallTypes [0], i + 1, j, i, j);
						}
					}
					if (j < dungeon.height - 1) {
						DungeonDataTile bottomTile = tiles [i, j + 1];
						if (bottomTile.room != null) {
							AddPassage (arr_passagesTypes [0], i, j, i, j + 1);
						} else if (bottomTile.room == null && bottomTile.road == null) {
							AddWall (arr_wallTypes [0], i, j + 1, i, j);
						}
					}
				} else {
					
					if (i < dungeon.width - 1) {
						DungeonDataTile rightTile = tiles [i + 1, j];

						if (rightTile.room != null || rightTile.road != null) {
							AddWall (arr_wallTypes [0], i, j, i + 1, j);
						}
					}
					if (j < dungeon.height - 1) {
						DungeonDataTile bottomTile = tiles [i, j + 1];
						
						if (bottomTile.room != null || bottomTile.road != null) {
							AddWall (arr_wallTypes [0], i, j, i, j + 1);
						}
					}
				}
			}
		}
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
						if (dungeon.Tiles [i, j].room == null) {
							
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

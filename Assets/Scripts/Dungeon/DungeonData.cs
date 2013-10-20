using UnityEngine;
using System.Collections.Generic;

public class DungeonData : ScriptableObject
{
	#region Constants
	private const int MAX_ROOMS_DEFAULT = 32;
	private const int MIN_ROOMS_SIZE = 3;
	private const int MAX_ROOMS_SIZE = 7;
	#endregion
	
	public int width;
	public int height;
	public int entranceX;
	public int entranceY;
	
	#region Properties
	public List<DungeonRoom> rooms;
	
	public List<DungeonRoom> Rooms {
		get {
			return this.rooms;
		}
		set {
			if (rooms != value) {
				_tiles_old = null;
				rooms = value;
			}
		}
	}
	
	public List<DungeonRoad> roads;
	
	public List<DungeonRoad> Roads {
		get {
			return this.roads;
		}
		set {
			if (roads != value) {
				_tiles_old = null;
				roads = value;
			}
		}
	}
	
	private Dictionary <int, DungeonDataTile> _tiles;
	
	/// <summary>
	/// The tiles of this dungeon. If rooms or roads had been changed, tiles need to be recreated.
	/// </summary>
	/// <value>
	/// The tiles of this dungeon.
	/// </value>
	public Dictionary <int, DungeonDataTile> Tiles {
		get {
			if (this._tiles == null) {
				this._tiles = GetTiles ();
			}
			return this._tiles;
		}
		set {
			this._tiles = value;
		}
	}
	
	private DungeonDataTile[,] _tiles_old;
	
	/// <summary>
	/// The tiles of this dungeon. If rooms or roads had been changed, tiles need to be recreated.
	/// </summary>
	/// <value>
	/// The tiles of this dungeon.
	/// </value>
	public DungeonDataTile[,] Tiles_old {
		get {
			if (this._tiles_old == null) {
//				this._tiles_old = GetTiles ();
			}
			return this._tiles_old;
		}
		set {
			this._tiles_old = value;
		}
	}
	#endregion
	
	#region Methods
	/// <summary>
	/// Retrieves the tiles of this dungeon.
	/// </summary>
	/// <returns>
	/// The tiles of this dungeon.
	/// </returns>
	public Dictionary <int, DungeonDataTile> GetTiles ()
	{
		DungeonDataTile[,] tilesGrid = new DungeonDataTile[width, height];
		
		for (int j = 0; j < height; j++) {
			for (int i = 0; i < width; i++) {
				tilesGrid [i, j] = new DungeonDataTile ();
			}
		}
		
		for (int k = 0; k < roads.Count; k++) {
			for (int j = 0; j <= roads[k].y2 - roads[k].y1; j++) {
				for (int i = 0; i <= roads[k].x2 - roads[k].x1; i++) {
					tilesGrid [i + roads [k].x1, j + roads [k].y1].road = roads [k];
				}
			}
		}
		
		for (int k = 0; k < rooms.Count; k++) {
			for (int j = 0; j < rooms[k].height; j++) {
				for (int i = 0; i < rooms[k].width; i++) {
					tilesGrid [i + rooms [k].x, j + rooms [k].y].room = rooms [k];
				}
			}
		}
		
		Dictionary <int, DungeonDataTile> ret = new Dictionary<int, DungeonDataTile> (width * height);

		for (int j = 0; j < height; j++) {
			for (int i = 0; i < width; i++) {
				ret.Add (DungeonUtils.GetCodeOfTileByIndex (i, j), tilesGrid [i, j]);
			}
		}
		
		return ret;
	}
	
	/// <summary>
	/// Returns a <see cref="System.String"/> that represents the current <see cref="DungeonData"/>.
	/// </summary>
	/// <returns>
	/// A <see cref="System.String"/> that represents the current <see cref="DungeonData"/>.
	/// </returns>
	public override string ToString ()
	{
		string ret = "";
		
		ret += "Dungeon " + width + "x" + height + ". Entrance in " + entranceX + ":" + entranceY + "\n";
		
		ret += "\nRooms: ";
		for (int i = 0; i < rooms.Count; i++) {
			ret += ("Room" + i + " (" + Rooms [i].x + "," + Rooms [i].y + "," + Rooms [i].width + "," + Rooms [i].height + "), ");
		}
		
		ret += "\n\nRoads: ";
		for (int i = 0; i < rooms.Count; i++) {
			ret += ("Road" + i + " (" + Roads [i].x1 + "," + Roads [i].y1 + "," + Roads [i].x2 + "," + Roads [i].y2 + "), ");
		}
		
		ret += "\n\nMap:\n";
		
		
		string[,] tiles = new string[width, height];
		foreach (KeyValuePair<int, DungeonDataTile> kvp in Tiles) {
			int code = kvp.Key;
			DungeonDataTile tile = kvp.Value;
			
			int i, j;
			DungeonUtils.GetIndexesOfTileByCode (code, out i, out j);
			if (tile.room != null) {
				tiles [i, j] = "0 ";
			} else if (tile.road != null) {
				tiles [i, j] = "X ";
			} else {
				tiles [i, j] = "- ";
			}
		}
		
		for (int j = 0; j < height; j++) {
			for (int i = 0; i < width; i++) {
				ret += tiles [i, j];
			}
			ret += "\n";
		}
		
		return ret;
	}
	#endregion
	
	#region Static methods
	/// <summary>
	/// Randomizes the new dungeon with default size of 16x16.
	/// </summary>
	/// <returns>
	/// The new dungeon 16x16.
	/// </returns>
	public static DungeonData RandomizeNewDungeon ()
	{
		return RandomizeNewDungeon (32, 32);
	}
	
	/// <summary>
	/// Randomly creates new dungeon.
	/// </summary>
	/// <returns>
	/// The new dungeon.
	/// </returns>
	/// <param name='width'>
	/// Width of new dungeon.
	/// </param>
	/// <param name='height'>
	/// Height of new dungeon.
	/// </param>
	public static DungeonData RandomizeNewDungeon (int width, int height)
	{
		DungeonData ret;
		
		ret = ScriptableObject.CreateInstance<DungeonData> ();
		
		// set base values
		ret.width = width;
		ret.height = height;
		
		// set entrance point. It should be placed at one of border of the dungeon.
		if (Random.value > 0.5f) {	
			ret.entranceX = Random.Range (1, width - 1);
			ret.entranceY = Random.value > 0.5f ? 0 : height - 1;
		} else {	
			ret.entranceX = Random.value > 0.5f ? 0 : width - 1;
			ret.entranceY = Random.Range (1, height - 1);
		}
		
		// create rooms
		ret.Rooms = PlaceRooms (
			DungeonData.MAX_ROOMS_DEFAULT,
			DungeonData.MIN_ROOMS_SIZE,
			DungeonData.MAX_ROOMS_SIZE,
			width,
			height,
			ret.entranceX,
			ret.entranceY
		);
		
		ret.Roads = PlaceRoads (ret.rooms);
		
		PrepareObjects (ret);
		
		return ret;
	}
	
	/// <summary>
	/// Places rooms, trying to randomize them and excluding overlapping rooms.
	/// First room will be placed at entrance.
	/// </summary>
	/// <returns>
	/// Placed rooms.
	/// </returns>
	/// <param name='maxRooms'>
	/// Max count of rooms generator will try to place.
	/// </param>
	/// <param name='minRoomSize'>
	/// Minimum room size.
	/// </param>
	/// <param name='maxRoomSize'>
	/// Maximum room size.
	/// </param>
	/// <param name='dungeonWidth'>
	/// Dungeon width.
	/// </param>
	/// <param name='dungeonHeight'>
	/// Dungeon height.
	/// </param>
	private static List<DungeonRoom> PlaceRooms (int maxRooms, int minRoomSize, int maxRoomSize, int dungeonWidth, int dungeonHeight, int entranceX, int entranceY)
	{
		List<DungeonRoom> ret = new List<DungeonRoom> ();
		
		// create first room 3x3 at entrance
		DungeonRoom firstRoom = new DungeonRoom ();
		if (entranceX == 0) {
			firstRoom.x = entranceX;
			firstRoom.y = entranceY - 1;
		} else if (entranceX == dungeonWidth - 1) {
			firstRoom.x = entranceX - 2;
			firstRoom.y = entranceY - 1;
		} else if (entranceY == 0) {
			firstRoom.x = entranceX - 1;
			firstRoom.y = entranceY;
		} else if (entranceY == dungeonHeight - 1) {
			firstRoom.x = entranceX - 1;
			firstRoom.y = entranceY - 2;
		}
		
		firstRoom.width = 3;
		firstRoom.height = 3;
		
		ret.Add (firstRoom);
		
		// create another rooms
		for (int i = 1; i < maxRooms; i++) {
			// create new room
			DungeonRoom newRoom = CreateRoom (minRoomSize, maxRoomSize, dungeonWidth, dungeonHeight);
			
			// validating room
			bool isNewRoomIntersecting = false;
			for (int j = 0; j < ret.Count; j++) {
				isNewRoomIntersecting = newRoom.IsIntersects (ret [j]);
				if (isNewRoomIntersecting) {
					break;
				}
			}
			
			// if its ok, save it
			if (!isNewRoomIntersecting) {
				ret.Add (newRoom);
			}
		}
		
		return ret;
	}
	
	/// <summary>
	/// Initializes new room with specified parameters.
	/// </summary>
	/// <returns>
	/// Resulted room.
	/// </returns>
	/// <param name='minRoomSize'>
	/// Minimum room size.
	/// </param>
	/// <param name='maxRoomSize'>
	/// Max room size.
	/// </param>
	/// <param name='dungeonWidth'>
	/// Dungeon width.
	/// </param>
	/// <param name='dungeonHeight'>
	/// Dungeon height.
	/// </param>
	private static DungeonRoom CreateRoom (int minRoomSize, int maxRoomSize, int dungeonWidth, int dungeonHeight)
	{
		DungeonRoom ret = new DungeonRoom ();
			
		// set base values
		ret.width = Random.Range (minRoomSize, maxRoomSize + 1);
		ret.height = Random.Range (minRoomSize, maxRoomSize + 1);
		ret.x = Random.Range (0, dungeonWidth - ret.width);
		ret.y = Random.Range (0, dungeonHeight - ret.height);
		
		return ret;
	}
	
	/// <summary>
	/// Places the roads between rooms one after another. At first places horizontal
	/// road, then vertical.
	/// </summary>
	/// <returns>
	/// List of roads.
	/// </returns>
	/// <param name='rooms'>
	/// Ordered list of rooms in dungeon.
	/// </param>
	private static List<DungeonRoad> PlaceRoads (List<DungeonRoom> rooms)
	{
		List<DungeonRoad> ret = new List<DungeonRoad> ();
		
		// for every pair of room
		for (int i = 1; i < rooms.Count; i++) {
			DungeonRoad horRoad = new DungeonRoad ();
			DungeonRoad vertRoad = new DungeonRoad ();
			
			// Parent room is a room which starts a horizontal road
			int parentRoomIndex = i;
			// Child room is a room in the end of vertical road
			int childRoomIndex = i;
			
			if (Random.value > 0.5f) {
				parentRoomIndex -= 1;
			} else {
				childRoomIndex -= 1;
			}
			
			// road starts in parent room and pulled left or right to acheieve X of child room
			int hx1 = rooms [parentRoomIndex].CenterX;
			int hy1 = rooms [parentRoomIndex].CenterY;
			int hx2 = rooms [childRoomIndex].CenterX;
			int hy2 = hy1;
			
			// road starts in the end of horizontal road and pulled up or down to end in child room
			int vx1 = rooms [childRoomIndex].CenterX;
			int vy1 = rooms [parentRoomIndex].CenterY;
			int vx2 = vx1;
			int vy2 = rooms [childRoomIndex].CenterY;
			
			// Horizontal roads carved from left to right
			horRoad.x1 = Mathf.Min (hx1, hx2);
			horRoad.y1 = Mathf.Min (hy1, hy2);
			horRoad.x2 = Mathf.Max (hx1, hx2);
			horRoad.y2 = Mathf.Max (hy1, hy2);
			
			// Vertical roads carved from top to bottom
			vertRoad.x1 = Mathf.Min (vx1, vx2);
			vertRoad.y1 = Mathf.Min (vy1, vy2);
			vertRoad.x2 = Mathf.Max (vx1, vx2);
			vertRoad.y2 = Mathf.Max (vy1, vy2);
							
			ret.Add (horRoad);
			ret.Add (vertRoad);
		}	
		
		return ret;
	}
	
	/// <summary>
	/// Prepares the objects for tiles that can be created later.
	/// </summary>
	/// <param name='dungeon'>
	/// Dungeon data.
	/// </param>
	public static void PrepareObjects (DungeonData dungeon)
	{
		PrepareBaseGeometry (dungeon);
		PrepareLights (dungeon);
	}
	
	/// <summary>
	/// Prepares tile objects such as a walls, passages, floors and ceilings.
	/// </summary>
	/// <param name='dungeon'>
	/// Dungeon.
	/// </param>
	public static void PrepareBaseGeometry (DungeonData dungeon)
	{
		Dictionary <int, DungeonDataTile> tiles = dungeon.Tiles;
		
		// For every tile in map
		foreach (KeyValuePair<int, DungeonDataTile> kvp in tiles) {
			int code = kvp.Key;
			DungeonDataTile tile = kvp.Value;
			
			int i, j;
			DungeonUtils.GetIndexesOfTileByCode (code, out i, out j);	
			
			// If current tile is a room..
			if (tiles [code].room != null) {
				// Set floor
				DungeonObject floor = new DungeonObject (code, DungeonObjectType.Floor, "dirt_floor");
				tile.Objects.Add (floor);
			
				// Set ceiling
				DungeonObject ceiling = new DungeonObject (code, DungeonObjectType.Ceiling, "dirt_ceiling");
				tile.Objects.Add (ceiling);
			
				// LEFT wall
				// If it is a left border set left wall
				if (i == 0) {
					DungeonObject newWall = PrepareWallObject (code, DungeonObjectType.Wall, "wall_rock", i - 1, j, i, j); 
					tile.Objects.Add (newWall);
				} else {
					int leftCode = DungeonUtils.GetCodeOfTileByIndex (i - 1, j);
					if (tiles.ContainsKey (leftCode)) {
						if (tiles [leftCode].room == null && tiles [leftCode].road != null) {
							DungeonObject newPassage = PrepareWallObject (code, DungeonObjectType.Passage, "passage_rock_arc", i - 1, j, i, j); 
							tile.Objects.Add (newPassage);
						} else if (tiles [leftCode].room == null && tiles [leftCode].road == null) {
							DungeonObject newWall = PrepareWallObject (code, DungeonObjectType.Wall, "wall_rock", i - 1, j, i, j); 
							tile.Objects.Add (newWall);
						}
					}
				}
				
				// TOP wall
				// If it is a top border set top wall
				if (j == 0) {
					DungeonObject newWall = PrepareWallObject (code, DungeonObjectType.Wall, "wall_rock", i, j - 1, i, j); 
					tile.Objects.Add (newWall);
				} else {
					int topCode = DungeonUtils.GetCodeOfTileByIndex (i, j - 1);
					if (tiles.ContainsKey (topCode)) {
						if (tiles [topCode].room == null && tiles [topCode].road != null) {
							DungeonObject newPassage = PrepareWallObject (code, DungeonObjectType.Passage, "passage_rock_arc", i, j - 1, i, j); 
							tile.Objects.Add (newPassage);
						} else if (tiles [topCode].room == null && tiles [topCode].road == null) {
							DungeonObject newWall = PrepareWallObject (code, DungeonObjectType.Wall, "wall_rock", i, j - 1, i, j); 
							tile.Objects.Add (newWall);
						}
					}
				}
				
				// RIGHT wall
				// If it is a right border set right wall
				if (i == dungeon.width - 1) {
					DungeonObject newWall = PrepareWallObject (code, DungeonObjectType.Wall, "wall_rock", i + 1, j, i, j); 
					tile.Objects.Add (newWall);
				} else {
					int rightCode = DungeonUtils.GetCodeOfTileByIndex (i + 1, j);
					if (tiles.ContainsKey (rightCode)) {
						if (tiles [rightCode].room == null && tiles [rightCode].road != null) {
							DungeonObject newPassage = PrepareWallObject (code, DungeonObjectType.Passage, "passage_rock_arc", i + 1, j, i, j); 
							tile.Objects.Add (newPassage);
						} else if (tiles [rightCode].room == null && tiles [rightCode].road == null) {
							DungeonObject newWall = PrepareWallObject (code, DungeonObjectType.Wall, "wall_rock", i + 1, j, i, j); 
							tile.Objects.Add (newWall);
						}
					}
				}
				
				// BOTTOM wall
				// If it is a bottom border set bottom wall
				if (j == dungeon.height - 1) {
					DungeonObject newWall = PrepareWallObject (code, DungeonObjectType.Wall, "wall_rock", i, j + 1, i, j); 
					tile.Objects.Add (newWall);
				} else {
					int bottomCode = DungeonUtils.GetCodeOfTileByIndex (i, j + 1);
					if (tiles.ContainsKey (bottomCode)) {
						if (tiles [bottomCode].room == null && tiles [bottomCode].road != null) {
							DungeonObject newPassage = PrepareWallObject (code, DungeonObjectType.Passage, "passage_rock_arc", i, j + 1, i, j); 
							tile.Objects.Add (newPassage);
						} else if (tiles [bottomCode].room == null && tiles [bottomCode].road == null) {
							DungeonObject newWall = PrepareWallObject (code, DungeonObjectType.Wall, "wall_rock", i, j + 1, i, j); 
							tile.Objects.Add (newWall);
						}
					}
				}
			} else if (tiles [code].road != null) {
				// Set floor
				DungeonObject floor = new DungeonObject (code, DungeonObjectType.Floor, "dirt_floor");
				tile.Objects.Add (floor);
			
				// Set ceiling
				DungeonObject ceiling = new DungeonObject (code, DungeonObjectType.Ceiling, "dirt_ceiling");
				tile.Objects.Add (ceiling);
			
				// LEFT wall
				// If it is a left border set left wall
				if (i == 0) {
					DungeonObject newWall = PrepareWallObject (code, DungeonObjectType.Wall, "wall_rock", i - 1, j, i, j); 
					tile.Objects.Add (newWall);
				} else {
					int leftCode = DungeonUtils.GetCodeOfTileByIndex (i - 1, j);
					if (tiles.ContainsKey (leftCode)) {
						if (tiles [leftCode].room != null) {
							DungeonObject newPassage = PrepareWallObject (code, DungeonObjectType.Passage, "passage_rock_arc", i - 1, j, i, j); 
							tile.Objects.Add (newPassage);
						} else if (tiles [leftCode].room == null && tiles [leftCode].road == null) {
							DungeonObject newWall = PrepareWallObject (code, DungeonObjectType.Wall, "wall_rock", i - 1, j, i, j); 
							tile.Objects.Add (newWall);
						}
					}
				}
				
				// TOP wall
				// If it is a top border set top wall
				if (j == 0) {
					DungeonObject newWall = PrepareWallObject (code, DungeonObjectType.Wall, "wall_rock", i, j - 1, i, j); 
					tile.Objects.Add (newWall);
				} else {
					int topCode = DungeonUtils.GetCodeOfTileByIndex (i, j - 1);
					if (tiles.ContainsKey (topCode)) {
						if (tiles [topCode].room != null) {
							DungeonObject newPassage = PrepareWallObject (code, DungeonObjectType.Passage, "passage_rock_arc", i, j - 1, i, j); 
							tile.Objects.Add (newPassage);
						} else if (tiles [topCode].room == null && tiles [topCode].road == null) {
							DungeonObject newWall = PrepareWallObject (code, DungeonObjectType.Wall, "wall_rock", i, j - 1, i, j); 
							tile.Objects.Add (newWall);
						}
					}
				}
				
				// RIGHT wall
				// If it is a right border set right wall
				if (i == dungeon.width - 1) {
					DungeonObject newWall = PrepareWallObject (code, DungeonObjectType.Wall, "wall_rock", i + 1, j, i, j); 
					tile.Objects.Add (newWall);
				} else {
					int rightCode = DungeonUtils.GetCodeOfTileByIndex (i + 1, j);
					if (tiles.ContainsKey (rightCode)) {
						if (tiles [rightCode].room != null) {
							DungeonObject newPassage = PrepareWallObject (code, DungeonObjectType.Passage, "passage_rock_arc", i + 1, j, i, j); 
							tile.Objects.Add (newPassage);
						} else if (tiles [rightCode].room == null && tiles [rightCode].road == null) {
							DungeonObject newWall = PrepareWallObject (code, DungeonObjectType.Wall, "wall_rock", i + 1, j, i, j); 
							tile.Objects.Add (newWall);
						}
					}
				}
				
				// BOTTOM wall
				// If it is a bottom border set bottom wall
				if (j == dungeon.height - 1) {
					DungeonObject newWall = PrepareWallObject (code, DungeonObjectType.Wall, "wall_rock", i, j + 1, i, j); 
					tile.Objects.Add (newWall);
				} else {
					int bottomCode = DungeonUtils.GetCodeOfTileByIndex (i, j + 1);
					if (tiles.ContainsKey (bottomCode)) {
						if (tiles [bottomCode].room != null) {
							DungeonObject newPassage = PrepareWallObject (code, DungeonObjectType.Passage, "passage_rock_arc", i, j + 1, i, j); 
							tile.Objects.Add (newPassage);
						} else if (tiles [bottomCode].room == null && tiles [bottomCode].road == null) {
							DungeonObject newWall = PrepareWallObject (code, DungeonObjectType.Wall, "wall_rock", i, j + 1, i, j); 
							tile.Objects.Add (newWall);
						}
					}
				}
			}
		}
	}

	public static void PrepareLights (DungeonData dungeon)
	{
		// TODO: Prepare lights
	}
	
	/// <summary>
	/// Prepares the wall object for tile.
	/// </summary>
	/// <returns>
	/// Prepared wall object as dungeon object.
	/// </returns>
	/// <param name='code'>
	/// Code of tile.
	/// </param>
	/// <param name='objectType'>
	/// Object type.
	/// </param>
	/// <param name='prefabReference'>
	/// Prefab string reference to resource.
	/// </param>
	/// <param name='fromI'>
	/// I index of source tile.
	/// </param>
	/// <param name='fromJ'>
	/// J index of source tile.
	/// </param>
	/// <param name='toI'>
	/// I index of target tile.
	/// </param>
	/// <param name='toJ'>
	/// J index of target tile.
	/// </param>
	public static DungeonObject PrepareWallObject (int code, DungeonObjectType objectType, string prefabReference, int fromI, int fromJ, int toI, int toJ)
	{
		DungeonObject newDungeonObject = new DungeonObject (code, objectType, prefabReference);
		newDungeonObject.rotation = DungeonUtils.GetRotationFromToTile (fromI, fromJ, toI, toJ);
		newDungeonObject.offset = newDungeonObject.rotation * new Vector3 (0f, 0f, -DungeonUtils.TileSize * 0.5f);
		return newDungeonObject;
	}
	#endregion
}

/// <summary>
/// Represents tile in dungeon. May contain <see cref="DungeonRoom"/>, 
/// <see cref="DungeonRoad"/> or both of them. In last case <see cref="DungeonRoom"/> 
/// overrides <see cref="DungeonRoad"/>.
/// </summary>
[System.Serializable]
public class DungeonDataTile
{
	public DungeonRoom room;
	public DungeonRoad road;
	
	#region Properties
	private List<DungeonObject> _objects;
	
	public List<DungeonObject> Objects {
		get {
			if (_objects == null) {
				_objects = new List<DungeonObject> ();
			}
			return _objects;
		}
	}
	#endregion
	
	/// <summary>
	/// Initializes a new instance of the <see cref="DungeonDataTile"/> class.
	/// </summary>
	public DungeonDataTile ()
	{
	}	
}

/// <summary>
/// Dungeon room.
/// </summary>
[System.Serializable]
public class DungeonRoom
{
	/// <summary>
	/// Left coordinate.
	/// </summary>
	public int x;
	/// <summary>
	/// Top coordinate.
	/// </summary>
	public int y;
	public int width;
	public int height;

	public int Left {
		get {
			return x;
		}
	}
	
	public int Top {
		get {
			return y;
		}
	}

	public int Right {
		get {
			return x + width;
		}
	}
	
	public int Bottom {
		get {
			return y + height;
		}
	}
	
	public int CenterX {
		get {
			return x + width / 2;
		}
	}

	public int CenterY {
		get {
			return y + height / 2;
		}
	}
	
	/// <summary>
	/// Initializes a new instance of the <see cref="DungeonRoom"/> class.
	/// </summary>
	public DungeonRoom ()
	{
	}
	
	/// <summary>
	/// Determines whether this Room is intersects the specified other.
	/// </summary>
	/// <returns>
	/// <c>true</c> if this Room is intersects the specified other; otherwise, <c>false</c>.
	/// </returns>
	/// <param name='other'>
	/// Another <see cref="DungeonRoom"/> that need to be checked with this.
	/// </param>
	public bool IsIntersects (DungeonRoom other)
	{
		return (
			Left <= other.Right &&
        	other.Left <= Right &&
        	Top <= other.Bottom &&
        	other.Top <= Bottom
		);
	}
}

/// <summary>
/// Dungeon road.
/// </summary>
[System.Serializable]
public class DungeonRoad
{
	public int x1;
	public int y1;
	public int x2;
	public int y2;
	
	/// <summary>
	/// Initializes a new instance of the <see cref="DungeonRoad"/> class.
	/// </summary>
	public DungeonRoad ()
	{
	}
	
	/// <summary>
	/// Returns a <see cref="System.String"/> that represents the current <see cref="DungeonRoad"/>.
	/// </summary>
	/// <returns>
	/// A <see cref="System.String"/> that represents the current <see cref="DungeonRoad"/>.
	/// </returns>
	public override string ToString ()
	{
		return "Road { x1:" + x1 + ", x2:" + x2 + ", y1:" + y1 + ", y2:" + y2 + "}";
	}
}

[System.Serializable]
public class DungeonObject
{
	public int code;
	public DungeonObjectType type;
	public string prefabReference;
	public Vector3 offset;
	public Quaternion rotation;
	
	public DungeonObject (int code, DungeonObjectType type, string prefabReference)
	{
		this.code = code;
		this.type = type;
		this.prefabReference = prefabReference;
	}
}

public enum DungeonObjectType
{
	Ceiling,
	Floor,
	LightSource,
	Passage,
	Wall
}
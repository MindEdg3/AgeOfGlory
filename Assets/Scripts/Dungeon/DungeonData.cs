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
				_tiles = null;
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
				_tiles = null;
				roads = value;
			}
		}
	}
	
	private DungeonDataTile[,] _tiles;
	
	/// <summary>
	/// The tiles of this dungeon. If rooms or roads had been changed, tiles need to be recreated.
	/// </summary>
	/// <value>
	/// The tiles of this dungeon.
	/// </value>
	public DungeonDataTile[,] Tiles {
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
	#endregion
	
	#region Methods
	/// <summary>
	/// Retrieves the tiles of this dungeon.
	/// </summary>
	/// <returns>
	/// The tiles of this dungeon.
	/// </returns>
	public DungeonDataTile[,] GetTiles ()
	{
		DungeonDataTile[,] ret = new DungeonDataTile[width, height];
		
		for (int j = 0; j < height; j++) {
			for (int i = 0; i < width; i++) {
				ret [i, j] = new DungeonDataTile ();
			}
		}
		
		for (int k = 0; k < roads.Count; k++) {
			for (int j = 0; j <= roads[k].y2 - roads[k].y1; j++) {
				for (int i = 0; i <= roads[k].x2 - roads[k].x1; i++) {
					ret [i + roads [k].x1, j + roads [k].y1].road = roads [k];
				}
			}
		}
		
		for (int k = 0; k < rooms.Count; k++) {
			for (int j = 0; j < rooms[k].height; j++) {
				for (int i = 0; i < rooms[k].width; i++) {
					ret [i + rooms [k].x, j + rooms [k].y].room = rooms [k];
				}
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
		
		ret += "Rooms: ";
		for (int i = 0; i < rooms.Count; i++) {
			ret += ("Room" + i + " (" + Rooms [i].x + ", " + Rooms [i].y + ", " + Rooms [i].width + ", " + Rooms [i].height + "), ");
		}
		
		ret += "\nRoads: ";
		for (int i = 0; i < rooms.Count; i++) {
			ret += ("Road" + i + " (" + Roads [i].x1 + ", " + Roads [i].y1 + ", " + Roads [i].x2 + ", " + Roads [i].y2 + "), ");
		}
		
		ret += "\n\nMap:\n";
		for (int j = 0; j < height; j++) {
			for (int i = 0; i < width; i++) {
				if (Tiles [i, j].room != null) {
					ret += "0 ";
				} else if (Tiles [i, j].road != null) {
					ret += "X ";
				} else {
					ret += "- ";
				}
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
		return RandomizeNewDungeon (16, 16);
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

}
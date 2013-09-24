using UnityEngine;
using System.Collections.Generic;

public class DungeonData : ScriptableObject
{
	#region Constants
	private const int MAX_ROOMS_DEFAULT = 32;
	private const int MIN_ROOMS_SIZE = 2;
	private const int MAX_ROOMS_SIZE = 8;
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
		
		//UNDONE: roads
		
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
		ret.rooms = PlaceRooms (
			DungeonData.MAX_ROOMS_DEFAULT,
			DungeonData.MIN_ROOMS_SIZE,
			DungeonData.MAX_ROOMS_SIZE,
			width,
			height,
			ret.entranceX,
			ret.entranceY
		);
		
		ret.roads = PlaceRoads (ret.rooms);
		
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
			DungeonRoom newRoom = new DungeonRoom ();
			
			// set base values
			newRoom.width = Random.Range (minRoomSize, maxRoomSize + 1);
			newRoom.height = Random.Range (minRoomSize, maxRoomSize + 1);
			newRoom.x = Random.Range (0, dungeonWidth - newRoom.width);
			newRoom.y = Random.Range (0, dungeonHeight - newRoom.height);
			
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

	private static List<DungeonRoad> PlaceRoads (List<DungeonRoom> rooms)
	{
		List<DungeonRoad> ret = new List<DungeonRoad> ();
		
		// UNDONE: Roads placing
		
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
using UnityEngine;
using System.Collections;

/// <summary>
/// Dungeon utilities.
/// </summary>
public static class DungeonUtils
{
	/// <summary>
	/// The width and height of the tile.
	/// </summary>
	public static float TileSize = 4;
	
	/// <summary>
	/// Gets position in center of the tile at specified index.
	/// </summary>
	/// <returns>
	/// The position by index.
	/// </returns>
	/// <param name='i'>
	/// I tile index.
	/// </param>
	/// <param name='j'>
	/// J tile index.
	/// </param>
	public static Vector3 GetPositionByIndex (int i, int j)
	{
		Vector3 ret;
		
		ret = new Vector3 (i * TileSize, 0f, -j * TileSize);
		
		return ret;
	}
	
	/// <summary>
	/// Gets the position between tiles specified by indexes.
	/// </summary>
	/// <returns>
	/// The position between specified tiles.
	/// </returns>
	/// <param name='i1'>
	/// First tile I index.
	/// </param>
	/// <param name='j1'>
	/// First tile J index.
	/// </param>
	/// <param name='i2'>
	/// Second tile I index.
	/// </param>
	/// <param name='j2'>
	/// Second tile J index.
	/// </param>
	public static Vector3 GetPositionBetweenIndexes (int i1, int j1, int i2, int j2)
	{
		Vector3 ret;
		
		ret = new Vector3 (((i1 + i2) * 0.5f) * TileSize, 0f, -((j1 + j2) * 0.5f) * TileSize);
		
		return ret;
	}
}

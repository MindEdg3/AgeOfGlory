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
	public static float TileSize = 4f;
	
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
	public static Vector3 GetPositionBetweenTilesByIndex (int i1, int j1, int i2, int j2)
	{
		Vector3 ret;
		
		ret = new Vector3 (((i1 + i2) * 0.5f) * TileSize, 0f, -((j1 + j2) * 0.5f) * TileSize);
		
		return ret;
	}
	
	/// <summary>
	/// Get coded pepresentation of the tile indexes.
	/// </summary>
	/// <returns>
	/// The code of tile indexes.
	/// </returns>
	/// <param name='i'>
	/// X index.
	/// </param>
	/// <param name='j'>
	/// Y index.
	/// </param>
	public static int GetCodeOfTileByIndex (int i, int j)
	{
		return ((i - 32768) << 16) | j;
	}
	
	/// <summary>
	/// Decode code to tile indexes.
	/// </summary>
	/// <param name='code'>
	/// Code.
	/// </param>
	/// <param name='i'>
	/// X out index.
	/// </param>
	/// <param name='j'>
	/// Y out index.
	/// </param>
	public static void GetIndexesOfTileByCode (int code, out int i, out int j)
	{
		i = (code >> 16) + 32768;
		j = code & 0xFFFF;
	}
	
	/// <summary>
	/// Gets the rotation of object directed from one tile to other.
	/// </summary>
	/// <returns>
	/// The directed rotation.
	/// </returns>
	/// <param name='fromI'>
	/// I of first tile.
	/// </param>
	/// <param name='fromJ'>
	/// J of first tile.
	/// </param>
	/// <param name='toI'>
	/// I of second tile.
	/// </param>
	/// <param name='toJ'>
	/// J of second tile.
	/// </param>
	public static Quaternion GetRotationFromToTile (int fromI, int fromJ, int toI, int toJ)
	{
		Quaternion rot = Quaternion.identity;
		
		if (toI == fromI && fromJ > toJ) {
			rot = Quaternion.identity;
		} else if (fromI < toI && fromJ == toJ) {
			rot = Quaternion.Euler (0f, 90f, 0f);
		} else if (fromI == toI && fromJ < toJ) {
			rot = Quaternion.Euler (0f, 180f, 0f);
		} else if (fromI > toI && fromJ == toJ) {
			rot = Quaternion.Euler (0f, 270f, 0f);
		}
		
		return rot;
	}
	
	/// <summary>
	/// Get coded representaion of indexes of two tiles
	/// </summary>
	/// <returns>
	/// The code of tiles indexes.
	/// </returns>
	/// <param name='i1'>
	/// First tile X index.
	/// </param>
	/// <param name='j1'>
	/// First tile Y index.
	/// </param>
	/// <param name='i2'>
	/// Second tile X index.
	/// </param>
	/// <param name='j2'>
	/// Second tile Y index.
	/// </param>
	public static int GetCodeBetweenTilesByIndex (int i1, int j1, int i2, int j2)
	{
		return ((i1 - 128) << 24) | (j1 << 16) | (i2 << 8) | j2;
	}
	
	/// <summary>
	/// Decode code to tile indexes.
	/// </summary>
	/// <param name='code'>
	/// Code.
	/// </param>
	/// <param name='i1'>
	/// First tile X index.
	/// </param>
	/// <param name='j1'>
	/// First tile Y index.
	/// </param>
	/// <param name='i2'>
	/// Second tile X index.
	/// </param>
	/// <param name='j2'>
	/// Second tile Y index.
	/// </param>
	public static void GetIndexesBetweenTilesByCode (int code, out int i1, out int j1, out int i2, out int j2)
	{
		i1 = (code >> 24) + 128;
		j1 = ((code >> 16) & 255);
		i2 = ((code >> 8) & 255);
		j2 = (code & 255);
	}
}

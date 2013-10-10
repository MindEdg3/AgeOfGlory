using UnityEngine;
using System.Collections;

public class CreatureEntity : DungeonEntity
{
	public float moveSpeed;
	[System.NonSerialized]
	public int _currentX;
	[System.NonSerialized]
	public int _currentY;
	protected Vector3 _targetPosition;
	protected Direction _currentDirection;
	
	/// <summary>
	/// Moves to tile according to pressed key realtive to current direction. E.g., if player directed Backwards and pressed key Strafe Right,
	/// player will move to tile in the left, relative to world coordinates.
	/// </summary>
	/// <returns>
	/// Did moving succeed or not.
	/// </returns>
	/// <param name='targetDirectionRelative'>
	/// Target direction relative to current direction.
	/// </param>
	protected bool MoveTo (Direction targetDirectionRelative)
	{
		bool ret = false;
		
		// Get <see cref="Direction"/> of target tile
		Direction targetDirectionAbsolute = (Direction)(((byte)_currentDirection + (byte)targetDirectionRelative) % 4);
		
		// set target position
		switch (targetDirectionAbsolute) {
		case Direction.Forward:
			if (CheckTileMoveability (_currentX, _currentY - 1)) {
				_targetPosition = DungeonUtils.GetPositionByIndex (_currentX, --_currentY);
				ret = true;
			}
			break;
		case Direction.Right:
			if (CheckTileMoveability (_currentX + 1, _currentY)) {
				_targetPosition = DungeonUtils.GetPositionByIndex (++_currentX, _currentY);
				ret = true;
			}
			break;
		case Direction.Backward:
			if (CheckTileMoveability (_currentX, _currentY + 1)) {
				_targetPosition = DungeonUtils.GetPositionByIndex (_currentX, ++_currentY);
				ret = true;
			}
			break;
		case Direction.Left:
			if (CheckTileMoveability (_currentX - 1, _currentY)) {
				_targetPosition = DungeonUtils.GetPositionByIndex (--_currentX, _currentY);
				ret = true;
			}
			break;
		default:
			break;
		}
		
		return ret;
	}
	
	/// <summary>
	/// Checks the tile moveability.
	/// </summary>
	/// <returns>
	/// The tile moveability.
	/// </returns>
	/// <param name='tileX'>
	/// X tile index.
	/// </param>
	/// <param name='tileY'>
	/// Y tile index.
	/// </param>
	protected bool CheckTileMoveability (int tileX, int tileY)
	{
		if (tileX >= 0 && tileX < Dm.CurrentDungeon.width && 
			tileY >= 0 && tileY < Dm.CurrentDungeon.height) {
			
			DungeonDataTile tile = Dm.CurrentDungeon.Tiles [tileX, tileY];
		
			return tile.road != null || tile.room != null;
		} else {
			return false;
		}
	}
	
	/// <summary>
	/// Turn creature.
	/// </summary>
	/// <param name='isToRight'>
	/// Is to turn to right.
	/// </param>
	protected virtual void Turn (bool isToRight)
	{
		// switch <see cref="Direction"/> to left or right direction
		byte newDirection = (byte)(((byte)_currentDirection + (isToRight ? 1 : 3)) % 4);
		_currentDirection = (Direction)(newDirection);
		Tr.rotation = Quaternion.Euler (0f, newDirection * 90f, 0f);
	}
}

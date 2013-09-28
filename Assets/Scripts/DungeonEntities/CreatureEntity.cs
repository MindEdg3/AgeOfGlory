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
	/// <param name='targetDirectionRelative'>
	/// Target direction relative to current direction.
	/// </param>
	protected void MoveTo (Direction targetDirectionRelative)
	{
		// Get <see cref="Direction"/> of target tile
		Direction targetDirectionAbsolute = (Direction)(((byte)_currentDirection + (byte)targetDirectionRelative) % 4);
		
		// set target position
		switch (targetDirectionAbsolute) {
		case Direction.Forward:
			_targetPosition = DungeonUtils.GetPositionByIndex (_currentX, --_currentY);
			break;
		case Direction.Right:
			_targetPosition = DungeonUtils.GetPositionByIndex (++_currentX, _currentY);
			break;
		case Direction.Backward:
			_targetPosition = DungeonUtils.GetPositionByIndex (_currentX, ++_currentY);
			break;
		case Direction.Left:
			_targetPosition = DungeonUtils.GetPositionByIndex (--_currentX, _currentY);
			break;
		default:
			break;
		}
	}
	
	/// <summary>
	/// Turn creature.
	/// </summary>
	/// <param name='isToRight'>
	/// Is to turn to right.
	/// </param>
	protected void Turn (bool isToRight)
	{
		// switch <see cref="Direction"/> to left or right direction
		byte newDirection = (byte)(((byte)_currentDirection + (isToRight ? 1 : 3)) % 4);
		_currentDirection = (Direction)(newDirection);
		Tr.rotation = Quaternion.Euler (0f, newDirection * 90f, 0f);
	}
}

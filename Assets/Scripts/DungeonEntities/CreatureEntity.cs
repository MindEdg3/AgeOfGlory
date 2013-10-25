using UnityEngine;
using System.Collections;

public class CreatureEntity : DungeonEntity, ITurnBasedAble
{
	public float moveSpeed;
	[System.NonSerialized]
	public int _currentI;
	[System.NonSerialized]
	public int _currentJ;
	protected Vector3 _targetPosition;
	protected Direction _currentDirection;
	protected bool _activeTurn;
	
	protected override void Start ()
	{
		GameManager.Instance.RegisterTurnBasedClient (this);
	}
	
	protected override void Update ()
	{
		base.Update ();
		
		if (_activeTurn) {
			EndTurn ();
		}
	}
	
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
		if (CheckTileMoveability (targetDirectionAbsolute)) {
			switch (targetDirectionAbsolute) {
			case Direction.Forward:
				_targetPosition = DungeonUtils.GetPositionByIndex (_currentI, --_currentJ);
				ret = true;
				break;
			case Direction.Right:
				_targetPosition = DungeonUtils.GetPositionByIndex (++_currentI, _currentJ);
				ret = true;
				break;
			case Direction.Backward:
				_targetPosition = DungeonUtils.GetPositionByIndex (_currentI, ++_currentJ);
				ret = true;
				break;
			case Direction.Left:
				_targetPosition = DungeonUtils.GetPositionByIndex (--_currentI, _currentJ);
				ret = true;
				break;
			default:
				break;
			}
		}
		
		return ret;
	}

	/// <summary>
	/// Checks the tile moveability.
	/// </summary>
	/// <returns>
	/// Can creature make move to tile in specified direction from current tile or can not.
	/// </returns>
	/// <param name='moveDirection'>
	/// Absolute direction of target tile relatively to current
	/// </param>
	protected bool CheckTileMoveability (Direction moveDirection)
	{
		DungeonDataTile tile = Dm.CurrentDungeon.Tiles [DungeonUtils.GetCodeOfTileByIndex (_currentI, _currentJ)];
		Quaternion rot;
		
		// Get rotation that should have wall to stop moving
		switch (moveDirection) {
		case Direction.Forward:
			rot = DungeonUtils.GetRotationFromToTile (0, 0, 0, 1);
			break;
		case Direction.Right:
			rot = DungeonUtils.GetRotationFromToTile (1, 0, 0, 0);
			break;
		case Direction.Backward:
			rot = DungeonUtils.GetRotationFromToTile (0, 1, 0, 0);
			break;
		case Direction.Left:
			rot = DungeonUtils.GetRotationFromToTile (0, 0, 1, 0);
			break;
		default:
			return false;
		}
		
		// Check if there is any wall nearly rotated against to movementDirection
		for (int i = 0; i < tile.Objects.Count; i++) {
			if (tile.Objects [i].type == DungeonObjectType.Wall && Quaternion.Angle (rot, tile.Objects [i].rotation) < 10) {
				return false;
			}
		}
		
		return true;
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

	#region ITurnBasedAble implementation
	public int RoundsToPass {
		get;
		set;
	}
	
	public virtual void TakeTurn ()
	{
		_activeTurn = true;
	}

	public virtual void EndTurn ()
	{
		_activeTurn = false;
		GameManager.Instance.ClientEndedTurn ();
	}
	#endregion
}

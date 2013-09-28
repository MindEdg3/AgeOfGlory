using UnityEngine;
using System.Collections;

/// <summary>
/// Player dungeon controller.
/// </summary>
public class PlayerDungeonController : DungeonEntity
{
	public float speed;
	private Vector3 _targetPosition;
	private Direction _currentDirection;
	private int _currentX = 16;
	private int _currentY = 16;
	
	#region Properties
	private PlayerState _state;
	
	/// <summary>
	/// Gets or sets the state of player.
	/// </summary>
	/// <value>
	/// Player state.
	/// </value>
	public PlayerState State {
		get {
			return this._state;
		}
		set {
			_state = value;
		}
	}
	
	protected Transform _tr;
	
	public Transform Tr {
		get {
			if (this._tr == null) {
				this._tr = transform;
			}
			return this._tr;
		}
	}
	#endregion
	
	// Use this for initialization
	void Start ()
	{
		State = PlayerState.Idle;
	}
	
	// Update is called once per frame
	protected override void Update ()
	{
		base.Update ();
		
		switch (State) {
		case PlayerState.Inactive:
			break;
		case PlayerState.Idle:
			HandleIdleState ();
			break;
		case PlayerState.Move:
			HandleMoveState ();
			break;
		default:
			break;
		}
	}
	
	/// <summary>
	/// Handles the Idle state. Awaiting for player input.
	/// </summary>
	protected void HandleIdleState ()
	{
		if (Input.GetButtonDown ("Right")) {
			Turn (true);
		} else if (Input.GetButtonDown ("Left")) {
			Turn (false);
		} else if (Input.GetButton ("Forward")) {
			MoveTo (Direction.Forward);
		} else if (Input.GetButton ("Backward")) {
			MoveTo (Direction.Backward);
		} else if (Input.GetButton ("Strafe Right")) {
			MoveTo (Direction.Right);
		} else if (Input.GetButton ("Strafe Left")) {
			MoveTo (Direction.Left);
		}
	}
	
	/// <summary>
	/// Handles the state of the move. Moves player.
	/// </summary>
	protected void HandleMoveState ()
	{
		Vector3 nextPosition = Vector3.MoveTowards (Tr.position, _targetPosition, speed * Time.deltaTime);
		if (Tr.position.Equals (nextPosition)) {
			State = PlayerState.Idle;
		} else {
			Tr.position = nextPosition;
		}	
	}
	
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
		
		State = PlayerState.Move;
	}
	
	/// <summary>
	/// Turn Player.
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

/// <summary>
/// Player state.
/// </summary>
public enum PlayerState : byte
{
	Inactive = 0,
	Idle = 1,
	Move = 2
}

/// <summary>
/// Direction.
/// </summary>
public enum Direction : byte
{
	Forward = 0,
	Right = 1,
	Backward = 2,
	Left = 3
}
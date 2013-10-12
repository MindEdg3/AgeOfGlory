using UnityEngine;
using System.Collections;

/// <summary>
/// Player dungeon controller.
/// </summary>
public class PlayerDungeonController : CreatureEntity
{
	public float rotationSpeed;
	
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
	#endregion
	
	// Use this for initialization
	protected override void Start ()
	{
	}
	
	Vector3 lastPosition;
	
	// Update is called once per frame
	protected override void Update ()
	{
		lastPosition = transform.position;
		
		
		base.Update ();
		
		if (Input.GetKeyDown (KeyCode.Space)) {
			Light l = GetComponentInChildren<Light> ();
			l.enabled = !l.enabled;
		}
		
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
		
		Tr.rotation = Quaternion.RotateTowards (Tr.rotation, Quaternion.Euler (0f, (byte)_currentDirection * 90f, 0f), Time.deltaTime * rotationSpeed);
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
			if (MoveTo (Direction.Forward)) {
				State = PlayerState.Move;
			}
		} else if (Input.GetButton ("Backward")) {
			if (MoveTo (Direction.Backward)) {
				State = PlayerState.Move;
			}
		} else if (Input.GetButton ("Strafe Right")) {
			if (MoveTo (Direction.Right)) {
				State = PlayerState.Move;
			}
		} else if (Input.GetButton ("Strafe Left")) {
			if (MoveTo (Direction.Left)) {
				State = PlayerState.Move;
			}
		}
	}
	
	/// <summary>
	/// Handles the state of the move. Moves player.
	/// </summary>
	protected void HandleMoveState ()
	{
		Vector3 nextPosition = Vector3.MoveTowards (Tr.position, _targetPosition, moveSpeed * Time.deltaTime);
		if (Tr.position.Equals (nextPosition)) {
			State = PlayerState.Idle;
		} else {
			Tr.position = nextPosition;
		}	
	}
	
	protected override void Turn (bool isToRight)
	{
		// switch <see cref="Direction"/> to left or right direction
		byte newDirection = (byte)(((byte)_currentDirection + (isToRight ? 1 : 3)) % 4);
		_currentDirection = (Direction)(newDirection);
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
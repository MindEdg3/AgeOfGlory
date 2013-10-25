using UnityEngine;
using System.Collections;

/// <summary>
/// Player dungeon controller.
/// </summary>
public class PlayerDungeonController : CreatureEntity
{
	#region Fields
	public float rotationSpeed;
	private bool _didPlayerMoved;
	#endregion
	
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
	
	#region Methods
	// Use this for initialization
	protected override void Start ()
	{
		base.Start ();
		Dm.UpdateVisibleTiles (_currentI, _currentJ);
		_targetPosition = Tr.position;
	}
	
	// Update is called once per frame
	protected override void Update ()
	{	
		if (_activeTurn) {
			#region Temp light
			if (Input.GetKeyDown (KeyCode.Space)) {
				Light l = GetComponentInChildren<Light> ();
				l.enabled = !l.enabled;
			}
			#endregion
			
			Vector3 nextPosition = Vector3.MoveTowards (Tr.position, _targetPosition, moveSpeed * Time.deltaTime);
			
			if (!_didPlayerMoved) {
				if (Input.GetButtonDown ("Right")) {
					Turn (true);
				} else if (Input.GetButtonDown ("Left")) {
					Turn (false);
				}
				
				HandleMovement ();
				
				Tr.rotation = Quaternion.RotateTowards (Tr.rotation, Quaternion.Euler (0f, (byte)_currentDirection * 90f, 0f), Time.deltaTime * rotationSpeed);
			} else if (Tr.position.Equals (nextPosition)) {
				EndTurn ();
			} else {
				Tr.position = Vector3.MoveTowards (Tr.position, _targetPosition, moveSpeed * Time.deltaTime);
			}
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
			if (MoveTo (Direction.Forward)) {
				State = PlayerState.Move;
				Dm.UpdateVisibleTiles (_currentI, _currentJ);
			}
		} else if (Input.GetButton ("Backward")) {
			if (MoveTo (Direction.Backward)) {
				State = PlayerState.Move;
				Dm.UpdateVisibleTiles (_currentI, _currentJ);
			}
		} else if (Input.GetButton ("Strafe Right")) {
			if (MoveTo (Direction.Right)) {
				State = PlayerState.Move;
				Dm.UpdateVisibleTiles (_currentI, _currentJ);
			}
		} else if (Input.GetButton ("Strafe Left")) {
			if (MoveTo (Direction.Left)) {
				State = PlayerState.Move;
				Dm.UpdateVisibleTiles (_currentI, _currentJ);
			}
		}
	}
	
	/// <summary>
	/// Moves player.
	/// </summary>
	protected void HandleMovement ()
	{
		if (Input.GetButton ("Forward")) {
			if (MoveTo (Direction.Forward)) {
				_didPlayerMoved = true;
				Dm.UpdateVisibleTiles (_currentI, _currentJ);
			}
		} else if (Input.GetButton ("Backward")) {
			if (MoveTo (Direction.Backward)) {
				_didPlayerMoved = true;
				Dm.UpdateVisibleTiles (_currentI, _currentJ);
			}
		} else if (Input.GetButton ("Strafe Right")) {
			if (MoveTo (Direction.Right)) {
				_didPlayerMoved = true;
				Dm.UpdateVisibleTiles (_currentI, _currentJ);
			}
		} else if (Input.GetButton ("Strafe Left")) {
			if (MoveTo (Direction.Left)) {
				_didPlayerMoved = true;
				Dm.UpdateVisibleTiles (_currentI, _currentJ);
			}
		}	
	}
	
	protected override void Turn (bool isToRight)
	{
		// switch <see cref="Direction"/> to left or right direction
		byte newDirection = (byte)(((byte)_currentDirection + (isToRight ? 1 : 3)) % 4);
		_currentDirection = (Direction)(newDirection);
	}
	
	public override void TakeTurn ()
	{
		base.TakeTurn ();
		_didPlayerMoved = false;
	}
	#endregion
}

/// <summary>
/// Player state.
/// </summary>
public enum PlayerState : byte
{
	Idle = 0,
	Move = 1
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
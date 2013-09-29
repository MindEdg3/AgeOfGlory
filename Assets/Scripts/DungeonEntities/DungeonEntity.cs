using UnityEngine;
using System.Collections;

public class DungeonEntity : MonoBehaviour
{
	#region Properties
	protected Transform _tr;
	
	public Transform Tr {
		get {
			if (this._tr == null) {
				this._tr = transform;
			}
			return this._tr;
		}
	}
	
	private DungeonManager _myDungeonManager;

	public DungeonManager Dm {
		get {
			if (_myDungeonManager == null) {
				_myDungeonManager = DungeonManager.Instance;
			}
			return _myDungeonManager;
		}
	}
	#endregion

	// Use this for initialization
	protected virtual void Start ()
	{
	
	}
	
	// Update is called once per frame
	protected virtual void Update ()
	{
	
	}
}

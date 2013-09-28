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

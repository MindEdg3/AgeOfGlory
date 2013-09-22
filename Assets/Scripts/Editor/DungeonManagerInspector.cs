using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(DungeonManager))]
public class DungeonManagerInspector : Editor
{
	private DungeonManager _myTarget;

	public DungeonManager MyTarget {
		get {
			if (_myTarget == null) {
				_myTarget = (DungeonManager)target;
			}
			return _myTarget;	
		}
	}

	
	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI();
	}
}

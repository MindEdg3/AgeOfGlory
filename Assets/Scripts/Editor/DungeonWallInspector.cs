using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(DungeonWall))]
public class DungeonWallInspector : Editor
{

	private DungeonWall _myTarget;

	public DungeonWall MyTarget {
		get {
			if (_myTarget == null) {
				_myTarget = (DungeonWall)target;
			}
			return _myTarget;	
		}
	}

	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();
		
		string[] walls = DungeonManager.Instance.WallTypes;
		MyTarget.SelectedWall = EditorGUILayout.Popup ("Selected Wall", MyTarget.SelectedWall, walls);
	}
}

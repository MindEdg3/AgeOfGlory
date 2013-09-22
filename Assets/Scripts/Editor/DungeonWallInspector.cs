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
	
	public int SelectedWall {
		get {
			return MyTarget.SelectedWall;
		}
		set {
			if (MyTarget.SelectedWall != value) {
				MyTarget.SelectedWall = value;
				EditorUtility.SetDirty (MyTarget);
			}
		}
	}

	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();
		
		string[] walls = DungeonManager.Instance.WallTypes;
		SelectedWall = EditorGUILayout.Popup ("Selected Wall", SelectedWall, walls);
		
	}
}

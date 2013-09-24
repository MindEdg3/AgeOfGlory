using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class DungeonWall : MonoBehaviour
{
	[HideInInspector]
	[SerializeField]
	private int selectedWall;

	public int SelectedWall {
		get {
			return this.selectedWall;
		}
		set {
			if (selectedWall != value) {
				selectedWall = value;
				SetWall (DungeonManager.Instance.WallTypes [value]);
			}
		}
	}

	// Use this for initialization
	void Start ()
	{
		SetWall (DungeonManager.Instance.WallTypes [selectedWall]);
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	public void SetWall (string wallName)
	{
		GameObject nextGraphics = (Resources.Load (DungeonManager.WALLS_PATH + wallName) as GameObject).gameObject;
#if UNITY_EDITOR
		if (transform.childCount > 0) {
			DestroyImmediate (transform.GetChild(0).gameObject);
		}
#else
		
		if (transform.childCount > 0) {
			Destroy (transform.GetChild (0).gameObject);
		}
#endif
		GameObject newGraphics = Instantiate (nextGraphics) as GameObject;
		newGraphics.name = nextGraphics.name;
		newGraphics.transform.parent = transform;
		newGraphics.transform.localPosition = nextGraphics.transform.localPosition;
		newGraphics.transform.localRotation = Quaternion.identity;
	}
}

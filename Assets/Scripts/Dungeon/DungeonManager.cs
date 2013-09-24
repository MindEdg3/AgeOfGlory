using UnityEngine;
using System.Collections;

public class DungeonManager : MonoBehaviour
{
	public const string WALLS_PATH = "Prefabs/DungeonWalls/";
	public string[] arr_wallTypes;

	public string[] WallTypes {
		get {
			return this.arr_wallTypes;
		}
		set {
			arr_wallTypes = value;
		}
	}
	
	#region Singletone
	private static DungeonManager _instance;

	public static DungeonManager Instance {
		get {
			if (_instance == null) {
				_instance = Component.FindObjectOfType (typeof(DungeonManager)) as DungeonManager;
			}
			return _instance;
		}
	}
	#endregion
	
	// Use this for initialization
	void Start ()
	{
		Debug.Log (DungeonData.RandomizeNewDungeon ().ToString ());
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}

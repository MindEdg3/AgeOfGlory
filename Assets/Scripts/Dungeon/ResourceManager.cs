using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Resource manager.
/// </summary>
public class ResourceManager
{
	#region Constants
	public const string WALLS_PATH = "Prefabs/DungeonWalls/";
	public const string PASSAGES_PATH = "Prefabs/DungeonPassages/";
	public const string FLOORS_PATH = "Prefabs/DungeonFloors/";
	public const string CEILINGS_PATH = "Prefabs/DungeonCeilings/";
	public const string LIGHTS_PATH = "Prefabs/Lights/";
	#endregion
	
	#region Properties
	private static Dictionary<string, object> _objects;
	
	/// <summary>
	/// Collection of loaded objects
	/// </summary>
	/// <value>
	/// The loaded resources.
	/// </value>
	public static  Dictionary<string, object> Objects {
		get {
			if (_objects == null) {
				_objects = new Dictionary<string, object> ();
			}
			return _objects;
		}
	}
	#endregion
	
	#region Methods
	/// <summary>
	/// Gets the resource by type and name.
	/// </summary>
	/// <returns>
	/// The resource.
	/// </returns>
	/// <param name='objectType'>
	/// Dungeon object type.
	/// </param>
	/// <param name='name'>
	/// Object name.
	/// </param>
	public static object GetResource (DungeonObjectType objectType, string name)
	{
		string path = GetPathByType (objectType, name);
		
		if (Objects.ContainsKey (path)) {
			return Objects [path];
		} else {
			object o = Resources.Load (path);
			Objects.Add (path, o);
			return o;
		}
	}
	
	/// <summary>
	/// Gets the path to the resource according to its type.
	/// </summary>
	/// <returns>
	/// The path by type.
	/// </returns>
	/// <param name='objectType'>
	/// Object type.
	/// </param>
	/// <param name='name'>
	/// Name.
	/// </param>
	private static string GetPathByType (DungeonObjectType objectType, string name)
	{
		switch (objectType) {
		default:
			return null;
		case DungeonObjectType.Ceiling:
			return CEILINGS_PATH + name;
		case DungeonObjectType.Floor:
			return FLOORS_PATH + name;
		case DungeonObjectType.LightSource:
			return LIGHTS_PATH + name;
		case DungeonObjectType.Passage:
			return PASSAGES_PATH + name;
		case DungeonObjectType.Wall:
			return WALLS_PATH + name;
		}
	}
	#endregion
}

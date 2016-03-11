namespace Fabric.Internal.Editor.Resources
{
	using UnityEngine;
	using UnityEditor;
	using System.Collections;
	
	public class Manager
	{
		private static readonly string root = "Assets/Fabric/Editor/GUI/Resources/";

		public static Texture2D Load(string resource)
		{
			return AssetDatabase.LoadAssetAtPath(root + resource, typeof(Texture2D)) as Texture2D;
		}
	}
}

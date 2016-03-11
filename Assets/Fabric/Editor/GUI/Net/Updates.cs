namespace Fabric.Internal.Editor.Net {
	using UnityEngine;
	using System.Collections;
	using System;
	
	public class Updates {

		public static IEnumerator GetLatestVersion(string url, Action<Version> callback) {
			WWW www = new WWW (url);
			
			while (!www.isDone) {
				yield return null;
			}
			
			Version latestVersion = null;
			if (string.IsNullOrEmpty (www.error)) {
				latestVersion = new Version (www.text);
			}
			
			callback (latestVersion);
		}		

	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEditor;
using UnityEngine;

public class EditorHelpers
{
	/// <summary>
	/// Used to search all assets for a specific type.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <returns></returns>
	public static List<T> FindAssetsByType<T>() where T : UnityEngine.Object
	{
		List<T> assets = new List<T>();

		string[] guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(T).ToString().Replace("UnityEngine.", "")));

		for (int i = 0; i < guids.Length; i++)
		{
			string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
			T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
			if (asset != null)
			{
				assets.Add(asset);
			}
		}
		return assets;
	}
}

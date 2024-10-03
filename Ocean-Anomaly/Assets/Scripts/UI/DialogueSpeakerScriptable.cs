using System;
using System.IO;
using UnityEditor;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "DialogueSpeaker", menuName = "Scriptable Objects/Dialogue/Dialogue Speaker")]
public class DialogueSpeakerScriptable : ScriptableObject
{
	public new string name = string.Empty;
	public string NickName = string.Empty;
	public bool UseNickName = false;
	public Color DisplayColor = Color.white;
	private void OnValidate()
	{
		if (name == string.Empty)
		{
			name = Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(this));
		}
	}
	public void ToggleNickName(bool value)
	{
		UseNickName = value;
	}
}
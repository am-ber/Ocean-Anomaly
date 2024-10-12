using OceanAnomaly.Components;
using System;
using System.Collections;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
[CreateAssetMenu(fileName = "Dialogue", menuName = "Scriptable Objects/Dialogue/Dialogue Object")]
public class DialogueScriptable : ScriptableObject
{
	public new string name = string.Empty;
	public string DialogueText = string.Empty;
	public DialogueSpeakerScriptable Speaker;
	public SoundScriptable DialogueAudio;
	public float TextDisplaySpeed = 0.1f;
	public float DialogueHoldTime = 3.0f;
	public UnityEvent OnDialogue;

#if UNITY_EDITOR
	private void OnValidate()
	{
		if (name == string.Empty)
		{
			if (DialogueAudio != null)
			{
				name = $"{DialogueAudio.name} Dialogue";
			} else
			{
				name = Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(this));
			}
		}
	}
#endif

}
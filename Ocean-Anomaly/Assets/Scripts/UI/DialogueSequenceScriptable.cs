using OceanAnomaly.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
[CreateAssetMenu(fileName = "Dialogue Sequence", menuName = "Scriptable Objects/Dialogue/Dialogue Sequence")]
public class DialogueSequenceScriptable : ScriptableObject
{
	public new string name = string.Empty;
	public float additionalWait = 1.0f;
	public DialogueUI dialoguePrefab;
	public List<DialogueScriptable> dialogues = new List<DialogueScriptable>();
	public UnityEvent OnSequenceStart;
	public UnityEvent OnSequenceFinish;
	private void OnValidate()
	{
		if (name == string.Empty)
		{
			name = Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(this));
		}
	}
	public IEnumerator StartSequence(GameObject parentUI = null)
	{
		foreach (DialogueScriptable dialogueScriptable in dialogues)
		{
			// Create the dialogue UI
			DialogueUI dialogueUI = Instantiate(dialoguePrefab);
			// If a parent UI was given, then we can nest it.
			if (parentUI != null)
			{
				dialogueUI.transform.parent = parentUI.transform;
			}
			// Initialize the DialogueUI
			dialogueUI.SetDialogue(dialogueScriptable);
			// Waiting events
			yield return GlobalTools.WaitUntilEvent(dialogueUI.OnDialogueFinish);
			yield return new WaitForSeconds(additionalWait);
		}
	}

}

using OceanAnomaly.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public struct EventDialogue
{
	public DialogueScriptable Dialogue;
	public bool ShowDialogueBeforeEvent;
	public bool Triggered;
	public UnityEvent OnEvent;
}
public class DialogueManager : MonoBehaviour
{
	public GameObject UIParent;
	public List<EventDialogue> dialogueEvents = new List<EventDialogue>();
	public List<DialogueSequenceScriptable> dialogueSequences = new List<DialogueSequenceScriptable>();
	[ReadOnly]
	public DialogueSequenceScriptable currentSequence;
	private void SetupEvents()
	{

	}
	public void StartSequence(DialogueSequenceScriptable sequence)
	{
		Debug.Log($"Starting Dialogue Sequence: {sequence.name}");
		// Check for existing sequences and stop them
		if (currentSequence != null)
		{
			StopCoroutine(currentSequence.sequence);
			currentSequence = null;
		}
		// Setup new sequence
		sequence.OnSequenceFinish.AddListener(() => {
			currentSequence = null;
		});
		sequence.sequence = sequence.StartSequence(UIParent);
		StartCoroutine(sequence.sequence);
		currentSequence = sequence;
	}

}

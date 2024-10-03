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
	public void StartSequence(DialogueSequenceScriptable sequence)
	{

	}
}

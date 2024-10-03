using System.Collections;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Events;

public class DialogueUI : MonoBehaviour
{
	public TMP_Text SpeakerText;
	public TMP_Text DialogueText;
	public DialogueScriptable Dialogue;
	public bool ReadingText = false;
	public bool DialogueFinished = false;
	public UnityEvent OnDialogueFinish;
	private void Awake()
	{
		Initialize();
	}
	public void Initialize()
	{
		if (Dialogue != null)
		{
			// Set the speaker text
			if (Dialogue.Speaker != null)
			{
				SpeakerText.color = Dialogue.Speaker.DisplayColor;
				// Inline if to check if we're using nicknames
				SpeakerText.text = Dialogue.Speaker.UseNickName ? Dialogue.Speaker.NickName : Dialogue.Speaker.name;
			}
			// Set the dialogue text
			StartCoroutine(ReadText());
			Dialogue.OnDialogue?.Invoke();
		}
	}
	public void SetDialogue(DialogueScriptable dialogueScriptable)
	{
		Dialogue = dialogueScriptable;
		Initialize();
	}
	private IEnumerator ReadText()
	{
		if (Dialogue == null)
		{
			yield return null;
		}
		ReadingText = true;
		// Set the dialogue blank before reading it off
		DialogueText.text = string.Empty;
		for (int i = 0; i < Dialogue.DialogueText.Length; i++)
		{
			// Add each character 1 by 1 with the display speed
			DialogueText.text += Dialogue.DialogueText[i];
			yield return new WaitForSeconds(Dialogue.TextDisplaySpeed);
		}
		ReadingText = false;
		// Alert listeners on finishing the dialogue
		if (Dialogue.DialogueHoldTime >= 0)
		{
			yield return new WaitForSeconds(Dialogue.DialogueHoldTime);
			DialogueFinished = true;
		} else
		{
			yield return new WaitUntil(() => DialogueFinished);
		}
		OnDialogueFinish?.Invoke();
	}
}

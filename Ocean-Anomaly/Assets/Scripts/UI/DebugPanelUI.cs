using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[Serializable]
public struct DebugUI
{
	public string buttonText;
	public UnityEvent eventToTrigger;
}
public class DebugPanelUI : MonoBehaviour
{
	[SerializeField]
	private GameObject debugInputsPanel;
	[SerializeField]
	private Button buttonPrefab;
	[SerializeField]
	private List<DebugUI> debugEvents = new List<DebugUI>();
	private void Start()
	{
		foreach (DebugUI debugEvent in debugEvents)
		{
			Button debugButton = Instantiate(buttonPrefab);
			debugButton.transform.SetParent(debugInputsPanel.transform);
			debugButton.GetComponentInChildren<TMP_Text>().text = debugEvent.buttonText;
			debugButton.onClick.AddListener(() => {
				debugEvent.eventToTrigger.Invoke();
			});
		}
	}
}

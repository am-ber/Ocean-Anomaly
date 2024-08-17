using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
public class StatsScreen : MonoBehaviour
{
	public TextMeshProUGUI fpsText;
	public TextMeshProUGUI updateCountText;
	public TextMeshProUGUI fixedUpdateCountText;
	public TextMeshProUGUI devicesListText;
	public TextMeshProUGUI timeRunningText;
	[SerializeField]
	private GameObject playerInputDataDisplay;
	[SerializeField]
	private GameObject playerInputAction;
	[SerializeField]
	private Color inputColor = new Color(90, 90, 90, 128);
	[SerializeField]
	private Color inputHeldColor = new Color(200, 90, 90, 128);
	private List<(GameObject, PlayerInputController, PlayerInputDataDisplay[])> playerDisplayInputObjects;
	private float deltaTime;
	private float updateCount = 0;
	private float fixedUpdateCount = 0;
	void OnDisable()
	{
		Debug.Log("Stats menu disabled.");
		updateCount = 0;
		fixedUpdateCount = 0;
		// I can't think of a more effecient way to do this, because you can't interact with an object disabled
		foreach ((GameObject, PlayerInputController, PlayerInputDataDisplay[]) playerInputObject in playerDisplayInputObjects)
		{
			Destroy(playerInputObject.Item1);
		}
	}
	void OnEnable()
	{
		Debug.Log("Stats menu enabled.");
		playerDisplayInputObjects = new List<(GameObject, PlayerInputController, PlayerInputDataDisplay[])>();
		foreach (PlayerInputController player in GlobalManager.players)
		{
			AddPlayerToStats(player);
		}
		StartCollectionCoroutines();
	}
	void Update()
	{
		updateCount += 1;
	}
	void FixedUpdate()
	{
		fixedUpdateCount += 1;
	}
	void LateUpdate()
	{
		deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
	}
	public void AddPlayerToStats(PlayerInputController player)
	{
		// Add an instance of our prefab for displaying players and pair it with the player calling this
		GameObject playerDisplay = Instantiate(playerInputDataDisplay, this.transform);
		playerDisplay.GetComponentInChildren<TextMeshProUGUI>().text = $"PlayerID: {player.PlayerInput.playerIndex}";
		List<PlayerInputDataDisplay> inputBufferDisplay = new List<PlayerInputDataDisplay>();
		for (int i = 0; i < player.InputBufferLimit; i++)
		{
			GameObject inputDisplay = Instantiate(playerInputAction, playerDisplay.transform.Find("PlayerActionsHolderDisplay"));
			inputBufferDisplay.Add(inputDisplay.GetComponent<PlayerInputDataDisplay>());
			inputDisplay.SetActive(false);
		}
		playerDisplayInputObjects.Add((playerDisplay, player, inputBufferDisplay.ToArray()));
	}
	private void StartCollectionCoroutines()
	{
		// These will NOT run unless the debug menu is open to save system resources
		StartCoroutine(UpdateStats());
		StartCoroutine(UpdateCycleCounts());
	}
	private IEnumerator UpdateStats()
	{
		while (true)
		{
			yield return new WaitForSeconds(0.1f);
			float fps = 1.0f / deltaTime;
			fpsText.text = "FPS: " + Mathf.Ceil(fps).ToString();
			foreach ((GameObject, PlayerInputController, PlayerInputDataDisplay[]) playerInputObject in playerDisplayInputObjects)
			{
				InstantiateAllInputs(playerInputObject);
			}
		}
	}
	private IEnumerator UpdateCycleCounts()
	{
		while(true)
		{
			yield return new WaitForSeconds(1);
			// Show the total updates in seconds
			updateCountText.text = "Updates: " + updateCount.ToString();
			fixedUpdateCountText.text = "FUpdates: " + fixedUpdateCount.ToString();
			updateCount = 0;
			fixedUpdateCount = 0;
			devicesListText.text = buildDeviceList(GlobalManager.GetInputDevices());
			// Grab game uptime
			TimeSpan t = TimeSpan.FromSeconds(Time.realtimeSinceStartupAsDouble);
			timeRunningText.text = t.ToString(@"hh\:mm\:ss");
		}
	}
	private void InstantiateAllInputs((GameObject, PlayerInputController, PlayerInputDataDisplay[]) displayObject)
	{
		PlayerInputData[] reversedBuffer = displayObject.Item2.InputBuffer.Reverse().ToArray();
		for (int i = 0; i < reversedBuffer.Length; i++)
		{
			displayObject.Item3[i].gameObject.SetActive(true);
			displayObject.Item3[i].Set(reversedBuffer[i]);
			if (i <= 0)
			{
				if (displayObject.Item3[i].Data.Held)
					displayObject.Item3[i].GetComponent<Image>().color = inputHeldColor;
				else
				{
					Color result = inputHeldColor;
					StartCoroutine(ColorLerp((sender) =>
					{
						result = sender;
					},inputHeldColor, inputColor, 0.1f));
					displayObject.Item3[i].GetComponent<Image>().color = result;
				}
			}
		}
	}
	private IEnumerator<Color> ColorLerp(Action<Color> callBack, Color c1, Color c2, float increment)
	{
		if (increment > 1 | increment < 0)
		{
			yield break;
		}
		for (float i = 0; i < 1; i += increment)
		{
			Color sender = Color.Lerp(c1, c2, i);
			yield return sender;
			callBack(sender);
		}
	}
	private string buildDeviceList(List<InputDevice> devices)
	{
		string sender = "Device List:\n";
		foreach (InputDevice device in devices)
		{
			sender += $"{device.name}\n";
		}
		return sender;
	}
}

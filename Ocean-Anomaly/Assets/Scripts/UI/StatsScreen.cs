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

namespace OceanAnomaly.UI
{
	public class StatsScreen : MonoBehaviour
	{
		public TextMeshProUGUI fpsText;
		public TextMeshProUGUI updateCountText;
		public TextMeshProUGUI fixedUpdateCountText;
		public TextMeshProUGUI deltaTimeText;
		public TextMeshProUGUI activeDeviceText;
		public TextMeshProUGUI devicesListText;
		public TextMeshProUGUI timeRunningText;
		[SerializeField]
		private Color inputColor = new Color(90, 90, 90, 128);
		[SerializeField]
		private Color inputHeldColor = new Color(200, 90, 90, 128);
		private float deltaTime;
		private float updateCount = 0;
		private float fixedUpdateCount = 0;
		void OnDisable()
		{
			Debug.Log("Stats menu disabled.");
			updateCount = 0;
			fixedUpdateCount = 0;
		}
		void OnEnable()
		{
			Debug.Log("Stats menu enabled.");
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
				deltaTimeText.text = "dTime: " + Time.deltaTime.ToString("F3");
			}
		}
		private IEnumerator UpdateCycleCounts()
		{
			while (true)
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
		public void OnLastDeviceUsed(InputDevice device)
		{
			if (activeDeviceText != null)
			{
				activeDeviceText.text = $"Active Device:\n{device.displayName}";
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
}
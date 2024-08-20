using OceanAnomaly.Controllers;
using OceanAnomaly.Managers;
using OceanAnomaly.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

namespace OceanAnomaly
{
	public class GlobalManager : MonoBehaviour
	{
		public static GlobalManager Instance;
		public static DateTime LaunchTime;
		public bool displayStats = false;
		public StatsScreen statsScreen;
		[SerializeField]
		private GameObject statsScreenPrefab;
		public Canvas bindPlayerScreen;
		public PlayerInputManager playerInputManager;
		public PlayerVirtualCameraController playerVirtualCamera;
		public EnemyFieldManager enemyFieldManager;
		public AudioManager audioManager;
		[SerializeField]
		private GameObject audioManagerPrefab;
		public List<PlayerInput> players;
		void Awake()
		{
			if (Instance == null)
			{
				Instance = this;
			} else
			{
				Destroy(this);
				return;
			}
			DontDestroyOnLoad(gameObject);

			InputSystem.onDeviceChange += deviceChange;
			LaunchTime = DateTime.Now;
		}
		void Start()
		{
			if (playerInputManager == null)
				playerInputManager = GetComponent<PlayerInputManager>();

			players = new List<PlayerInput>();

			if (statsScreen == null)
			{
				statsScreen = GetComponentInChildren<StatsScreen>();
				if (statsScreen == null)
				{
					Instantiate(statsScreenPrefab);
				}
			}
			if (audioManager == null)
			{
				audioManager = AudioManager.Instance;
				if (audioManager == null)
				{
					Instantiate(audioManagerPrefab);
				}
			}
			statsScreen.gameObject.SetActive(displayStats);
		}
		void Update()
		{
			if (Input.GetKeyDown(KeyCode.F1))
			{
				displayStats = !displayStats;
				if (statsScreen != null)
				{
					statsScreen.gameObject.SetActive(displayStats);
				}
			}
		}
		public void OnPlayerJoined(PlayerInput playerInput)
		{
			Debug.Log($"Joined New Player ({playerInput.user.id})");
			if (bindPlayerScreen != null && bindPlayerScreen.gameObject.activeSelf)
			{
				bindPlayerScreen.gameObject.SetActive(false);
			}
			if (playerVirtualCamera != null)
			{
				playerVirtualCamera.CameraTargets.Add(playerInput.transform);
			}
			players.Add(playerInput);
		}
		public void OnEnemyJoin(GameObject enemyGameObject)
		{
			if (playerVirtualCamera != null)
			{
				playerVirtualCamera.CameraTargets.Add(enemyGameObject.transform);
			}
		}
		void deviceChange(InputDevice device, InputDeviceChange inputDeviceChange)
		{
			Debug.Log($"{device.displayName} changed!");
		}
		public static List<InputDevice> GetInputDevices()
		{
			return new List<InputDevice>(InputSystem.devices);
		}
		/// <summary>
		/// Figures the number of fixed frames from a given fixed time from the start of the game.
		/// </summary>
		/// <param name="fixedTime"></param>
		/// <returns></returns>
		public long FixedFramesFrom(float fixedTime = -1)
		{
			if (fixedTime < 0)
				fixedTime = Time.fixedTime;
			return Mathf.RoundToInt(fixedTime / Time.fixedDeltaTime);
		}
	}
}
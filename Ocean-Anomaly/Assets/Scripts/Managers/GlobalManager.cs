using OceanAnomaly.Controllers;
using OceanAnomaly.Managers;
using OceanAnomaly.Tools;
using OceanAnomaly.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

namespace OceanAnomaly
{
	public enum GameState
	{
		GamePlay,
		GamePlayPaused,
		GamePlayMenuNoPause,
		Cutscene,
		MainMenu
	}
	public class GlobalManager : Subject<GameState>
	{
		public static GlobalManager Instance;
		public static DateTime LaunchTime;
		public bool displayStats = false;
		[SerializeField]
		private GameState currentGameState = GameState.GamePlay;
		[Header("Required Objects")]
		public PlayerInputManager playerInputManager;
		public StatsScreen statsScreen;
		[SerializeField]
		private GameObject statsScreenPrefab;
		[SerializeField]
		public Canvas bindPlayerScreen;
		[SerializeField]
		private GameObject bindPlayerScreenPrefab;
		public PlayerVirtualCameraController playerVirtualCamera;
		[SerializeField]
		private GameObject playerVirtualCameraPrefab;
		public EnemyFieldManager enemyFieldManager;
		[SerializeField]
		private GameObject enemyFieldPrefab;
		public AudioManager audioManager;
		[SerializeField]
		private GameObject audioManagerPrefab;
		public List<PlayerInput> players;
		void Awake()
		{
			Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
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

			if (enemyFieldManager ==  null)
			{
				enemyFieldManager = gameObject.RecursiveFindComponentLocal<EnemyFieldManager>(enemyFieldPrefab);
			}
			if (playerVirtualCamera == null)
			{
				playerVirtualCamera = gameObject.RecursiveFindComponentLocal<PlayerVirtualCameraController>(playerVirtualCameraPrefab);
			}
		}
		void Start()
		{
			if (playerInputManager == null)
				playerInputManager = GetComponent<PlayerInputManager>();

			players = new List<PlayerInput>();

			if (statsScreen == null)
			{
				statsScreen = gameObject.RecursiveFindComponentLocal<StatsScreen>(statsScreenPrefab);
			}
			if (audioManager == null)
			{
				audioManager = AudioManager.Instance;
				if (audioManager == null)
				{
					audioManager = gameObject.RecursiveFindComponentLocal<AudioManager>(audioManagerPrefab);
				}
			}
			
			statsScreen.gameObject.SetActive(displayStats);
			
			SetGameState(currentGameState);
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
		public void SetGameState(GameState state)
		{
			// Call all observers that we just changed states
			Notify(state);
			// Also determine other things for now, but this will change to more stateful code
			switch (state)
			{
				case GameState.GamePlay:
					enemyFieldManager.gameObject.SetActive(true);
					playerVirtualCamera.gameObject.SetActive(true);
					break;
				case GameState.GamePlayPaused:
					playerInputManager.EnableJoining();
					enemyFieldManager.gameObject.SetActive(true);
					playerVirtualCamera.gameObject.SetActive(true);
					break;
				case GameState.Cutscene:
				case GameState.MainMenu:
					playerInputManager.DisableJoining();
					enemyFieldManager.gameObject.SetActive(false);
					playerVirtualCamera.gameObject.SetActive(false);
					break;
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
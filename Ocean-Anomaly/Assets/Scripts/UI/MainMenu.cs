using Cinemachine;
using OceanAnomaly.Attributes;
using OceanAnomaly.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
	public CinemachineVirtualCamera settingsMenuCamera;
	public CinemachineVirtualCamera mainMenuCamera;
	public string mainSceneName = string.Empty;
	[ReadOnly]
	[SerializeField]
	private bool playedMainMusic = false;
	public void StartGame()
	{
		Debug.Log("Started Game");
		if (mainSceneName == string.Empty)
		{
			SceneManager.LoadScene(mainSceneName);
		}
	}
	private void LateUpdate()
	{
		if (!playedMainMusic)
		{
			try
			{
				AudioManager.Instance.SetMainMusicPlaying(true);
				playedMainMusic = true;
			}
			catch
			{

			}
		}
	}
	public void SettingsMenu()
	{
		Debug.Log("Settings Menu");
		settingsMenuCamera.enabled = true;
		mainMenuCamera.enabled = false;
	}
	public void QuitGame()
	{
		Debug.Log("Quitted");
		Application.Quit();
	}
}

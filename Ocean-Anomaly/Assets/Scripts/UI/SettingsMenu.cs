using Cinemachine;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class SettingsMenu : MonoBehaviour
{
	public CinemachineVirtualCamera settingsMenuCamera;
	public CinemachineVirtualCamera mainMenuCamera;
	public AudioMixer mainAudioMixer;
	public void SetMasterVolume(float volume)
	{
		if (volume < -30)
		{
			volume = -80;
		}
		mainAudioMixer.SetFloat("MasterVolume", volume);
	}
	public void SetMusicVolume(float volume)
	{
		if (volume < -30)
		{
			volume = -80;
		}
		mainAudioMixer.SetFloat("MusicVolume", volume);
	}
	public void SetSoundEffectsVolume(float volume)
	{
		if (volume < -30)
		{
			volume = -80;
		}
		mainAudioMixer.SetFloat("SetSoundEffectsVolume", volume);
	}
	public void SetQuality(int qualityIndex)
	{
		Debug.Log($"Set quality to: {QualitySettings.names[qualityIndex]}");
		QualitySettings.SetQualityLevel(qualityIndex);
	}
	public void SetFullScreen(bool isFullScreen)
	{
		Debug.Log($"Toggled Fullscreen to: {isFullScreen}");
		Screen.fullScreen = isFullScreen;
	}
	public void BackToMainMenu()
	{
		Debug.Log("Back to Main Menu");
		mainMenuCamera.enabled = true;
		settingsMenuCamera.enabled = false;
	}
}

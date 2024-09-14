using Cinemachine;
using OceanAnomaly.Tools;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
	public CinemachineVirtualCamera settingsMenuCamera;
	public CinemachineVirtualCamera mainMenuCamera;
	public AudioMixer mainAudioMixer;
	[Header("UI References")]
	public Slider masterVolume;
	public Slider musicVolume;
	public Slider soundEffectsVolume;
	public Toggle fullScreenToggle;
	private void Start()
	{
		InitializeMenuFromSavedData(SaveSystem.LoadSettings());
	}
	public void InitializeMenuFromSavedData(SettingsSaveData settingsData)
	{
		masterVolume.value = settingsData.MasterVolume;
		musicVolume.value = settingsData.MusicVolume;
		soundEffectsVolume.value = settingsData.SoundEffectsVolume;
		fullScreenToggle.isOn = settingsData.IsFullScreen;
	}
	public void SetMasterVolume(float volume)
	{
		mainAudioMixer.SetFloat("MasterVolume", GlobalTools.dbLog(volume));
	}
	public void SetMusicVolume(float volume)
	{
		mainAudioMixer.SetFloat("MusicVolume", GlobalTools.dbLog(volume));
	}
	public void SetSoundEffectsVolume(float volume)
	{
		mainAudioMixer.SetFloat("SoundEffectsVolume", GlobalTools.dbLog(volume));
	}
	public void SetQuality(int qualityIndex)
	{
		Debug.Log($"Set quality to:[{qualityIndex}] {QualitySettings.names[qualityIndex]}");
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
		SaveSystem.SaveSettings(this);
		mainMenuCamera.enabled = true;
		settingsMenuCamera.enabled = false;
	}
}

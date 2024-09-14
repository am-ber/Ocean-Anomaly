using OceanAnomaly.Tools;
using System;
using UnityEngine;

[Serializable]
public class SettingsSaveData : SaveData
{
	public float MasterVolume;
	public float MusicVolume;
	public float SoundEffectsVolume;
	public bool IsFullScreen;
	public bool IsNew { get; set; }
	public SettingsSaveData()
	{
		IsNew = true;
		MasterVolume = 0.9f;        // This defaults to like -0.9 db maybe
		MusicVolume = 0.9f;         // This defaults to like -0.9 db probably
		SoundEffectsVolume = 0.9f;  // This defaults to like -0.9 db or something like that
		IsFullScreen = true;        // Default to full screen
	}
	public SettingsSaveData(SettingsMenu settingsMenu)
	{
		IsNew = false;
		MasterVolume = settingsMenu.masterVolume.value;
		MusicVolume = settingsMenu.musicVolume.value;
		SoundEffectsVolume = settingsMenu.soundEffectsVolume.value;
		IsFullScreen = settingsMenu.fullScreenToggle.isOn;
	}
}

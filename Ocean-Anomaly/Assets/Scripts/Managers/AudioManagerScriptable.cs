using OceanAnomaly;
using OceanAnomaly.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "AudioManager", menuName = "Scriptable Objects/Audio/Manager")]
public class AudioManagerScriptable : ScriptableObject, Observer<GameState>
{
	[SerializeField]
	[ExposedScriptableObject]
	private SoundScriptable[] soundScriptables;
	public AudioMixerSnapshot normalVolume;
	public AudioMixerSnapshot musicLowPass;
	public AudioMixerSnapshot mainMenu;
	public AudioMixerSnapshot lowIntensity;
	public SoundScriptable FindSound(string name)
	{
		return Array.Find(soundScriptables, sound => sound.name == name);
	}
	/// <summary>
	/// Play a sound by string name.
	/// </summary>
	/// <param name="name"></param>
	/// <returns></returns>
	public Sound Play(string name, GameObject caller = null)
	{
		return Play(FindSound(name), caller);
	}
	/// <summary>
	/// If you know exactly what sound you want to play, you can just put it in here.
	/// </summary>
	/// <param name="soundScriptable"></param>
	/// <param name="caller"></param>
	/// <returns></returns>
	public Sound Play(SoundScriptable soundScriptable, GameObject caller = null)
	{
		return Array.Find(soundScriptables, sound => sound.Equals(soundScriptable)).Play(caller);
	}
	public Sound[] PlayAll(GameObject caller = null)
	{
		Sound[] soundGameObjects = new Sound[soundScriptables.Length];
		for (int i = 0; i < soundScriptables.Length; i++)
		{
			soundGameObjects[i] = soundScriptables[i].Play(caller);
		}
		return soundGameObjects;
	}
	public void TransitionTo(AudioMixerSnapshot snapshot, float seconds = 1f)
	{
		snapshot.TransitionTo(seconds);
	}
	public void OnNotify(GameState state)
	{
		switch (state)
		{
			case GameState.GamePlay:
				TransitionTo(lowIntensity);
				break;
			case GameState.Cutscene:
			case GameState.GamePlayPaused:
				TransitionTo(musicLowPass);
				break;
			case GameState.MainMenu:
				TransitionTo(mainMenu);
				break;
		}
	}
}

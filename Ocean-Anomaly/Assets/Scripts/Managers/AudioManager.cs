using UnityEngine.Audio;
using UnityEngine;
using System;
using OceanAnomaly.Components;

namespace OceanAnomaly.Managers
{
	public class AudioManager : MonoBehaviour, Observer<GameState>
	{
		public static AudioManager Instance;
		public Sound[] sounds;
		public AudioMixerSnapshot normalVolume;
		public AudioMixerSnapshot musicLowPass;
		public AudioMixerSnapshot mainMenu;
		public AudioMixerSnapshot lowIntensity;

		void Awake()
		{
			if (Instance != null)
			{
				Destroy(gameObject);
				return;
			}
			Instance = this;
			DontDestroyOnLoad(gameObject);

			foreach (Sound s in sounds)
			{
				s.source = gameObject.AddComponent<AudioSource>();
				s.source.clip = s.clip;
				s.source.volume = s.volume;
				s.source.pitch = s.pitch;
				s.source.loop = s.loop;
				s.source.outputAudioMixerGroup = s.audioMixer;
			}
		}
		private void Start()
		{
			GlobalManager.Instance.AddObserver(this);

			SetMainMusicPlaying(true);
		}
		public void Play(string name)
		{
			Sound s = FindSound(name);
			if (s == null)
			{
				Debug.Log($"Can't play {name}");
			}
			s.source.Play();
		}

		public void Stop(string name)
		{
			Sound s = FindSound(name);
			s.source.Stop();
		}
		public Sound FindSound(string name)
		{
			return Array.Find(sounds, sound => sound.name == name);
		}
		public bool IsSoundPlaying(string name)
		{
			Sound s = FindSound(name);
			return s.source.isPlaying;
		}
		public void TransitionTo(AudioMixerSnapshot snapshot, float seconds = 1f)
		{
			snapshot.TransitionTo(seconds);
		}
		public void SetMainMusicPlaying(bool state = true)
		{
			foreach (var sound in sounds)
			{
				if (sound.musicTrack)
				{
					if (state)
					{
						sound.source.Play();
					} else
					{
						sound.source.Stop();
					}
				}
			}
		}
		public bool IsMusicPlaying()
		{
			foreach (var sound in sounds)
			{
				if (sound.musicTrack)
				{
					if (!sound.source.isPlaying)
						return false;
				}
			}
			return true;
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
}
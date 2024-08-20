using UnityEngine.Audio;
using UnityEngine;
using System;
using OceanAnomaly.Components;

namespace OceanAnomaly.Managers
{
	public class AudioManager : MonoBehaviour
	{
		public static AudioManager Instance;
		public Sound[] sounds;
		public AudioMixerSnapshot normalVolume;
		public AudioMixerSnapshot musicLowPass;

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

		public void Play(string name)
		{
			Sound s = Array.Find(sounds, sound => sound.name == name);
			if (s == null)
			{
				Debug.Log($"Can't play {name}");
			}
			s.source.Play();
		}

		public void Stop(string name)
		{
			Sound s = Array.Find(sounds, sound => sound.name == name);
			s.source.Stop();
		}

		public void TransitionTo(AudioMixerSnapshot snapshot, float seconds = 1f)
		{
			snapshot.TransitionTo(seconds);
		}
	}
}
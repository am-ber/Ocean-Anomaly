using OceanAnomaly;
using OceanAnomaly.Attributes;
using OceanAnomaly.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace OceanAnomaly.Components
{
	[CreateAssetMenu(fileName = "Sound", menuName = "Scriptable Objects/Audio/Sound")]
	public class SoundScriptable : ScriptableObject
	{
		public new string name;
		public AudioClip clip;

		[Range(0, 1)]
		public float volume = 0.9f;
		[Range(0.1f, 3)]
		public float pitch = 1;
		public bool loop = false;
		[Range(0f, 1f)]
		public float spatialBlend = 0f;
		public AudioMixerGroup audioMixer;
		[field: SerializeField]
		public float soundTrueLength {  get; private set; }
		private void OnValidate()
		{
			Initialize();
		}
		private void OnEnable()
		{
			Initialize();
		}
		private void Initialize()
		{
			if ((name == null || name == string.Empty) && clip != null)
			{
				name = clip.name;
			}
			if (clip != null)
			{
				soundTrueLength = clip.length / pitch;
			}
		}
		/// <summary>
		/// Play the sound. The calling GameObject is optional as we will throw the source onto the GlobalManager.Instance.
		/// If it can't find the GlobalManager.Instance then nothing will happen!
		/// </summary>
		/// <param name="callingGameObject"></param>
		/// <returns></returns>
		public Sound Play(GameObject callingGameObject = null)
		{
			// Null checks when we ask to Play
			if (callingGameObject == null)
			{
				callingGameObject = GlobalManager.Instance.gameObject;
			}
			// Create a new object
			GameObject audioGameObject = new GameObject($"{name} Audio Clip");
			// If the callingGameObject isn't null then lets not try to set the parent of the audio clip
			if (callingGameObject != null)
			{
				audioGameObject.transform.parent = callingGameObject.transform;
			}
			// Disable the created GameObject to add the component before the Awake() method is called
			audioGameObject.SetActive(false);
			Sound createdSound = audioGameObject.AddComponent<Sound>();
			createdSound.referencedSound = this;
			audioGameObject.SetActive(true);
			// Return the source we just created incase the caller needs to reference it themselves
			return createdSound;
		}
	}
}

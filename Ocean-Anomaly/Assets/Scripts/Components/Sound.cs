using UnityEngine.Audio;
using UnityEngine;
using OceanAnomaly.Attributes;
using OceanAnomaly.Tools;
using UnityEngine.Events;
using Unity.VisualScripting;
using static Unity.VisualScripting.Member;

namespace OceanAnomaly.Components
{
	public class Sound : MonoBehaviour
	{
		public AudioSource referencedSource;
		public SoundScriptable referencedSound;
		public UnityEvent OnSoundFinish;
		private void Awake()
		{
			// Create the Component
			referencedSource = gameObject.AddComponent<AudioSource>();
			PrepareSourceFromSoundScriptable();
		}
		/// <summary>
		/// Used to prepare the AudioSource with all the information from the SoundScriptable
		/// </summary>
		private void PrepareSourceFromSoundScriptable()
		{
			referencedSource.clip = referencedSound.clip;
			referencedSource.volume = referencedSound.volume;
			referencedSource.pitch = referencedSound.pitch;
			referencedSource.loop = referencedSound.loop;
			referencedSource.spatialBlend = referencedSound.spatialBlend;
			referencedSource.outputAudioMixerGroup = referencedSound.audioMixer;
		}
		private void Start()
		{
			referencedSource.Play();
			// If we need to loop then lets not remove the audio clip
			if (!referencedSound.loop)
			{
				DestroyThisGameObject();
			}
		}
		public void DestroyThisGameObject(bool immediately = false)
		{
			// If we need to destroy the game object immediately, we set the destroy time to 0 seconds.
			float destroyTime = immediately ? 0f : referencedSound.soundTrueLength;
			// Finally Destroy the GameObject
			Destroy(gameObject, destroyTime);
		}
		private void OnDisable()
		{
			// Then check if the referencedSource still exists... this only happens in cases where we've left the scene early
			DestroyThisGameObject(true);
		}
		private void OnDestroy()
		{
			if (!gameObject.scene.isLoaded)
			{
				return;
			}
			// Call anyone we need to on finishing
			OnSoundFinish?.Invoke();
		}
	}
}
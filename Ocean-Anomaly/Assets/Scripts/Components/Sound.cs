using UnityEngine.Audio;
using UnityEngine;

namespace OceanAnomaly.Components
{
    [System.Serializable]
    public class Sound
    {

        public string name;
        public AudioClip clip;

        [Range(0, 1)]
        public float volume = 0.9f;
        [Range(0.1f, 3)]
        public float pitch = 1;
        public bool loop = false;
        public bool musicTrack = false;
        public AudioMixerGroup audioMixer;

        [HideInInspector]
        public AudioSource source;
    }
}
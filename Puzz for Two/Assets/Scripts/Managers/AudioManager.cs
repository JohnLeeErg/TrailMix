using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{

    // vvv script addapted from: https://www.youtube.com/watch?v=6OT43pvUyfY

    [Serializable]
    public class Sound
    {
        public string name;

        public AudioClip clip;

        [Range(0, 1)]
        public float volume = 1;
        [Range(.1f, 3f)]
        public float pitch = 1;

        [HideInInspector]
        public AudioSource source;

        public bool looping = false;
    }


    public Sound[] sounds;

    public static AudioManager instence;

    // Use this for initialization
    void Awake()
    {

        instence = this;

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.looping;
        }
    }

    public void PlaySingleSound(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.source.Play();
    }

    public void PlaySingleRandomizedPitch(string name, Vector2 pitchRange)
    {
        float pitch = UnityEngine.Random.Range(pitchRange.x, pitchRange.y);

        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.source.pitch = pitch;
        s.source.Play();

    }

    public void PlayRandomFromArray(string[] names)
    {
        int index = UnityEngine.Random.Range(0, names.Length);
        string singleName = names[index];

        Sound s = Array.Find(sounds, sound => sound.name == singleName);
        s.source.Play();
    }

    public void PlayRandomFromArrayWithPitch(string[] names, Vector2 pitchRange)
    {
        int index = UnityEngine.Random.Range(0, names.Length);
        string singleName = names[index];

        float pitch = UnityEngine.Random.Range(pitchRange.x, pitchRange.y);

        Sound s = Array.Find(sounds, sound => sound.name == singleName);
        s.source.pitch = pitch;
        s.source.Play();
    }

    public void StopSingleSound(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.source.Stop();
    }

    public bool CheckIfSoundIsPlaying(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s.source.isPlaying == true)
            return true;
        else
            return false;
    }
}

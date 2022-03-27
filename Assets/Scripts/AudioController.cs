using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

public class AudioController : MonoBehaviour
{
    public Sound[] sounds;

    void Awake()
    {
        foreach (Sound sound in sounds)
        {
            sound.source        = gameObject.AddComponent<AudioSource>();
            sound.source.clip   = sound.clip;
            sound.source.volume = sound.volume;
            sound.source.pitch  = sound.pitch;
            sound.source.loop   = sound.loop;
        }
    }

    void Start()
    {
        Play("Music");
    }

    public void Play(string name, float randRange = 0, bool asyncPlay = false)
    {
        Sound sound = Array.Find(sounds, sound => sound.name == name);
        if (sound == null)
        {
            Debug.Log($"Unable to find sound: {name}.");
            return;
        }

        if (randRange != 0)
            sound.source.pitch = sound.pitch * (1f + UnityEngine.Random.Range(-randRange / 2f, randRange / 2f));
        else
            sound.source.pitch = sound.pitch;

        if (!asyncPlay && sound.source.isPlaying)
            sound.source.Stop();
        sound.source.Play();
    }
}

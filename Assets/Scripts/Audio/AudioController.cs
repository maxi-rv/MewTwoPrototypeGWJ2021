using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioController : MonoBehaviour
{
    //VARIABLES
    public static AudioController instance;
    public AudioFile[] audioFiles;
    
    void Awake()
    {
        foreach (AudioFile af in audioFiles)
        {
            af.source = gameObject.AddComponent<AudioSource>();

            af.source.clip = af.clip;

            af.source.volume = af.volume;
            af.source.pitch = af.pitch;
            af.source.loop = af.loop;
        }
    }

    // 
    public void Play(string name)
    {
        AudioFile audioFile = Array.Find(audioFiles, af => af.name == name);

        if(audioFile != null)
            audioFile.source.Play();
    }

    public void Stop(string name)
    {
        AudioFile audioFile = Array.Find(audioFiles, af => af.name == name);

        if(audioFile != null)
            audioFile.source.Stop();
    }
}
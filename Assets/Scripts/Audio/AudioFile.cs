using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class AudioFile
{
    //AUDIO CLIP FILE
    public AudioClip clip;

    //VARIABLES
    public string name;
    [Range(0f, 1f)] public float volume;
    [Range(0.1f, 3f)] public float pitch;
    public bool loop;

    //HIDDEN VARIABLES
    [HideInInspector] public AudioSource source;
}
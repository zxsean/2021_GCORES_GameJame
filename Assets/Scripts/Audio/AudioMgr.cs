using System.Collections.Generic;
using UnityEngine;

public static class AudioMgr
{
    public static GameObject Root { get; set; }

    private static AudioSource musicSource;
    private static List<AudioSource> soundSources = new List<AudioSource>();

    public static void PlayMusic(AudioClip clip)
    {
        if (musicSource == null)
        {
            musicSource = Root.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = false;
        }
        
        musicSource.clip = clip;
        musicSource.Play();
    }       
    
    public static void PlayContinueMusic(AudioClip clip)
    {
        if (musicSource == null)
        {
            musicSource = Root.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = false;
        }

        if (musicSource.clip == clip)
        {
            return;
        }
        
        musicSource.clip = clip;
        musicSource.Play();
    }
    
    public static void PlaySound(AudioClip clip)
    {
        AudioSource freeSource = null;
        foreach (var source in soundSources)
        {
            if (!source.isPlaying)
            {
                freeSource = source;
                break;
            }
        }

        if (freeSource == null)
        {
            freeSource = Root.AddComponent<AudioSource>();
            freeSource.loop = false;
            freeSource.playOnAwake = false;
            soundSources.Add(freeSource);
        }

        freeSource.clip = clip;
        freeSource.Play();
    }

    public static void PlayContinueSound(AudioClip clip)
    {
        AudioSource contSource = null;
        foreach (var source in soundSources)
        {
            if (source.clip == clip)
            {
                contSource = source;
                break;
            }
        }

        if (contSource == null)
        {
            PlaySound(clip);
            return;
        }

        if (contSource.isPlaying) return;
        contSource.Play();
    }

    public static void StopSound(AudioClip clip)
    {
        AudioSource stopSource = null;
        foreach (var source in soundSources)
        {
            if (source.clip == clip)
            {
                stopSource = source;
                break;
            }
        }

        if (stopSource == null) return;
        stopSource.Stop();
    }
}
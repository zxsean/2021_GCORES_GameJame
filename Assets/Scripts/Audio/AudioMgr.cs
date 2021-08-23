using System.Collections.Generic;
using UnityEngine;

public static class AudioMgr
{
    public static GameObject Root { get; set; }

    private static AudioSource musicSource;
    private static List<Sound> soundSources = new List<Sound>();

    private class Sound
    {
        public AudioSource source;
        public Transform bind;
    }

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
    
    public static void PlaySound(AudioClip clip, Transform bind)
    {
        Sound freeSource = null;
        foreach (var source in soundSources)
        {
            if (!source.source.isPlaying)
            {
                freeSource = source;
                break;
            }
        }

        if (freeSource == null)
        {
            freeSource = new Sound();
            var source = Root.AddComponent<AudioSource>();
            source.loop = false;
            source.playOnAwake = false;
            source.volume = 0.5f;
            freeSource.source = source;
            soundSources.Add(freeSource);
        }

        freeSource.bind = bind;
        freeSource.source.clip = clip;
        freeSource.source.Play();
    }

    public static void PlayContinueSound(AudioClip clip, Transform bind)
    {
        Sound freeSource = null;
        foreach (var source in soundSources)
        {
            if (source.source.clip == clip)
            {
                freeSource = source;
                break;
            }
        }

        if (freeSource == null)
        {
            PlaySound(clip, bind);
            return;
        }

        if (freeSource.source.isPlaying) return;
        freeSource.bind = bind;
        freeSource.source.Play();
    }

    public static void StopSound(AudioClip clip)
    {
        Sound stopSource = new Sound();
        foreach (var source in soundSources)
        {
            if (source.source.clip == clip)
            {
                stopSource = source;
                break;
            }
        }

        if (stopSource.source == null) return;
        stopSource.source.Stop();
    }

    public static void StopAllSound()
    {
        foreach (var source in soundSources)
        {
            source.source.Stop();
        }
    }

    public static void Update()
    {
        //随距离平方衰减
        var pos = CameraMgr.CameraTrans.position;
        foreach (var source in soundSources)
        {
            if (source.source.isPlaying && source.bind != null)
            {
                var sourcePos = source.bind.position;
                sourcePos.z = pos.z;
                var dis2 = Vector3.Dot(sourcePos - pos, sourcePos - pos);
                var volume = dis2 > 300 ? 0 : Mathf.Max(0.0f, 0.5f / (0.5f + dis2));
                source.source.volume = Mathf.Min(0.5f, volume);
            }
        }
    }
}
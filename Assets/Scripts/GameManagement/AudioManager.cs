using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager{

    private GameObject audioManagerObject;

    private AudioSource source;
    private AudioSource loopSource;
    
    public AudioManager(bool includeAudioListener)
    {
        this.audioManagerObject = new GameObject();
        Object.DontDestroyOnLoad(audioManagerObject);
        Globals.savedObjects.Add(audioManagerObject);

        this.source = audioManagerObject.AddComponent<AudioSource>();
        this.loopSource = audioManagerObject.AddComponent<AudioSource>();
        if (includeAudioListener)
        {
            audioManagerObject.AddComponent<AudioListener>();
        }
    }

    public AudioSource GetSource()
    {
        return this.source;
    }
    public AudioSource GetLoopSource()
    {
        return this.loopSource;
    }

    public void PlayInfinitClip(string introClipPath, string loopClipPath)
    {
        //play theme song
        source.clip = Resources.Load<AudioClip>(introClipPath);
        loopSource.clip = Resources.Load<AudioClip>(loopClipPath);
        source.Play();
        loopSource.PlayDelayed(source.clip.length);
        loopSource.loop = true;
    }
    public void PlayClip(string sourceClipPath)
    {
        source.clip = Resources.Load<AudioClip>(sourceClipPath);
        source.Play();
    }
    public void StopCurrentClip()
    {
        source.Stop();
    }
}

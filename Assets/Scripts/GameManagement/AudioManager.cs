using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager{

    public GameObject gameObject;

    private AudioSource source;
    private AudioSource loopSource;
    
    public AudioManager(bool includeAudioListener)
    {
        this.gameObject = new GameObject("AudioManager_index_"+Globals.savedGameObjects.Count+"");
        Object.DontDestroyOnLoad(gameObject);
        Globals.savedGameObjects.Add(gameObject);

        this.source = gameObject.AddComponent<AudioSource>();
        this.loopSource = gameObject.AddComponent<AudioSource>();
        if (includeAudioListener)
        {
            gameObject.AddComponent<AudioListener>();
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

    public void PlayInfiniteClip(string introClipPath, string loopClipPath)
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

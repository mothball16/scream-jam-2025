using Assets.Scripts.Gameplay.Attributes;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SoundManager : MonoBehaviour
{

    //jam - learning audio

    //Tasks
    //Hold references to audioclips.
    //Subscribe to game events in OnEnable and unsubscribe in OnDisable.
    //Play the right sound when the event handler is called.

    [Header("Clips that are OneShot")]
    [SerializeField] private AudioClip packageDroppedClip;
    [SerializeField] private AudioClip packageSpawnedClip;
    [SerializeField] private AudioClip correctChoice;
    [SerializeField] private AudioClip incorrectChoice;
    [SerializeField] private AudioClip scalePress;


    [SerializeField] private AudioClip packageProcessedClip;
    [SerializeField] private AudioClip ConveryorSound;

    [Header("Clips that are Loops")]
    // looping clips to auto-play when game starts
    [SerializeField] private AudioClip[] startLoopClips; 

    [Header("Audio Sources")]
    // one-shot sound effects go through this source
    [SerializeField] private AudioSource sfxSource; 

    // Active looping sounds (key = string ID, value = AudioSource that plays the loop)
    private Dictionary<string, AudioSource> activeLoops = new();

    //holds all our event subscriptions
    private List<IDisposable> _subs;

    private AudioSource audioSource;



    private void Start()
    {
        //add an AudioSource to this GameObject at runtime
        audioSource = gameObject.AddComponent<AudioSource>();

        // Play all the loops assigned in inspector as soon as the game starts
        foreach (var clip in startLoopClips)
        {
            if (clip != null)
            {
                // Use clip name as unique key (safe if all names differ)
                PlayLoop(clip.name, clip);
            }
        }
    }



    private void OnEnable()
    {
        // When the object is enabled, we subscribe to events
        _subs = new List<IDisposable>
        {
            EventBus.Subscribe<PackageDroppedEvent>(
                (e) => PlaySound(packageDroppedClip, 0.7f)),
            EventBus.Subscribe<PackageSpawnedEvent>(
                (e) => PlaySound(packageSpawnedClip, 0.8f)),
            EventBus.Subscribe<CorrectChoiceEvent>(
                (e) => PlaySound(packageSpawnedClip, 0.8f)),
            EventBus.Subscribe<IncorrectChoiceEvent>(
                (e) => PlaySound(incorrectChoice, 0.8f)),
            EventBus.Subscribe<PackageProcessedEvent>(
                (e) => PlaySound(packageProcessedClip, 0.5f)),
            EventBus.Subscribe<ScalePressedEvent>(
                (e) => PlaySound(scalePress, 0.8f))
        };

    }

    private void OnDisable()
    {
        //When the object is disabled, unsubscribe from events
        foreach (var sub in _subs)
        {
            //unsubscribes from EventBus
            sub.Dispose(); 
            
        }
    }
    
    //Helper 
    private void PlaySound(AudioClip clip, float volume = 1f)
    {

        if (clip != null)
        {
            //playOneShot means to play this sound once, don't stop other sounds. 
            audioSource.PlayOneShot(clip);

        }
    }


    // ===========================================
    // ----------- continuous looping -----------
    // ===========================================


    /// <summary>
    /// Starts playing a looping sound under a unique key.
    /// Multiple loops can run at the same time.
    /// </summary>
    /// <param name="key">Unique name for this loop </param>
    /// <param name="clip">AudioClip to loop</param>
    /// <param name="volume">Volume of the loop</param>
    public void PlayLoop(string key, AudioClip clip, float volume = 1f)
    {
        //check
        if (string.IsNullOrEmpty(key) || clip == null)
        {
            return;
        }

        //prevent starting the same loop twice
        if (activeLoops.ContainsKey(key))
        {
            return;
        }

        //create a new GameObject to hold the AudioSource
        GameObject loopObj = new GameObject("Loop_" + key);
        loopObj.transform.SetParent(this.transform);

        //add AudioSource and configure it
        AudioSource source = loopObj.AddComponent<AudioSource>();
        source.clip = clip;
        source.loop = true;
        source.volume = volume;
        source.Play();

        //store in dictionary for management
        activeLoops[key] = source;
    }


    /// <summary>
    /// Stops and removes a loop by its key.
    /// Example: StopLoop("conv")
    /// </summary>
    public void StopLoop(string key)
    {
        if (activeLoops.TryGetValue(key, out var source))
        {
            source.Stop();
            //clean up the GameObject
            Destroy(source.gameObject); 
            activeLoops.Remove(key);
        }
    }



    /// <summary>
    /// stops and removes all currently active loops.
    /// </summary>
    public void StopAllLoops()
    {
        foreach (var source in activeLoops.Values)
        {
            if (source != null)
            {
                source.Stop();
                //Destroy(source.gameObject);
            }
        }
        activeLoops.Clear();
    }





}

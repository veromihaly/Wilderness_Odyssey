using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance {get;set;}

    // Sound FX
    public AudioSource dropItemSound;
    public AudioSource toolSwingSound;
    public AudioSource chopTreeSound;
    public AudioSource grassWalkSound;
    public AudioSource craftSound;
    public AudioSource pickupItemSound;
    public AudioSource treeFallSound;
    public AudioSource eatConsumableSound;
    public AudioSource drawToolSound;

    //Music
    public AudioSource startingZoneBGMusic;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void PlaySound(AudioSource soundToPlay)
    {
        if(!soundToPlay.isPlaying)
        {
            soundToPlay.Play();
        }
    }
}

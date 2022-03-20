using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    AudioClip collect;
    [SerializeField]
    AudioClip hit;
    [SerializeField]
    AudioSource audioSource;

    public void PlayCollect()
    {
        audioSource.clip = collect;
        audioSource.Play();
    }

    public void PlayHit()
    {
        audioSource.clip = hit;
        audioSource.Play();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioControl : MonoBehaviour
{
    private AudioSource _audioSource;
    // Start is called before the first frame update
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }
    
    public void PlayAudio(string audioID)
    {
        AudioClip clip = Resources.Load<AudioClip>("Audios/" + audioID);
        _audioSource.clip = clip;
        _audioSource.Play();
    }
}

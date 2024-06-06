using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Possible FIXME: os clips de audio só passam som no inicio sendo que ficam os proximos ~3 a serem played mas sem som;
// Isto pode eventualmente vir a intervir com alguma coisa
public class AudioManager : MonoBehaviour
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
        // Possible FIXME: .wav audio clips are quieter than .ogg
        //_audioSource.volume = 5f;
        _audioSource.Play();
    }
}

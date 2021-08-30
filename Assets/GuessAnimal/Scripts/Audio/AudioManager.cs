using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hellmade.Sound;

public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance;
    public static AudioManager instance { get { return _instance; } }
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    [System.Serializable]
    public struct EazySoundAudioControls
    {
        public string ID;
        public AudioClip audioclip;
        public Audio audio;
        public float volume;
    }

    [SerializeField] EazySoundAudioControls bgm;
    [SerializeField] EazySoundAudioControls[] sfx;
    public bool audioPaused = false;

    private void Start()
    {
        PlayBGM();
    }

    public void PlayBGM()
    {
        if (bgm.audio == null)
        {
            int audioID = EazySoundManager.PlayMusic(bgm.audioclip, bgm.volume, true, false);
            bgm.audio = EazySoundManager.GetAudio(audioID);
        }
        else if (bgm.audio != null && bgm.audio.Paused)
        {
            bgm.audio.Resume();
        }
        else
        {
            bgm.audio.Play();
        }
    }

    public void TogglePauseBGM()
    {
        if (bgm.audio != null)
        {
            if (bgm.audio.IsPlaying)
            {
                bgm.audio.Pause();
                audioPaused = true;
            }
            else
            {
                bgm.audio.Play();
                audioPaused = false;
            }
        }
    }

    public void PlaySFX(int index)
    {
        int audioID = EazySoundManager.PlaySound(sfx[index].audioclip, sfx[index].volume, false, gameObject.transform);
    }

}

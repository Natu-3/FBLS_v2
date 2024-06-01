using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public AudioSource bgmSource; // BGM AudioSource
    public AudioSource sfxSource;
    public AudioMixer masterMixer;
    public Slider bgmSlider;
    public Slider sfxSlider;
    public GameObject bgmOff;
    public GameObject sfxOff;
    public AudioClip[] sfx_clips;
    private bool isBgmMuted = false; // BGM 음소거 상태
    private bool isSfxMuted = false; // SFX 음소거 상태
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void bgmAudioControl()
    {
        float bgm = bgmSlider.value;
        if (bgm == -20f)
        {
            masterMixer.SetFloat("BGM", -80);
        }
        else
        {
            masterMixer.SetFloat("BGM", bgm);
        }
    }
    public void sfxAudioControl()
    {
        float sfx = bgmSlider.value;
        if (sfx == -20f)
        {
            masterMixer.SetFloat("SFX", -80);
        }
        else
        {
            masterMixer.SetFloat("SFX", sfx);
        }
    }

    public void ToggleBgmMute()
    {
        isBgmMuted = !isBgmMuted; 
        bgmSource.mute = isBgmMuted; 
        bgmOff.SetActive(isBgmMuted);
    }

    public void ToggleSfxMute()
    {
        isSfxMuted = !isSfxMuted; 
        sfxSource.mute = isSfxMuted;
        sfxOff.SetActive(isSfxMuted);
    }
}
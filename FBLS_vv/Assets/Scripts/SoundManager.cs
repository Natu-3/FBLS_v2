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
    public BgmData[] bgmDataArr;
    public SfxData[] sfxDataArr;
    private Dictionary<BgmType, BgmData> bgmDataDic;
    private Dictionary<SfxType, SfxData> sfxDataDic;
    public static SoundManager Instance;
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        bgmDataDic = new Dictionary<BgmType, BgmData>();
        foreach(BgmData data in bgmDataArr)
        {
            bgmDataDic.Add(data.bgmType, data);
        }

        sfxDataDic = new Dictionary<SfxType, SfxData>();
        foreach(SfxData data in sfxDataArr)
        {
            sfxDataDic.Add(data.sfxType, data);
        }
        playBgm(BgmType.Main);
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
        float sfx = sfxSlider.value;
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
    public void playBgm(BgmType type)
    {
        BgmData bgmData = bgmDataDic[type];
        bgmSource.clip = bgmData.clip;
        bgmSource.volume = bgmData.volume;
        bgmSource.Play();
    }
    public void playSfx(SfxType type)
    {
        SfxData sfxData = sfxDataDic[type];
        sfxSource.volume = sfxData.volume;
        sfxSource.PlayOneShot(sfxData.clip);
    }
    [System.Serializable]
    public class SoundData
    {
        public AudioClip clip;
        public float volume = 1f;
    }
    [System.Serializable]
    public class BgmData: SoundData
    {
        public BgmType bgmType;

    }
    [System.Serializable]
    public class SfxData: SoundData
    {
        public SfxType sfxType;
  
    }
    public IEnumerator skillSound(SfxType type)
    {
        playSfx(type);
        yield return new WaitForSeconds(5f);
        sfxSource.Stop();
    }
}
public enum BgmType
{
    None = 0,
    Main = 10,
}
public enum SfxType
{
    None = 0,
    Sun = 1,
    Rain = 2,
    Snow = 3,
    Shield = 4,
    Click = 5,
    Fall = 6,
    Destroy = 7,
    Iced = 8,
    Uniced = 9,
    Warning = 10,
    Panalty = 11,
}
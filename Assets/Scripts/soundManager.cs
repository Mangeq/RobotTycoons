using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
public class soundManager : MonoBehaviour
{
    public AudioSource music;
    public AudioSource Sfx;
    public static bool sfx;
    public float musicValue;
    public float sfxValue;
    public AudioClip sell;
    public AudioClip upgrade;
    public AudioClip click;
    public static AudioClip[] machineSounds;
    void Awake()
    {
        machineSounds = new AudioClip[]
        {
            Resources.Load<AudioClip>("Sounds/Smelter"),
            Resources.Load<AudioClip>("Sounds/Pattern"),
            Resources.Load<AudioClip>("Sounds/Electronics")
        };
        Sfx = GameObject.Find("Sound Manager").GetComponent<AudioSource>();
        music = GameObject.Find("BackgroundMusic").GetComponent<AudioSource>();
        musicValue = GameObject.Find("BackgroundMusic").GetComponent<AudioSource>().volume;
        sfxValue = GameObject.Find("Sound Manager").GetComponent<AudioSource>().volume;
        musicValue = PlayerPrefs.GetFloat("musicValue", 0.009f);
        sfxValue = PlayerPrefs.GetFloat("sfxValue", 0.072f);
    }
    public void MusicVolume()
    {
        GameObject text = GameObject.Find("MusicOnOffText");
        if (music.volume > 0)
        {
            text.GetComponent<TextMeshProUGUI>().text = "OFF";
            music.volume = 0;
        }
        else
        {
            text.GetComponent<TextMeshProUGUI>().text = "ON";
            music.volume = 0.009f;
        }
    }
    public void SfxVolume()
    {
        GameObject text = GameObject.Find("SFXOnOffText");
        if (sfx == true)
        {
            Sfx.volume = 0;
            text.GetComponent<TextMeshProUGUI>().text = "OFF";
            sfx = false;
        }
        else
        {
            Sfx.volume = 0.072f;
            text.GetComponent<TextMeshProUGUI>().text = "ON";
            sfx = true;
        }
    }
    public void sellSound()
    {
        Sfx.PlayOneShot(sell);
    }
    public void upgradeSound()
    {
        Sfx.PlayOneShot(upgrade);
    }
    public void clickSound()
    {
        Sfx.PlayOneShot(click);
    }
}

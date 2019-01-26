using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public enum ESFX
    {
        AlarmAir1 = 0,
        AlarmFood1,
        AlarmFuel1,
        Button1,
        Button2,
        Button3,
        Button4,
        Clocks1,
    }
    public enum EBGM
    {
        GameTheme = 0,
        MenuTheme,
        Misc1,
        Misc2
    }

    [SerializeField]
    private AudioClip[] m_SFXClips = null;    
    [SerializeField]
    private AudioClip[] m_BGMClips = null;

    private AudioSource m_SFXSource = null;
    private AudioSource m_BGMSource = null;

    public void Init()
    {
        m_SFXSource = gameObject.AddComponent<AudioSource>();
        m_BGMSource = gameObject.AddComponent<AudioSource>();
        m_SFXSource.playOnAwake = false;
        m_BGMSource.playOnAwake = false;
    }

    public void PlayBGM(EBGM bgm, bool loop = true)
    {
        AudioClip ac = m_BGMClips[(int)bgm];
        m_BGMSource.clip = ac;
        m_BGMSource.loop = loop;
        m_BGMSource.Play();
        
    }
    public void PlaySFX(ESFX sfx, bool loop = false)
    {
        Debug.LogFormat("Play SFX {0}", sfx);
        AudioClip ac = m_SFXClips[(int)sfx];
        m_SFXSource.clip = ac;
        m_SFXSource.loop = loop;
        m_SFXSource.Play();
    }

    public void StopAll()
    {
        m_BGMSource.Stop();
        m_SFXSource.Stop();
    }
}

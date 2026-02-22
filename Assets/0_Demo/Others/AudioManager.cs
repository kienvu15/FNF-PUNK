using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public bool isPaused = false;

    [Range(0f, 1f)]
    public float bgmVolume = 1f;
    [Range(0f, 1f)]
    public float sfxVolume = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            //Destroy(gameObject);
        }
    }

    private void Start()
    {
        
    }

    public void ActtiveSeting()
    {
        isPaused = true;
        Time.timeScale = 0f;
        AudioListener.pause = true;
    }

    public void UnActibeSting()
    {
        isPaused = false;
        Time.timeScale = 1f;
        AudioListener.pause = false;
    }

    public void SetBGMVolume(float value)
    {
        bgmVolume = value;
        foreach (var source in GameObject.FindGameObjectsWithTag("BGM"))
        {
            source.GetComponent<AudioSource>().volume = bgmVolume;
        }
    }

    public void SetSFXVolume(float value)
    {
        sfxVolume = value;
        foreach (var source in GameObject.FindGameObjectsWithTag("SFX"))
        {
            source.GetComponent<AudioSource>().volume = sfxVolume;
        }
    }
}

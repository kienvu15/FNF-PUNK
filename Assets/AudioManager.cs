using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public GameObject audioStteing;

    public bool isPaused = false;
    public GameObject pauseButton;

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
        pauseButton.SetActive(false);
        audioStteing.SetActive(true);
    }

    public void UnActibeSting()
    {
        isPaused = false;
        Time.timeScale = 1f;
        AudioListener.pause = false;
        pauseButton.SetActive(true);
        audioStteing.SetActive(false);
    }

    public void SetBGMVolume(float value)
    {
        bgmVolume = value;
        // chỉnh cho tất cả AudioSource gắn tag "BGM"
        foreach (var source in GameObject.FindGameObjectsWithTag("BGM"))
        {
            source.GetComponent<AudioSource>().volume = bgmVolume;
        }
    }

    public void SetSFXVolume(float value)
    {
        sfxVolume = value;
        // chỉnh cho tất cả AudioSource gắn tag "SFX"
        foreach (var source in GameObject.FindGameObjectsWithTag("SFX"))
        {
            source.GetComponent<AudioSource>().volume = sfxVolume;
        }
    }
}

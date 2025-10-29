using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class SongManager : MonoBehaviour
{
    public static SongManager Instance;

    [Header("Audio")]
    public AudioSource audioSource;

    [Header("Gameplay Settings")]
    public Lane[] lanes;
    public float songDelayInSeconds;
    public double marginOfError;                 
    public int inputDelayInMilliseconds;
    public float noteTime;
    public float noteSpawnY;
    public float noteTapY;

    [Header("File Settings")]
    public string fileLocation;

    public static event Action OnMidiLoaded;
    public static MidiFile midiFile;

    private bool songStarted = false;

    public float noteDespawnY
    {
        get { return noteTapY - (noteSpawnY - noteTapY); }
    }

    // Start is called before the first frame update
    private void Start()
    {
        Instance = this;

        if (Application.streamingAssetsPath.StartsWith("http://") || Application.streamingAssetsPath.StartsWith("https://"))
        {
            StartCoroutine(ReadFromWebsite());
        }
        else
        {
            ReadFromFile();
        }
    }

    private IEnumerator ReadFromWebsite()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(Application.streamingAssetsPath + "/" + fileLocation))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                byte[] results = www.downloadHandler.data;

                using (var stream = new MemoryStream(results))
                {
                    midiFile = MidiFile.Read(stream);
                    GetDataFromMidi();
                    OnMidiLoaded?.Invoke();   // 🔔 báo cho ai đăng ký
                }
            }
        }
    }

    private void ReadFromFile()
    {
        midiFile = MidiFile.Read(Application.streamingAssetsPath + "/" + fileLocation);
        GetDataFromMidi();
        OnMidiLoaded?.Invoke();
    }

    public void GetDataFromMidi()
    {
        var notes = midiFile.GetNotes();
        var array = new Melanchall.DryWetMidi.Interaction.Note[notes.Count];
        notes.CopyTo(array, 0);

        foreach (var lane in lanes)
        {
            lane.SetTimeStamps(array);
        }

        // Thay vì gọi ngay StartSong thì gọi Countdown
        FindFirstObjectByType<CountdownManager>().StartCountdown();
    }

    public void StartSong()
    {
        audioSource.volume = AudioManager.Instance.bgmVolume;
        audioSource.Play();
        songStarted = true;
    }

    public static double GetAudioSourceTime()
    {
        return (double)Instance.audioSource.timeSamples / Instance.audioSource.clip.frequency;
    }

    private void ShowResultPanel()
    {
        FindFirstObjectByType<ResultPanel>().Show();
        SaveBestScore(ScoreManager.score);
    }

    private void Update()
    {
        if (songStarted && !AudioManager.Instance.isPaused && !audioSource.isPlaying && audioSource.time > 0f)
        {
            ShowResultPanel();
            songStarted = false;
        }
    }

    public void SaveBestScore(int currentScore)
    {
        string levelName = SceneManager.GetActiveScene().name;
        string key = "BestScore_" + levelName;

        int bestScore = PlayerPrefs.GetInt(key, 0);
        if (currentScore > bestScore)
        {
            PlayerPrefs.SetInt(key, currentScore);
        }
    }

    
}

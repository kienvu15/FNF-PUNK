using Melanchall.DryWetMidi.Interaction;
using System.Collections;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    public AudioSource hitSFX;
    public AudioSource missSFX;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI comboText; // thêm cái này
    static int maxCombo;
    static int comboScore;

    public static int totalNotes;
    public static int perfectCount;
    public static int goodCount;
    public static int missCount;
    public static int score;

    void OnEnable()
    {
        SongManager.OnMidiLoaded += InitScore; // đăng ký lắng nghe
    }

    void OnDisable()
    {
        SongManager.OnMidiLoaded -= InitScore; // hủy đăng ký để tránh memory leak
    }

    private void InitScore()
    {
        if (SongManager.midiFile != null)
        {
            totalNotes = SongManager.midiFile.GetNotes().Count;
            Debug.Log("Tổng số note = " + totalNotes);
        }
        else
        {
            Debug.LogError("MIDI file null dù đã load!");
        }
    }

    void Start()
    {
        Instance = this;
        comboScore = 0;
        maxCombo = 0;
        perfectCount = 0;
        goodCount = 0;
        missCount = 0;
        score = 0;
    }

    public static void Perfect()
    {
        comboScore += 1;
        if (comboScore > maxCombo) maxCombo = comboScore;
        Instance.UpdateComboUI();

        perfectCount++;
        score += 100;

        Instance.hitSFX.volume = AudioManager.Instance.sfxVolume;
        Instance.hitSFX.pitch = Random.Range(0.8f, 1.2f); // 🎵 random pitch
        Instance.hitSFX.Play();
    }

    public static void Good()
    {
        comboScore += 1;
        if (comboScore > maxCombo) maxCombo = comboScore;
        Instance.UpdateComboUI();

        goodCount++;
        score += 50;

        Instance.hitSFX.volume = AudioManager.Instance.sfxVolume;
        Instance.hitSFX.pitch = Random.Range(0.8f, 1.5f); // 🎵 random pitch
        Instance.hitSFX.Play();
    }


    public static void Miss()
    {
        comboScore = 0;
        Instance.UpdateComboUI();

        missCount++;
    }


    private void Update()
    {
        scoreText.text = "Score: " + score.ToString();
    }
    private void UpdateComboUI()
    {
        if (comboScore > 1) // chỉ hiện khi combo >= 2
        {
            comboText.text = "x" + comboScore + " Combo!";
            StopAllCoroutines();
            StartCoroutine(AnimateCombo(comboText.transform));
        }
        else
        {
            comboText.text = "";
        }
    }

    private IEnumerator AnimateCombo(Transform target)
    {
        // scale effect
        float duration = 0.2f;
        float time = 0f;

        Vector3 start = Vector3.one * 0.8f;
        Vector3 end = Vector3.one * 1.2f;

        target.localScale = start;

        while (time < duration)
        {
            float t = time / duration;
            target.localScale = Vector3.Lerp(start, end, t);
            time += Time.deltaTime;
            yield return null;
        }

        target.localScale = Vector3.one;
    }

    // dùng khi cần kết quả cuối
    public static int GetMaxCombo() => maxCombo;
}


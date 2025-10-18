using Melanchall.DryWetMidi.Interaction;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("Audio")]
    public AudioSource hitSFX;
    public AudioSource missSFX;

    [Header("UI References")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI comboText;

    [Header("Score Colors")]
    public Color perfectColor = new Color(0.3f, 1f, 1f);   // Cyan
    public Color goodColor = new Color(0.3f, 1f, 0.3f);     // Green
    public Color badColor = new Color(1f, 0.8f, 0.3f);      // Yellow
    public Color normalColor = Color.white;

    // Static stats
    static int maxCombo;
    static int comboScore;
    public static int totalNotes;
    public static int perfectCount;
    public static int goodCount;
    public static int badCount;
    public static int missCount;
    public static int score;

    void OnEnable() => SongManager.OnMidiLoaded += InitScore;
    void OnDisable() => SongManager.OnMidiLoaded -= InitScore;

    private void InitScore()
    {
        if (SongManager.midiFile != null)
        {
            totalNotes = SongManager.midiFile.GetNotes().Count;
            Debug.Log("Tổng số note = " + totalNotes);
        }
        else Debug.LogError("MIDI file null dù đã load!");
    }

    void Start()
    {
        Instance = this;
        comboScore = 0;
        maxCombo = 0;
        perfectCount = 0;
        goodCount = 0;
        badCount = 0;
        missCount = 0;
        score = 0;
    }

    // ==============================================================
    // 🟢 MAIN ADD SCORE FUNCTION
    // ==============================================================
    private void AddScore(int amount, Color color, float pitchMin, float pitchMax, bool resetCombo = false)
    {
        RectTransform rect = scoreText.GetComponent<RectTransform>();
        scoreText.DOKill(true);
        rect.DOKill();

        if (resetCombo)
        {
            comboScore = 0;
            UpdateComboUI();
        }
        else
        {
            comboScore++;
            if (comboScore > maxCombo) maxCombo = comboScore;
            UpdateComboUI();
        }

        score += amount;
        scoreText.text = "Score: " + score;

        // SFX
        hitSFX.volume = AudioManager.Instance.sfxVolume;
        hitSFX.pitch = Random.Range(pitchMin, pitchMax);
        hitSFX.Play();

        // Hiệu ứng DOTween Sequence
        Sequence seq = DOTween.Sequence();
        seq.Append(rect.DOScale(1.3f, 0.15f).SetEase(Ease.OutBack))
           .Join(scoreText.DOColor(color, 0.1f))
           .AppendInterval(0.05f)
           .Append(rect.DOScale(1f, 0.15f))
           .Join(scoreText.DOColor(normalColor, 0.25f));

        HealthBar.Instance.AddHealth(0.023f);
    }

    public static void Perfect()
    {
        Instance.AddScore(100, Instance.perfectColor, 0.9f, 1.2f);
        perfectCount++;
    }

    public static void Good()
    {
        Instance.AddScore(50, Instance.goodColor, 0.8f, 1.1f);
        goodCount++;
    }

    public static void Bad()
    {
        Instance.AddScore(20, Instance.badColor, 0.6f, 0.9f, resetCombo: true);
        badCount++;
    }

    public static void Miss()
    {
        comboScore = 0;
        Instance.UpdateComboUI();
        HealthBar.Instance.SubtractHealth(0.04f);
        missCount++;
    }

    private void UpdateComboUI()
    {
        if (comboScore > 1)
        {
            comboText.text = "x" + comboScore + " Combo!";
            comboText.transform.DOKill();
            comboText.transform.localScale = Vector3.one;
            comboText.transform
                .DOScale(1.2f, 0.2f)
                .SetEase(Ease.OutBack)
                .SetLoops(2, LoopType.Yoyo);
        }
        else
        {
            comboText.text = "";
        }
    }

    private void Update()
    {
        scoreText.text = "Score: " + score;
    }

    public static int GetMaxCombo() => maxCombo;
}

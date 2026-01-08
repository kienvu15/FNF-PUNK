using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultPanel : MonoBehaviour
{
    public GameObject panel;
    public TextMeshProUGUI percentText;
    public TextMeshProUGUI perfectText;
    public TextMeshProUGUI goodText;
    public TextMeshProUGUI missText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI rankText;

    public GameObject setting;
    public GameObject scorep;

    public void Show()
    {
        panel.SetActive(true);

        setting.SetActive(false);
        scorep.SetActive(false);

        SaveHighScore();   // 👈 thêm dòng này

        int totalNotes = ScoreManager.totalNotes;
        int hitNotes = ScoreManager.perfectCount + ScoreManager.goodCount;
        float accuracy = (totalNotes > 0) ? (hitNotes / (float)totalNotes) * 100f : 0f;

        percentText.text = $"Accuracy: ............ {accuracy:0.0}%";
        perfectText.text = $"Perfect: ............ {ScoreManager.perfectCount}";
        goodText.text = $"Good: ............ {ScoreManager.goodCount}";
        missText.text = $"Miss: ............ {ScoreManager.missCount}";
        scoreText.text = $"Score: ............ {ScoreManager.score}";
        rankText.text = $"Rank: ................. {GetRank(accuracy)}";
    }


    private void SaveHighScore()
    {
        int levelID = LevelManager.currentLevelID;
        int score = ScoreManager.score;

        string key = $"HighScore_Level_{levelID}";
        int oldScore = PlayerPrefs.GetInt(key, 0);

        if (score > oldScore)
        {
            PlayerPrefs.SetInt(key, score);
            PlayerPrefs.Save();
        }
    }


    private string GetRank(float accuracy)
    {
        if (accuracy == 100) return "S";
        if (accuracy >= 95) return "A";
        if (accuracy >= 85) return "B";
        if (accuracy >= 70) return "C";
        if (accuracy >= 50) return "D";
        return "F";
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene(0);
    }
}

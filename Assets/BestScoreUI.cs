using TMPro;
using UnityEngine;

public class BestScoreUI : MonoBehaviour
{
    public TMP_Text bestScoreText;
    public string levelName; // đặt trong Inspector (VD: "Level1", "Level2")

    void Start()
    {
        string key = "BestScore_" + levelName;
        int bestScore = PlayerPrefs.GetInt(key, 0);
        bestScoreText.text = "BEST: " + bestScore.ToString();
    }
}

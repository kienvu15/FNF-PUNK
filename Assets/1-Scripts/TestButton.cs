using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening;


public class TestButton : MonoBehaviour
{
    public Button button;
    public TextMeshProUGUI nameText;
    public int volumeLevel = 1;

    [Header("Transition Settings")]
    public int sceneToLoad = 5;
    public float fontGrow = 10f;          
    public float flashDuration = 0.15f;   
    public int flashCount = 3;            
    public Color flashColor = Color.yellow;

    private float originalFontSize;
    private Color originalColor;

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip confirmClickSound;

    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);

        originalFontSize = nameText.fontSize;
        originalColor = nameText.color;
    }

    public void OnClick()
    {
        button.interactable = false; // Ngăn spam bấm

        Sequence seq = DOTween.Sequence();

        for (int i = 0; i < flashCount; i++)
        {
            // Phóng to chữ và đổi màu
            seq.Append(DOTween.To(() => nameText.fontSize, x => nameText.fontSize = x, originalFontSize + fontGrow, flashDuration / 2).SetEase(Ease.OutQuad));
            seq.Join(nameText.DOColor(flashColor, flashDuration / 2));

            // Thu nhỏ chữ và trở lại màu gốc
            seq.Append(DOTween.To(() => nameText.fontSize, x => nameText.fontSize = x, originalFontSize, flashDuration / 2).SetEase(Ease.InQuad));
            seq.Join(nameText.DOColor(originalColor, flashDuration / 2));
        }

        audioSource.PlayOneShot(confirmClickSound);

        if (confirmClickSound != null)
            seq.AppendInterval(confirmClickSound.length);

        seq.OnComplete(() =>
        {
            SceneManager.LoadScene(sceneToLoad);
        });
    }
}
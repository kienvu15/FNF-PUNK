using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HealthBar : MonoBehaviour
{
    public static HealthBar Instance;

    [Header("References")]
    public Image playerFill;
    public Image opponentFill;
    public Image playerAvatar;    // 🧍 Avatar bên Player
    public Image opponentAvatar;  // 💀 Avatar bên Enemy

    [Header("Settings")]
    [Range(0f, 2f)] public float health = 1.0f; // 0–2 range (FNF style)
    public float changeSpeed = 3f;

    [Header("Effect Settings")]
    public float shakeDuration = 0.25f;
    public float shakeStrength = 8f;
    public float scaleUp = 1.1f;
    public float colorFlashTime = 0.2f;

    [Header("Colors")]
    public Color gainColor = new Color(0.3f, 1f, 0.3f);
    public Color loseColor = new Color(1f, 0.3f, 0.3f);

    private Vector3 originalScale;
    private Color playerBaseColor;
    private Color opponentBaseColor;

    private void Awake()
    {
        Instance = this;
        originalScale = transform.localScale;

        playerBaseColor = playerFill.color;
        opponentBaseColor = opponentFill.color;
    }

    void Update()
    {
        // Giới hạn giá trị health
        health = Mathf.Clamp(health, 0f, 2f);

        // Tính tỷ lệ hai bên
        float playerRatio = health / 2f;
        float opponentRatio = 1f - playerRatio;

        // Cập nhật thanh máu mượt
        playerFill.fillAmount = Mathf.Lerp(playerFill.fillAmount, playerRatio, Time.deltaTime * changeSpeed);
        opponentFill.fillAmount = Mathf.Lerp(opponentFill.fillAmount, opponentRatio, Time.deltaTime * changeSpeed);

        // Di chuyển avatar theo mép thanh máu
        if (playerAvatar != null)
        {
            playerAvatar.rectTransform.anchoredPosition = new Vector2(
                (playerFill.rectTransform.rect.width + 0f) * (1f-playerFill.fillAmount),
                playerAvatar.rectTransform.anchoredPosition.y
            );
        }

        if (opponentAvatar != null)
        {
            opponentAvatar.rectTransform.anchoredPosition = new Vector2(
                -opponentFill.rectTransform.rect.width * (1f - opponentFill.fillAmount),
                opponentAvatar.rectTransform.anchoredPosition.y
            );
        }
    }

    public void AddHealth(float amount)
    {
        health += amount;
        AnimateChange(true);
    }

    public void SubtractHealth(float baseAmount)
    {
        // Tỉ lệ trừ máu dựa theo lợi thế hiện tại
        float multiplier = Mathf.Lerp(0.6f, 1.6f, Mathf.InverseLerp(0f, 2f, health));
        float actualDamage = baseAmount * multiplier;

        health -= actualDamage;
        AnimateChange(false);

        if (health <= 0f)
        {
            Debug.Log("YOU LOSE!");
            // TODO: Gọi GameOver()
        }
    }

    private void AnimateChange(bool gained)
    {
        transform.DOKill();
        transform.localScale = originalScale;
        transform.DOScale(originalScale * scaleUp, 0.15f).SetLoops(2, LoopType.Yoyo);

        float strength = gained ? shakeStrength * 0.6f : shakeStrength;
        transform.DOShakePosition(shakeDuration, strength, 20, 90, false, true);

        Color targetColor = gained ? gainColor : loseColor;
        playerFill.DOKill();
        opponentFill.DOKill();

        playerFill.DOColor(targetColor, 0.05f)
            .OnComplete(() => playerFill.DOColor(playerBaseColor, colorFlashTime));
        opponentFill.DOColor(targetColor, 0.05f)
            .OnComplete(() => opponentFill.DOColor(opponentBaseColor, colorFlashTime));

        // Avatar effect
        if (playerAvatar != null)
        {
            playerAvatar.transform.DOKill();
            playerAvatar.transform.DOScale(1.1f, 0.15f).SetLoops(2, LoopType.Yoyo);
        }
        if (opponentAvatar != null)
        {
            opponentAvatar.transform.DOKill();
            opponentAvatar.transform.DOScale(1.1f, 0.15f).SetLoops(2, LoopType.Yoyo);
        }
    }
}
